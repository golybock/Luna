import { PageBlockView } from "@/types/page/pageBlockView";

export interface PageVersionView{
	id: string;
	pageId: string;
	version: number;
	content: PageBlockView[];
	createdAt: string;
	updatedAt: string;
	createdBy: string;
	changeDescription: string;
}