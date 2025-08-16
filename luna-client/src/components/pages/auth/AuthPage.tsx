"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import styles from "./AuthPage.module.scss";
import Link from "next/link";
import { useAuth } from "@/hooks/useAuth";
import { GoogleAuthButton } from "@/components/ui/button/GoogleAuthButton";
import { PrimaryButton } from "@/components/ui/button/PrimaryButton";
import { useActions } from "@/store/hooks/useActions";

export const AuthPage: React.FC = () => {

	const [email, setEmail] = useState<string>('teplovartem3094@gmail.com');
	const [code, setCode] = useState<string>('');
	const [error, setError] = useState<string>('');
	const { loginGoogle, requestCode, signIn, codeRequested, codeRequestAt } = useAuth();
	const { resetCodeRequest } = useActions();
	const router = useRouter();

	useEffect(() => {
		if(codeRequestAt != null && codeRequestAt + 360 < Date.now()){
			resetCodeRequest();
			console.log("reset code");
		}
	}, []);

	const handleGoogleClick = async () => {
		setError('');
		try {
			await loginGoogle()
			router.push('/start');
		} catch (err) {
			setError('Check email and password');
		}
	};

	const handleCodeRequested = async () => {
		await requestCode(email);
	}

	const handleSignIn = async () => {
		await signIn(email, code);
		console.log("sign in");
	}

	return (
		<div className={styles.container}>

			<h2>{codeRequested ? "Send code" : "SignIn"}</h2>

			<div className={styles.content}>
				{error && (
					<p className="error-message">
						{error}
					</p>
				)}

				<input
					type="email"
					value={email}
					disabled={codeRequested}
					onChange={(e) => setEmail(e.target.value)}
					placeholder="Email"
				/>

				{codeRequested && (
					<input
						type="text"
						maxLength={6}
						value={code}
						onChange={(e) => setCode(e.target.value)}
						placeholder="Code"
					/>
				)}

				<PrimaryButton
					onClick={async () => {
						codeRequested ? await handleSignIn() : await handleCodeRequested()
					}}
				>
					<p>{codeRequested ? "SignIn" : "Send code"}</p>
				</PrimaryButton>

				<GoogleAuthButton onClick={handleGoogleClick}/>

				<div className={styles.link}>
					<Link href="/">
						Back to main page
					</Link>
				</div>
			</div>
		</div>
	)
}