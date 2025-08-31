import React, { InputHTMLAttributes, ReactNode } from "react";
import styles from "./Label.module.scss";

interface LabelProps extends Omit<InputHTMLAttributes<HTMLLabelElement>, 'size'> {
	icon?: ReactNode;
	className?: string;
	children?: ReactNode;
}

const Label: React.FC<LabelProps> = ({
	disabled = false,
	icon,
	children,
	className = '',
	...props
}) => {
	const labelClasses = [
		styles.label,
		icon && styles.withIcon,
		className
	].filter(Boolean).join(' ');

	return (
		<div className={styles.inputGroup}>
			<div className={styles.wrapper}>
				{icon && (
					<div className={styles.inputIcon}>
						{icon}
					</div>
				)}
				<h5 className={labelClasses}>
					{children}
				</h5>
			</div>
		</div>
	);
};

export default Label;