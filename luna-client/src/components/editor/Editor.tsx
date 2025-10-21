import React, { useEffect, useRef } from "react";
import styles from "./Editor.module.scss";
import { PageBlockView } from "@/models/page/view/PageBlockView";
import { PageBlockBlank } from "@/models/page/blank/PageBlockBlank";
import { CursorOverlay } from "@/components/editor/addons/CursorOverlay";
import { UserCursorView } from "@/models/cursor/UserCursorView";

interface EditorData {
	time: number;
	blocks: (PageBlockView | PageBlockBlank)[];
	version?: number; // Версия сервера
}

interface EditorProps {
	data: EditorData;
	onChange: (blocks: (PageBlockView | PageBlockBlank)[]) => void;
	scrollToBlockId?: string | null;
	cursors?: UserCursorView[];
	onCursorChange?: (blockId: string, position: number) => void;
}

export const Editor: React.FC<EditorProps> = ({
	data,
	onChange,
	scrollToBlockId,
	cursors,
	onCursorChange
}) => {
	const holderRef = useRef<HTMLDivElement | null>(null);
	const editorRef = useRef<any>(null);
	const pendingSaveRef = useRef<NodeJS.Timeout | null>(null);
	const cursorUpdateTimeoutRef = useRef<NodeJS.Timeout | null>(null);

	const applyingRemoteRef = useRef(false);
	const lastSignatureRef = useRef<string | undefined>(undefined);

	// Функция для отправки позиции курсора
	const sendCursorPosition = () => {
		if (!editorRef.current || !onCursorChange) return;

		try {
			const currentIndex = editorRef.current.blocks.getCurrentBlockIndex?.();
			if (currentIndex === undefined || currentIndex < 0) return;

			const currentBlock = editorRef.current.blocks.getBlockByIndex(currentIndex);
			if (!currentBlock || !currentBlock.id) return;

			// Получаем текущую позицию курсора в блоке
			const selection = window.getSelection();
			if (!selection || selection.rangeCount === 0) return;

			const range = selection.getRangeAt(0);
			const position = range.startOffset;

			onCursorChange(currentBlock.id, position);
		} catch (err) {
			console.error('[Editor] Error sending cursor position:', err);
		}
	};

	const handleCursorMove = () => {
		if (cursorUpdateTimeoutRef.current) {
			clearTimeout(cursorUpdateTimeoutRef.current);
		}

		cursorUpdateTimeoutRef.current = setTimeout(() => {
			sendCursorPosition();
		}, 300);
	};

	// Mount
	useEffect(() => {
		let mounted = true;

		const scrollToBlock = async () => {
			try {
				await editorRef.current.isReady;

				const blocksCount = editorRef.current.blocks.getBlocksCount();
				let targetIndex = -1;

				// Ищем индекс блока по id
				for (let i = 0; i < blocksCount; i++) {
					const block = editorRef.current.blocks.getBlockByIndex(i);
					if (block?.id === scrollToBlockId) {
						targetIndex = i;
						break;
					}
				}

				if (targetIndex !== -1) {
					// Устанавливаем фокус на блок
					editorRef.current.caret.setToBlock?.(targetIndex, 'start');

					// Скроллим к блоку (ищем DOM элемент)
					const blockElement = holderRef.current?.querySelector(`[data-id="${scrollToBlockId}"]`);
					if (blockElement) {
						blockElement.scrollIntoView({
							behavior: 'smooth',
							block: 'center'
						});

						// Добавляем временную подсветку
						blockElement.classList.add(styles.highlighted);
						setTimeout(() => {
							blockElement.classList.remove(styles.highlighted);
						}, 2000);
					}
				}
			} catch (err) {
				console.error('Error scrolling to block:', err);
			}
		};

		const init = async () => {
			if (!mounted) return;
			try {
				const [
					EditorJSMod,
					HeaderMod,
					ListMod,
					EmbedMod,
					ImageMod,
					SimpleImageMod,
					InlineCodeMod,
					LinkMod,
					MarkerMod,
					ParagraphMod,
					QuoteMod,
					RawMod,
					TableMod,
					WarningMod,
					DelimiterMod,
					CodeMod,
					ChecklistMod,
				] = await Promise.all([
					import("@editorjs/editorjs"),
					import("@editorjs/header"),
					import("@editorjs/list"),
					import("@editorjs/embed"),
					import("@editorjs/image"),
					import("@editorjs/simple-image"),
					import("@editorjs/inline-code"),
					import("@editorjs/link"),
					import("@editorjs/marker"),
					import("@editorjs/paragraph"),
					import("@editorjs/quote"),
					import("@editorjs/raw"),
					import("@editorjs/table"),
					import("@editorjs/warning"),
					import("@editorjs/delimiter"),
					import("@editorjs/code"),
					import("@editorjs/checklist"),
				]);

				const EditorJS = (EditorJSMod.default ?? EditorJSMod) as any;
				const Header = (HeaderMod.default ?? HeaderMod) as any;
				const List = (ListMod.default ?? ListMod) as any;
				const Embed = (EmbedMod.default ?? EmbedMod) as any;
				const ImageTool = (ImageMod.default ?? ImageMod) as any;
				const SimpleImage = (SimpleImageMod.default ?? SimpleImageMod) as any;
				const InlineCode = (InlineCodeMod.default ?? InlineCodeMod) as any;
				const LinkTool = (LinkMod.default ?? LinkMod) as any;
				const Marker = (MarkerMod.default ?? MarkerMod) as any;
				const Paragraph = (ParagraphMod.default ?? ParagraphMod) as any;
				const Quote = (QuoteMod.default ?? QuoteMod) as any;
				const RawTool = (RawMod.default ?? RawMod) as any;
				const Table = (TableMod.default ?? TableMod) as any;
				const Warning = (WarningMod.default ?? WarningMod) as any;
				const Delimiter = (DelimiterMod.default ?? DelimiterMod) as any;
				const CodeTool = (CodeMod.default ?? CodeMod) as any;
				const Checklist = (ChecklistMod.default ?? ChecklistMod) as any;

				if (editorRef.current?.destroy) {
					await editorRef.current.destroy();
					editorRef.current = null;
				}

				await new Promise(requestAnimationFrame);

				if (!mounted || !holderRef.current) return;

				editorRef.current = new EditorJS({
					holder: holderRef.current,
					data: data,
					autofocus: true,
					minHeight: 220,
					placeholder: "Enter anything here...",
					tools: {
						header: {
							class: Header,
							inlineToolbar: true
						},
						list: List,
						embed: Embed,
						table: Table,
						quote: Quote,
						code: CodeTool,
						delimiter: Delimiter,
						warning: Warning,
						checklist: Checklist,
						raw: RawTool,
						marker: Marker,
						inline: InlineCode,
						linkTool: { class: LinkTool, config: {} },
						paragraph: Paragraph,
						simpleImage: SimpleImage,
					},
					onChange: async (api: any) => {
						console.log('[Editor] onChange called, applyingRemote:', applyingRemoteRef.current);
						if (applyingRemoteRef.current) {
							applyingRemoteRef.current = false;
							console.log('[Editor] onChange suppressed');
							return;
						}

						try {
							const savedData = await api.saver.save();
							onChange(savedData.blocks);
							lastSignatureRef.current = JSON.stringify(savedData.blocks);
						} catch (err) {
							console.error("Error saving editor data:", err);
						}
					},
				});

				await editorRef.current.isReady;
				lastSignatureRef.current = JSON.stringify(data.blocks ?? []);
				console.log("Editor initialized");

				if (holderRef.current && onCursorChange) {
					holderRef.current.addEventListener('keyup', handleCursorMove);
					holderRef.current.addEventListener('mouseup', handleCursorMove);
					holderRef.current.addEventListener('click', handleCursorMove);
				}

				if (scrollToBlockId) {
					console.log("Scrolling to:", scrollToBlockId);
					await scrollToBlock();
				}

			} catch (err) {
				console.error("Editor init error:", err);
			}
		};

		init();

		return () => {
			mounted = false;
			if (pendingSaveRef.current) {
				clearTimeout(pendingSaveRef.current);
			}
			if (cursorUpdateTimeoutRef.current) {
				clearTimeout(cursorUpdateTimeoutRef.current);
			}

			if (holderRef.current) {
				holderRef.current.removeEventListener('keyup', handleCursorMove);
				holderRef.current.removeEventListener('mouseup', handleCursorMove);
				holderRef.current.removeEventListener('click', handleCursorMove);
			}

			if (editorRef.current?.destroy) {
				editorRef.current.destroy();
			}

			editorRef.current = null;
		};
	}, []);

	// Эффект для применения удалённых обновлений без полного рендера
	useEffect(() => {
		const api = editorRef.current;
		if (!api) return;

		const incomingSignature = JSON.stringify(data.blocks ?? []);

		// последние изменения такие же как новые
		if (lastSignatureRef.current == incomingSignature) {
			return;
		}

		// Применяем патч аккуратно
		const applyRemote = async () => {
			applyingRemoteRef.current = true;
			try {

				const getCount = () => api.blocks.getBlocksCount();
				const getByIndexSafe = (i: number) => (i >= 0 && i < getCount()) ? api.blocks.getBlockByIndex(i) : null;

				// 1) Сохраняем текущий фокус-блок/индекс
				const currentIndex = api.blocks.getCurrentBlockIndex?.() ?? 0;
				const currentBlock = getByIndexSafe(currentIndex);
				const currentId = currentBlock?.id ?? null;

				// 2) Построим текущий список блоков
				const currentCount = getCount();
				const currentBlocks: any[] = [];
				for (let i = 0; i < currentCount - 1; i++) {
					const b = api.blocks.getBlockByIndex(i);
					if (!b) continue;
					currentBlocks.push({ id: b.id, type: b.type, data: b.data });
				}

				const nextBlocks = (data.blocks ?? []) as any[];

				// Удаляем «лишние» блоки с конца или там, где id не совпадает
				// (идём с конца, чтобы индексы не сдвигались при удалении)
				for (let i = Math.max(currentBlocks.length, nextBlocks.length) - 1; i >= 0; i--) {
					const cur = currentBlocks[i];
					const nxt = nextBlocks[i];
					if (!nxt) {
						if (i < getCount()) api.blocks.delete(i);
						continue;
					}
					if (!cur) continue;
					if (cur.id !== nxt.id) {
						if (i < getCount()) api.blocks.delete(i);
					}
				}

				// Вставки/обновления (идём слева направо)
				for (let i = 0; i < nextBlocks.length; i++) {
					const nxt = nextBlocks[i];
					const cur = getByIndexSafe(i);

					if (!cur) {
						api.blocks.insert(nxt.type, nxt.data, undefined, i, false);
						continue;
					}

					const curShape = { id: cur.id, type: cur.name, data: cur.data };
					if (curShape.id !== nxt.id) {
						// Явная замена: безопаснее удалить, затем вставить на то же место
						if (i < getCount()) api.blocks.delete(i);
						api.blocks.insert(nxt.type, nxt.data, undefined, i, false);
					} else {
						const sameData = JSON.stringify(curShape.data) == JSON.stringify(nxt.data);
						const sameType = curShape.type == nxt.type;
						if (!sameData || !sameType) {
							const hasUpdate = typeof api.blocks.update == 'function';
							if (hasUpdate) {
								api.blocks.update(cur.id, { type: nxt.type, data: nxt.data });
							} else {
								if (i < getCount()) api.blocks.delete(i);
								api.blocks.insert(nxt.type, nxt.data, undefined, i, false);
							}
						}
					}
				}

				// 4) Восстановим каретку
				if (currentId) {
					// найдём новый индекс блока с тем же id
					const count = api.blocks.getBlocksCount();
					let newIndex = -1;
					for (let i = 0; i < count - 1; i++) {
						const b = api.blocks.getBlockByIndex(i);
						if (b?.id === currentId) {
							newIndex = i;
							break;
						}
					}
					if (newIndex >= 0) {
						api.caret.setToBlock?.(newIndex, 'end');
					} else {
						// если блока уже нет — поставим в конец
						api.caret.setToBlock?.(Math.max(0, api.blocks.getBlocksCount() - 1), 'end');
					}
				}

				lastSignatureRef.current = incomingSignature;
			} catch (e) {
				console.error('Failed to apply remote changes', e);
			} finally {
				await new Promise(resolve => setTimeout(resolve, 500));
			}
		};

		applyRemote();
	}, [data]);

	return (
		<div ref={holderRef} className={styles.editorjs}>
			{holderRef.current && (
				<CursorOverlay
					editorElement={holderRef.current}
					cursors={cursors}
				/>
			)}
		</div>
	);
};