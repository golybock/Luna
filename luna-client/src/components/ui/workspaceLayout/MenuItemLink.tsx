import React from "react";
import Link from "next/link";
import styles from "./MenuItemLink.module.scss";
import Label from "@/ui/label/Label";
import Image from "next/image";
import { MenuItem } from "@/models/ui/MenuItem";

interface MenuItemLinkProps {
	item: MenuItem;
}

export const MenuItemLink: React.FC<MenuItemLinkProps> = ({ item }) => {

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
		<>
			{item.path ? (
				<Link className={styles.menuItem} href={item.path}>
					<Label icon={getImage()}>
						{item.name}
					</Label>
				</Link>
			) : (
				<div className={styles.menuItem} onClick={item.onClick} role="button">
					<Label icon={getImage()}>
						{item.name}
					</Label>
				</div>
			)}
		</>

	)
}