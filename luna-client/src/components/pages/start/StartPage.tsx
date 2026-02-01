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
	}, [selectedWorkspaceId, router, dispatch]);

	const selectWorkspace = (selectedWorkspaceId: string) => {
		setSelectedWorkspace(selectedWorkspaceId);
	};

	const handleCreateWorkspace = () => {
		openModal(<CreateWorkspaceModal/>);
	};

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<div className={styles.header}>
					<h3>Select workspace</h3>
					<p>Choose where you want to work or create a new workspace.</p>
				</div>
				<div className={styles.cards}>
					<Card className={styles.createCard} onClick={handleCreateWorkspace}>
						<div className={styles.createIcon}>
							<Plus size={18} />
						</div>
						<p>Create workspace</p>
					</Card>
					{workspaces.map((workspace) => (
						<Card
							key={workspace.id}
							className={styles.workspaceCard}
							role="button"
							onClick={() => selectWorkspace(workspace.id)}
						>
							<div className={styles.workspaceName}>
								<h5 title={workspace.name}>{workspace.name}</h5>
								<p>{workspace.users.length} members</p>
							</div>
						</Card>
					))}
				</div>
			</div>
		</div>
	)
}