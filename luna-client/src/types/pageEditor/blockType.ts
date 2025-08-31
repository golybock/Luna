import { BlockTypes } from "@/types/pageEditor/blolckTypes";

export type BlockType = typeof BlockTypes[keyof typeof BlockTypes];