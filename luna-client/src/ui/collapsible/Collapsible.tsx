import React, { ReactNode, useState } from "react";
import { ChevronRight } from "lucide-react";
import styles from "./Collapsible.module.scss";

interface CollapsibleProps {
	title: string;
	children: ReactNode;
	defaultOpen?: boolean;
	className?: string;
}

const Collapsible: React.FC<CollapsibleProps> = ({ title, children, defaultOpen = false, className = '' }) => {
	const [isOpen, setIsOpen] = useState<boolean>(defaultOpen);

	return (
		<div className={`${styles.collapsible} ${className}`}>
			<button
				className={styles.trigger}
				onClick={() => setIsOpen(!isOpen)}
			>
				<ChevronRight className={`${styles.icon} ${isOpen ? styles.iconOpen : ''}`} size={16} />
				{title}
			</button>
			{isOpen && (
				<div className={styles.content}>
					<div className={styles.contentInner}>
						{children}
					</div>
				</div>
			)}
		</div>
	);
};

export default Collapsible;