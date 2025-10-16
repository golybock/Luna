import { PageView } from "@/models/page/view/PageView";
import { PageVersionView } from "@/models/page/view/PageVersionView";

export interface PageFullView{
	page: PageView;
	pageVersionView?: PageVersionView;
}