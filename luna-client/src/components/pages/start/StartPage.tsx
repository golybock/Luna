"use client";

import React, { useEffect } from "react";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { Spinner } from "@/components/ui/spinner/Spinner";
import { useRouter } from "next/navigation";
import styles from "./StartPage.module.scss";
import { CreateWorkspaceBlock } from "@/components/pages/start/CreateWorkspaceBlock";
import { useActions } from "@/store/hooks/useActions";
import Card from "@/ui/card/Card";
import { Car, Plus } from "lucide-react";
import { AppDispatch } from "@/store/store";
import { useDispatch } from "react-redux";
import { getAvailableWorkspaces } from "@/store/slices/workspaceSlice";
import { useModal } from "@/layout/ModalContext";
import { CreateWorkspaceModal } from "@/components/modals/createWorkspace/CreateWorkspaceModal";

export const StartPage: React.FC = () => {

	const { selectedWorkspaceId, workspaces, isFetchingWorkspaces } = useWorkspaces();
	const { setSelectedWorkspace } = useActions();
	const router = useRouter();
	const dispatch = useDispatch<AppDispatch>();

	const { openModal } = useModal();


	useEffect(() => {
		if (selectedWorkspaceId != null) {
			router.push(`${selectedWorkspaceId}`);
		}

		// dispatch(getAvailableWorkspaces());

	}, [selectedWorkspaceId]);

	if (isFetchingWorkspaces) {
		return <Spinner/>
	}

	const selectWorkspace = (selectedWorkspaceId: string) => {
		setSelectedWorkspace(selectedWorkspaceId);
	};

	const handleCreateWorkspace = () => {
		openModal(<CreateWorkspaceModal/>);
	}

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<h3>
					Select workspace
				</h3>
				<div className={styles.cards}>
					{workspaces.length && workspaces.map((workspace) => (
						<Card
							key={workspace.id}
							className={styles.workspaceCard}
							role="button"
							onClick={() => selectWorkspace(workspace.id)}
						>
							<div>
								<h5>{workspace.name}</h5>
								<h6>Users: {workspace.users.length + 1}</h6>
							</div>
						</Card>
					))}
					<Card className={styles.workspaceCard} onClick={handleCreateWorkspace}>
						<div>
							<Plus color="white"></Plus>
						</div>
					</Card>
				</div>
			</div>
		</div>
	)
}