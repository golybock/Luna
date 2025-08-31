import { PageBlockView } from "@/types/page/pageBlockView";
import { PageBlockBlank } from "@/types/page/pageBlockBlank";

export type EditorBlock = (PageBlockView | PageBlockBlank) & { id?: string };