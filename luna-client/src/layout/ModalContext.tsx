import React, { createContext, useContext, useState, ReactNode } from 'react';
import ReactDOM from 'react-dom';
import { GlobalOverflow } from "@/tools/globalOverflow";

interface ModalContextProps {
	openModal: (content: ReactNode) => void;
	closeModal: () => void;
}

export interface ModalProps {
	closeModal: () => void;
}

const ModalContext = createContext<ModalContextProps | undefined>(undefined);

export const useModal = (): ModalContextProps => {
	const context = useContext(ModalContext);
	if (!context) {
		throw new Error('useModal must be used within a ModalProvider');
	}
	return context;
};

export const ModalProvider = ({ children }: { children: ReactNode }) => {
	const [modalContent, setModalContent] = useState<ReactNode | null>(null);

	const openModal = (content: ReactNode) => {
		GlobalOverflow.hide();
		setModalContent(content);
	};

	const closeModal = () => {

		// закрываем окно и показываем GlobalOverflow
		GlobalOverflow.show();
		setModalContent(null);

		// если у контента есть переданный метод closeModal выполняем и его
		if (React.isValidElement(modalContent)) {
			const { closeModal: closeModal } = modalContent.props as ModalProps;

			if (typeof closeModal === 'function') {
				closeModal();
			}
		}
	};

	return (
		<ModalContext.Provider value={{ openModal, closeModal }}>
			{children}
			{modalContent &&
				ReactDOM.createPortal(
					React.isValidElement(modalContent)
						? React.cloneElement(modalContent as React.ReactElement<ModalProps>, { closeModal: closeModal })
						: modalContent,
					document.body
				)}
		</ModalContext.Provider>
	);
};
