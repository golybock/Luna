import styles from "./EmpojiPicker.module.scss";
import React, { useEffect, useRef, useState } from "react";
import { Smile } from "lucide-react";

const EMOJI_LIST = [
	'😀', '😃', '😄', '😁', '😆', '😅', '😂', '🤣',
	'😊', '😇', '🙂', '🙃', '😉', '😌', '😍', '🥰',
	'😘', '😗', '😙', '😚', '😋', '😛', '😝', '😜',
	'🤪', '🤨', '🧐', '🤓', '😎', '🥸', '🤩', '🥳',
	'😏', '😒', '😞', '😔', '😟', '😕', '🙁', '☹️',
	'😣', '😖', '😫', '😩', '🥺', '😢', '😭', '😤',
	'😠', '😡', '🤬', '🤯', '😳', '🥵', '🥶', '😱',
	'😨', '😰', '😥', '😓', '🤗', '🤔', '🤭', '🤫',
	'🤥', '😶', '😐', '😑', '😬', '🙄', '😯', '😦',
	'😧', '😮', '😲', '🥱', '😴', '🤤', '😪', '😵',
	'🤐', '🥴', '🤢', '🤮', '🤧', '😷', '🤒', '🤕',
	'🤑', '🤠', '😈', '👿', '👹', '👺', '🤡', '💩',
	'👻', '💀', '☠️', '👽', '👾', '🤖', '🎃', '😺',
	'😸', '😹', '😻', '😼', '😽', '🙀', '😿', '😾',
	'❤️', '🧡', '💛', '💚', '💙', '💜', '🖤', '🤍',
	'🤎', '💔', '❣️', '💕', '💞', '💓', '💗', '💖',
	'💘', '💝', '💟', '☮️', '✝️', '☪️', '🕉️', '☸️',
	'✡️', '🔯', '🕎', '☯️', '☦️', '🛐', '⛎', '♈',
	'👍', '👎', '👌', '🤌', '🤏', '✌️', '🤞', '🤟',
	'🤘', '🤙', '👈', '👉', '👆', '🖕', '👇', '☝️',
];

interface EmojiPickerProps {
	error?: string;
	value?: string;
	onChange?: (emoji: string) => void;
	disabled?: boolean;
	className?: string;
	placeholder?: string;
}

export const EmojiPicker: React.FC<EmojiPickerProps> = ({
	error,
	value = '',
	onChange,
	disabled = false,
	className,
	placeholder
}) => {
	const [isOpen, setIsOpen] = useState(false);
	const [isHovered, setIsHovered] = useState(false);
	const [hoveredEmoji, setHoveredEmoji] = useState('');
	const dropdownRef = useRef<HTMLDivElement>(null);
	const buttonRef = useRef<HTMLButtonElement>(null);

	// Закрытие dropdown при клике вне компонента
	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node) &&
				buttonRef.current && !buttonRef.current.contains(event.target as Node)) {
				setIsOpen(false);
			}
		};

		document.addEventListener('mousedown', handleClickOutside);
		return () => {
			document.removeEventListener('mousedown', handleClickOutside);
		};
	}, []);

	const handleEmojiSelect = (emoji: string) => {
		onChange?.(emoji);
		setIsOpen(false);
	};

	const handleButtonClick = () => {
		if (!disabled) {
			setIsOpen(!isOpen);
		}
	};

	const buttonClasses = [
		styles.emojiButton,
		isOpen && styles.emojiButtonFocus,
		(!value && isHovered) && styles.emojiButtonHover,
		!value && styles.emojiButtonEmpty
	].filter(Boolean).join(' ');

	const placeholderClasses = [
		styles.placeholder,
		isHovered && !value && styles.placeholderVisible
	].filter(Boolean).join(' ');

	return (
		<div className={styles.inputGroup}>
			<div className={styles.wrapper}>
				<button
					ref={buttonRef}
					type="button"
					className={buttonClasses}
					onClick={handleButtonClick}
					disabled={disabled}
					onMouseEnter={() => setIsHovered(true)}
					onMouseLeave={() => setIsHovered(false)}
				>
					{value || <Smile size={24}/>}
					<div className={placeholderClasses}>
						{placeholder}
					</div>
				</button>

				{isOpen && (
					<div ref={dropdownRef} className={styles.dropdown}>
						<div className={styles.emojiGrid}>
							{EMOJI_LIST.map((emoji, index) => (
								<button
									key={index}
									className={`${styles.emojiItem} ${hoveredEmoji === emoji ? styles.emojiItemHover : ''}`}
									onClick={() => handleEmojiSelect(emoji)}
									onMouseEnter={() => setHoveredEmoji(emoji)}
									onMouseLeave={() => setHoveredEmoji('')}
								>
									{emoji}
								</button>
							))}
						</div>
					</div>
				)}
			</div>
			{error && (
				<div className={styles.errorMessage}>
					<Smile size={16}/>
					{error}
				</div>
			)}
		</div>
	);
};