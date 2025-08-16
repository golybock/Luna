"use client";

import React, { useEffect } from "react";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { Spinner } from "@/components/ui/spinner/Spinner";
import { useRouter } from "next/navigation";
import styles from "./StartPage.module.scss";
import { CreateWorkspaceBlock } from "@/components/pages/start/CreateWorkspaceBlock";
import { useActions } from "@/store/hooks/useActions";

export const StartPage: React.FC = () => {

	const { selectedWorkspaceId, workspaces, isFetchingWorkspaces } = useWorkspaces();
	const { setSelectedWorkspace } = useActions();
	const router = useRouter();

	useEffect(() => {
		if (selectedWorkspaceId != null) {
			router.push(`${selectedWorkspaceId}`);
		}
	}, [selectedWorkspaceId]);

	if (isFetchingWorkspaces) {
		return <Spinner/>
	}

	const selectWorkspace = (selectedWorkspaceId: string) => {
		setSelectedWorkspace(selectedWorkspaceId);
	}

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<h2>
					{workspaces.length == 0 ? "Create workspace" : "Select workspace"}
				</h2>
				{workspaces.length && workspaces.map((workspace) => (
					<div
						key={workspace.id}
						className={styles.workspaceCard}
						role="button"
						onClick={() => selectWorkspace(workspace.id)}
					>
						<h4>{workspace.name}</h4>
						<h4>Users: {workspace.users.length + 1}</h4>
					</div>
				))}
				{workspaces.length === 0 && (
					<CreateWorkspaceBlock/>
				)}
			</div>
		</div>
	)
}