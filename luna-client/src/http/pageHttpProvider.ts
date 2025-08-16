import { HttpProviderBase } from "./httpProviderBase";
import { ICreatePageBlank } from "@/types/page/createPageBlank";
import { IPageLightView } from "@/types/page/pageLightView";

class PageHttpProvider extends HttpProviderBase {

	constructor() {
		super()
	}

	async getWorkspacePages(workspaceId: string): Promise<IPageLightView[]> {
		return this.get('/pages/getworkspacepages?workspaceId=' + workspaceId);
	}

	async createPage(createPageBlank: ICreatePageBlank): Promise<void> {
		return this.post('/pages/createpage', createPageBlank);
	}

	async deletePage(pageId: string): Promise<void> {
		return this.delete('/pages/deletepage?pageId=' + pageId);
	}
}

export const pageHttpProvider = new PageHttpProvider();