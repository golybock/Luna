export interface PageView{
	id: string;
	title: string;
	description?: string;
	createdAt: string;
	updatedAt: string;
	workspaceId: string;
	latestVersion: number;
	ownerId: string;
	parentId?: string;
	icon?: string;
	cover?: string;
	emoji?: string;
	type: string;
	path?: string;
	index?: number;
	isTemplate: boolean;
	archivedAt?: string;
	pinned: boolean;
	customSlug?: string;
	properties?: object;
}