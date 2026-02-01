import React from "react";
import Modal from "@/components/ui/modal/Modal";
import { ProfilePage } from "@/components/pages/profile/ProfilePage";
import styles from "./SettingsModal.module.scss";
import { Tabs } from "@/components/ui/tabs/Tabs";
import { Tab } from "@/components/ui/tabs/Tab";
import { WorkspaceSettings } from "@/components/modals/settings/childs/workspace/WorkspaceSettings";

interface SettingsModalProps {
	closeModal?: () => void;
}

export const SettingsModal: React.FC<SettingsModalProps> = ({ closeModal }) => {

	if(!closeModal){
		throw new Error("Modal doesn't have close method");
	}

	return (
		<Modal closeModal={closeModal} containerClassName={styles.modalContainer}>
			<div className={styles.container}>
				<div className={styles.header}>
					<h3>Settings</h3>
					<p>Manage your profile and workspace preferences.</p>
				</div>
				<Tabs defaultActiveTab={0}>
					<Tab title="Profile">
						<ProfilePage/>
					</Tab>
					<Tab title="Workspace">
						<WorkspaceSettings/>
					</Tab>
				</Tabs>
			</div>
		</Modal>
	)
}