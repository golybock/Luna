import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { BlockTypes } from "@/types/pageEditor/blolckTypes";
import { EditorBlock } from "@/types/pageEditor/editorBlock";
import { SlashItem } from "@/types/pageEditor/slashItem";
import { BlockType } from "@/types/pageEditor/blockType";
import { Guid } from "guid-typescript";

const SLASH_ITEMS: SlashItem[] = [
	{ id: "paragraph", label: "Текст", type: BlockTypes.Paragraph, action: "transform" },
	{ id: "h1", label: "Заголовок 1", type: BlockTypes.H1, action: "transform" },
	{ id: "h2", label: "Заголовок 2", type: BlockTypes.H2, action: "transform" },
	{ id: "h3", label: "Заголовок 3", type: BlockTypes.H3, action: "transform" },
	{ id: "quote", label: "Цитата", type: BlockTypes.Quote, action: "transform" },
	{ id: "todo", label: "Список задач", type: BlockTypes.Todo, action: "transform" },
	{ id: "divider", label: "Разделитель", type: BlockTypes.Divider, action: "insert" },
	{ id: "image", label: "Изображение", type: BlockTypes.Image, action: "insert" },
	{ id: "container", label: "Контейнер", type: BlockTypes.Container, action: "transform" },
];

// Utility functions
function generateId(): string {
	return Guid.create().toString();
}

function normalizeIndices(blocks: EditorBlock[]): EditorBlock[] {
	return blocks
		.slice()
		.sort((a, b) => (a.index ?? 0) - (b.index ?? 0))
		.map((b, i) => ({ ...b, index: i }));
}

function getTextContent(block: EditorBlock): string {
	const content: any = block.content || {};
	if (block.type === BlockTypes.Paragraph || block.type === BlockTypes.H1 || block.type === BlockTypes.H2 || block.type === BlockTypes.H3 || block.type === BlockTypes.Quote) {
		return content.text ?? "";
	}
	if (block.type === BlockTypes.Todo) {
		return content.text ?? "";
	}
	return "";
}

function setTextContent(block: EditorBlock, text: string): EditorBlock {
	const content: any = block.content || {};
	if (block.type === BlockTypes.Todo) {
		return { ...block, content: { ...content, text } };
	}
	// @ts-ignore
	if ([BlockTypes.Paragraph, BlockTypes.H1, BlockTypes.H2, BlockTypes.H3, BlockTypes.Quote, BlockTypes.Todo].includes(block.type as BlockType)) {
		return { ...block, content: { ...content, text } };
	}
	return block;
}

// Nested updates utilities
function mapBlocksRecursive(blocks: EditorBlock[], fn: (b: EditorBlock, parent?: EditorBlock) => EditorBlock, parent?: EditorBlock): EditorBlock[] {
	return blocks.map((b) => {
		const nb = fn(b, parent);
		if (nb.type === BlockTypes.Container) {
			const content: any = nb.content || {};
			const children: EditorBlock[] = Array.isArray(content.children) ? content.children : [];
			const mappedChildren = mapBlocksRecursive(children, fn, nb);
			return { ...nb, content: { ...content, children: normalizeIndices(mappedChildren) } };
		}
		return nb;
	});
}

function findBlockById(blocks: EditorBlock[], id: string): {
	block?: EditorBlock;
	parent?: EditorBlock;
	pathIds: string[]
} {
	const path: string[] = [];
	const dfs = (arr: EditorBlock[], parent?: EditorBlock): EditorBlock | undefined => {
		for (const b of arr) {
			path.push(b.id as string);
			if (b.id === id) return b;
			if (b.type === BlockTypes.Container) {
				const children: EditorBlock[] = (b.content as any)?.children ?? [];
				const found = dfs(children, b);
				if (found) return found;
			}
			path.pop();
		}
		return undefined;
	};
	const found = dfs(blocks);
	return { block: found, parent: undefined, pathIds: [...path] };
}

