import { BlockType } from "@/types/pageEditor/blockType";

export interface SlashItem {
	id: string;
	label: string;
	type: BlockType;
	action: "transform" | "insert";
}