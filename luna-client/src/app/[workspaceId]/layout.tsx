"use client";

import React, { useEffect, useMemo } from "react";
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
	const { user } = useAuth();
	const { setSelectedWorkspace, setPages } = useActions();
	const { openModal } = useModal();
	const { selectedWorkspaceId } = useWorkspaces();
	const dispatch = useDispatch<AppDispatch>();

	const router = useRouter();

	const handleOnClickSettings = () => {
		openModal(<SettingsModal/>)
	}

	useEffect(() => {
		dispatch(getWorkspacePages(selectedWorkspaceId))
	}, [selectedWorkspaceId])

	const topMenuItems: MenuItem[] = useMemo(() => {
		return [
			{
				name: "Home",
				imagePath: "/icons/home_24.svg",
				path: `/${selectedWorkspaceId}`,
			},
			{
				name: "Inbox",
				imagePath: "/icons/inbox_24.svg",
				path: "/inbox",
			}
		];
	}, [selectedWorkspaceId])

	const bottomMenuItems: MenuItem[] = useMemo(() => {
		return [
			{
				name: "Settings",
				onClick: handleOnClickSettings,
				imagePath: "/icons/settings_24.svg",
			},
			{
				name: "Templates",
				path: `/${selectedWorkspaceId}/templates`,
				imagePath: "/icons/template_24.svg",
			},
			{
				name: "Trash",
				path: `/${selectedWorkspaceId}/trash`,
				imagePath: "/icons/trash_24.svg",
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

	return (
		<div className={styles.container}>
			<nav className={styles.navbar}>
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
				</div>
			</nav>
			<div className={styles.content}>
				{children}
			</div>
		</div>
	)
}