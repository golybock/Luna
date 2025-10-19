import Card from "@/ui/card/Card";
import React, { useMemo } from "react";
import styles from "./PageCard.module.scss";
import { EmojiPicker } from "@/ui/emojiPicker/EmpojiPicker";
import Image from "next/image";
import { SearchPageBlockView } from "@/models/search/SearchPageBlockView";

interface PageWithBlockCardProps {
	page: SearchPageBlockView;
	onClick?: (pageId: string, blockId: string) => void;
}

export const PageWithBlockCard: React.FC<PageWithBlockCardProps> = ({page, onClick}) => {

	const pageView = useMemo(() => page.page, [page]);

	return (
		<Card
			padding="none"
			className={styles.pageCardWithBlock}
			onClick={() => onClick(page.pageId, page.blockId)}
		>
			{pageView.cover && (
				<Image
					src={pageView.cover}
					alt="cover"
					width={200}
					height={100}
				/>
			)}
			<div className={styles.pageCardWithBlockContent}>
				<div className={styles.pageCardWithBlockHeader}>
					{pageView.emoji && (
						<EmojiPicker
							value={pageView.emoji}
							disabled={true}
							className={styles.pageEmojiPicker}
						/>
					)}
					<h4>{pageView.title}</h4>
				</div>
				<div className={styles.searchResult}>
					<h6>Found in content: </h6>
					<p>{page.content}</p>
				</div>
			</div>
		</Card>
	)
}