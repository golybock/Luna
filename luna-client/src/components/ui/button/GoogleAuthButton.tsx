"use client";

import React from "react";
import styles from "./GoogleAuthButton.module.scss";
import Image from "next/image";
import Button from "@/ui/button/Button";

type TypeButton = React.ButtonHTMLAttributes<HTMLButtonElement>;

export const GoogleAuthButton: React.FC<TypeButton> = ({className, ...rest}) => {
	return (
		<Button className={`${styles.primaryButton} ${className}`} {...rest} variant="primary" >
			<Image src={"/resources/google.webp"} width={24} height={24} alt="google"/>
			<p>Login google</p>
		</Button>
	)
}