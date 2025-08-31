import { AlertCircle, Check, Info, X } from "lucide-react";
import React, { ReactNode, useEffect } from "react";
import styles from "./Toast.module.scss";

interface ToastProps {
	message: string;
	type?: ToastType;
	isVisible: boolean;
	onClose: () => void;
	autoClose?: boolean;
}

const Toast: React.FC<ToastProps> = ({ message, type = 'info', isVisible, onClose, autoClose = true }) => {
	useEffect(() => {
		if (isVisible && autoClose) {
			const timer = setTimeout(() => {
				onClose();
			}, 5000);
			return () => clearTimeout(timer);
		}
	}, [isVisible, autoClose, onClose]);

	const getIcon = (): ReactNode => {
		switch (type) {
			case 'success':
				return <Check size={20}/>;
			case 'warning':
				return <AlertCircle size={20}/>;
			case 'error':
				return <X size={20}/>;
			default:
				return <Info size={20}/>;
		}
	};

	const classes = [
		styles.toast,
		styles[type],
		isVisible && styles.visible
	].filter(Boolean).join(' ');

	return (
		<div className={classes}>
			{getIcon()}
			<div className={styles.message}>{message}</div>
			<button className={styles.closeButton} onClick={onClose}>
				<X size={16}/>
			</button>
		</div>
	);
};

export default Toast;