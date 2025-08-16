import React, { useState } from "react";
import styles from "./Tabs.module.scss";

interface TabsProps {
	defaultActiveTab: number;
	children: React.ReactNode;
}

export const Tabs: React.FC<TabsProps> = ({ children, defaultActiveTab = 0 }) => {
	const [activeTab, setActiveTab] = useState(defaultActiveTab);

	const tabPanes = React.Children.toArray(children);

	return (
		<div className={styles.tabsContainer}>
			<div className={styles.tabsHeader}>
				<div className={styles.tabsNav}>
					{tabPanes.map((tabPane, index) => (
						<button
							key={index}
							className={`${styles.tabButton} ${activeTab === index ? 'active' : ''}`}
							onClick={() => setActiveTab(index)}
						>
							{(tabPane as any).props.title}
						</button>
					))}
				</div>
				<div className={styles.tabsUnderline}>
					<div
						className={styles.activeUnderline}
						style={{
							transform: `translateX(${activeTab * 100}%)`,
							width: `${100 / tabPanes.length}%`
						}}
					/>
				</div>
			</div>

			<div className={styles.tabContent}>
				<div className={styles.tabPane}>
					{tabPanes[activeTab]}
				</div>
			</div>
		</div>
	);
};