function replaceBlockById(blocks: EditorBlock[], id: string, replacer: (b: EditorBlock) => EditorBlock | EditorBlock[] | null): EditorBlock[] {
	const recur = (arr: EditorBlock[]): EditorBlock[] =>
		arr.map((b) => {
			if (b.id === id) {
				const rep = replacer(b);
				if (rep === null) return null as any;
				if (Array.isArray(rep)) return rep as any;
				return rep as any;
			}
			if (b.type === BlockTypes.Container) {
				const content: any = b.content || {};
				const children: EditorBlock[] = Array.isArray(content.children) ? content.children : [];
				const newChildren = recur(children).filter(Boolean) as EditorBlock[];
				return { ...b, content: { ...content, children: normalizeIndices(newChildren) } } as EditorBlock;
			}
			return b;
		}).filter(Boolean) as EditorBlock[];

	return normalizeIndices(recur(blocks));
}

function insertAfterId(blocks: EditorBlock[], id: string, newBlock: EditorBlock): EditorBlock[] {
	const recur = (arr: EditorBlock[]): EditorBlock[] => {
		const out: EditorBlock[] = [];
		for (const b of arr) {
			out.push(b);
			if (b.id === id) {
				out.push(newBlock);
			}
			if (b.type === BlockTypes.Container) {
				const content: any = b.content || {};
				const children: EditorBlock[] = Array.isArray(content.children) ? content.children : [];
				const newChildren = recur(children);
				out[out.length - 1] = {
					...b,
					content: { ...content, children: normalizeIndices(newChildren) }
				} as EditorBlock;
			}
		}
		return out;
	};
	return normalizeIndices(recur(blocks));
}

function removeById(blocks: EditorBlock[], id: string): EditorBlock[] {
	const recur = (arr: EditorBlock[]): EditorBlock[] => {
		const out: EditorBlock[] = [];
		for (const b of arr) {
			if (b.id === id) continue;
			if (b.type === BlockTypes.Container) {
				const content: any = b.content || {};
				const children: EditorBlock[] = Array.isArray(content.children) ? content.children : [];
				const newChildren = recur(children);
				out.push({ ...b, content: { ...content, children: normalizeIndices(newChildren) } } as EditorBlock);
			} else {
				out.push(b);
			}
		}
		return out;
	};
	return normalizeIndices(recur(blocks));
}

function createEmptyParagraph(): EditorBlock {
	return { id: generateId(), type: BlockTypes.Paragraph, index: 0, content: { text: "" } } as any;
}

interface UsePageEditorProps {
	value: EditorBlock[];
	onChange: (value: EditorBlock[]) => void;
}

