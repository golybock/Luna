import React, { useState } from "react";
import Modal from "@/components/ui/modal/Modal";
import styles from "./PageSettingsModal.module.scss";
import Button from "@/ui/button/Button";
import Image from "next/image";

interface PageSettingsModalProps {
	closeModal?: () => void;
	setPageCover?: (coverUrl: string) => void;
	cover?: string;
}

export const PageSettingsModal: React.FC<PageSettingsModalProps> = ({ closeModal, cover, setPageCover }) => {

	if (!closeModal) {
		throw new Error("Modal doesn't exist");
	}

	const [innerCover, setInnerCover] = useState<string | undefined>(cover ?? "");

	const handleSave = async () => {
		try {
			setPageCover(innerCover);
			closeModal();
		} catch (e) {
			console.error(e);
		}
	}

	return (
		<Modal closeModal={closeModal} containerClassName={styles.modalContainer}>
			<div className={styles.container}>
				<h4>Page cover</h4>
				<div className={styles.previewBox}>
					{innerCover ? (
						<img
							src={innerCover}
							alt="cover"
							sizes="100vw"
							className={styles.previewImage}
						/>
					) : (
						<span className={styles.previewPlaceholder}>Preview</span>
					)}
				</div>
				<label className={styles.label}>Cover url</label>
				<textarea
					className={styles.textarea}
					placeholder="Enter URL"
					value={innerCover}
					onChange={(e) => setInnerCover(e.target.value)}
					rows={3}
				/>
				<Button onClick={handleSave} variant="secondary" className={styles.notionButton}>
					Save
				</Button>
			</div>
		</Modal>
	)
}