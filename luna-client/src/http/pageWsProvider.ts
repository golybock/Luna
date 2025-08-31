import * as signalR from "@microsoft/signalr";
import { UpdatePageContentBlank } from "@/types/page/updatePageContentBlank";
import { PatchPageBlank } from "@/types/page/patchPageBlank";
import { CreatePageCommentBlank } from "@/types/page/createPageCommentBlank";
import { PatchPageCommentBlank } from "@/types/page/patchPageCommentBlank";
import { PageFullView } from "@/types/page/pageFullView";
import { PageBlockView } from "@/types/page/pageBlockView";

export class PageWsProvider {
	private connection: signalR.HubConnection | null = null;
	private readonly baseUrl: string;
	private readonly hubPath: string = "/ws/v1/pageHub";

	constructor() {
		this.baseUrl = process.env.NEXT_PUBLIC_PAGES_WS_HOST ?? "http://localhost:8000";
	}

	isConnected(): boolean {
		return this.connection?.state === signalR.HubConnectionState.Connected;
	}

	async connect(): Promise<void> {
		if (this.isConnected()) return;

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

		await this.connection.start();
	}

	async disconnect(): Promise<void> {
		if (!this.connection) return;
		try {
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

	// Event subscriptions (client methods)
	onUserJoinedPage(handler: (payload: { userId: string, pageId: string }) => void) { this.on("UserJoinedPage", handler); }
	onUserLeftPage(handler: (payload: { userId: string, pageId: string }) => void) { this.on("UserLeftPage", handler); }
	onPageData(handler: (page: PageFullView) => void) { this.on("PageData", handler); }
	onPageUpdated(handler: (payload: { pageId: string }) => void) { this.on("PageUpdated", handler); }
	onPageContentUpdated(handler: (payload: { pageId: string, blocks: PageBlockView[] }) => void) { this.on("PageContentUpdated", handler); }
	onPageComments(handler: (payload: { pageId: string, comments: any[] }) => void) { this.on("PageComments", handler); }
	onPageCommentsUpdated(handler: (payload: { pageId: string, comments: any[] }) => void) { this.on("PageCommentsUpdated", handler); }
	onCommentUpdated(handler: (payload: { commentId: string }) => void) { this.on("CommentUpdated", handler); }
	onCommentDeleted(handler: (payload: { commentId: string }) => void) { this.on("CommentDeleted", handler); }
	onPong(handler: (payload: any) => void) { this.on("Pong", handler); }

	private on<T = any>(event: string, handler: (payload: T) => void) {
		if (!this.connection) return;
		this.connection.on(event, handler as any);
	}

	private wireDefaultLogging() {
		if (!this.connection) return;
		this.connection.onreconnected((id) => console.info("[PageWs] Reconnected", id));
		this.connection.onreconnecting((e) => console.warn("[PageWs] Reconnecting", e));
		this.connection.onclose((e) => console.warn("[PageWs] Closed", e));
	}

	private async ensureConnected() {
		if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
			await this.connect();
		}
	}
}