import React, { HTMLAttributes, ReactNode } from "react";
import styles from "./Card.module.scss";

interface CardProps extends HTMLAttributes<HTMLDivElement> {
	children: ReactNode;
	className?: string;
	padding?: CardPadding;
	hover?: boolean;
	elevated?: boolean;
	bordered?: boolean;
}

const Card: React.FC<CardProps> = ({
	children,
	className = '',
	padding = 'medium',
	hover = false,
	elevated = false,
	bordered = false,
	onClick,
	...props
}) => {
	const classes = [
		styles.card,
		styles[`padding${padding.charAt(0).toUpperCase() + padding.slice(1)}`],
		hover && styles.hover,
		elevated && styles.elevated,
		bordered && styles.bordered,
		className
	].filter(Boolean).join(' ');

	return (
		<div className={classes} onClick={onClick} {...props}>
			{children}
		</div>
	);
};

export default Card;