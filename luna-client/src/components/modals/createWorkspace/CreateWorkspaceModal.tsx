import React from "react";
import Modal from "@/components/ui/modal/Modal";
import { CreateWorkspaceBlock } from "@/components/pages/start/CreateWorkspaceBlock";

interface CreateWorkspaceModalProps {
	closeModal?: () => void;
}

export const CreateWorkspaceModal: React.FC<CreateWorkspaceModalProps> = ({ closeModal }) => {

	if(!closeModal){
		throw new Error("Modal doesn't exist");
	}

	return (
		<Modal closeModal={closeModal}>
			<CreateWorkspaceBlock onCreate={closeModal}/>
		</Modal>
	)
}