import { useCallback, useEffect, useRef, useState } from 'react';
import { PageFullView } from '@/models/page/view/PageFullView';
import { PageWsProvider } from "@/http/pageWsProvider";
import { PatchPageBlank } from "@/models/page/blank/PatchPageBlank";
import { UserCursorView } from "@/models/cursor/UserCursorView";
import { UserCursorBlank } from "@/models/cursor/UserCursorBlank";
import { Status, Statuses } from "@/models/ui/Status";
import UserView from "@/models/auth/UserView";

interface UsePageWsOptions {
	autoConnect?: boolean;
	autoFetchData?: boolean;
	onConnected?: () => void;
	onError?: (error: Error) => void;
}

interface UsePageWsReturn {
	page: PageFullView | null;
	pageDocument: any;
	cursors: UserCursorView[];
	emoji: string | null;
	pageTitle: string | null;
	cover: string | null;
	description: string | null;
	isConnected: boolean;
	isConnecting: boolean;
	error: Error | null;
	status: Status | null;
	setPageDocument: (doc: any) => void;
	setCursor: (cursor: UserCursorBlank) => void;
	setEmoji: (emoji: string | null) => void;
	setCover: (cover: string | null) => void;
	setPageTitle: (title: string | null) => void;
	setDescription: (description: string | null) => void;
	savePageData: (patchPageBlank: PatchPageBlank) => Promise<void>;
	saveDocument: (document: any) => Promise<void>;
	updatePageContent: (document: any, changeDescription?: string) => Promise<void>;
	refetchPageData: () => Promise<void>;
	provider: PageWsProvider | null;
}

