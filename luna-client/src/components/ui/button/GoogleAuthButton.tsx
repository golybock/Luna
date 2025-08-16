"use client";

import React from "react";
import styles from "./GoogleAuthButton.module.scss";
import Image from "next/image";

type TypeButton = React.HTMLAttributes<HTMLButtonElement>;

export const GoogleAuthButton: React.FC<TypeButton> = ({className, ...rest}) => {
	return (
		<button className={`${styles.primaryButton} ${className}`} {...rest}>
			<Image src={"/resources/google.webp"} width={24} height={24} alt="google"/>
			<p>Login google</p>
		</button>
	)
}