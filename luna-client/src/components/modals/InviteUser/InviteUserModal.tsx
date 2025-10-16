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
	const [permission, setPermission] = useState<PermissionOption>(null);

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
			await navigator.clipboard.writeText(`http://localhost:3000/invite/inviteId?=${invite.inviteId}`);
			closeModal();
		}
		catch (e: any){
			console.error(e.message);
		}
	};

	const loadPermissionOptions = async (value: string) => {
		const permissions = await workspaceHttpProvider.getAvailableWorkspacePermissions();
		return permissions.map((item) => {
			return { value: item, label: item };
		})
	};

	const darkThemeStyles = {
		control: (base: any) => ({
			...base,
			backgroundColor: '#222222',
			borderColor: '#333',
			fontFamily: "Noto sans",
			fontWeight: 'bold',
			'&:hover': {
				borderColor: '#555'
			}
		}),
		menu: (base: any) => ({
			...base,
			backgroundColor: '#222222',
			border: '1px solid #333',
		}),
		option: (base: any, state: any) => ({
			...base,
			backgroundColor: state.isFocused ? '#333' : '#1a1a1a',
			color: "#928DAB",
			fontWeight: 'bold',
			fontFamily: "Noto sans",
			'&:hover': {
				backgroundColor: '#333'
			}
		}),
		multiValue: (base: any) => ({
			...base,
			backgroundColor: '#333'
		}),
		multiValueLabel: (base: any) => ({
			...base,
			color: "#928DAB",
		}),
		multiValueRemove: (base: any) => ({
			...base,
			color: '#999',
			'&:hover': {
				backgroundColor: '#555',
				color: "#928DAB",
			}
		}),
		input: (base: any) => ({
			...base,
			color: "#928DAB",
		}),
		placeholder: (base: any) => ({
			...base,
			color: "#928DAB",
		}),
		singleValue: (base: any) => ({
			...base,
			color: "#928DAB",
		})
	};

	return (
		<Modal closeModal={closeModal}>
			<div className={styles.container}>
				<Input
					id="email"
					type="email"
					placeholder="Email"
					value={email}
					onChange={handleEmailChange}
				/>
				<AsyncSelect
					id="permissions"
					styles={darkThemeStyles}
					value={permission}
					onChange={(selectedOption) => setPermission(selectedOption)}
					defaultOptions
					loadOptions={loadPermissionOptions}
					placeholder="Select permission"
				/>
				<Button variant="primary" onClick={handleInviteClick}>
					Send invite
				</Button>
			</div>
		</Modal>
	)
}