"use client";

import React, { useEffect, useState } from "react";
import Card from "@/ui/card/Card";
import { PageFullView } from "@/types/page/pageFullView";
import { PageWsProvider } from "@/http/pageWsProvider";
import Image from "next/image";
import Input from "@/ui/input/Input";
import styles from "./Page.module.scss";
import { useModal } from "@/layout/ModalContext";
import { PageSettingsModal } from "@/components/modals/pageSettings/PageSettingsModal";
import Button from "@/ui/button/Button";
import { EmojiPicker } from "@/ui/emojiPicker/EmpojiPicker";
import { PageEditor } from "@/components/ui/pageEditor/PageEditor";
import { EditorBlock } from "@/types/pageEditor/editorBlock";
import { PageBlockBlank } from "@/types/page/pageBlockBlank";

interface PageProps {
	pageId: string;
}

export const Page: React.FC<PageProps> = ({ pageId }) => {


	const [pageWsProvider, setPageWsProvider] = useState<PageWsProvider>(new PageWsProvider());

	const [page, setPage] = useState<PageFullView>();

	const { openModal } = useModal();

	const [emoji, setEmoji] = useState<string>();
	const [blocks, setBlocks] = useState<EditorBlock[]>([]);

	useEffect(() => {
		let isActive = true;

		const handlePageData = (data: PageFullView) => {
			if (!isActive) return;
			setPage(data);
			setBlocks(data?.pageVersionView?.content ?? []);
			setEmoji(data.page.emoji)
		};

		(async () => {
			try {
				await pageWsProvider.connect();
				await pageWsProvider.joinPage(pageId);
				pageWsProvider.onPageData(handlePageData);
				await pageWsProvider.getPageData(pageId);
			} catch (e) {
				console.error("Failed to init page ws", e);
			}
		})();

		return () => {
			isActive = false;
			pageWsProvider.leavePage(pageId).catch(() => void 0);
		};
	}, [pageId]);

	const handleOpenSettings = () => {
		if(page){
			openModal(<PageSettingsModal page={page.page}/>)
		}
	}

	const handleSaveBlocks = async (e: EditorBlock[]) => {
		setBlocks(e);

		const pageBlocks = e.map((item) => {
			return {...item} as PageBlockBlank;
		})

		await pageWsProvider.connect();
		await pageWsProvider.joinPage(pageId);

		try {
			await pageWsProvider.updatePageContent(pageId, {blocks: pageBlocks, changeDescription: "Update"})
			console.log("update page")
		}
		catch (e){
			console.error(e);
		}
		await pageWsProvider.leavePage(pageId);
	}

	return (
		<div className={styles.container}>
			{page?.page.cover && (
				<div className={styles.imageContainer}>
					<Image src={page.page.cover} alt={"cover"} width={1000} height={200}/>
				</div>
			)}
			<div className={styles.content}>
				<Card>
					{page && (
						<div className={styles.mainContent}>
							<div className={styles.mainContentData}>
								<div className={styles.title}>
									<div className={styles.emoji}>
										<EmojiPicker value={emoji} onChange={setEmoji} />
									</div>
									<Input value={page.page?.title}/>
								</div>
								<Input value={page.page?.description} placeholder={"Enter description here"}/>
							</div>
							<div className={styles.mainContentActions}>
								<Button variant="ghost" onClick={handleOpenSettings}>
									<Image
										src={"/icons/settings_24.svg"}
										alt="settings"
										width={24}
										height={24}
									/>
								</Button>
								<p>Ver: {page.pageVersionView?.version}</p>
							</div>
						</div>
					)}
				</Card>
				<PageEditor
					value={blocks}
					onChange={handleSaveBlocks}
				/>
			</div>
		</div>
	)
}