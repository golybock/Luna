"use client";

import React, { ChangeEvent, useCallback, useEffect, useMemo, useRef, useState } from "react";
import Card from "@/ui/card/Card";
import Image from "next/image";
import Input from "@/ui/input/Input";
import styles from "./Page.module.scss";
import { useModal } from "@/layout/ModalContext";
import { PageSettingsModal } from "@/components/modals/pageSettings/PageSettingsModal";
import Button from "@/ui/button/Button";
import { EmojiPicker } from "@/ui/emojiPicker/EmpojiPicker";
import { usePageWs } from "@/hooks/usePageWs";
import { TiptapEditor } from "@/components/editor/TiptapEditor";
import { AvatarImage } from "@/components/ui/avatarImage/AvatarImage";
import { getInitials } from "@/tools/stringTools";

interface PageProps {
	pageId: string;
	blockId?: string;
}

export const Page: React.FC<PageProps> = ({ pageId, blockId = undefined }) => {

	const { openModal } = useModal();
	const initialTimeRef = useRef<number>(Date.now());

	const {
		page,
		pageDocument,
		emoji,
		pageTitle,
		cover,
		description,
		cursors,
		users,
		error,
		setEmoji,
		status,
		setCursor,
		setPageTitle,
		setDescription,
		setCover,
		saveDocument,
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

	const editorData = useMemo(() => ({
		document: pageDocument,
		time: page?.pageVersionView?.updatedAt
			? new Date(page.pageVersionView.updatedAt).getTime()
			: initialTimeRef.current,
		version: page?.pageVersionView?.version
	}), [pageDocument, page?.pageVersionView?.updatedAt, page?.pageVersionView?.version]);

	const handleEditorChange = useCallback(async (newDocument: any) => {
		await saveDocument(newDocument);
	}, [saveDocument]);

	const handleCursorChange = useCallback(async (blockId: string, position: number) => {
		setCursor({ blockId, position });
	}, [setCursor]);

	const handleOpenSettings = () => {
		if (page) {
			openModal(<PageSettingsModal cover={cover} setPageCover={handleCoverChange}/>)
		}
	};

	if (error) return <div>Ошибка: {error.message}</div>;
	if (!page) return <div>Загрузка...</div>;

	return (
		<div className={styles.container}>

			<div className={styles.imageContainer} style={{ height: cover ? "auto" : "24px" }}>
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
							<div className={styles.additionalInfo}>
								<div className={styles.usersList}>
									{users.map((cursor) => (
										<AvatarImage
											key={cursor.id}
											src={cursor.image}
											alt={cursor.displayName || cursor.username || cursor.id}
											initials={getInitials(cursor.displayName || cursor.username || cursor.id)}
										/>
									))}
								</div>
							</div>
						</div>
					)}
				</Card>
				<TiptapEditor
					onChange={handleEditorChange}
					onCursorChange={handleCursorChange}
					cursors={cursors}
					data={editorData}
					scrollToBlockId={blockId}
				/>
				<div className={`${styles.statusBadge} ${styles[status.tone]}`}>
					<span className={styles.statusDot}/>
					{status.label && (
						<span className={styles.statusText}>{status.label}</span>
					)}
				</div>
			</div>
		</div>
	)
}