export function usePageWs(
	pageId: string,
	options: UsePageWsOptions = {}
): UsePageWsReturn {

	const {
		autoConnect = true,
		autoFetchData = true,
		onConnected,
		onError
	} = options;

	const [page, setPage] = useState<PageFullView | null>(null);
	const [status, setStatus] = useState<Status | null>(null);
	const [pageDocument, setPageDocument] = useState<any>(null);
	const [emoji, setEmoji] = useState<string | null>(null);
	const [cover, setCover] = useState<string | null>(null);
	const [pageTitle, setPageTitle] = useState<string | null>(null);
	const [description, setDescription] = useState<string | null>(null);
	const [cursors, setCursors] = useState<UserCursorView[]>([]);
	const [users, setUsers] = useState<UserView[]>([]);
	const [isConnected, setIsConnected] = useState(false);
	const [isConnecting, setIsConnecting] = useState(false);
	const [error, setError] = useState<Error | null>(null);
	const [displayStatus, setDisplayStatus] = useState<Status | null>(null);

	const providerRef = useRef<PageWsProvider | null>(null);
	const isJoinedRef = useRef(false);
	const unsubscribersRef = useRef<(() => void)[]>([]);
	const pageVersionRef = useRef<number>(-1);
	const lastIncomingSignatureRef = useRef<string | null>(null);
	const lastSentSignatureRef = useRef<string | null>(null);
	const currentSignatureRef = useRef<string | null>(null);

	const callbacksRef = useRef({ onConnected, onError });

	useEffect(() => {
		callbacksRef.current = { onConnected, onError };
	}, [onConnected, onError]);

	if (!providerRef.current) {
		providerRef.current = new PageWsProvider();
	}

	useEffect(() => {
		pageVersionRef.current = -1;
		lastIncomingSignatureRef.current = null;
		lastSentSignatureRef.current = null;
		currentSignatureRef.current = null;
	}, [pageId]);

	useEffect(() => {
		const timeout = setTimeout(() => setDisplayStatus(status), 150);
		const timeout2 = setTimeout(() => setDisplayStatus(Statuses.Empty), 1000);
		return () => {
			clearTimeout(timeout);
			clearTimeout(timeout2);
		};
	}, [status]);

	const docSignature = useCallback((value: any) => {
		try {
			return JSON.stringify(value ?? null);
		} catch {
			return null;
		}
	}, []);

	const hasDocChanged = useCallback((prev: any, next: any) => {
		return docSignature(prev) !== docSignature(next);
	}, [docSignature]);

	const saveDocument = useCallback(async (newDoc: any) => {
		const provider = providerRef.current;
		if (!provider) {
			const err = new Error('Provider not initialized');
			setError(err);
			throw err;
		}

		const sig = docSignature(newDoc);
		if (sig && sig === currentSignatureRef.current) {
			lastSentSignatureRef.current = sig;
			setPageDocument(newDoc);
			return;
		}

		currentSignatureRef.current = sig;
		lastSentSignatureRef.current = sig;
		setPageDocument(newDoc);

		try {
			await provider.updatePageContent(pageId, {
				document: newDoc,
				changeDescription: 'Update'
			});
			console.log('[usePageWs] Document updated successfully');
		} catch (err) {
			const error = err instanceof Error ? err : new Error('Failed to update document');
			console.error('[usePageWs] Failed to update document:', error);
			setError(error);
			throw error;
		}
	}, [pageId, docSignature]);

	const savePageData = useCallback(async (patchPageBlank: PatchPageBlank) => {
		const provider = providerRef.current;
		if (!provider) {
			const err = new Error('Provider not initialized');
			setError(err);
			throw err;
		}

		try {
			await provider.updatePage(pageId, patchPageBlank);
			console.log('[usePageWs] Page updated successfully');
		} catch (err) {
			const error = err instanceof Error ? err : new Error('Failed to update page');
			console.error('[usePageWs] Failed to update page:', error);
			setError(error);
			throw error;
		}
	}, [pageId]);

	const setCursor = useCallback(async (cursorBlank: UserCursorBlank) => {
		const provider = providerRef.current;
		if (!provider) {
			const err = new Error('Provider not initialized');
			setError(err);
			throw err;
		}

		try {
			await provider.setCursor(pageId, cursorBlank);
			console.log('[usePageWs] Cursor updated successfully');
		} catch (err) {
			const error = err instanceof Error ? err : new Error('Failed to update cursor');
			console.error('[usePageWs] Failed to update cursor:', error);
			setError(error);
			throw error;
		}
	}, [pageId]);

	// Метод для обновления контента с кастомным описанием
	const updatePageContent = useCallback(async (
		newDocument: any,
		changeDescription: string = 'Update'
	) => {
		const provider = providerRef.current;
		if (!provider) {
			const err = new Error('Provider not initialized');
			setError(err);
			throw err;
		}

		try {
			await provider.updatePageContent(pageId, {
				document: newDocument,
				changeDescription
			});
			console.log('[usePageWs] Page content updated successfully');
			const sig = docSignature(newDocument);
			currentSignatureRef.current = sig;
			lastSentSignatureRef.current = sig;
			setPageDocument(newDocument);
		} catch (err) {
			const error = err instanceof Error ? err : new Error('Failed to update page content');
			console.error('[usePageWs] Failed to update page content:', error);
			setError(error);
			throw error;
		}
	}, [pageId, docSignature]);

	// Метод для перезапроса данных страницы
	const refetchPageData = useCallback(async () => {
		const provider = providerRef.current;
		if (!provider || !isJoinedRef.current) {
			console.warn('[usePageWs] Cannot refetch: not connected or joined');
			return;
		}

		try {
			await provider.getPageData(pageId);
		} catch (err) {
			const error = err instanceof Error ? err : new Error('Failed to refetch page data');
			console.error('[usePageWs] Failed to refetch page data:', error);
			setError(error);
		}
	}, [pageId]);

	// Основной эффект для подключения и подписки (с паузой при скрытии вкладки)
	useEffect(() => {
		const provider = providerRef.current;
		if (!provider || !autoConnect) return;

		let isActive = true;
		let isSuspendedByVisibility = false;

		const cleanup = async () => {
			try {
				unsubscribersRef.current.forEach(unsub => unsub());
				unsubscribersRef.current = [];

				if (isJoinedRef.current && provider.isConnected()) {
					setStatus(Statuses.Leaving);
					await provider.leavePage(pageId);
					isJoinedRef.current = false;
				}

				if (provider.isConnected()) {
					setStatus(Statuses.Disconnecting);
					await provider.disconnect();
					setIsConnected(false);
				}
			} catch (error) {
				console.error('[usePageWs] Cleanup error:', error);
			}
		};

		const connectAndSubscribe = async () => {
			try {
				if (document.hidden) {
					setStatus(Statuses.Paused);
					isSuspendedByVisibility = true;
					return;
				}

				isSuspendedByVisibility = false;
				setIsConnecting(true);
				setStatus(Statuses.Connecting);
				setError(null);

				await provider.connect();

				if (!isActive) return;

				setIsConnected(true);
				setStatus(Statuses.Connected);

				const unsubs = [
					provider.onPageData((data: PageFullView) => {
						setStatus(Statuses.PageDataReceived);
						pageVersionRef.current = data.pageVersionView?.version ?? -1;

						const doc = data?.pageVersionView?.document ?? null;
						const sig = docSignature(doc);
						lastIncomingSignatureRef.current = sig;
						currentSignatureRef.current = sig;

						setPage(data);
						setPageDocument(doc);
						setEmoji(data.page?.emoji ?? null);
						setPageTitle(data.page?.title ?? null);
						setCover(data.page?.cover ?? null);
						setDescription(data.page?.description ?? null);
						setError(null);
					}),

					provider.onPageUpdated((payload: { pageId: string }) => {
						setStatus(Statuses.PageDataUpdated);
						console.log('[usePageWs] Page updated', payload);
						if (provider && isJoinedRef.current) {
							provider.getPageData(pageId).catch(console.error);
						}
					}),

					provider.onPageContentUpdated((payload) => {
						setStatus(Statuses.PageDataUpdated);
						const incomingSignature = docSignature(payload.document);
						const currentSig = currentSignatureRef.current;

						if (incomingSignature && incomingSignature === currentSig) {
							lastIncomingSignatureRef.current = incomingSignature;
							return;
						}

						if (incomingSignature && incomingSignature === lastIncomingSignatureRef.current) {
							return;
						}

						if (incomingSignature && incomingSignature === lastSentSignatureRef.current) {
							lastIncomingSignatureRef.current = incomingSignature;
							currentSignatureRef.current = incomingSignature;
							return;
						}

						setPageDocument(prev => {
							const incomingVersion = (payload as any)?.version ?? null;
							if (incomingVersion !== null && incomingVersion < pageVersionRef.current) {
								console.debug('[usePageWs] Skip stale content', {
									incomingVersion,
									current: pageVersionRef.current
								});
								return prev;
							}

							if (incomingVersion !== null) {
								pageVersionRef.current = incomingVersion;
							}

							if (!hasDocChanged(prev, payload.document)) {
								return prev;
							}

							lastIncomingSignatureRef.current = incomingSignature;
							currentSignatureRef.current = incomingSignature;
							return payload.document;
						});
					}),

					provider.onUserJoinedPage((payload: { userId: string; pageId: string }) => {
						setStatus(Statuses.UserJoined);
					}),

					provider.onUsersSet((payload: UserView[]) => {
						setUsers(payload);
						setStatus(Statuses.UserLeft);
					}),

					provider.onPageComments((payload: { pageId: string; comments: any[] }) => {
						setStatus(Statuses.PageDataReceived);
					}),

					provider.onPageCommentsUpdated((payload: { pageId: string; comments: any[] }) => {
						setStatus(Statuses.PageDataUpdated);
					}),

					provider.onCursorSet((payload: UserCursorView[]) => {
						setCursors(payload)
						console.log(payload)
					}),
				];

				unsubscribersRef.current = unsubs;

				setStatus(Statuses.Connecting);
				await provider.joinPage(pageId);
				isJoinedRef.current = true;

				if (!isActive) return;

				if (autoFetchData) {
					setStatus(Statuses.Fetching);
					await provider.getPageData(pageId);
				}

				callbacksRef.current.onConnected?.();

			} catch (err) {
				const error = err instanceof Error ? err : new Error('Connection failed');
				setStatus(Statuses.Error);
				setError(error);
				callbacksRef.current.onError?.(error);
			} finally {
				if (isActive) {
					setIsConnecting(false);
				}
			}
		};

		const handleVisibilityChange = async () => {
			if (!isActive) return;
			if (document.hidden) {
				setStatus(Statuses.Paused);
				await cleanup();
				isSuspendedByVisibility = true;
			} else if (isSuspendedByVisibility) {
				setStatus(Statuses.Reconnecting);
				await connectAndSubscribe();
			}
		};

		connectAndSubscribe();
		window.document.addEventListener('visibilitychange', handleVisibilityChange);

		return () => {
			isActive = false;
			window.document.removeEventListener('visibilitychange', handleVisibilityChange);
			cleanup();
		};
	}, [pageId, autoConnect, autoFetchData, hasDocChanged]);

	return {
		page,
		pageDocument,
		emoji,
		pageTitle,
		cover,
		description,
		cursors,
		isConnected,
		isConnecting,
		error,
		status: displayStatus ?? status,
		setPageDocument,
		setEmoji,
		setCover,
		setCursor,
		setPageTitle,
		setDescription,
		saveDocument,
		savePageData,
		updatePageContent,
		refetchPageData,
		provider: providerRef.current,
	};
}