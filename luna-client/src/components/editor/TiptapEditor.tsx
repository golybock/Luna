"use client";

import React, { useMemo } from "react";
import { JSONContent } from "@tiptap/react";
import { UserCursorView } from "@/models/cursor/UserCursorView";
import { CursorOverlay } from "@/components/editor/addons/CursorOverlay";
import { NotionLikeEditor } from "@/components/editor/NotionLikeEditor";

const EMPTY_DOC = { type: "doc", content: [] as any[] };

type TiptapDoc = Record<string, any>;

interface EditorData {
	document?: TiptapDoc | null;
	version?: number;
	time?: number;
}

interface EditorProps {
	data: EditorData;
	onChange: (document: TiptapDoc) => void;
	scrollToBlockId?: string | null;
	cursors?: UserCursorView[];
	onCursorChange?: (blockId: string, position: number) => void;
}

const makeId = () => {
	if (typeof crypto !== "undefined" && crypto.randomUUID) return crypto.randomUUID();
	return Math.random().toString(36).slice(2);
};

const ensureBlockIds = (node?: JSONContent, usedIds: Set<string> = new Set()): JSONContent => {
	if (!node) return node ?? {};

	// Генерируем уникальный ID, проверяя что он не используется
	let blockId = (node.attrs as any)?.blockId;
	if (!blockId || usedIds.has(blockId)) {
		do {
			blockId = makeId();
		} while (usedIds.has(blockId));
	}
	usedIds.add(blockId);

	const newNode: JSONContent = {
		...node,
		attrs: {
			...(node.attrs ?? {}),
			blockId
		}
	};

	if (node.content) {
		newNode.content = node.content.map(child => ensureBlockIds(child as JSONContent, usedIds));
	}

	return newNode;
};

const ensureDoc = (doc?: TiptapDoc | null): JSONContent => {
	const base = (doc as JSONContent) ?? EMPTY_DOC;
	if (!base.type) base.type = "doc";
	if (!Array.isArray(base.content)) base.content = [];

	// Используем Set для отслеживания использованных ID в рамках всего документа
	const usedIds = new Set<string>();

	return {
		...base,
		content: (base.content ?? []).map((n: any) => ensureBlockIds(n, usedIds))
	};
};

export const TiptapEditor: React.FC<EditorProps> = ({
	data,
	onChange,
	scrollToBlockId,
	cursors,
	onCursorChange
}) => {

	const currentContent = useMemo(() => {
		return ensureDoc(data.document);
	}, [data.document]);

	const handleChange = (content: JSONContent) => {
		const json = ensureDoc(content);
		onChange(json as TiptapDoc);
	};

	return (
		<div>
			<NotionLikeEditor
				content={currentContent}
				onChange={handleChange}
				editable={true}
			/>
			{/*<CursorOverlay cursors={cursors}/>*/}
		</div>
	);
};

