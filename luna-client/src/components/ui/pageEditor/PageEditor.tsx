import React from "react";
import styles from "./PageEditor.module.scss";
import Card from "@/ui/card/Card";
import { usePageEditor } from "@/hooks/usePageEditor";
import { EditorBlock } from "@/types/pageEditor/editorBlock";

// Props for the PageEditor
interface PageEditorProps {
	value: EditorBlock[];
	onChange: (value: EditorBlock[]) => void;
	className?: string;
}

export const PageEditor: React.FC<PageEditorProps> = ({ value, onChange, className }) => {
	const {
		blocks,
		BlockTypes,
		editorRef,
		registerBlockRef,
		slashForId,
		slashPos,
		filteredItems,
		handleTextInput,
		handleKeyDown,
		handleTodoToggle,
		handleContainerActivate,
		handleSlashSelect,
		getTextContent,
	} = usePageEditor({ value, onChange });

	const renderTextEditable = (block: EditorBlock, opts?: { placeholder?: string; className?: string }) => {
		const refCallback = (el: HTMLDivElement | null) => {
			registerBlockRef(block.id as string, el);
		};

		return (
			<div
				key={block.id}
				ref={refCallback}
				className={[styles.textEditable, opts?.className].filter(Boolean).join(" ")}
				contentEditable
				dir="ltr"
				suppressContentEditableWarning
				data-placeholder={opts?.placeholder ?? "Enter text here"}
				onInput={(e) => handleTextInput(block.id as string, e)}
				onKeyDown={(e) => handleKeyDown(block, e)}
			>
				{getTextContent(block)}
			</div>
		);
	};

	const renderBlock = (block: EditorBlock): React.ReactNode => {
		switch (block.type) {
			case BlockTypes.H1:
				return (
					<div key={block.id} className={styles.blockRow}>
						{renderTextEditable(block, { className: styles.h1, placeholder: "Заголовок 1" })}
					</div>
				);
			case BlockTypes.H2:
				return (
					<div key={block.id} className={styles.blockRow}>
						{renderTextEditable(block, { className: styles.h2, placeholder: "Заголовок 2" })}
					</div>
				);
			case BlockTypes.H3:
				return (
					<div key={block.id} className={styles.blockRow}>
						{renderTextEditable(block, { className: styles.h3, placeholder: "Заголовок 3" })}
					</div>
				);
			case BlockTypes.Quote:
				return (
					<div key={block.id} className={styles.blockRow}>
						<div className={styles.quote}>
							{renderTextEditable(block, { placeholder: "Цитата" })}
						</div>
					</div>
				);
			case BlockTypes.Todo: {
				const content: any = block.content || {};
				const checked = !!content.checked;
				return (
					<div key={block.id} className={styles.blockRow}>
						<label className={styles.todo}>
							<input
								type="checkbox"
								checked={checked}
								onChange={(e) => handleTodoToggle(block.id as string, e.target.checked)}
							/>
							{renderTextEditable(block, { placeholder: "Задача" })}
						</label>
					</div>
				);
			}
			case BlockTypes.Divider:
				return (
					<div key={block.id} className={styles.blockRow}>
						<hr className={styles.divider}/>
					</div>
				);
			case BlockTypes.Image: {
				const url = (block.content as any)?.url as string | undefined;
				return (
					<div key={block.id} className={styles.blockRow}>
						{url ? (
							// eslint-disable-next-line @next/next/no-img-element
							<img src={url} alt="image" className={styles.image}/>
						) : (
							<div className={styles.imagePlaceholder}>Нет URL изображения</div>
						)}
					</div>
				);
			}
			case BlockTypes.Container: {
				const content: any = block.content || {};
				const children: EditorBlock[] = Array.isArray(content.children) ? content.children : [];
				return (
					<div key={block.id} className={styles.blockRow}>
						<Card className={styles.containerCard} padding="medium" bordered>
							<div className={styles.containerInner}>
								{children.length === 0 && (
									<div className={styles.emptyHint}
										 onClick={() => handleContainerActivate(block.id as string)}>
										Внутри контейнера можно печатать. Наберите // для выбора блока.
									</div>
								)}
								{children.map((ch) => (
									<React.Fragment key={ch.id}>
										{renderBlock(ch)}
									</React.Fragment>
								))}
							</div>
						</Card>
					</div>
				);
			}
			case BlockTypes.Paragraph:
			default:
				return (
					<div key={block.id} className={styles.blockRow}>
						{renderTextEditable(block, { placeholder: "Enter text here" })}
					</div>
				);
		}
	};

	return (
		<div className={[styles.editor, className].filter(Boolean).join(" ")} ref={editorRef}>
			{blocks.map((b) => (
				<div key={b.id} className={styles.blockWrapper}>
					{renderBlock(b)}
				</div>
			))}

			{slashForId && slashPos && (
				<div id="slash-menu" className={styles.slashMenu} style={{ top: slashPos.top, left: slashPos.left }}>
					{filteredItems.map((item) => (
						<div key={item.id} className={styles.menuItem} onClick={() => handleSlashSelect(item)}>
							{item.label}
						</div>
					))}
					{filteredItems.length === 0 && <div className={styles.menuEmpty}>Ничего не найдено</div>}
				</div>
			)}
		</div>
	);
};