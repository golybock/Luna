import Card from "@/ui/card/Card";
import React from "react";
import styles from "./PageCard.module.scss";
import { LightPageView } from "@/models/page/view/LightPageView";
import { EmojiPicker } from "@/ui/emojiPicker/EmpojiPicker";
import Image from "next/image";

interface PageCardProps {
	page: LightPageView;
	onClick?: (pageId: string) => void;
}

export const PageCard: React.FC<PageCardProps> = ({page, onClick}) => {
	return (
		<Card
			padding="none"
			className={styles.pageCard}
			onClick={() => onClick(page.id)}
		>
			{page.cover && (
				<Image
					src={page.cover}
					alt="cover"
					width={150}
					height={75}
				/>
			)}
			<div className={styles.pageCardContent}>
				{page.emoji && (
					<EmojiPicker
						value={page.emoji}
						disabled={true}
						className={styles.pageEmojiPicker}
					/>
				)}
				<h4>{page.title}</h4>
			</div>
		</Card>
	)
}