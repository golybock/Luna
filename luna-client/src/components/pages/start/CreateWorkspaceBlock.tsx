import React, { useState } from "react";
import { IWorkspaceBlank } from "@/types/workspace/IWorkspaceBlank";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store/store";
import { getAvailableWorkspaces } from "@/store/slices/workspaceSlice";
import styles from "./CreateWorkspaceBlock.module.scss";
import Button from "@/ui/button/Button";
import Input from "@/ui/input/Input";

interface CreateWorkspaceBlockProps {
	onCreate: () => void;
}

export const CreateWorkspaceBlock: React.FC<CreateWorkspaceBlockProps> = ({ onCreate }) => {

	const [name, setName] = useState<string>('');
	const [description, setDescription] = useState<string>('');

	const createWorkspace = async () => {
		const workspaceBlank: IWorkspaceBlank = {
			name: name,
			description: description,
			visibility: "",
			defaultPermission: ""
		}

		await workspaceHttpProvider.createWorkspace(workspaceBlank);

		onCreate();
	}

	return (
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
	)
}