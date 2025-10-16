"use client";

import { PropsWithChildren } from "react";
import Providers from "@/layout/providers/Providers";
import DataInitializer from "@/layout/DataInitializer";
import { ModalProvider } from "@/layout/ModalContext";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export default function Layout({ children }: PropsWithChildren) {
	return (
		<Providers>
			<DataInitializer>
				<ModalProvider>
					{children}
					<ToastContainer
						position="top-right"
						theme="colored"
						style={{ zIndex: "99999" }}
					/>
				</ModalProvider>
			</DataInitializer>
		</Providers>
	);
}
