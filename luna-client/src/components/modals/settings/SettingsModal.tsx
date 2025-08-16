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
	return (
		<Modal closeModal={closeModal!}>
			<div className={styles.container}>
				<h1>Settings</h1>
				<Tabs defaultActiveTab={0}>
					<Tab title="Profile">
						<ProfilePage/>
					</Tab>
					<Tab title="Workspace">
						<WorkspaceSettings/>
					</Tab>
					<Tab title="User settings">
						user settings
					</Tab>
				</Tabs>
			</div>
		</Modal>
	)
}