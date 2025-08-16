"use client";

import React, { useEffect, useMemo, useState } from "react";
import styles from "./layout.module.scss";
import Image from "next/image";
import { IMenuItem } from "@/types/IMenuItem";
import Link from "next/link";
import { useActions } from "@/store/hooks/useActions";
import { useAuth } from "@/hooks/useAuth";
import { useRouter } from "next/navigation";
import { useModal } from "@/layout/ModalContext";
import { SettingsModal } from "@/components/modals/settings/SettingsModal";
import { IPageLightView } from "@/types/page/pageLightView";
import { pageHttpProvider } from "@/http/pageHttpProvider";
import { useWorkspaces } from "@/store/hooks/useWorkspaces";
import CreatePageBadge from "@/components/ui/createPageBadge/CreatePageBadge";
import { PrimaryButton } from "@/components/ui/button/PrimaryButton";

export default function MainLayout({ children }: Readonly<{ children: React.ReactNode; }>) {

	const { user } = useAuth();
	const { setSelectedWorkspace } = useActions();
	const router = useRouter();
	const { openModal } = useModal();
	const [pages, setPages] = useState<IPageLightView[]>([]);
	const { selectedWorkspaceId } = useWorkspaces();

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
				path: "/",
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
				path: "/home/templates",
				imagePath: "/icons/template_24.svg",
			},
			{
				name: "Trash",
				onClick: handleOnClickSettings,
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
							<div className={styles.profileBadgeImage}>
								<Image
									src="/icons/accout_circle_24.svg"
									alt="profileImage"
									width={24}
									height={24}
								/>
							</div>
							<h4>{user?.user?.username ?? user?.email}</h4>
						</div>
						<div className={styles.createIcon}>
							<Image
								src="/icons/edit_24.svg"
								alt="edt"
								width={24}
								height={24}
							/>
						</div>
					</div>
					<div className={styles.navbarContent}>
						<div className={styles.menuItems}>
							{topMenuItems.map((item, i) => {
								return item.path ? (
									<Link className={styles.menuItem} key={i} href={item.path}>
										{item.imagePath && (
											<Image
												src={item.imagePath}
												width={24}
												height={24}
												alt={item.name}
											/>
										)}
										<h6>{item.name}</h6>
									</Link>
								) : (
									<div className={styles.menuItem} key={i} onClick={item.onClick}>
										{item.imagePath && (
											<Image
												src={item.imagePath}
												width={24}
												height={24}
												alt={item.name}
											/>
										)}
										<h6>{item.name}</h6>
									</div>
								)
							})}
						</div>
						<div className={styles.menuItemsFavoritePages}>

						</div>
						<div className={styles.menuItemsPrivatePages}>

						</div>
						<div className={styles.menuItemsPublicPages}>

							{pagesMenuItems.length ?
								(
									<>
										{pagesMenuItems.map((item, i) => (
											<>
												<Link className={styles.menuItem} key={i} href={item.path!}>
													{item.emoji && (
														<p>{item.emoji}</p>
													)}
													<h5>{item.name}</h5>
												</Link>
												<PrimaryButton onClick={async () => {
													await pageHttpProvider.deletePage(item.path?.split("/")[2]!)
												}}>
													-
												</PrimaryButton>
											</>
										))}
									</>
								)
								: (
									<div className={styles.createPageContainer}>
										<h5>No pages, create one</h5>
										<CreatePageBadge onCreated={() => setSelectedWorkspace(selectedWorkspaceId)}/>
									</div>
								)}
						</div>
						<div className={styles.menuItems}>
							{bottomMenuItems.map((item, i) => {
								return item.path ? (
									<Link className={styles.menuItem} key={i} href={item.path}>
										{item.imagePath && (
											<Image
												src={item.imagePath}
												width={24}
												height={24}
												alt={item.name}
											/>
										)}
										<h6>{item.name}</h6>
									</Link>
								) : (
									<div className={styles.menuItem} key={i} onClick={item.onClick}>
										{item.imagePath && (
											<Image
												src={item.imagePath}
												width={24}
												height={24}
												alt={item.name}
											/>
										)}
										<h6>{item.name}</h6>
									</div>
								)
							})}
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