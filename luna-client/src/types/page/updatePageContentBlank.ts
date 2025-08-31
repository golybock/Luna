import { PageBlockBlank } from "@/types/page/pageBlockBlank";

export interface UpdatePageContentBlank{
	blocks: PageBlockBlank[];
	changeDescription: string;
}