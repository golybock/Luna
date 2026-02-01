import React, { useEffect, useRef, useState, forwardRef, useImperativeHandle } from "react";
import { useEditor, EditorContent, JSONContent, Editor, ReactRenderer } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import TextAlign from "@tiptap/extension-text-align";
import Highlight from "@tiptap/extension-highlight";
import Typography from "@tiptap/extension-typography";
import { TextStyle } from "@tiptap/extension-text-style";
import Color from "@tiptap/extension-color";
import Subscript from "@tiptap/extension-subscript";
import Superscript from "@tiptap/extension-superscript";
import Mention from "@tiptap/extension-mention";
import { Mathematics } from "@tiptap/extension-mathematics";
import HorizontalRule from "@tiptap/extension-horizontal-rule";
import Placeholder from "@tiptap/extension-placeholder";
import { Extension } from "@tiptap/core";
import Suggestion, { SuggestionOptions, SuggestionProps } from "@tiptap/suggestion";
import { PluginKey } from "@tiptap/pm/state";
import tippy, { Instance as TippyInstance } from "tippy.js";
import styles from "./NotionLikeEditor.module.scss";

interface NotionLikeEditorProps {
	content: JSONContent;
	onChange: (content: JSONContent) => void;
	editable?: boolean;
	placeholder?: string;
	className?: string;
	onCursorChange?: (blockId: string, position: number) => void;
}

// Команды для slash menu
interface CommandItem {
	title: string;
	description: string;
	icon: string;
	command: (editor: Editor) => void;
}

const commands: CommandItem[] = [
	{
		title: "Heading 1",
		description: "Large section heading",
		icon: "H1",
		command: (editor) => editor.chain().focus().toggleHeading({ level: 1 }).run(),
	},
	{
		title: "Heading 2",
		description: "Medium section heading",
		icon: "H2",
		command: (editor) => editor.chain().focus().toggleHeading({ level: 2 }).run(),
	},
	{
		title: "Heading 3",
		description: "Small section heading",
		icon: "H3",
		command: (editor) => editor.chain().focus().toggleHeading({ level: 3 }).run(),
	},
	{
		title: "Bullet List",
		description: "Create a simple bullet list",
		icon: "•",
		command: (editor) => editor.chain().focus().toggleBulletList().run(),
	},
	{
		title: "Numbered List",
		description: "Create a list with numbering",
		icon: "1.",
		command: (editor) => editor.chain().focus().toggleOrderedList().run(),
	},
	{
		title: "Quote",
		description: "Capture a quote",
		icon: '"',
		command: (editor) => editor.chain().focus().toggleBlockquote().run(),
	},
	{
		title: "Code Block",
		description: "Capture a code snippet",
		icon: "{",
		command: (editor) => editor.chain().focus().toggleCodeBlock().run(),
	},
	{
		title: "Divider",
		description: "Visually divide blocks",
		icon: "―",
		command: (editor) => editor.chain().focus().setHorizontalRule().run(),
	},
];

// Компонент списка команд
interface CommandListProps {
	items: CommandItem[];
	command: (item: CommandItem) => void;
}

interface CommandListRef {
	onKeyDown: (props: { event: KeyboardEvent }) => boolean;
}

const CommandList = forwardRef<CommandListRef, CommandListProps>(
	({ items, command }, ref) => {
		const [selectedIndex, setSelectedIndex] = useState(0);

		useEffect(() => {
			setSelectedIndex(0);
		}, [items]);

		const selectItem = (index: number) => {
			const item = items[index];
			if (item) {
				command(item);
			}
		};

		useImperativeHandle(ref, () => ({
			onKeyDown: ({ event }: { event: KeyboardEvent }) => {
				if (event.key === "ArrowUp") {
					setSelectedIndex((selectedIndex + items.length - 1) % items.length);
					return true;
				}

				if (event.key === "ArrowDown") {
					setSelectedIndex((selectedIndex + 1) % items.length);
					return true;
				}

				if (event.key === "Enter") {
					selectItem(selectedIndex);
					return true;
				}

				return false;
			},
		}));

		return (
			<div className={styles.slashCommandsPopup}>
				{items.map((item, index) => (
					<button
						key={index}
						className={`${styles.commandItem} ${index === selectedIndex ? styles.selected : ""}`}
						onClick={() => selectItem(index)}
						title={`${item.title} — ${item.description}`}
						aria-label={item.title}
					>
						<span className={styles.commandIcon}>{item.icon}</span>
					</button>
				))}
			</div>
		);
	}
);

