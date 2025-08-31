import React, { InputHTMLAttributes, ReactNode } from "react";
import { AlertCircle } from "lucide-react";
import styles from "./Input.module.scss";

interface InputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'size'> {
	label?: string;
	error?: string;
	icon?: ReactNode;
	className?: string;
}

const Input: React.FC<InputProps> = ({
	label,
	error,
	type = 'text',
	placeholder,
	value,
	onChange,
	disabled = false,
	icon,
	className = '',
	...props
}) => {
	const inputClasses = [
		styles.input,
		error && styles.error,
		icon && styles.withIcon,
		className
	].filter(Boolean).join(' ');

	return (
		<div className={styles.inputGroup}>
			{label && (
				<label className={styles.label}>
					{label}
				</label>
			)}
			<div className={styles.wrapper}>
				{icon && (
					<div className={styles.inputIcon}>
						{icon}
					</div>
				)}
				<input
					type={type}
					placeholder={placeholder}
					value={value}
					defaultValue={props.defaultValue}
					onChange={onChange}
					disabled={disabled}
					className={inputClasses}
					{...props}
				/>
			</div>
			{error && (
				<span className={styles.errorMessage}>
          			<AlertCircle size={16}/>
					{error}
				</span>
			)}
		</div>
	);
};

export default Input;