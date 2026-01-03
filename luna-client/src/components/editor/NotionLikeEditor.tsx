import React, { useEffect, useRef, useState, forwardRef, useImperativeHandle } from "react";
import { useEditor, EditorContent, JSONContent, Editor, ReactRenderer } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import { Table } from "@tiptap/extension-table";
import TableRow from "@tiptap/extension-table-row";
import TableCell from "@tiptap/extension-table-cell";
import TableHeader from "@tiptap/extension-table-header";
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
import styles from "./NotionLikeEditor.module.scss";

interface NotionLikeEditorProps {
	content: JSONContent;
	onChange: (content: JSONContent) => void;
	editable?: boolean;
	placeholder?: string;
	className?: string;
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
		title: "Table",
		description: "Insert a table",
		icon: "⊞",
		command: (editor) =>
			editor.chain().focus().insertTable({ rows: 3, cols: 3, withHeaderRow: true }).run(),
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
					>
						<span className={styles.commandIcon}>{item.icon}</span>
						<div className={styles.commandText}>
							<div className={styles.commandTitle}>{item.title}</div>
							<div className={styles.commandDescription}>{item.description}</div>
						</div>
					</button>
				))}
			</div>
		);
	}
);

CommandList.displayName = "CommandList";

// Расширение для slash команд
const SlashCommands = Extension.create({
	name: "slashCommands",

	addOptions() {
		return {
			suggestion: {
				char: "/",
				startOfLine: false,
				command: ({ editor, range, props }: any) => {
					props.command({ editor, range });
					editor.chain().focus().deleteRange(range).run();
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
					let popup: HTMLElement | null = null;

					return {
						onStart: (props: SuggestionProps) => {
							component = new ReactRenderer(CommandList, {
								props: {
									...props,
									command: (item: CommandItem) => {
										item.command(props.editor);
										props.editor.chain().focus().deleteRange(props.range).run();
									},
								},
								editor: props.editor,
							});

							if (!props.clientRect) {
								return;
							}

							popup = document.createElement("div");
							popup.style.position = "absolute";
							popup.style.zIndex = "1000";
							document.body.appendChild(popup);
							popup.appendChild(component.element);

							const rect = props.clientRect();
							if (rect) {
								popup.style.top = `${rect.bottom + window.scrollY}px`;
								popup.style.left = `${rect.left + window.scrollX}px`;
							}
						},

						onUpdate(props: SuggestionProps) {
							component.updateProps({
								...props,
								command: (item: CommandItem) => {
									item.command(props.editor);
									props.editor.chain().focus().deleteRange(props.range).run();
								},
							});

							if (!props.clientRect || !popup) {
								return;
							}

							const rect = props.clientRect();
							if (rect) {
								popup.style.top = `${rect.bottom + window.scrollY}px`;
								popup.style.left = `${rect.left + window.scrollX}px`;
							}
						},

						onKeyDown(props: any) {
							if (props.event.key === "Escape") {
								if (popup) {
									popup.remove();
								}
								return true;
							}

							return component.ref?.onKeyDown(props) ?? false;
						},

						onExit() {
							if (popup) {
								component.destroy();
								popup.remove();
							}
						},
					};
				},
			}),
		];
	},
});

export const NotionLikeEditor: React.FC<NotionLikeEditorProps> = ({
	content,
	onChange,
	editable = true,
	className = "",
}) => {
	const editorRef = useRef<Editor | null>(null);
	const isUpdatingRef = useRef(false);

	const editor = useEditor({
		immediatelyRender: false,
		extensions: [
			StarterKit.configure({
				heading: {
					levels: [1, 2, 3, 4, 5, 6],
				},
			}),
			Table.configure({
				resizable: true,
			}),
			TableRow,
			TableCell,
			TableHeader,
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
						return "Type '/' for commands or start writing...";
					}

					return 'Type / for commands';
				},
				showOnlyWhenEditable: false,
				showOnlyCurrent: false,
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

			// Восстанавливаем позицию курсора
			try {
				// Проверяем, что позиция всё ещё валидна
				const docSize = editor.state.doc.content.size;
				if (from <= docSize && to <= docSize) {
					editor.commands.setTextSelection({ from, to });
				}
			} catch (e) {
				// Если не удалось восстановить, ставим в конец
				console.warn('Could not restore cursor position', e);
			}
		}
	}, [content, editor]);

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
			<MenuBar editor={editor}/>
			<EditorContent editor={editor}/>
			<TableMenu editor={editor}/>
		</div>
	);
};

// Панель инструментов
interface MenuBarProps {
	editor: Editor;
}