CommandList.displayName = "CommandList";

// Расширение для slash команд
const slashCommandsPluginKey = new PluginKey("slashCommands");

const SlashCommands = Extension.create({
	name: "slashCommands",

	addOptions() {
		return {
			suggestion: {
				char: "/",
				pluginKey: slashCommandsPluginKey,
				startOfLine: false,
				allow: ({ state, range }) => {
					const { $from } = state.selection;
					const parent = $from.parent;
					const start = $from.start();
					const offset = Math.max(0, range.from - start);
					const textBefore = parent.textBetween(0, offset, "\n", "\n");
					return textBefore.trim().length === 0;
				},
				command: ({ editor, range, props }: any) => {
					const { doc } = editor.state;
					let from = range.from;
					if (from > 0 && doc.textBetween(from - 1, from, "\n", "\n") === "/") {
						from -= 1;
					}
					editor.chain().focus().deleteRange({ from, to: range.to }).run();
					props.command(editor);
					editor.view.dispatch(editor.state.tr.setMeta(slashCommandsPluginKey, { action: "close" }));
				},
			} as Partial<SuggestionOptions>,
		};
	},

	addProseMirrorPlugins() {
		return [
			Suggestion({
				editor: this.editor,
				...this.options.suggestion,
				items: ({ query }: { query: string }) => {
					return commands.filter((item) =>
						item.title.toLowerCase().includes(query.toLowerCase())
					);
				},
				render: () => {
					let component: ReactRenderer<CommandListRef>;
					let popup: TippyInstance | null = null;
					const closePopup = () => {
						popup?.destroy();
						popup = null;
						component?.destroy();
					};
					const closeSuggestion = (editor: Editor) => {
						editor.view.dispatch(editor.state.tr.setMeta(slashCommandsPluginKey, { action: "close" }));
					};

					return {
						onStart: (props: SuggestionProps) => {
							if (!props.items?.length) {
								return;
							}
							component = new ReactRenderer(CommandList, {
								props: {
									...props,
									command: (item: CommandItem) => {
										props.command(item);
										closePopup();
										closeSuggestion(props.editor);
									},
								},
								editor: props.editor,
							});

							if (!props.clientRect) {
								return;
							}
							popup = tippy(document.body, {
								getReferenceClientRect: props.clientRect,
								appendTo: () => document.body,
								content: component.element,
								showOnCreate: true,
								interactive: true,
								trigger: "manual",
								placement: "bottom-start",
							});
						},

						onUpdate(props: SuggestionProps) {
							if (!props.items?.length) {
								closePopup();
								return;
							}
							component.updateProps({
								...props,
								command: (item: CommandItem) => {
									props.command(item);
									closePopup();
									closeSuggestion(props.editor);
								},
							});

							if (!props.clientRect || !popup) {
								return;
							}

							popup.setProps({
								getReferenceClientRect: props.clientRect,
							});
						},

						onKeyDown(props: any) {
							if (props.event.key === "Escape") {
								closePopup();
								return true;
							}

							return component.ref?.onKeyDown(props) ?? false;
						},

						onExit() {
							closePopup();
						},
					};
				},
			}),
		];
	},
});

const BlockIdAttributes = Extension.create({
	name: "blockIdAttributes",
	addGlobalAttributes() {
		return [
			{
				types: ["paragraph", "heading", "blockquote", "listItem", "codeBlock", "tableCell", "tableHeader"],
				attributes: {
					blockId: {
						default: null,
						parseHTML: element => element.getAttribute("data-block-id"),
						renderHTML: attributes => {
							return attributes.blockId ? { "data-block-id": attributes.blockId } : {};
						},
					},
				},
			},
		];
	},
});

