"use client";

import { PropsWithChildren } from "react";
import Providers from "@/layout/providers/Providers";
import DataInitializer from "@/layout/DataInitializer";
import { ModalProvider } from "@/layout/ModalContext";


export default function Layout({ children }: PropsWithChildren) {
	return (
		<Providers>
			<DataInitializer>
				<ModalProvider>
					{children}
				</ModalProvider>
			</DataInitializer>
		</Providers>
	);
}
