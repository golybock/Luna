import React, { useState } from "react";
import { InputGroup } from "react-bootstrap";
import { PrimaryButton } from "@/components/ui/button/PrimaryButton";
import { IWorkspaceBlank } from "@/types/workspace/IWorkspaceBlank";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store/store";
import { getAvailableWorkspaces } from "@/store/slices/workspaceSlice";
import styles from "./CreateWorkspaceBlock.module.scss";

export const CreateWorkspaceBlock: React.FC = () => {

	const [name, setName] = useState<string>('');
	const [description, setDescription] = useState<string>('');
	const dispatch = useDispatch<AppDispatch>();

	const createWorkspace = async () => {
		const workspaceBlank: IWorkspaceBlank = {
			name: name,
			description: description,
			visibility: "",
			defaultPermission: ""
		}

		await workspaceHttpProvider.createWorkspace(workspaceBlank);

		dispatch(getAvailableWorkspaces());
	}

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<h4>Workspace name</h4>
				<input
					id="name"
					className={styles.input}
					value={name}
					onChange={(e) => setName(e.target.value)}
				/>
				<h4>Description</h4>
				<input
					id="description"
					className={styles.input}
					value={description}
					onChange={(e) => setDescription(e.target.value)}
				/>
				<PrimaryButton onClick={createWorkspace} className={styles.input}>
					<p>Create</p>
				</PrimaryButton>
			</div>
		</div>
	)
}