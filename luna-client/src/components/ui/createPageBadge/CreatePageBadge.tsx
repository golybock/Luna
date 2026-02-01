import React from "react";
import styles from "./CreatePageBadge.module.scss";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { pageHttpProvider } from "@/http/pageHttpProvider";
import Button from "@/ui/button/Button";
import Image from "next/image";
import { CreatePageBlank } from "@/models/page/blank/CreatePageBlank";


const CreatePageBadge: React.FC = () => {

	const { selectedWorkspaceId } = useWorkspaces();

	const handleCreatePage = async () => {

		const pageBlank: CreatePageBlank = {
			workspaceId: selectedWorkspaceId!,
			title: "New page",
			emoji: "📄"
		}

		await pageHttpProvider.createPage(pageBlank);
	}

	return (
		<div className={styles.container}>
			<Button onClick={handleCreatePage} variant="ghost">
				<Image src={"/icons/plus_24.svg"} alt={"save"} width={16} height={16}/>
			</Button>
		</div>
	)
}

export default CreatePageBadge;