import React, { ChangeEvent, forwardRef, useImperativeHandle, useState } from "react";
import styles from "./FormInput.module.scss";

export interface FormInputHandle {
	validate: () => boolean;
	reset: () => void;
	getValue: () => string;
}

interface FormInputProps {
	name?: string;
	className?: string;
	placeholder?: string;
	defaultValue?: string;
	maxLength?: number;
	required?: boolean;
	type?: "text" | "email" | "tel" | "password" | "number";
	disabled?: boolean;
	mask?: string;
	onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
	onValidationChange?: (isValid: boolean) => void;
	customValidator?: (value: string) => string | null; // Возвращает текст ошибки или null
}

const FormInput: React.ForwardRefRenderFunction<FormInputHandle, FormInputProps> = (props, ref) => {
	const [inputValue, setInputValue] = useState<string>(props.defaultValue ?? "");
	const [touched, setTouched] = useState<boolean>(false);
	const [focused, setFocused] = useState<boolean>(false);
	const [hasError, setHasError] = useState<boolean>(false);

	const validateInput = (value: string) => {
		const trimmed = value.trim();
		let error = false;

		// Проверка обязательности
		if (props.required && !trimmed) {
			error = true;
		}
		// Кастомная валидация
		else if (props.customValidator) {
			const customError = props.customValidator(value);
			if (customError) {
				error = true;
			}
		}

		setHasError(error);
		const isValid = !error;

		// Уведомляем родителя о результате валидации
		if (props.onValidationChange) {
			props.onValidationChange(isValid);
		}

		return isValid;
	};

	const validate = () => {
		setTouched(true);
		return validateInput(inputValue);
	};

	const handleOnChange = (e: ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		setInputValue(newValue);

		// Уведомляем родителя об изменении
		if (props.onChange) {
			props.onChange(e);
		}

		if (touched) {
			validateInput(newValue);
		}
	};

	const handleFocus = () => {
		setFocused(true);
	};

	const handleBlur = () => {
		setFocused(false);
		setTouched(true);
		validateInput(inputValue);
	};

	useImperativeHandle(ref, () => ({
		validate,
		reset: () => {
			setInputValue(props.defaultValue ?? "");
			setHasError(false);
			setTouched(false);
			setFocused(false);
		},
		getValue: () => inputValue
	}), [inputValue, props.defaultValue, validate]);

	// Определяем, должна ли подсказка быть наверху
	const shouldLabelFloat = focused || inputValue.length > 0;

	return (
		<div className={`${styles.container} ${props.className}`}>
			<div className={styles.inputWrapper}>
				<input
					className={`${styles.slidingInput} ${hasError ? styles.inputError : ''}`}
					value={inputValue}
					maxLength={props.maxLength}
					title={props.placeholder}
					name={props.name}
					type={props.type || "text"}
					disabled={props.disabled}
					onChange={handleOnChange}
					onFocus={handleFocus}
					onBlur={handleBlur}
				/>

				<p className={`${styles.floatingLabel} ${shouldLabelFloat ? styles.floatingLabelActive : ''} ${hasError ? styles.labelError : ''}`}>
					{props.placeholder}
				</p>
			</div>
		</div>
	);
};

export default forwardRef(FormInput);