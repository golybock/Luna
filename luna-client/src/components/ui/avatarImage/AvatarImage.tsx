import React from "react";
import styles from "./AvatarImage.module.scss";

interface AvatarImageProps {
	src: string;
	alt: string;
	initials?: string;
}

export const AvatarImage: React.FC<AvatarImageProps> = ({src, alt, initials}) => {
	return (
		<div className={styles.avatar}>
			{src ? (
				<img
					src={src}
					alt={alt}
					className={styles.avatarImage}
				/>
			) : (
				<div className={styles.avatarPlaceholder}>
					{initials}
				</div>
			)}
		</div>
	)
}