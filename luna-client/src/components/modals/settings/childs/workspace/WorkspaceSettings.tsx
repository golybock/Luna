import React, { useEffect, useState } from "react";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import styles from "./WorkspaceSettings.module.scss"
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import Image from "next/image";
import { WorkspaceUserBlank } from "@/models/workspace/WorkspaceUserBlank";
import { WorkspaceBlank } from "@/models/workspace/WorkspaceBlank";
import { WorkspaceDetailedView } from "@/models/workspace/WorkspaceDetailedView";
import Input from "@/ui/input/Input";
import Button from "@/ui/button/Button";
import { useModal } from "@/layout/ModalContext";
import { InviteUserModal } from "@/components/modals/InviteUser/InviteUserModal";

export const WorkspaceSettings: React.FC = () => {

	const { selectedWorkspaceId } = useWorkspaces();
	const [workspace, setWorkspace] = useState<WorkspaceDetailedView>();
	const [workspaceBlank, setWorkspaceBlank] = useState<WorkspaceBlank | null>();
	const [editableUserWorkspace, setEditableUserWorkspace] = useState<WorkspaceUserBlank | null>();
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

	const formatDate = (dateString?: string) => {
		if (!dateString) return 'Never';
		return new Date(dateString).toLocaleDateString('ru-RU', {
			day: '2-digit',
			month: '2-digit',
			year: 'numeric'
		});
	};

	if (!workspace) {
		return (
			<div className={styles.container}>
				<div className={styles.loading}>
					<h2>Loading...</h2>
				</div>
			</div>
		)
	}

	const formatLastActive = (lastActive: Date) => {
		const now = new Date();
		const diff = now.getTime() - new Date(lastActive).getTime();
		const days = Math.floor(diff / (1000 * 60 * 60 * 24));

		if (days === 0) return 'Сегодня';
		if (days === 1) return 'Вчера';
		if (days < 7) return `${days} дн. назад`;
		return new Date(lastActive).toLocaleDateString('ru-RU');
	};

	const getInitials = (displayName?: string, username?: string) => {
		const name = displayName || username || '';
		return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
	};

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

	return (
		<div className={styles.container}>
			<div className="row">
				<Input
					name="Name"
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
					name="Description"
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
					name="Users count"
					defaultValue={(workspace.users.length + 1).toString()}
					type="number"
					placeholder="Users count"
					disabled={true}
				/>
				<div className={styles.workspaceActions}>
					{!workspaceBlank ? (
						<>
							<Button variant="ghost" onClick={handleStartEditing}>
								<Image
									src="/icons/edit_24.svg"
									width={24}
									height={24}
									alt="edit"
								/>
							</Button>
							<Button variant="ghost">
								<Image
									src="/icons/trash_24.svg"
									width={24}
									height={24}
									alt="delete"
								/>
							</Button>
							<Button variant="ghost" onClick={handleInviteClick}>
								<Image
									src="/icons/plus_24.svg"
									width={24}
									height={24}
									alt="invite"
								/>
							</Button>
						</>
					) : (
						<>
							<button onClick={handleSaveEditing}>
								<Image
									src="/icons/check_24.svg"
									width={24}
									height={24}
									alt="save"
								/>
							</button>
							<button onClick={handleCancelEditing}>
								<Image
									src="/icons/cancel_24.svg"
									width={24}
									height={24}
									alt="cancel"
								/>
							</button>
						</>
					)}
				</div>
			</div>
			<div className={styles.tableContainer}>
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
									<div className={styles.avatar}>
										{workspaceUser.user.image ? (
											<img
												src={workspaceUser.user.image}
												alt={workspaceUser.user.displayName || workspaceUser.user.username}
												className={styles.avatarImage}
											/>
										) : (
											<div className={styles.avatarPlaceholder}>
												{getInitials(workspaceUser.user.displayName, workspaceUser.user.username)}
											</div>
										)}
									</div>
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
		</div>
	)
}