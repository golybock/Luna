import React from "react";
import Link from "next/link";
import styles from "./MenuItemLink.module.scss";
import Label from "@/ui/label/Label";
import { MenuItem } from "@/models/ui/MenuItem";

interface PageMenuItemProps {
	item: MenuItem;
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