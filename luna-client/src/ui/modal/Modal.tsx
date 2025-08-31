import React, { ReactNode, useEffect } from "react";
import styles from "./Modal.module.scss";
import Button from "@/ui/button/Button";
import { X } from "lucide-react";

interface ModalProps {
	isOpen: boolean;
	onClose: () => void;
	title?: string;
	children: ReactNode;
	size?: ModalSize;
	footer?: ReactNode;
	className?: string;
}

const Modal: React.FC<ModalProps> = ({
	isOpen,
	onClose,
	title,
	children,
	size = 'medium',
	footer,
	className = ''
}) => {

	useEffect(() => {
		const handleEscape = (e: KeyboardEvent) => {
			if (e.key === 'Escape') {
				onClose();
			}
		};

		if (isOpen) {
			document.addEventListener('keydown', handleEscape);
			document.body.style.overflow = 'hidden';
		}

		return () => {
			document.removeEventListener('keydown', handleEscape);
			document.body.style.overflow = 'unset';
		};
	}, [isOpen, onClose]);

	if (!isOpen) return null;

	const modalClasses = [
		styles.modal,
		styles[size],
		className
	].filter(Boolean).join(' ');

	return (
		<div className={styles.overlay} onClick={(e) => e.target === e.currentTarget && onClose()}>
			<div className={modalClasses}>
				{title && (
					<div className={styles.header}>
						<h3 className={styles.title}>{title}</h3>
						<Button
							variant="ghost"
							size="small"
							onClick={onClose}
							icon={<X size={18} />}
						/>
					</div>
				)}
				<div className={styles.content}>
					{children}
				</div>
				{footer && (
					<div className={styles.footer}>
						{footer}
					</div>
				)}
			</div>
		</div>
	);
};

export default Modal;