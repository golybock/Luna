"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import styles from "./AuthPage.module.scss";
import Link from "next/link";
import { useAuth } from "@/hooks/useAuth";
import { GoogleAuthButton } from "@/components/ui/button/GoogleAuthButton";
import { useActions } from "@/store/hooks/useActions";
import Button from "@/ui/button/Button";
import Input from "@/ui/input/Input";

export const AuthPage: React.FC = () => {

	const [email, setEmail] = useState<string>("");
	const [code, setCode] = useState<string>("");
	const [error, setError] = useState<string>("");
	const { loginGoogle, requestCode, signIn, codeRequested, codeRequestAt, isAuthenticated } = useAuth();
	const { resetCodeRequest } = useActions();
	const router = useRouter();

	useEffect(() => {
		if (codeRequestAt != null && codeRequestAt + 360 < Date.now()) {
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
	}

	useEffect(() => {
		if (isAuthenticated) {
			router.push("/start");
		}
	}, [isAuthenticated])

	return (
		<div className={styles.container}>

			<div className={styles.content}>

				<h3>SignIn / SignUp</h3>

				<GoogleAuthButton
					onClick={handleGoogleClick}
					style={{ width: '100%' }}
				/>

				<div className={styles.signInContent}>

					{error && (
						<p className="error-message">
							{error}
						</p>
					)}

					<Input
						type="email"
						label="Email"
						value={email}
						disabled={codeRequested}
						onChange={(e) => setEmail(e.target.value)}
						placeholder="Email"
					/>

					{codeRequested && (
						<Input
							type="text"
							label="Code"
							maxLength={6}
							value={code}
							onChange={(e) => setCode(e.target.value)}
							placeholder="Code"
						/>
					)}

					<Button variant="primary"
							onClick={async () => {
								codeRequested ? await handleSignIn() : await handleCodeRequested()
							}}
					>
						<p>{codeRequested ? "SignIn" : "Send code"}</p>
					</Button>

					<div className={styles.link}>
						<Link href="/">
							Back to main page
						</Link>
					</div>
				</div>
			</div>
		</div>
	)
}