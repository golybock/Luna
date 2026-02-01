"use client";

import React, { useEffect, useState } from "react";
import { WorkspaceView } from "@/models/workspace/WorkspaceView";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import Image from "next/image";
import Card from "@/ui/card/Card";
import styles from "./InvitePage.module.scss";
import Button from "@/ui/button/Button";
import { useRouter } from "next/navigation";

interface InvitePageProps {
	inviteId: string;
}

export const InvitePage : React.FC<InvitePageProps> = ({inviteId}) => {

	const [workspace, setWorkspace] = useState<WorkspaceView>();
	const router = useRouter();

	useEffect(() => {
		const getWorkspace = async () => {
			if(inviteId != null){
				const workspace = await workspaceHttpProvider.getWorkspaceByInvite(inviteId);
				setWorkspace(workspace);
			}
		}

		getWorkspace();
	}, [inviteId]);

	const handleAcceptInviteClick = async () => {
		try {
			await workspaceHttpProvider.acceptInvite(inviteId);
			router.push("/start");
		}
		catch (e: any){
			console.error(e.message);
		}
	}

	const handleBackToHomeClick = () => {
		router.push("/start");
	}

	return (
		<div className={styles.container}>
			{workspace && (
				<div className={styles.content}>
					<h2>You invited in workspace</h2>
					<Card padding="small" className={styles.contentCard}>
						<h3>Name: {workspace.name}</h3>
						{workspace.icon && (
							<Image
								src={workspace.icon}
								alt="icon"
								width={48}
								height={48}
							/>
						)}
						<p>Users count: {workspace.users.length}</p>
						<Button variant="primary" onClick={handleAcceptInviteClick}>
							Accept invite
						</Button>
						<Button variant="secondary" onClick={handleBackToHomeClick}>
							Back to home
						</Button>
					</Card>
				</div>
			)}
		</div>
	)
}