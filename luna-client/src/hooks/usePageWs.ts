import { useEffect, useRef, useState, useCallback } from 'react';
import { PageFullView } from '@/models/page/view/PageFullView';
import { PageBlockView } from '@/models/page/view/PageBlockView';
import { PageBlockBlank } from '@/models/page/blank/PageBlockBlank';
import { PageWsProvider } from "@/http/pageWsProvider";
import { PatchPageBlank } from "@/models/page/blank/PatchPageBlank";

interface UsePageWsOptions {
	autoConnect?: boolean;
	autoFetchData?: boolean;
	onConnected?: () => void;
	onError?: (error: Error) => void;
}

interface UsePageWsReturn {
	page: PageFullView | null;
	blocks: PageBlockView[];
	emoji: string | null;
	pageTitle: string | null;
	cover: string | null;
	description: string | null;
	isConnected: boolean;
	isConnecting: boolean;
	error: Error | null;
	status: string | null;
	setBlocks: (blocks: PageBlockView[]) => void;
	setEmoji: (emoji: string | null) => void;
	setCover: (cover: string | null) => void;
	setPageTitle: (title: string | null) => void;
	setDescription: (description: string | null) => void;
	savePageData: (patchPageBlank: PatchPageBlank) => Promise<void>;
	saveBlocks: (blocks: PageBlockView | PageBlockBlank[]) => Promise<void>;
	updatePageContent: (blocks: PageBlockView[], changeDescription?: string) => Promise<void>;
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
	const [status, setStatus] = useState(null);
	const [blocks, setBlocks] = useState<PageBlockView[]>([]);
	const [emoji, setEmoji] = useState<string | null>(null);
	const [cover, setCover] = useState<string | null>(null);
	const [pageTitle, setPageTitle] = useState<string | null>(null);
	const [description, setDescription] = useState<string | null>(null);
	const [isConnected, setIsConnected] = useState(false);
	const [isConnecting, setIsConnecting] = useState(false);
	const [error, setError] = useState<Error | null>(null);

	const providerRef = useRef<PageWsProvider | null>(null);
	const isJoinedRef = useRef(false);
	const unsubscribersRef = useRef<(() => void)[]>([]);

	const callbacksRef = useRef({ onConnected, onError });

	useEffect(() => {
		callbacksRef.current = { onConnected, onError };
	}, [onConnected, onError]);

	if (!providerRef.current) {
		providerRef.current = new PageWsProvider();
	}

	const saveBlocks = useCallback(async (newBlocks: PageBlockView[]) => {
		const provider = providerRef.current;
		if (!provider) {
			const err = new Error('Provider not initialized');
			setError(err);
			throw err;
		}

		setBlocks(newBlocks);

		const pageBlocks = newBlocks.map((item) => {
			return { ...item } as PageBlockBlank;
		});

		try {
			await provider.updatePageContent(pageId, {
				blocks: pageBlocks,
				changeDescription: 'Update'
			});
			console.log('[usePageWs] Blocks updated successfully');
		} catch (err) {
			const error = err instanceof Error ? err : new Error('Failed to update blocks');
			console.error('[usePageWs] Failed to update blocks:', error);
			setError(error);
			throw error;
		}
	}, [pageId]);

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

	// Метод для обновления контента с кастомным описанием
	const updatePageContent = useCallback(async (
		newBlocks: PageBlockView[],
		changeDescription: string = 'Update'
	) => {
		const provider = providerRef.current;
		if (!provider) {
			const err = new Error('Provider not initialized');
			setError(err);
			throw err;
		}

		const pageBlocks = newBlocks.map((item) => {
			return { ...item } as PageBlockBlank;
		});

		try {
			await provider.updatePageContent(pageId, {
				blocks: pageBlocks,
				changeDescription
			});
			console.log('[usePageWs] Page content updated successfully');
			setBlocks(newBlocks);
		} catch (err) {
			const error = err instanceof Error ? err : new Error('Failed to update page content');
			console.error('[usePageWs] Failed to update page content:', error);
			setError(error);
			throw error;
		}
	}, [pageId]);

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

	// Основной эффект для подключения и подписки
	useEffect(() => {
		const provider = providerRef.current;
		if (!provider || !autoConnect) return;

		let isActive = true;

		const connectAndSubscribe = async () => {
			try {
				setIsConnecting(true);
				setStatus("Connecting");
				setError(null);

				await provider.connect();

				if (!isActive) return;

				setIsConnected(true);
				setStatus("Connected");

				const unsubs = [
					provider.onPageData((data: PageFullView) => {
						setStatus("Page data received");
						console.log('[usePageWs] Page data received');
						setPage(data);
						setBlocks(data?.pageVersionView?.content ?? []);
						setEmoji(data.page?.emoji ?? null);
						setPageTitle(data.page?.title ?? null);
						setCover(data.page?.cover ?? null);
						setDescription(data.page?.description ?? null);
						setError(null);
					}),

					provider.onPageUpdated((payload: { pageId: string }) => {
						setStatus("Page updated");
						console.log('[usePageWs] Page updated', payload);
						if (provider && isJoinedRef.current) {
							provider.getPageData(pageId).catch(console.error);
						}
					}),

					provider.onPageContentUpdated((payload) => {
						setStatus("Content updated");
						setBlocks(prev => {
							const same = JSON.stringify(prev) === JSON.stringify(payload.blocks);
							return same ? prev : payload.blocks;
						});
					}),

					provider.onUserJoinedPage((payload: { userId: string; pageId: string }) => {
						setStatus("User joined");
					}),

					provider.onUserLeftPage((payload: { userId: string; pageId: string }) => {
						setStatus("User left");
					}),

					provider.onPageComments((payload: { pageId: string; comments: any[] }) => {
						setStatus("Page comments loaded");
					}),

					provider.onPageCommentsUpdated((payload: { pageId: string; comments: any[] }) => {
						setStatus("Page comments updated");
					}),
				];

				unsubscribersRef.current = unsubs;

				setStatus("Joining page");
				await provider.joinPage(pageId);
				isJoinedRef.current = true;

				if (!isActive) return;

				if (autoFetchData) {
					setStatus("Fetching page data");
					await provider.getPageData(pageId);
				}

				callbacksRef.current.onConnected?.();

			} catch (err) {
				const error = err instanceof Error ? err : new Error('Connection failed');
				setStatus("Error connecting");
				setError(error);
				callbacksRef.current.onError?.(error);
			} finally {
				if (isActive) {
					setIsConnecting(false);
				}
			}
		};

		connectAndSubscribe();

		return () => {
			isActive = false;

			const cleanup = async () => {
				try {
					unsubscribersRef.current.forEach(unsub => unsub());
					unsubscribersRef.current = [];

					if (isJoinedRef.current && provider.isConnected()) {
						setStatus("Leaving page");
						await provider.leavePage(pageId);
						isJoinedRef.current = false;
					}

					if (provider.isConnected()) {
						setStatus("Disconnecting");
						await provider.disconnect();
						setIsConnected(false);
					}
				} catch (error) {
					console.error('[usePageWs] Cleanup error:', error);
				}
			};

			cleanup();
		};
	}, [pageId, autoConnect, autoFetchData]);

	return {
		page,
		blocks,
		emoji,
		pageTitle,
		cover,
		description,
		isConnected,
		isConnecting,
		error,
		status,
		setBlocks,
		setEmoji,
		setCover,
		setPageTitle,
		setDescription,
		saveBlocks,
		savePageData,
		updatePageContent,
		refetchPageData,
		provider: providerRef.current,
	};
}