"use client";

import React, { useEffect } from "react";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { useRouter } from "next/navigation";
import styles from "./StartPage.module.scss";
import { useActions } from "@/store/hooks/useActions";
import Card from "@/ui/card/Card";
import { Plus } from "lucide-react";
import { AppDispatch } from "@/store/store";
import { useDispatch } from "react-redux";
import { useModal } from "@/layout/ModalContext";
import { CreateWorkspaceModal } from "@/components/modals/createWorkspace/CreateWorkspaceModal";
import { getAvailableWorkspaces } from "@/store/slices/workspaceSlice";

export const StartPage: React.FC = () => {

	const { selectedWorkspaceId, workspaces } = useWorkspaces();
	const { setSelectedWorkspace } = useActions();
	const { openModal } = useModal();

	const router = useRouter();
	const dispatch = useDispatch<AppDispatch>();

	useEffect(() => {
		if (selectedWorkspaceId != null) {
			router.push(`${selectedWorkspaceId}`);
		}

		dispatch(getAvailableWorkspaces());
	}, [selectedWorkspaceId]);

	const selectWorkspace = (selectedWorkspaceId: string) => {
		setSelectedWorkspace(selectedWorkspaceId);
	};

	const handleCreateWorkspace = () => {
		openModal(<CreateWorkspaceModal/>);
	};

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<h3>Select workspace</h3>
				<div className={styles.cards}>
					<Card className={styles.workspaceCard} onClick={handleCreateWorkspace}>
						<Plus color="gray"/>
					</Card>
					{workspaces.map((workspace) => (
						<Card
							key={workspace.id}
							className={styles.workspaceCard}
							role="button"
							onClick={() => selectWorkspace(workspace.id)}
						>
							<div className={styles.workspaceName}>
								<h5>{workspace.name}</h5>
								<p>Users: {workspace.users.length}</p>
							</div>
						</Card>
					))}
				</div>
			</div>
		</div>
	)
}