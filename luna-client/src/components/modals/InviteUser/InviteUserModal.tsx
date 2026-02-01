import React, { ChangeEvent, useState } from "react";
import Modal from "@/components/ui/modal/Modal";
import styles from "./InviteUserModal.module.scss";
import Input from "@/ui/input/Input";
import Button from "@/ui/button/Button";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { InviteUserBlank } from "@/models/invite/InviteUserBlank";
import AsyncSelect from "react-select/async";

interface InviteUserModalProps {
	closeModal?: () => void;
}

interface PermissionOption {
	value: string;
	label: string;
}

export const InviteUserModal: React.FC<InviteUserModalProps> = ({ closeModal }) => {

	const { selectedWorkspaceId } = useWorkspaces();

	const [email, setEmail] = useState("");
	const [permission, setPermission] = useState<PermissionOption | null>(null);

	if (!closeModal) {
		throw new Error("Modal doesn't have close method");
	}

	const handleEmailChange = (e: ChangeEvent<HTMLInputElement>) => {
		setEmail(e.target.value);
	};

	const handleInviteClick = async () => {

		const blank: InviteUserBlank = {
			workspaceId: selectedWorkspaceId,
			email: email,
			permissions: [permission.value]
		};

		try {
			const invite = await workspaceHttpProvider.inviteUserToWorkspace(blank);
			await navigator.clipboard.writeText(`http://localhost:3000/invite?inviteId=${invite.inviteId}`);
			closeModal();
		}
		catch (e: any){
			console.error(e.message);
		}
	};

	const loadPermissionOptions = async () => {
		const permissions = await workspaceHttpProvider.getAvailableWorkspacePermissions();
		return permissions.map((item) => {
			return { value: item, label: item };
		})
	};

	const darkThemeStyles = {
		control: (base: any) => ({
			...base,
			backgroundColor: 'var(--secondary-container)',
			borderColor: 'var(--teriary-container)',
			fontFamily: "Noto sans",
			fontWeight: '500',
			borderRadius: '6px',
			minHeight: '40px',
			'&:hover': {
				borderColor: 'var(--gray)'
			}
		}),
		menu: (base: any) => ({
			...base,
			backgroundColor: 'var(--secondary-container)',
			border: '1px solid var(--teriary-container)',
		}),
		option: (base: any, state: any) => ({
			...base,
			backgroundColor: state.isFocused ? 'var(--teriary-container)' : 'var(--secondary-container)',
			color: "var(--text)",
			fontWeight: '500',
			fontFamily: "Noto sans",
			'&:hover': {
				backgroundColor: 'var(--teriary-container)'
			}
		}),
		multiValue: (base: any) => ({
			...base,
			backgroundColor: 'var(--teriary-container)'
		}),
		multiValueLabel: (base: any) => ({
			...base,
			color: "var(--text)",
		}),
		multiValueRemove: (base: any) => ({
			...base,
			color: 'var(--gray)',
			'&:hover': {
				backgroundColor: 'var(--teriary-container)',
				color: "var(--text)",
			}
		}),
		input: (base: any) => ({
			...base,
			color: "var(--text)",
		}),
		placeholder: (base: any) => ({
			...base,
			color: "var(--gray)",
		}),
		singleValue: (base: any) => ({
			...base,
			color: "var(--text)",
		})
	};

	return (
		<Modal closeModal={closeModal} containerClassName={styles.modalContainer}>
			<div className={styles.container}>
				<div className={styles.header}>
					<h4>Invite member</h4>
					<p>Send an invite link to add someone to this workspace.</p>
					<p>User only with this email can join by link.</p>
				</div>
				<Input
					id="email"
					label="Email"
					type="email"
					placeholder="name@example.com"
					value={email}
					onChange={handleEmailChange}
				/>
				<div className={styles.selectLabel}>Permission</div>
				<AsyncSelect
					id="permissions"
					styles={darkThemeStyles}
					value={permission}
					onChange={(selectedOption) => setPermission(selectedOption)}
					defaultOptions
					loadOptions={loadPermissionOptions}
					placeholder="Select permission"
				/>
				<Button variant="primary" onClick={handleInviteClick} className={styles.submit}>
					Send invite
				</Button>
			</div>
		</Modal>
	)
}