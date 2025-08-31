import React from "react";
import { IMenuItem } from "@/types/IMenuItem";
import Link from "next/link";
import styles from "./MenuItem.module.scss";
import Label from "@/ui/label/Label";

interface PageMenuItemProps {
	item: IMenuItem;
}

export const PageMenuItem: React.FC<PageMenuItemProps> = ({ item }) => {

	return (
		<Link className={styles.menuItem} href={item.path!}>
			<Label icon={<p>{item.emoji}</p>}>
				{item.name}
			</Label>
		</Link>
	)
}