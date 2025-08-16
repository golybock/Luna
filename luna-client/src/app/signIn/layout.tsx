"use client";

import React from "react";
import styles from "./layout.module.scss";
import Image from "next/image";

export default function AuthLayout({children}: Readonly<{ children: React.ReactNode; }>) {
	return (
		<div className={styles.container}>
			<div className={styles.image}>
				<Image src={"/resources/woodcuts.jpg"} alt="background" width={400} height={400} />
			</div>
			<div className={styles.content}>
				{children}
			</div>
		</div>
	)
}