import React, { useState } from "react";
import Modal from "@/components/ui/modal/Modal";
import styles from "./CreateWorkspaceModal.module.scss";
import Input from "@/ui/input/Input";
import { WorkspaceBlank } from "@/models/workspace/WorkspaceBlank";
import Button from "@/ui/button/Button";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store/store";
import { getAvailableWorkspaces } from "@/store/slices/workspaceSlice";

interface CreateWorkspaceModalProps {
	closeModal?: () => void;
}

export const CreateWorkspaceModal: React.FC<CreateWorkspaceModalProps> = ({ closeModal }) => {

	if(!closeModal){
		throw new Error("Modal doesn't have close method");
	}

	const [name, setName] = useState<string>('');
	const [description, setDescription] = useState<string>('');
	const dispatch = useDispatch<AppDispatch>();

	const createWorkspace = async () => {
		const workspaceBlank: WorkspaceBlank = {
			name: name,
			description: description,
			defaultPermission: ""
		}

		await workspaceHttpProvider.createWorkspace(workspaceBlank);
		await dispatch(getAvailableWorkspaces());
		closeModal();
	}

	return (
		<Modal closeModal={closeModal} containerClassName={styles.modalContainer}>
			<div className={styles.container}>
				<div className={styles.header}>
					<h4>Create workspace</h4>
					<p>Set a name and optional description for your workspace.</p>
				</div>
				<div className={styles.content}>
					<Input
						id="name"
						label="Workspace name"
						className={styles.input}
						value={name}
						onChange={(e) => setName(e.target.value)}
					/>
					<label className={styles.label} htmlFor="description">
						Description
					</label>
					<textarea
						id="description"
						className={styles.textarea}
						placeholder="Optional"
						value={description}
						onChange={(e) => setDescription(e.target.value)}
						rows={3}
					/>
					<Button onClick={createWorkspace} className={styles.submit} variant="primary">
						Create workspace
					</Button>
				</div>
			</div>
		</Modal>
	)
}