import React, { ReactNode, useEffect, useRef, useState } from "react";
import styles from "./Dropdown.module.scss";

interface DropdownItem {
	label?: string;
	icon?: ReactNode;
	value?: string;
	disabled?: boolean;
	separator?: boolean;
}

interface DropdownProps {
	trigger: ReactNode;
	items: DropdownItem[];
	onSelect: (item: DropdownItem) => void;
	className?: string;
}

const Dropdown: React.FC<DropdownProps> = ({ trigger, items, onSelect, className = '' }) => {
	const [isOpen, setIsOpen] = useState<boolean>(false);
	const ref = useRef<HTMLDivElement>(null);

	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (ref.current && !ref.current.contains(event.target as Node)) {
				setIsOpen(false);
			}
		};

		document.addEventListener('mousedown', handleClickOutside);
		return () => document.removeEventListener('mousedown', handleClickOutside);
	}, []);

	return (
		<div ref={ref} className={`${styles.dropdown} ${className}`}>
			<div className={styles.trigger} onClick={() => setIsOpen(!isOpen)}>
				{trigger}
			</div>
			{isOpen && (
				<div className={styles.menu}>
					{items.map((item, index) => (
						item.separator ? (
							<div key={index} className={styles.separator}/>
						) : (
							<div
								key={index}
								className={`${styles.item} ${item.disabled ? styles.disabled : ''}`}
								onClick={() => {
									if (!item.disabled) {
										onSelect(item);
										setIsOpen(false);
									}
								}}
							>
								{item.icon && item.icon}
								{item.label}
							</div>
						)
					))}
				</div>
			)}
		</div>
	);
};

export default Dropdown;