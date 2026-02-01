import React, { ReactNode, useState } from "react";
import styles from "./Tabs.module.scss";

interface TabsProps {
	children: ReactNode;
	defaultActiveTab?: number;
	className?: string;
}

interface TabPaneProps {
	children: ReactNode;
	title: string;
}

const Tabs: React.FC<TabsProps> = ({ children, defaultActiveTab = 0, className = '' }) => {
	const [activeTab, setActiveTab] = useState<number>(defaultActiveTab);
	const tabPanes = React.Children.toArray(children) as React.ReactElement<TabPaneProps>[];

	return (
		<div className={`${styles.tabsContainer} ${className}`}>
			<div className={styles.tabsHeader}>
				<div className={styles.tabsNav}>
					{tabPanes.map((tabPane, index) => (
						<button
							key={index}
							className={`${styles.tabButton} ${activeTab === index ? styles.active : ''}`}
							onClick={() => setActiveTab(index)}
						>
							{tabPane.props.title}
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

export default Tabs;