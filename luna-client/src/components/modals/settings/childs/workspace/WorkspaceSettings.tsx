import React, { useEffect, useState } from "react";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import styles from "./WorkspaceSettings.module.scss"
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import Image from "next/image";
import { WorkspaceBlank } from "@/models/workspace/WorkspaceBlank";
import { WorkspaceDetailedView } from "@/models/workspace/WorkspaceDetailedView";
import Input from "@/ui/input/Input";
import Button from "@/ui/button/Button";
import { useModal } from "@/layout/ModalContext";
import { InviteUserModal } from "@/components/modals/InviteUser/InviteUserModal";
import { AvatarImage } from "@/components/ui/avatarImage/AvatarImage";
import { formatDate, formatLastActive, getInitials } from "@/tools/stringTools";
import { Loading } from "@/components/ui/loading/Loading";

export const WorkspaceSettings: React.FC = () => {

	const { selectedWorkspaceId } = useWorkspaces();
	const [workspace, setWorkspace] = useState<WorkspaceDetailedView>();
	const [workspaceBlank, setWorkspaceBlank] = useState<WorkspaceBlank | null>();
	const { openModal } = useModal();

	useEffect(() => {
		const getWorkspace = async () => {
			if (selectedWorkspaceId) {
				const workspace = await workspaceHttpProvider.getWorkspaceDetailed(selectedWorkspaceId);

				setWorkspace(workspace);
			}
		}

		getWorkspace();
	}, []);

	const handleStartEditing = () => {
		if (!workspaceBlank) {
			setWorkspaceBlank({ ...workspace });
		} else {
			setWorkspaceBlank(null);
		}
	};

	const handleCancelEditing = () => {
		setWorkspaceBlank(null);
	};

	const handleSaveEditing = async () => {
		if (workspaceBlank) {
			await workspaceHttpProvider.updateWorkspace(workspace.id, workspaceBlank);
			setWorkspaceBlank(null);
		}
	};

	const handleInviteClick = () => {
		openModal(<InviteUserModal/>)
	};

	if (!workspace) {
		return (<Loading/>)
	}

	return (
		<div className={styles.container}>
			<div className={styles.sectionHeader}>
				<h4>Workspace</h4>
				<p>Update basic details or manage members.</p>
			</div>
			<div className={styles.summaryCard}>
				<div className={styles.fields}>
					<Input
						label="Workspace name"
						maxLength={50}
						defaultValue={workspace.name}
						type="text"
						placeholder="Name"
						disabled={workspaceBlank == null}
						onChange={(e) => {
							if (workspaceBlank != null) {
								setWorkspaceBlank({ ...workspaceBlank, name: e.target.value });
							}
						}}
					/>
					<Input
						label="Description"
						maxLength={200}
						defaultValue={workspace.description}
						type="text"
						placeholder="Description"
						disabled={workspaceBlank == null}
						onChange={(e) => {
							if (workspaceBlank != null) {
								setWorkspaceBlank({ ...workspaceBlank, description: e.target.value });
							}
						}}
					/>
					<Input
						label="Users"
						defaultValue={(workspace.users.length + 1).toString()}
						type="number"
						placeholder="Users count"
						disabled={true}
					/>
				</div>
				<Button
					variant="secondary"
					className={styles.inviteButton}
					onClick={handleInviteClick}
					title="Invite members"
				>
					<Image
						src="/icons/plus_24.svg"
						width={18}
						height={18}
						alt="invite"
					/>
					Invite
				</Button>
				<div className={styles.workspaceActions}>
					{!workspaceBlank ? (
						<>
							<Button variant="ghost" onClick={handleStartEditing} title="Edit workspace">
								<Image
									src="/icons/edit_24.svg"
									width={20}
									height={20}
									alt="edit"
								/>
							</Button>
						</>
					) : (
						<>
							<Button variant="ghost" onClick={handleSaveEditing} title="Save changes">
								<Image
									src="/icons/check_24.svg"
									width={20}
									height={20}
									alt="save"
								/>
							</Button>
							<Button variant="ghost" onClick={handleCancelEditing} title="Cancel">
								<Image
									src="/icons/cancel_24.svg"
									width={20}
									height={20}
									alt="cancel"
								/>
							</Button>
						</>
					)}
				</div>
			</div>
			<div className={styles.tableContainer}>
				<div className={styles.tableHeader}>
					<h5>Members</h5>
					<p>Manage roles and access for this workspace.</p>
				</div>
				<table className={styles.table}>
					<thead>
					<tr>
						<th>
							<span>User</span>
						</th>
						<th>
							<span>Last active</span>
						</th>
						<th>
							<span>Joined</span>
						</th>
						<th>
							<span>Actions</span>
						</th>
					</tr>
					</thead>
					<tbody>
					{workspace.users.map((workspaceUser) => (
						<tr key={workspaceUser.id} className={styles.tableRow}>
							<td className={styles.userCell}>
								<div className={styles.userInfo}>
									<AvatarImage
										src={workspaceUser.user.image}
										alt={workspaceUser.user.displayName || workspaceUser.user.username}
										initials={getInitials(workspaceUser.user.displayName || workspaceUser.user.username)}
									/>
									<div className={styles.userDetails}>
										<div className={styles.userName}>
											{workspaceUser.user.displayName || workspaceUser.user.username}
										</div>
										<div className={styles.userEmail}>
											@{workspaceUser.user.username}
										</div>
										{workspaceUser.user.bio && (
											<div className={styles.userBio}>
												{workspaceUser.user.bio}
											</div>
										)}
									</div>
								</div>
							</td>
							<td>
								<span className={styles.lastActive}>
									{formatLastActive(workspaceUser.user.lastActive)}
								</span>
							</td>
							<td>
								<span className={styles.joinDate}>
									{formatDate(workspaceUser.acceptedAt)}
								</span>
							</td>
							<td>
								<div className={styles.actions}>
									<button className={styles.actionButton}>
										⚙️
									</button>
								</div>
							</td>
						</tr>
					))}
					</tbody>
				</table>
			</div>
			<Button variant="danger" className={styles.deleteButton}>
				Delete workspace
			</Button>
		</div>
	)
}