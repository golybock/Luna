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
					<div className={styles.avatar}>
						{user?.user?.image ? (
							<Image
								src={user.user?.image}
								alt="userImage"
								priority={false}
								width={72}
								height={72}
							/>
						) : (
							<div className={styles.avatarFallback}>
								{(user?.user?.displayName || user?.user?.username || user?.email || "")
									.slice(0, 2)
									.toUpperCase()}
							</div>
						)}
					</div>
					<div className={styles.userData}>
						<h3>{user?.user?.displayName || user?.user?.username}</h3>
						<p className={styles.muted}>@{user?.user?.username}</p>
						<p>{user?.email}</p>
						{user?.user?.bio && (
							<p className={styles.bio}>{user.user.bio}</p>
						)}
					</div>
				</div>
			</div>
		</div>
	)
}