export const usePageEditor = ({ value, onChange }: UsePageEditorProps) => {
	const editorRef = useRef<HTMLDivElement | null>(null);
	const blockRefs = useRef<Map<string, HTMLDivElement>>(new Map());

	// Slash menu state
	const [slashForId, setSlashForId] = useState<string | null>(null);
	const [slashQuery, setSlashQuery] = useState<string>("");
	const [slashPos, setSlashPos] = useState<{ top: number; left: number } | null>(null);

	// Ensure all blocks have IDs and are normalized
	const ensureIds = useMemo(() => {
		const withIds = mapBlocksRecursive(
			value && value.length ? value : [createEmptyParagraph()],
			(b) => ({ ...b, id: b.id || generateId() })
		);
		return normalizeIndices(withIds);
	}, [value]);

	// Filtered menu items
	const filteredItems = useMemo(() => {
		if (!slashQuery) return SLASH_ITEMS;
		const q = slashQuery.toLowerCase();
		return SLASH_ITEMS.filter((i) => i.label.toLowerCase().includes(q) || i.type.includes(q));
	}, [slashQuery]);

	// Close slash menu when clicking outside
	useEffect(() => {
		const onWindowClick = (e: MouseEvent) => {
			const menu = document.getElementById("slash-menu");
			if (menu && e.target instanceof Node && !menu.contains(e.target)) {
				setSlashForId(null);
				setSlashQuery("");
			}
		};
		window.addEventListener("click", onWindowClick);
		return () => window.removeEventListener("click", onWindowClick);
	}, []);

	const updateBlocks = useCallback((blocks: EditorBlock[]) => {
		onChange(normalizeIndices(blocks));
	}, [onChange]);

	const focusBlock = useCallback((id?: string) => {
		if (!id) return;
		const ref = blockRefs.current.get(id);
		if (ref) {
			ref.focus();
			// move caret to end
			const range = document.createRange();
			const sel = window.getSelection();
			range.selectNodeContents(ref);
			range.collapse(false);
			sel?.removeAllRanges();
			sel?.addRange(range);
		}
	}, []);

	const handleTextInput = useCallback((blockId: string, e: React.FormEvent<HTMLDivElement>) => {
		const el = e.currentTarget;
		const text = el.innerText.replace(/\n/g, "\n");
		let blocks = ensureIds;
		blocks = replaceBlockById(blocks, blockId, (b) => setTextContent(b, text));

		// Detect // and open slash menu
		const idx = text.indexOf("//");
		if (idx !== -1) {
			const query = text.slice(idx + 2).trim();
			setSlashForId(blockId);
			setSlashQuery(query);
			// position menu below the block element
			const ref = blockRefs.current.get(blockId);
			if (ref) {
				const rect = ref.getBoundingClientRect();
				const editorRect = editorRef.current?.getBoundingClientRect();
				const top = rect.bottom - (editorRect?.top ?? 0) + 4;
				const left = (rect.left - (editorRect?.left ?? 0)) + 8;
				setSlashPos({ top, left });
			}
		} else if (slashForId === blockId) {
			setSlashForId(null);
			setSlashQuery("");
		}

		updateBlocks(blocks);
		// Ensure caret stays at the end after rerender
		queueMicrotask(() => focusBlock(blockId));
	}, [ensureIds, updateBlocks, slashForId, focusBlock]);

	const handleKeyDown = useCallback((block: EditorBlock, e: React.KeyboardEvent<HTMLDivElement>) => {
		if (e.key === "Enter" && !e.shiftKey) {
			e.preventDefault();
			const newBlock = createEmptyParagraph();
			const nextBlocks = insertAfterId(ensureIds, block.id as string, newBlock);
			updateBlocks(nextBlocks);
			// focus next block after render
			queueMicrotask(() => focusBlock(newBlock.id as string));
			return;
		}

		if (e.key === "Backspace") {
			const text = getTextContent(block);
			if (!text || text.length === 0) {
				e.preventDefault();
				const next = removeById(ensureIds, block.id as string);
				updateBlocks(next.length ? next : [createEmptyParagraph()]);
				// Focus previous block if exists
				const prevIndex = (block.index ?? 0) - 1;
				const prev = next.find((b) => b.index === prevIndex);
				queueMicrotask(() => focusBlock(prev?.id as string));
			}
		}

		if (slashForId && e.key === "Escape") {
			setSlashForId(null);
			setSlashQuery("");
		}
	}, [ensureIds, updateBlocks, slashForId, focusBlock]);

	const handleTodoToggle = useCallback((blockId: string, checked: boolean) => {
		const next = replaceBlockById(ensureIds, blockId, (b) => ({
			...b,
			content: { ...(b.content as any), checked }
		}));
		updateBlocks(next);
	}, [ensureIds, updateBlocks]);

	const handleContainerActivate = useCallback((containerId: string) => {
		let createdChildId: string | undefined;
		const next = replaceBlockById(ensureIds, containerId, (b) => {
			const content: any = b.content || {};
			const children: EditorBlock[] = Array.isArray(content.children) ? content.children : [];
			if (children.length > 0) return b;
			const child = {
				id: generateId(),
				type: BlockTypes.Paragraph,
				index: 0,
				content: { text: "" }
			} as EditorBlock;
			createdChildId = child.id as string;
			return { ...b, content: { ...content, children: [child] } } as EditorBlock;
		});
		updateBlocks(next);
		if (createdChildId) {
			queueMicrotask(() => focusBlock(createdChildId));
		}
	}, [ensureIds, updateBlocks, focusBlock]);

	const handleSlashSelect = useCallback((item: SlashItem) => {
		if (!slashForId) return;

		const current = findBlockById(ensureIds, slashForId).block;
		let currentText = current ? getTextContent(current) : "";
		// remove // and query from text
		const idx = currentText.indexOf("//");
		if (idx !== -1) currentText = currentText.slice(0, idx).trimEnd();

		if (!current) return;

		let next: EditorBlock[] = ensureIds;

		if (item.type === BlockTypes.Image && item.action === "insert") {
			const url = window.prompt("Вставьте URL изображения");
			if (!url) {
				setSlashForId(null);
				setSlashQuery("");
				return;
			}
			const newBlock: EditorBlock = {
				id: generateId(),
				type: BlockTypes.Image,
				index: (current.index ?? 0) + 1,
				content: { url },
			} as any;
			next = insertAfterId(next, slashForId, newBlock);
			updateBlocks(next);
			setSlashForId(null);
			setSlashQuery("");
			queueMicrotask(() => focusBlock(newBlock.id as string));
			return;
		}

		if (item.type === BlockTypes.Divider && item.action === "insert") {
			const newBlock: EditorBlock = {
				id: generateId(),
				type: BlockTypes.Divider,
				index: (current.index ?? 0) + 1,
				content: {},
			} as any;
			next = insertAfterId(next, slashForId, newBlock);
			updateBlocks(next);
			setSlashForId(null);
			setSlashQuery("");
			queueMicrotask(() => focusBlock(newBlock.id as string));
			return;
		}

		if (item.type === BlockTypes.Container && item.action === "transform") {
			// transform current block into container and move text into child paragraph
			const child: EditorBlock = {
				id: generateId(),
				type: BlockTypes.Paragraph,
				index: 0,
				content: { text: currentText }
			} as any;
			next = replaceBlockById(next, slashForId, (b) => ({
				...b,
				type: BlockTypes.Container,
				content: { children: [child] },
			}));
			updateBlocks(next);
			setSlashForId(null);
			setSlashQuery("");
			queueMicrotask(() => focusBlock(child.id as string));
			return;
		}

		// Transform text-based block types
		const textTypes: BlockType[] = [BlockTypes.Paragraph, BlockTypes.H1, BlockTypes.H2, BlockTypes.H3, BlockTypes.Quote, BlockTypes.Todo];
		if (textTypes.includes(item.type)) {
			next = replaceBlockById(next, slashForId, (b) => {
				const base: any = { ...b, type: item.type };
				if (item.type === BlockTypes.Todo) {
					return {
						...base,
						content: { text: currentText, checked: (b.content as any)?.checked ?? false }
					} as EditorBlock;
				}
				return { ...base, content: { text: currentText } } as EditorBlock;
			});
			updateBlocks(next);
			setSlashForId(null);
			setSlashQuery("");
			queueMicrotask(() => focusBlock(slashForId));
			return;
		}
	}, [ensureIds, slashForId, updateBlocks, focusBlock]);

	const registerBlockRef = useCallback((id: string, el: HTMLDivElement | null) => {
		if (el) {
			blockRefs.current.set(id, el);
		} else {
			blockRefs.current.delete(id);
		}
	}, []);

	return {
		// Data
		blocks: ensureIds,
		BlockTypes,

		// Refs
		editorRef,
		registerBlockRef,

		// Slash menu state
		slashForId,
		slashPos,
		filteredItems,

		// Handlers
		handleTextInput,
		handleKeyDown,
		handleTodoToggle,
		handleContainerActivate,
		handleSlashSelect,

		// Utilities
		getTextContent,
		focusBlock,
	};
};