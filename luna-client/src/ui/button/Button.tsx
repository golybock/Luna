import React, { ButtonHTMLAttributes, ReactNode } from "react";
import styles from "./Button.module.scss";

interface ButtonProps extends Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'size'> {
	children?: ReactNode;
	variant?: ButtonVariant;
	size?: ButtonSize;
	disabled?: boolean;
	icon?: ReactNode;
	className?: string;
}

const Button: React.FC<ButtonProps> = ({
	children,
	variant = 'primary',
	size = 'medium',
	disabled = false,
	icon,
	onClick,
	className = '',
	...props
}) => {

	const classes = [
		styles.button,
		styles[variant],
		styles[size],
		className
	].filter(Boolean).join(' ');

	return (
		<button
			className={classes}
			onClick={onClick}
			disabled={disabled}
			{...props}
		>
			{icon && <span className={styles.icon}>{icon}</span>}
			{children}
		</button>
	);
};

export default Button;