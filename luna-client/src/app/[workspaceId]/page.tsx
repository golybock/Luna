'use client';

import { useRouter } from 'next/navigation';
import { useEffect } from "react";
import { useAuth } from "@/hooks/useAuth";

export default function HomePage() {
	const { user, logout } = useAuth();
	const router = useRouter();

	const handleLogout = async () => {
		await logout();
		router.push('/');
	};

	useEffect(() => {
		// getUser();
	}, []);

	return (
		<div className="min-h-screen bg-gray-50">
			<nav className="bg-white shadow">
				<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
					<div className="flex justify-between h-16">
						<div className="flex items-center">
							<h1 className="text-xl font-semibold">Luna App</h1>
						</div>
						<div className="flex items-center space-x-4">
							<span className="text-gray-700">Hello, {user?.email}!</span>
							<button
								onClick={handleLogout}
								className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md text-sm font-medium"
							>
								Выйти
							</button>
						</div>
					</div>
				</div>
			</nav>

			<main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
				<div className="px-4 py-6 sm:px-0">
					<div className="bg-white shadow rounded-lg p-6">
						<h2 className="text-2xl font-bold text-gray-900 mb-4">
							Добро пожаловать на домашнюю страницу!
						</h2>
						<p className="text-gray-600">
							Это защищенная страница, доступная только авторизованным пользователям.
						</p>
						<div className="mt-4 p-4 bg-blue-50 rounded-md">
							<h3 className="font-medium text-blue-900">Информация о пользователе:</h3>
							<p className="text-blue-700">ID: {user?.userId}</p>
							<p className="text-blue-700">Email: {user?.email}</p>
						</div>
					</div>
				</div>
			</main>
		</div>
	);
}