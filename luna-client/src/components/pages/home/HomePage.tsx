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
	const [isCreating, setIsCreating] = useState(false);

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

	const handleCreatePage = useCallback(async () => {
		if (!selectedWorkspaceId || isCreating) return;
		setIsCreating(true);
		try {
			await pageHttpProvider.createPage({
				workspaceId: selectedWorkspaceId,
				title: "New page",
				emoji: "📄"
			});
			await dispatch(getWorkspacePages(selectedWorkspaceId));
			await dispatch(getPageStatistic(selectedWorkspaceId));
		} finally {
			setIsCreating(false);
		}
	}, [selectedWorkspaceId, isCreating, dispatch]);

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
				<div className={styles.header}>
					<h3>Home</h3>
					<p>Find pages fast or create a new one.</p>
				</div>

				{pages.length === 0 ? (
					<Card className={styles.emptyCard} bordered hover onClick={handleCreatePage}>
						<div className={styles.emptyIcon}>
							<Image src="/icons/plus_24.svg" alt="create" width={20} height={20}/>
						</div>
						<div className={styles.emptyText}>
							<h4>Create your first page</h4>
							<p>Start writing and organize your workspace.</p>
						</div>
						<Button variant="primary" className={styles.emptyButton} disabled={isCreating}>
							{isCreating ? "Creating..." : "Create page"}
						</Button>
					</Card>
				) : (
					<>
						<h4 className={styles.sectionTitle}>Search</h4>
						<Card className={styles.searchCard} padding="small">
							<div className={styles.searchInputWrap}>
								<Input
									placeholder="Search pages, blocks, or keywords"
									value={searchQuery}
									onKeyDown={handleOnKeyDown}
									onChange={(e) => setSearchQuery(e.target.value)}
								/>
							</div>
							<Button
								variant="secondary"
								className={styles.searchButton}
								onClick={handleSearchClick}
							>
								<Image
									src="/icons/search_24.svg"
									alt="search"
									width={20}
									height={20}
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
							<p className={styles.sectionTitle}>Recent pages</p>
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
					</>
				)}
			</div>
		</div>
	)
}