import React from "react";
import styles from "./Tabs.module.scss";

interface TabProps{
	title: string;
	children: React.ReactNode;
}

export const Tab : React.FC<TabProps> = ({title, children}) => {
	return(
		<div className={styles.tabPane}>
			{children}
		</div>
	)
}