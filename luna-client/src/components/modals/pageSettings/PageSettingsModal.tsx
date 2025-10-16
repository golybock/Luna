import React, { useState } from "react";
import Modal from "@/components/ui/modal/Modal";
import styles from "./PageSettingsModal.module.scss";
import Input from "@/ui/input/Input";
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
		<Modal closeModal={closeModal}>
			<div className={styles.container}>
				<h4>Page cover</h4>
				<Input
					label="Cover url"
					type="url"
					placeholder="Enter URL"
					value={innerCover}
					onChange={(e) => setInnerCover(e.target.value)}
				/>
				{innerCover && (
					<div>
						<h5>Preview</h5>
						<Image src={innerCover} alt="cover" width={250} height={50} />
					</div>
				)}
				<Button onClick={handleSave} variant="primary">
					Save
				</Button>
			</div>
		</Modal>
	)
}