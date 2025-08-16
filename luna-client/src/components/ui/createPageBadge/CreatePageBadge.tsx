import React, { useState } from "react";
import styles from "./CreatePageBadge.module.scss";
import { ICreatePageBlank } from "@/types/page/createPageBlank";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { pageHttpProvider } from "@/http/pageHttpProvider";
import { PrimaryButton } from "@/components/ui/button/PrimaryButton";


interface CreatePageBadgeProps {
	onCreated: () => void;
}

const CreatePageBadge: React.FC<CreatePageBadgeProps> = ({ onCreated }) => {

	const { selectedWorkspaceId } = useWorkspaces();

	const [title, setTitle] = useState("");
	const [emoji, setEmoji] = useState("📄");


	const onChangeTitle = (e: React.ChangeEvent<HTMLInputElement>) => {
		setTitle(e.target.value)
	};

	const onChangeEmoji = (e: React.ChangeEvent<HTMLInputElement>) => {
		setEmoji(e.target.value)
	};

	const handleCreatePage = async () => {

		const pageBlank: ICreatePageBlank = {
			workspaceId: selectedWorkspaceId!,
			title: title,
			emoji: emoji,
		}

		await pageHttpProvider.createPage(pageBlank);

		onCreated();
	}

	return (
		<div className={styles.container}>
			<input onChange={onChangeTitle} value={title}/>
			<PrimaryButton onClick={handleCreatePage}>+</PrimaryButton>
		</div>
	)
}

export default CreatePageBadge;