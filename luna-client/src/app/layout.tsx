import type { Metadata } from "next";
import { Noto_Sans } from "next/font/google";
import "../styles/global.scss";
import React from "react";
import Layout from "@/layout/Layout";

const notoSans = Noto_Sans({
	variable: "--font-geist-sans",
	subsets: ["latin"],
});

export const metadata: Metadata = {
	title: "Luna",
	description: "Luna",
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode; }>) {
	return (
		<html lang="en" data-bs-theme="dark">
		<body className={`${notoSans.variable} ${notoSans.variable} antialiased`}>
		<Layout>
			{children}
		</Layout>
		</body>
		</html>
	);
}
