"use client";

import React from "react";
import styles from "./ProfilePage.module.scss";
import Image from "next/image";
import { useAuth } from "@/hooks/useAuth";

export const ProfilePage: React.FC = () => {

	const { user } = useAuth();

	return (
		<div className={styles.container}>
			<div className={styles.content}>
				<div className={styles.card}>
					<div className={styles.imageCircle}>
						{user?.user?.image && (
							<Image
								src={user.user?.image}
								alt="userImage"
								priority={false}
								width={192}
								height={192}
							/>
						)}
					</div>
					<div className={styles.userData}>
						<h3>{user?.user?.username}</h3>
						<p>{user?.email}</p>
						<p>{user?.user?.displayName}</p>
						<p>{user?.user?.bio}</p>
					</div>
				</div>
			</div>
		</div>
	)
}