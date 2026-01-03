export interface PageVersionView{
	id: string;
	pageId: string;
	version: number;
	document?: any;
	createdAt: string;
	updatedAt: string;
	createdBy: string;
	changeDescription: string;
}