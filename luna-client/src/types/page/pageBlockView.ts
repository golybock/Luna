export interface PageBlockView{
	id: string;
	type: string;
	content: object;
	parentId?: string;
	index: number;
	properties?: object;
}