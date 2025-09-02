'use client';

import React, { useEffect, useState } from "react";
import styles from "./HomePage.module.scss";
import Card from "@/ui/card/Card";
import Input from "@/ui/input/Input";
import Button from "@/ui/button/Button";
import Image from "next/image";
import { IPageLightView } from "@/types/page/pageLightView";
import { pageHttpProvider } from "@/http/pageHttpProvider";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { EmojiPicker } from "@/ui/emojiPicker/EmpojiPicker";

export const HomePage : React.FC= () => {

	const [pages, setPages] = useState<IPageLightView[]>([]);
	const { selectedWorkspaceId } = useWorkspaces();

	useEffect(() => {
		const getPages = async () => {
			if (selectedWorkspaceId) {
				const pages = await pageHttpProvider.getWorkspacePages(selectedWorkspaceId);

				setPages(pages);
			}
		}

		getPages();
	}, [selectedWorkspaceId])

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<h4>Search anything</h4>
				<Card className={styles.contentContainer} padding="small">
					<Input placeholder="search"/>
					<Button variant="secondary" className={styles.searchButton}>
						<Image src="/icons/search_24.svg" alt="search" width={24} height={24} />
					</Button>
				</Card>
				<div className={styles.contentContainer}>
					<p>Workspace pages</p>
					<div className={styles.pagesCarousel}>
						{pages.map((page) => (
							<Card padding="small" key={page.id} className={styles.pageCard}>
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
						))}
					</div>
				</div>
			</div>
		</div>
	)
}