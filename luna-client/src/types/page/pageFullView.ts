import { PageView } from "@/types/page/pageView";
import { PageVersionView } from "@/types/page/pageVersionView";

export interface PageFullView{
	page: PageView;
	pageVersionView?: PageVersionView;
}