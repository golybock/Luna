import React, { useState } from "react";
import Modal from "@/components/ui/modal/Modal";
import styles from "./CreateWorkspaceModal.module.scss";
import Input from "@/ui/input/Input";
import { WorkspaceBlank } from "@/models/workspace/WorkspaceBlank";
import Button from "@/ui/button/Button";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";

interface CreateWorkspaceModalProps {
	closeModal?: () => void;
}

export const CreateWorkspaceModal: React.FC<CreateWorkspaceModalProps> = ({ closeModal }) => {

	if(!closeModal){
		throw new Error("Modal doesn't have close method");
	}

	const [name, setName] = useState<string>('');
	const [description, setDescription] = useState<string>('');

	const createWorkspace = async () => {
		const workspaceBlank: WorkspaceBlank = {
			name: name,
			description: description,
			defaultPermission: ""
		}

		await workspaceHttpProvider.createWorkspace(workspaceBlank);
		closeModal();
	}

	return (
		<Modal closeModal={closeModal}>
			<div className={styles.container}>
				<div className={styles.content}>
					<Input
						id="name"
						label="Workspace name"
						className={styles.input}
						value={name}
						onChange={(e) => setName(e.target.value)}
					/>
					<Input
						id="description"
						label="Description"
						className={styles.input}
						value={description}
						onChange={(e) => setDescription(e.target.value)}
					/>
					<Button onClick={createWorkspace} className={styles.input} variant="primary">
						<p>Create</p>
					</Button>
				</div>
			</div>
		</Modal>
	)
}