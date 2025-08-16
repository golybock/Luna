import React from "react";
import styles from "./PrimaryButton.module.scss";

interface PrimaryButtonProps {
	children?: React.ReactNode;
}

type TypeButton = React.HTMLAttributes<HTMLButtonElement>;

export const PrimaryButton: React.FC<PrimaryButtonProps & TypeButton> = ({children, className, ...rest}) => {
	return (
		<button className={`${styles.primaryButton} ${className}`} {...rest}>
			{children}
		</button>
	)
};