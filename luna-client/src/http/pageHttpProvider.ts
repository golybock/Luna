import { HttpProviderBase } from "./httpProviderBase";
import { LightPageView } from "@/models/page/view/LightPageView";
import { CreatePageBlank } from "@/models/page/blank/CreatePageBlank";
import { PageStatisticView } from "@/models/page/PageStatisticView";

class PageHttpProvider extends HttpProviderBase {

	constructor() {
		super()
	}

	async getWorkspacePages(workspaceId: string): Promise<LightPageView[]> {
		return this.get('/pages/getworkspacepages?workspaceId=' + workspaceId);
	}

	async searchPagesByTitle(workspaceId: string, title: string): Promise<LightPageView[]> {
		return this.get('/pages/searchPagesByTitle?workspaceId=' + workspaceId + '&title=' + title);
	}

	async searchPages(workspaceId: string, query: string): Promise<LightPageView[]> {
		return this.get('/pages/searchPages?workspaceId=' + workspaceId + '&query=' + query);
	}

	async createPage(createPageBlank: CreatePageBlank): Promise<void> {
		return this.post('/pages/createpage', createPageBlank);
	}

	async deletePage(pageId: string): Promise<void> {
		return this.delete('/pages/deletepage?pageId=' + pageId);
	}

	async getPageStatistic(workspaceId: string): Promise<PageStatisticView> {
		return this.get(`/pages/getpagestatistic?workspaceId=${workspaceId}`);
	}
}

export const pageHttpProvider = new PageHttpProvider();