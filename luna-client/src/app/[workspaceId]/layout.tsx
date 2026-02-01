"use client";

import React, { useEffect, useMemo, useRef, useState } from "react";
import styles from "./layout.module.scss";
import Image from "next/image";
import { useActions } from "@/store/hooks/useActions";
import { useAuth } from "@/hooks/useAuth";
import { useRouter } from "next/navigation";
import { useModal } from "@/layout/ModalContext";
import { SettingsModal } from "@/components/modals/settings/SettingsModal";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { PageMenuItem } from "@/components/ui/workspaceLayout/PageMenuItem";
import CreatePageBadge from "@/components/ui/createPageBadge/CreatePageBadge";
import { MenuItem } from "@/models/ui/MenuItem";
import { MenuItemLink } from "@/components/ui/workspaceLayout/MenuItemLink";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store/store";
import { usePages } from "@/store/hooks/usePages";
import { getWorkspacePages } from "@/store/slices/pagesSlice";

export default function MainLayout({ children }: Readonly<{ children: React.ReactNode; }>) {

	const { pages } = usePages();
	const { user, logout } = useAuth();
	const { setSelectedWorkspace, setPages } = useActions();
	const { openModal } = useModal();
	const { selectedWorkspaceId } = useWorkspaces();
	const dispatch = useDispatch<AppDispatch>();
	const [sidebarWidth, setSidebarWidth] = useState<number>(240);
	const [isResizing, setIsResizing] = useState(false);
	const resizeRafRef = useRef<number | null>(null);
	const [isMobile, setIsMobile] = useState(false);
	const [isSidebarOpen, setIsSidebarOpen] = useState(false);
	const containerRef = useRef<HTMLDivElement | null>(null);

	const router = useRouter();

	const handleOnClickSettings = () => {
		openModal(<SettingsModal/>)
	}

	useEffect(() => {
		dispatch(getWorkspacePages(selectedWorkspaceId))
	}, [selectedWorkspaceId])

	useEffect(() => {
		const mediaQuery = window.matchMedia("(max-width: 1024px)");
		const handleChange = () => {
			setIsMobile(mediaQuery.matches);
			if (!mediaQuery.matches) {
				setIsSidebarOpen(false);
			}
		};
		handleChange();
		mediaQuery.addEventListener("change", handleChange);
		return () => mediaQuery.removeEventListener("change", handleChange);
	}, []);

	useEffect(() => {
		if (!isResizing) return;

		const handleMouseMove = (event: MouseEvent) => {
			const containerLeft = containerRef.current?.getBoundingClientRect().left ?? 0;
			const nextWidth = Math.min(360, Math.max(200, event.clientX - containerLeft));
			if (resizeRafRef.current) {
				cancelAnimationFrame(resizeRafRef.current);
			}
			resizeRafRef.current = requestAnimationFrame(() => {
				setSidebarWidth(nextWidth);
			});
		};

		const handleMouseUp = () => {
			setIsResizing(false);
		};

		document.body.classList.add(styles.resizing);
		window.addEventListener("mousemove", handleMouseMove);
		window.addEventListener("mouseup", handleMouseUp);
		return () => {
			if (resizeRafRef.current) {
				cancelAnimationFrame(resizeRafRef.current);
				resizeRafRef.current = null;
			}
			document.body.classList.remove(styles.resizing);
			window.removeEventListener("mousemove", handleMouseMove);
			window.removeEventListener("mouseup", handleMouseUp);
		};
	}, [isResizing]);

	const topMenuItems: MenuItem[] = useMemo(() => {
		return [
			{
				name: "Home",
				imagePath: "/icons/home_24.svg",
				path: `/${selectedWorkspaceId}`,
			},
		];
	}, [selectedWorkspaceId])

	const bottomMenuItems: MenuItem[] = useMemo(() => {
		return [
			{
				name: "Settings",
				onClick: handleOnClickSettings,
				imagePath: "/icons/settings_24.svg",
			},
		]
	}, [])

	const pagesMenuItems: MenuItem[] = useMemo(() => {
		return pages.map((page) => {
			const menuItem: MenuItem = {
				name: page.title,
				emoji: page.emoji,
				path: `/${selectedWorkspaceId}/${page.id}`
			};
			return menuItem;
		})
	}, [pages])

	const navigateToSelectWorkspace = () => {
		setSelectedWorkspace(null);
		setPages([]);
		router.push("/start")
	}
	const handleLogout = async () => {
		await logout();
		router.push("/signIn")
	}

	return (
		<div className={styles.container} ref={containerRef}>
			{isMobile && (
				<button
					className={styles.hamburger}
					onClick={() => setIsSidebarOpen(true)}
					aria-label="Open navigation"
				>
					<Image src="/icons/menu_24.svg" alt="menu" width={20} height={20}/>
				</button>
			)}
			{isMobile && isSidebarOpen && (
				<div
					className={styles.overlay}
					onClick={() => setIsSidebarOpen(false)}
				/>
			)}
			<nav
				className={`${styles.navbar} ${isResizing ? styles.navbarResizing : ""} ${isMobile ? styles.navbarMobile : ""} ${isSidebarOpen ? styles.navbarOpen : ""}`}
				style={{ width: isMobile ? undefined : sidebarWidth }}
			>
				<div>
					<div className={styles.navbarHeader} onClick={handleOnClickSettings} role="button">
						<div className={styles.profileBadge}>
							<h6>{user?.user?.username ?? user?.email}</h6>
						</div>
						<div className={styles.createIcon}>
							<Image
								src="/icons/edit_24.svg"
								alt="edt"
								width={16}
								height={16}
							/>
						</div>
					</div>
					<div className={styles.navbarContent}>
						<div className={styles.menuItems}>
							{topMenuItems.map((item, i) => (
								<MenuItemLink item={item} key={i}/>
							))}
						</div>
						<div className={styles.menuItemsFavoritePages}>

						</div>
						<div className={styles.menuItemsPrivatePages}>

						</div>
						<div className={styles.menuItemsPublicPages}>
							<div className={styles.createPageContainer}>
								<h5 className={styles.pagesLabel}>
									Your pages
								</h5>
								<CreatePageBadge/>
							</div>
							<div>
								{pagesMenuItems.map((item, i) => (
									<PageMenuItem item={item} key={i}/>
								))}
							</div>
						</div>
						<div className={styles.menuItems}>
							{bottomMenuItems.map((item, i) => (
								<MenuItemLink item={item} key={i}/>
							))}
						</div>
					</div>
				</div>
				<div className={styles.navbarFooter}>
					<p role="button" onClick={navigateToSelectWorkspace}>
						Select other workspace
					</p>
					<p role="button" onClick={handleLogout}>
						Logout
					</p>
				</div>
				{!isMobile && (
					<div
						className={styles.resizer}
						onMouseDown={() => setIsResizing(true)}
						role="presentation"
					/>
				)}
			</nav>
			<div className={styles.content}>
				{children}
			</div>
		</div>
	)
}