const MenuBar: React.FC<MenuBarProps> = ({ editor }) => {
	if (!editor) return null;

	return (
		<div className={styles.menuBar}>
			<div className={styles.menuGroup}>
				<button
					onClick={() => editor.chain().focus().toggleBold().run()}
					className={editor.isActive("bold") ? styles.isActive : ""}
					title="Bold (Ctrl+B)"
				>
					<strong>B</strong>
				</button>
				<button
					onClick={() => editor.chain().focus().toggleItalic().run()}
					className={editor.isActive("italic") ? styles.isActive : ""}
					title="Italic (Ctrl+I)"
				>
					<em>I</em>
				</button>
				<button
					onClick={() => editor.chain().focus().toggleStrike().run()}
					className={editor.isActive("strike") ? styles.isActive : ""}
					title="Strikethrough"
				>
					<s>S</s>
				</button>
				<button
					onClick={() => editor.chain().focus().toggleCode().run()}
					className={editor.isActive("code") ? styles.isActive : ""}
					title="Code (Ctrl+K)"
				>
					{"</>"}
				</button>
				<button
					onClick={() => editor.chain().focus().toggleHighlight().run()}
					className={editor.isActive("highlight") ? styles.isActive : ""}
					title="Highlight"
				>
					H
				</button>
			</div>

			<div className={styles.menuGroup}>
				<button
					onClick={() => editor.chain().focus().toggleHeading({ level: 1 }).run()}
					className={editor.isActive("heading", { level: 1 }) ? styles.isActive : ""}
					title="Heading 1"
				>
					H1
				</button>
				<button
					onClick={() => editor.chain().focus().toggleHeading({ level: 2 }).run()}
					className={editor.isActive("heading", { level: 2 }) ? styles.isActive : ""}
					title="Heading 2"
				>
					H2
				</button>
				<button
					onClick={() => editor.chain().focus().toggleHeading({ level: 3 }).run()}
					className={editor.isActive("heading", { level: 3 }) ? styles.isActive : ""}
					title="Heading 3"
				>
					H3
				</button>
			</div>

			<div className={styles.menuGroup}>
				<button
					onClick={() => editor.chain().focus().toggleBulletList().run()}
					className={editor.isActive("bulletList") ? styles.isActive : ""}
					title="Bullet List"
				>
					• List
				</button>
				<button
					onClick={() => editor.chain().focus().toggleOrderedList().run()}
					className={editor.isActive("orderedList") ? styles.isActive : ""}
					title="Numbered List"
				>
					1. List
				</button>
				<button
					onClick={() => editor.chain().focus().toggleCodeBlock().run()}
					className={editor.isActive("codeBlock") ? styles.isActive : ""}
					title="Code Block"
				>
					{"{ }"}
				</button>
				<button
					onClick={() => editor.chain().focus().toggleBlockquote().run()}
					className={editor.isActive("blockquote") ? styles.isActive : ""}
					title="Quote"
				>
					" "
				</button>
			</div>

			<div className={styles.menuGroup}>
				<button
					onClick={() => editor.chain().focus().setTextAlign("left").run()}
					className={editor.isActive({ textAlign: "left" }) ? styles.isActive : ""}
					title="Align Left"
				>
					⬅
				</button>
				<button
					onClick={() => editor.chain().focus().setTextAlign("center").run()}
					className={editor.isActive({ textAlign: "center" }) ? styles.isActive : ""}
					title="Align Center"
				>
					↔
				</button>
				<button
					onClick={() => editor.chain().focus().setTextAlign("right").run()}
					className={editor.isActive({ textAlign: "right" }) ? styles.isActive : ""}
					title="Align Right"
				>
					➡
				</button>
			</div>

			<div className={styles.menuGroup}>
				<button
					onClick={() =>
						editor
							.chain()
							.focus()
							.insertTable({ rows: 3, cols: 3, withHeaderRow: true })
							.run()
					}
					title="Insert Table"
				>
					⊞ Table
				</button>
				<button
					onClick={() => editor.chain().focus().setHorizontalRule().run()}
					title="Horizontal Rule"
				>
					―
				</button>
			</div>

			<div className={styles.menuGroup}>
				<button
					onClick={() => editor.chain().focus().undo().run()}
					disabled={!editor.can().undo()}
					title="Undo (Ctrl+Z)"
				>
					↶
				</button>
				<button
					onClick={() => editor.chain().focus().redo().run()}
					disabled={!editor.can().redo()}
					title="Redo (Ctrl+Shift+Z)"
				>
					↷
				</button>
			</div>
		</div>
	);
};

// Меню для работы с таблицами
const TableMenu: React.FC<MenuBarProps> = ({ editor }) => {
	const isInTable = editor.isActive("table");

	if (!isInTable) return null;

	return (
		<div className={styles.tableMenu}>
			<button onClick={() => editor.chain().focus().addColumnBefore().run()}>
				+ Column Before
			</button>
			<button onClick={() => editor.chain().focus().addColumnAfter().run()}>
				+ Column After
			</button>
			<button onClick={() => editor.chain().focus().deleteColumn().run()}>
				− Column
			</button>
			<button onClick={() => editor.chain().focus().addRowBefore().run()}>
				+ Row Before
			</button>
			<button onClick={() => editor.chain().focus().addRowAfter().run()}>
				+ Row After
			</button>
			<button onClick={() => editor.chain().focus().deleteRow().run()}>
				− Row
			</button>
			<button onClick={() => editor.chain().focus().deleteTable().run()}>
				Delete Table
			</button>
			<button onClick={() => editor.chain().focus().mergeCells().run()}>
				Merge Cells
			</button>
			<button onClick={() => editor.chain().focus().splitCell().run()}>
				Split Cell
			</button>
		</div>
	);
};