import React from "react";
import { IMenuItem } from "@/types/IMenuItem";
import Link from "next/link";
import styles from "./MenuItem.module.scss";
import Label from "@/ui/label/Label";
import Image from "next/image";

interface MenuItemProps {
	item: IMenuItem;
}

export const MenuItem: React.FC<MenuItemProps> = ({ item }) => {

	const getImage = () => {
		return (
			<Image
				src={item.imagePath!}
				width={24}
				height={24}
				alt={item.name}
			/>
		)
	}

	return (
		<Link className={styles.menuItem} onClick={item.onClick} href={item.path ?? "/"}>
			<Label icon={getImage()}>
				{item.name}
			</Label>
		</Link>
	)
}