export const NotionLikeEditor: React.FC<NotionLikeEditorProps> = ({
	content,
	onChange,
	editable = true,
	onCursorChange,
	placeholder,
	className = "",
}) => {
	const editorRef = useRef<Editor | null>(null);
	const isUpdatingRef = useRef(false);
	const lastCursorRef = useRef<{ blockId: string | null; position: number } | null>(null);

	const editor = useEditor({
		immediatelyRender: false,
		extensions: [
			BlockIdAttributes,
			StarterKit.configure({
				heading: {
					levels: [1, 2, 3, 4, 5, 6],
				},
			}),
			TextAlign.configure({
				types: ["heading", "paragraph"],
			}),
			Highlight.configure({
				multicolor: true,
			}),
			Typography,
			TextStyle,
			Color,
			Subscript,
			Superscript,
			HorizontalRule,
			Mathematics,
			Mention.configure({
				HTMLAttributes: {
					class: "mention",
				},
			}),
			Placeholder.configure({
				placeholder: ({ pos }) => {
					if (pos === 1) {
						return placeholder || "Type '/' for commands or start writing...";
					}

					return placeholder || 'Type / for commands';
				},
				showOnlyWhenEditable: false,
				showOnlyCurrent: true,
			}),
			SlashCommands
		],
		content,
		editable,
		editorProps: {
			attributes: {
				class: `${styles.notionEditor} ${className}`,
				spellcheck: "false",
			},
			handleKeyDown: (view, event) => {
				// Горячие клавиши
				if (event.ctrlKey || event.metaKey) {
					switch (event.key) {
						case "b":
							event.preventDefault();
							editor?.chain().focus().toggleBold().run();
							return true;
						case "i":
							event.preventDefault();
							editor?.chain().focus().toggleItalic().run();
							return true;
						case "u":
							event.preventDefault();
							editor?.chain().focus().toggleUnderline?.().run();
							return true;
						case "k":
							event.preventDefault();
							editor?.chain().focus().toggleCode().run();
							return true;
					}
				}

				// Shift + Enter для мягкого переноса
				if (event.key === "Enter" && event.shiftKey) {
					event.preventDefault();
					editor?.commands.setHardBreak();
					return true;
				}

				return false;
			},
		},
		onUpdate: ({ editor }) => {
			if (isUpdatingRef.current) return;

			const json = editor.getJSON();
			onChange(json);
		},
		onSelectionUpdate: ({ editor }) => {
			if (!onCursorChange) return;
			const { blockId, position } = getBlockIdAndPos(editor);
			const last = lastCursorRef.current;
			if (last && last.blockId === blockId && last.position === position) return;
			lastCursorRef.current = { blockId, position };
			onCursorChange(blockId, position);
		},
	});

	// Синхронизация внешних изменений
	useEffect(() => {
		if (!editor || !content) return;

		const currentContent = JSON.stringify(editor.getJSON());
		const newContent = JSON.stringify(content);

		if (currentContent !== newContent) {
			// Сохраняем текущую позицию курсора
			const { from, to } = editor.state.selection;

			isUpdatingRef.current = true;
			editor.commands.setContent(content);
			isUpdatingRef.current = false;
			
			try {
				const docSize = editor.state.doc.content.size;
				if (from <= docSize && to <= docSize) {
					editor.commands.setTextSelection({ from, to });
					
					if (onCursorChange) {
						const { blockId, position } = getBlockIdAndPos(editor);
						lastCursorRef.current = { blockId, position };
						onCursorChange(blockId, position);
					}
				}
			} catch (e) {
				console.warn('Could not restore cursor position', e);
			}
		}
	}, [content, editor]);

	function getBlockIdAndPos(editor: any) {
		const { $from } = editor.state.selection;
		// обходим вверх по глубинам в поисках node с attrs.blockId
		for (let depth = $from.depth; depth > 0; depth--) {
			const node = $from.node(depth);
			if (node?.attrs?.blockId || node?.attrs?.id) {
				const blockId: string = node.attrs.blockId ?? node.attrs.id;
				const blockStartPos: number = $from.start(depth); // абсолютная позиция начала ноды
				return { blockId, position: blockStartPos };
			}
		}
		// если не нашли — можно вернуть null или сгенерировать
		return { blockId: null, position: $from.pos };
	}

	// Обновление editable
	useEffect(() => {
		if (editor) {
			editor.setEditable(editable);
		}
	}, [editable, editor]);

	useEffect(() => {
		editorRef.current = editor;
		return () => {
			editor?.destroy();
		};
	}, [editor]);

	if (!editor) {
		return null;
	}

	return (
		<div className={styles.notionEditorWrapper}>
			<EditorContent editor={editor}/>
		</div>
	);
};
