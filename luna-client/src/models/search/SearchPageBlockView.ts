import { LightPageView } from "@/models/page/view/LightPageView";

export interface SearchPageBlockView {
	blockId: string;
	pageId: string;
	type: string;
	content: string;
	page: LightPageView;
}