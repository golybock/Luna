import React from "react";
import styles from "./SecondaryButton.module.scss.module.scss";

interface SecondaryButtonProps {
	children?: React.ReactNode;
}

type TypeButton = React.HTMLAttributes<HTMLButtonElement>;

export const SecondaryButton: React.FC<SecondaryButtonProps & TypeButton> = ({children, className, ...rest}) => {
	return (
		<button className={`${styles.primaryButton} ${className}`} {...rest}>
			{children}
		</button>
	)
};