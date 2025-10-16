'use client';

import React, { useCallback, useEffect, useState } from "react";
import styles from "./HomePage.module.scss";
import Card from "@/ui/card/Card";
import Input from "@/ui/input/Input";
import Button from "@/ui/button/Button";
import Image from "next/image";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { EmojiPicker } from "@/ui/emojiPicker/EmpojiPicker";
import { useRouter } from "next/navigation";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store/store";
import { getWorkspacePages } from "@/store/slices/pagesSlice";
import { usePages } from "@/store/hooks/usePages";
import { getPageStatistic } from "@/store/slices/workspaceSlice";
import { pageHttpProvider } from "@/http/pageHttpProvider";
import { LightPageView } from "@/models/page/view/LightPageView";

export const HomePage: React.FC = () => {

	const { pages } = usePages();
	const { selectedWorkspaceId, selectedWorkspacePageStatistic } = useWorkspaces();

	const [searchQuery, setSearchQuery] = useState("");
	const [searchedPages, setSearchedPages] = useState<LightPageView[]>([]);

	const router = useRouter();
	const dispatch = useDispatch<AppDispatch>();

	useEffect(() => {
		if (selectedWorkspaceId) {
			dispatch(getWorkspacePages(selectedWorkspaceId));
			dispatch(getPageStatistic(selectedWorkspaceId));
		}
	}, [selectedWorkspaceId])

	const handleOnPageClick = useCallback((pageId: string) => {
		router.push(`/${selectedWorkspaceId}/${pageId}`);
	}, [selectedWorkspaceId]);

	const handleSearchClick = useCallback(async () => {
		if(selectedWorkspaceId == null) return;

		if(searchQuery == ""){
			setSearchedPages([]);
			return;
		}

		const pages = await pageHttpProvider.searchPages(selectedWorkspaceId, searchQuery);
		setSearchedPages(pages);
	}, [searchQuery])

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<h4>Search anything</h4>
				<Card className={styles.contentContainer} padding="small">
					<Input
						placeholder="search"
						value={searchQuery}
						onChange={(e) => setSearchQuery(e.target.value)}
					/>
					<Button
						variant="secondary"
						className={styles.searchButton}
						onClick={handleSearchClick}
					>
						<Image
							src="/icons/search_24.svg"
							alt="search"
							width={24}
							height={24}
						/>
					</Button>
				</Card>
				<div className={styles.contentContainer}>
					{searchedPages.length > 0 && (
						<>
							<p>Search result</p>
							<div className={styles.pagesCarousel}>
								{searchedPages.map((page) => (
									<Card
										padding="none"
										key={page.id}
										className={styles.pageCard}
										onClick={() => handleOnPageClick(page.id)}
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
								))}
							</div>
						</>
					)}
					<p>Recent pages</p>
					<div className={styles.pagesCarousel}>
						{pages.map((page) => (
							<Card
								padding="none"
								key={page.id}
								className={styles.pageCard}
								onClick={() => handleOnPageClick(page.id)}
							>
								{page.cover && (
									<Image src={page.cover} alt={"cover"} width={150} height={75}/>
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
						))}
					</div>
				</div>
				{selectedWorkspacePageStatistic && (
					<Card>
						<div>
							<h5>Statistic</h5>
							<div className="col">
								<p>Total pages: {selectedWorkspacePageStatistic.totalPages}</p>
								<p>Archived pages: {selectedWorkspacePageStatistic.archivedPages}</p>
								<p>Pages in trash: {selectedWorkspacePageStatistic.deletedPages}</p>
								<p>Pinned pages: {selectedWorkspacePageStatistic.pinnedPages}</p>
								<p>Templates: {selectedWorkspacePageStatistic.templates}</p>
							</div>
						</div>
					</Card>
				)}
			</div>
		</div>
	)
}