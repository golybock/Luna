import React, {  useState } from "react";
import Modal from "@/components/ui/modal/Modal";
import styles from "./PageSettingsModal.module.scss";
import { PageView } from "@/types/page/pageView";
import Input from "@/ui/input/Input";
import Button from "@/ui/button/Button";
import { PageWsProvider } from "@/http/pageWsProvider";

interface PageSettingsModalProps {
	closeModal?: () => void;
	page: PageView;
}

export const PageSettingsModal: React.FC<PageSettingsModalProps> = ({ closeModal, page }) => {

	if(!closeModal){
		throw new Error("Modal doesn't exist");
	}

	const [pageWsProvider, setPageWsProvider] = useState<PageWsProvider>(new PageWsProvider());

	const [cover, setCover] = useState<string | undefined>(page?.cover);

	const handleSave = async () => {

		try {
			await pageWsProvider.connect();
			await pageWsProvider.joinPage(page.id);
			await pageWsProvider.updatePage(page.id, {cover: cover});
			console.log("Saved");
			pageWsProvider.leavePage(page.id).catch(() => void 0);

			closeModal();
		}
		catch (e){
			console.error(e);
		}
	}

	return (
		<Modal closeModal={closeModal!}>
			<div className={styles.container}>
				<h2>Page settings</h2>
				<Input
					label="Cover url"
					value={cover}
					onChange={(e) => setCover(e.target.value)}
				/>
				<Button onClick={handleSave}>Save</Button>
			</div>
		</Modal>
	)
}