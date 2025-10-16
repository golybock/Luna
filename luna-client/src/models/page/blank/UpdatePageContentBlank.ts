import { PageBlockBlank } from "@/models/page/blank/PageBlockBlank";

export interface UpdatePageContentBlank{
	blocks: PageBlockBlank[];
	changeDescription: string;
}