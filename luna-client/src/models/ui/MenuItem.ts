import { StaticImport } from "next/dist/shared/lib/get-img-props";

export interface MenuItem {
	name: string;
	emoji?: string | undefined;
	imagePath?: string | StaticImport | undefined;
	onClick?: () => void;
	path?: string;
}