import {StaticImport} from "next/dist/shared/lib/get-img-props";

export interface IMenuItem {
	name: string;
	emoji?: string | undefined;
	imagePath?: string | StaticImport | undefined;
	onClick?: () => void;
	path?: string;
}