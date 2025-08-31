"use client";

import React, { useEffect, useMemo, useState } from "react";
import styles from "./layout.module.scss";
import Image from "next/image";
import { IMenuItem } from "@/types/IMenuItem";
import { useActions } from "@/store/hooks/useActions";
import { useAuth } from "@/hooks/useAuth";
import { useRouter } from "next/navigation";
import { useModal } from "@/layout/ModalContext";
import { SettingsModal } from "@/components/modals/settings/SettingsModal";
import { IPageLightView } from "@/types/page/pageLightView";
import { pageHttpProvider } from "@/http/pageHttpProvider";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import { MenuItem } from "@/components/ui/workspaceLayout/MenuItem";
import { PageMenuItem } from "@/components/ui/workspaceLayout/PageMenuItem";
import CreatePageBadge from "@/components/ui/createPageBadge/CreatePageBadge";

export default function MainLayout({ children }: Readonly<{ children: React.ReactNode; }>) {

	const [pages, setPages] = useState<IPageLightView[]>([]);

	const { user } = useAuth();
	const { setSelectedWorkspace } = useActions();
	const { openModal } = useModal();
	const { selectedWorkspaceId } = useWorkspaces();

	const router = useRouter();

	const handleOnClickSettings = () => {
		openModal(<SettingsModal/>)
	}

	useEffect(() => {
		const getPages = async () => {
			if (selectedWorkspaceId) {
				const pages = await pageHttpProvider.getWorkspacePages(selectedWorkspaceId);

				setPages(pages);
			}
		}

		getPages();
	}, [selectedWorkspaceId])

	const topMenuItems: IMenuItem[] = useMemo(() => {
		return [
			{
				name: "Search",
				imagePath: "/icons/search_24.svg",
				path: "/search"
			},
			{
				name: "Home",
				imagePath: "/icons/home_24.svg",
				path: "/start",
			},
			{
				name: "Inbox",
				imagePath: "/icons/inbox_24.svg",
				path: "/inbox",
			}
		];
	}, [])

	const bottomMenuItems: IMenuItem[] = useMemo(() => {
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

	const pagesMenuItems: IMenuItem[] = useMemo(() => {
		return pages.map((page) => {
			const menuItem: IMenuItem = {
				name: page.title,
				emoji: page.emoji,
				path: `/${selectedWorkspaceId}/${page.id}`
			};
			return menuItem;
		})
	}, [pages])

	const navigateToSelectWorkspace = () => {
		setSelectedWorkspace(null);
		router.push("/start")
	}

	return (
		<div className={styles.container}>
			<nav className={styles.navbar}>
				<div>
					<div className={styles.navbarHeader}>
						<div className={styles.profileBadge} onClick={handleOnClickSettings}>
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
								<MenuItem item={item} key={i}/>
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
								<MenuItem item={item} key={i}/>
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