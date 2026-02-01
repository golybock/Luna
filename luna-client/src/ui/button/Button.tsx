import React, { ReactNode } from "react";
import styles from "./Button.module.scss";

type ButtonBaseProps = {
	children?: ReactNode;
	ref?: React.Ref<any>;
	variant?: "primary" | "secondary" | "ghost" | "danger";
	size?: 'small' | 'medium' | 'large' | 'default' | "icon";
	icon?: ReactNode;
	className?: string;
};

type ButtonAsButton = ButtonBaseProps & Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, 'size'> & {
	href?: undefined;
};

type ButtonAsAnchor = ButtonBaseProps & React.AnchorHTMLAttributes<HTMLAnchorElement> & {
	href: string;
};

type ButtonProps = ButtonAsButton | ButtonAsAnchor;

const Button: React.FC<ButtonProps> = ({
	ref,
	children,
	variant = 'primary',
	size = 'medium',
	icon,
	className = '',
	...props
}: ButtonProps) => {

	const classes = [
		styles.button,
		styles[variant],
		styles[size],
		className
	].filter(Boolean).join(' ');

	if ('href' in props) {
		const { href, rel, onClick, ...anchorProps } = props as ButtonAsAnchor;
		return (
			<a
				className={classes}
				href={href}
				rel={rel}
				ref={ref}
				onClick={onClick}
				{...anchorProps}
			>
				{icon && <span className={styles.icon}>{icon}</span>}
				{children}
			</a>
		);
	}

	const { disabled, onClick, ...buttonProps } = props as ButtonAsButton;

	return (
		<button
			className={classes}
			onClick={onClick}
			ref={ref}
			disabled={disabled}
			{...buttonProps}
		>
			{icon && <span className={styles.icon}>{icon}</span>}
			{children}
		</button>
	);
};

export default Button;