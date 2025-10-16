export interface CreatePageCommentBlank {
	pageId: string;
	content?: string;
	parentId?: string;
	blockId?: string;
	reactions?: object;
}