"use client";

import React, { ChangeEvent, useCallback } from "react";
import Card from "@/ui/card/Card";
import Image from "next/image";
import Input from "@/ui/input/Input";
import styles from "./Page.module.scss";
import { useModal } from "@/layout/ModalContext";
import { PageSettingsModal } from "@/components/modals/pageSettings/PageSettingsModal";
import Button from "@/ui/button/Button";
import { EmojiPicker } from "@/ui/emojiPicker/EmpojiPicker";
import { usePageWs } from "@/hooks/usePageWs";
import { Editor } from "@/components/editor/Editor";
import { PageBlockView } from "@/models/page/view/PageBlockView";
import { PageBlockBlank } from "@/models/page/blank/PageBlockBlank";

interface PageProps {
	pageId: string;
}

export const Page: React.FC<PageProps> = ({ pageId }) => {

	const { openModal } = useModal();

	const {
		page,
		blocks,
		emoji,
		pageTitle,
		cover,
		description,
		isConnecting,
		error,
		setEmoji,
		status,
		setPageTitle,
		setDescription,
		setCover,
		saveBlocks,
		savePageData,
	} = usePageWs(pageId, {
		autoConnect: true,
		autoFetchData: true,
		onConnected: () => console.log('Connected!'),
		onError: (err) => console.error('Connection error:', err)
	});

	const handleTitleChange = async (e: ChangeEvent<HTMLInputElement>) => {
		setPageTitle(e.target.value);
		await savePageData({ title: e.currentTarget.value });
	};

	const handleCoverChange = async (cover: string | null) => {
		setCover(cover);
		await savePageData({ cover: cover });
	};

	const handleDescriptionChange = async (e: ChangeEvent<HTMLInputElement>) => {
		setDescription(e.target.value);
		await savePageData({ description: e.currentTarget.value });
	};

	const handleEmojiChange = async (emoji: string) => {
		setEmoji(emoji);
		await savePageData({ emoji: emoji });
	};

	const handleEditorChange = useCallback(async (newBlocks: PageBlockBlank[] | PageBlockView[]) => {
		await saveBlocks(newBlocks);
	}, []);

	const handleOpenSettings = () => {
		if (page) {
			openModal(<PageSettingsModal cover={cover} setPageCover={handleCoverChange}/>)
		}
	};

	if (isConnecting) return <div>Подключение...</div>;
	if (error) return <div>Ошибка: {error.message}</div>;
	if (!page) return <div>Загрузка...</div>;

	return (
		<div className={styles.container}>

			{/*<div className={styles.status}>*/}
			{/*	<p>{status}</p>*/}
			{/*</div>*/}

			<div className={styles.imageContainer} style={{height: cover ? "auto" : "24px"}}>
				{cover && (
					<Image
						src={cover}
						alt="cover"
						width={2000}
						height={200}
					/>
				)}
				<div className={styles.changeImageButton} onClick={handleOpenSettings}>
					<Button variant="ghost" size="small">
						<p>Change cover</p>
					</Button>
				</div>
			</div>

			<div className={styles.content}>
				<Card className={styles.card}>
					{page && (
						<div className={styles.mainContent}>
							<div className={styles.mainContentData}>
								<div className={styles.title}>
									<div className={styles.emoji}>
										<EmojiPicker
											value={emoji}
											onChange={handleEmojiChange}
										/>
									</div>
									<Input
										value={pageTitle}
										onChange={handleTitleChange}
									/>
								</div>
								<div className={styles.description}>
									<Input
										value={description ?? ""}
										placeholder={"Enter description here"}
										onChange={handleDescriptionChange}
									/>
								</div>
							</div>
						</div>
					)}
				</Card>
				<Editor
					onChange={handleEditorChange}
					data={{
						blocks: blocks,
						time: Date.now(),
						version: page.pageVersionView?.version
					}}
				/>
			</div>
		</div>
	)
}