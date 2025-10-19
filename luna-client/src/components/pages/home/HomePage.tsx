'use client';

import React, { useCallback, useEffect, useState } from "react";
import styles from "./HomePage.module.scss";
import Card from "@/ui/card/Card";
import Input from "@/ui/input/Input";
import Button from "@/ui/button/Button";
import Image from "next/image";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { useRouter } from "next/navigation";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store/store";
import { getWorkspacePages } from "@/store/slices/pagesSlice";
import { usePages } from "@/store/hooks/usePages";
import { getPageStatistic } from "@/store/slices/workspaceSlice";
import { pageHttpProvider } from "@/http/pageHttpProvider";
import { LightPageView } from "@/models/page/view/LightPageView";
import { toast } from "react-toastify";
import { SearchPageBlockView } from "@/models/search/SearchPageBlockView";
import { PageCard } from "@/components/ui/pageCard/PageCard";
import { PageWithBlockCard } from "@/components/ui/pageCard/PageWithBlockCard";

export const HomePage: React.FC = () => {

	const { pages } = usePages();
	const { selectedWorkspaceId } = useWorkspaces();

	const [searchQuery, setSearchQuery] = useState("");
	const [searchedPages, setSearchedPages] = useState<LightPageView[]>([]);
	const [searchedPagesBlocks, setSearchedPagesBlocks] = useState<SearchPageBlockView[]>([]);

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

	const handleOnPageBlockClick = useCallback((pageId: string, blockId: string) => {
		router.push(`/${selectedWorkspaceId}/${pageId}?blockId=${blockId}`);
	}, [selectedWorkspaceId]);

	const handleCloseSearch = () => {
		setSearchedPages([]);
		setSearchQuery("");
	};

	const handleOnKeyDown = async (e: any) => {
		if (e.key === "Enter") {
			await handleSearchClick()
		}
	};

	const handleSearchClick = useCallback(async () => {
		if (selectedWorkspaceId == null) return;

		if (searchQuery == "") {
			setSearchedPages([]);
			setSearchedPagesBlocks([]);
			return;
		}

		const pages = await pageHttpProvider.searchPages(selectedWorkspaceId, searchQuery);
		const pagesBlocks = await pageHttpProvider.searchPagesBlocks(selectedWorkspaceId, searchQuery);

		setSearchedPages(pages);
		setSearchedPagesBlocks(pagesBlocks);

		if (pages.length == 0 && pagesBlocks.length == 0) {
			toast.info("No pages found");
		}

	}, [searchQuery])

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<h4>Search anything</h4>
				<Card className={styles.contentContainer} padding="small">
					<Input
						placeholder="search"
						value={searchQuery}
						onKeyDown={handleOnKeyDown}
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
							<div className={styles.searchHeader}>
								<p>Search result</p>
								<Button variant="ghost" onClick={handleCloseSearch}>
									<Image
										src="/icons/close_24.svg"
										alt="close"
										width={16}
										height={16}
									/>
								</Button>
							</div>
							<div className={styles.pagesCarousel}>
								{searchedPages.map((page) => (
									<PageCard
										key={`page-${page.id}`}
										onClick={handleOnPageClick}
										page={page}
									/>
								))}
							</div>
						</>
					)}
					{searchedPagesBlocks.length > 0 && (
						<>
							<div className={styles.searchHeader}>
								<p>Search result in blocks</p>
							</div>
							<div className={styles.pagesCarousel}>
								{searchedPagesBlocks.map((page) => (
									<PageWithBlockCard
										key={`${page.pageId}-${page.blockId}`}
										onClick={handleOnPageBlockClick}
										page={page}
									/>
								))}
							</div>
						</>
					)}
					<p>Recent pages</p>
					<div className={styles.pagesCarousel}>
						{pages.map((page) => (
							<PageCard
								key={`page-${page.id}`}
								onClick={handleOnPageClick}
								page={page}
							/>
						))}
					</div>
				</div>
			</div>
		</div>
	)
}