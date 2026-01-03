import * as signalR from "@microsoft/signalr";
import { PatchPageBlank } from "@/models/page/blank/PatchPageBlank";
import { UpdatePageContentBlank } from "@/models/page/blank/UpdatePageContentBlank";
import { CreatePageCommentBlank } from "@/models/page/blank/CreatePageCommentBlank";
import { PatchPageCommentBlank } from "@/models/page/blank/PatchPageCommentBlank";
import { PageFullView } from "@/models/page/view/PageFullView";
import { UserCursorBlank } from "@/models/cursor/UserCursorBlank";
import { UserCursorView } from "@/models/cursor/UserCursorView";
import UserView from "@/models/auth/UserView";

type EventHandler<T = any> = (payload: T) => void;

interface EventHandlers {
	[eventName: string]: EventHandler[];
}

export class PageWsProvider {
	private connection: signalR.HubConnection | null = null;
	private readonly baseUrl: string;
	private readonly hubPath: string = "/ws/v1/pageHub";
	private eventHandlers: EventHandlers = {};
	private isConnecting: boolean = false;
	private connectionPromise: Promise<void> | null = null;

	constructor() {
		this.baseUrl = process.env.NEXT_PUBLIC_PAGES_WS_HOST ?? "http://localhost:8000";
	}

	isConnected(): boolean {
		return this.connection?.state === signalR.HubConnectionState.Connected;
	}

	async connect(): Promise<void> {
		// Если уже подключены, ничего не делаем
		if (this.isConnected()) return;

		// Если идет процесс подключения, ждем его завершения
		if (this.isConnecting && this.connectionPromise) {
			return this.connectionPromise;
		}

		this.isConnecting = true;
		this.connectionPromise = this._connect();

		try {
			await this.connectionPromise;
		} finally {
			this.isConnecting = false;
			this.connectionPromise = null;
		}
	}

	private async _connect(): Promise<void> {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(`${this.baseUrl}${this.hubPath}`, {
				withCredentials: true
			})
			.withAutomaticReconnect({
				nextRetryDelayInMilliseconds: retryContext => {
					const attempt = retryContext.previousRetryCount + 1;
					return Math.min(1000 * Math.pow(2, attempt), 10000);
				}
			})
			.configureLogging(signalR.LogLevel.Information)
			.build();

		this.wireDefaultLogging();
		this.wireReconnectionHandlers();

		await this.connection.start();
	}

	async disconnect(): Promise<void> {
		if (!this.connection) return;

		try {
			// Отписываемся от всех событий
			this.removeAllEventHandlers();
			await this.connection.stop();
		} finally {
			this.connection = null;
		}
	}

	// Server invocations
	async joinPage(pageId: string): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("JoinPage", pageId);
	}

	async leavePage(pageId: string): Promise<void> {
		if (!this.connection) return;
		await this.connection.invoke("LeavePage", pageId);
	}

	async pong(date: Date = new Date()): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("Pong", date.toISOString());
	}

	async getPageData(pageId: string): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("GetPageData", pageId);
	}

	async setCursor(pageId: string, userCursorBlank: UserCursorBlank): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("SetCursor", pageId, userCursorBlank);
	}

	async updatePage(pageId: string, patchPageBlank: PatchPageBlank): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("UpdatePage", pageId, patchPageBlank);
	}

	async updatePageContent(pageId: string, updateBlank: UpdatePageContentBlank): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("UpdatePageContent", pageId, updateBlank);
	}

	async getPageComments(pageId: string): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("GetPageComments", pageId);
	}

	async createComment(pageId: string, createBlank: CreatePageCommentBlank): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("CreateComment", pageId, createBlank);
	}

	async updateComment(commentId: string, patchBlank: PatchPageCommentBlank): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("UpdateComment", commentId, patchBlank);
	}

	async deleteComment(commentId: string): Promise<void> {
		await this.ensureConnected();
		await this.connection!.invoke("DeleteComment", commentId);
	}

	// Event subscriptions (client methods) - улучшенные версии
	onUserJoinedPage(handler: EventHandler<{ userId: string, pageId: string }>) {
		return this.on("UserJoinedPage", handler);
	}

	onUsersSet(handler: EventHandler<UserView[]>) {
		return this.on("UsersSet", handler);
	}

	onPageData(handler: EventHandler<PageFullView>) {
		return this.on("PageData", handler);
	}

	onPageUpdated(handler: EventHandler<{ pageId: string }>) {
		return this.on("PageUpdated", handler);
	}

	onPageContentUpdated(handler: EventHandler<{ pageId: string, document: any, version?: number, updatedAt?: string }>) {
		return this.on("PageContentUpdated", handler);
	}

	onPageComments(handler: EventHandler<{ pageId: string, comments: any[] }>) {
		return this.on("PageComments", handler);
	}

	onPageCommentsUpdated(handler: EventHandler<{ pageId: string, comments: any[] }>) {
		return this.on("PageCommentsUpdated", handler);
	}

	onCursorSet(handler: EventHandler<UserCursorView[]>) {
		return this.on("CursorSet", handler);
	}

	onCommentUpdated(handler: EventHandler<{ commentId: string }>) {
		return this.on("CommentUpdated", handler);
	}

	onCommentDeleted(handler: EventHandler<{ commentId: string }>) {
		return this.on("CommentDeleted", handler);
	}

	onPong(handler: EventHandler<any>) {
		return this.on("Pong", handler);
	}

	private on<T = any>(event: string, handler: EventHandler<T>): () => void {
		if (!this.connection) {
			console.warn(`[PageWs] Cannot subscribe to ${event}: no connection`);
			return () => {};
		}

		if (!this.eventHandlers[event]) {
			this.eventHandlers[event] = [];
		}
		this.eventHandlers[event].push(handler);

		this.connection.on(event, handler as any);

		return () => this.off(event, handler);
	}

	off<T = any>(event: string, handler: EventHandler<T>): void {
		if (!this.connection) return;

		if (this.eventHandlers[event]) {
			this.eventHandlers[event] = this.eventHandlers[event].filter(h => h !== handler);
		}

		this.connection.off(event, handler as any);
	}

	offEvent(event: string): void {
		if (!this.connection) return;

		const handlers = this.eventHandlers[event] || [];
		handlers.forEach(handler => {
			this.connection!.off(event, handler as any);
		});

		delete this.eventHandlers[event];
	}

	private removeAllEventHandlers(): void {
		if (!this.connection) return;

		Object.keys(this.eventHandlers).forEach(event => {
			this.offEvent(event);
		});

		this.eventHandlers = {};
	}

	private resubscribeAllHandlers(): void {
		if (!this.connection) return;

		Object.keys(this.eventHandlers).forEach(event => {
			const handlers = this.eventHandlers[event];
			handlers.forEach(handler => {
				this.connection!.on(event, handler as any);
			});
		});
	}

	private wireReconnectionHandlers(): void {
		if (!this.connection) return;

		this.connection.onreconnected((connectionId) => {
			console.info("[PageWs] Reconnected", connectionId);
			this.resubscribeAllHandlers();
		});
	}

	private wireDefaultLogging(): void {
		if (!this.connection) return;

		this.connection.onreconnecting((error) => {
			console.warn("[PageWs] Reconnecting", error);
		});

		this.connection.onclose((error) => {
			console.warn("[PageWs] Closed", error);
			this.eventHandlers = {};
		});
	}

	private async ensureConnected(): Promise<void> {
		if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
			await this.connect();
		}
	}

	getConnectionState(): signalR.HubConnectionState | null {
		return this.connection?.state ?? null;
	}

	isConnectingNow(): boolean {
		return this.isConnecting;
	}
}