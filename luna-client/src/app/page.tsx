'use client';

import Link from 'next/link';
import "../styles/global.scss"

export default function LandingPage() {
	return (
		<div className="min-h-screen flex flex-col items-center justify-center bg-gray-50">
			<div className="max-w-md w-full space-y-8 p-8">
				<div className="text-center">
					<h1 className="text-4xl font-bold text-gray-900 mb-4">
						Добро пожаловать в Luna
					</h1>
					<p className="text-lg text-gray-600 mb-8">
						Войдите в свой аккаунт или создайте новый
					</p>
				</div>

				<div className="space-y-4">
					<Link
						href="/signIn"
						className="w-full flex justify-center py-3 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
					>
						Войти
					</Link>
				</div>
			</div>
		</div>
	);
}