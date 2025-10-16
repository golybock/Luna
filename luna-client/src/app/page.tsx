'use client';

import styles from "./Page.module.scss"
import Button from "@/ui/button/Button";
import Card from "@/ui/card/Card";
import Image from "next/image";
import { useRouter } from "next/navigation";

export default function LandingPage() {

	const router = useRouter();

	const handleAboutClick = () => {

	};

	const handleAuthClick = () => {
		router.push("/signIn");
	}

	return (
		<div className={styles.container}>

			<header className={styles.header}>
				<div className={styles.nav}>
					<div className={styles.logo}>
						<Image
							src={"/logo.png"}
							alt="logo"
							width={64}
							height={64}
						/>
						<h3>Luna</h3>
					</div>
				</div>
			</header>

			<main className={styles.main}>
				<section>
					<div>
						<h2>Пет проект с рабочими пространствами Notion-like</h2>
						<p>Реализован для портфолио и обучения</p>
						<div className={styles.heroActions}>
							<Button variant="primary" size="medium" onClick={handleAuthClick}>
								Войти
							</Button>
							<Button variant="ghost" size="medium" onClick={handleAboutClick}>
								Подробнее о проекте
							</Button>
						</div>
					</div>
					<Card elevated padding="medium">
						<div className={styles.mockupContent}>
							<h4>Notion-like</h4>
							<p>Рабочие пространства со страницами</p>
						</div>
					</Card>
				</section>

				<section>
					<h3>Особенности</h3>
					<div className={styles.featuresGrid}>
						<Card hover padding="medium">
							<h4>Блочный редактор</h4>
							<p>Контент страниц состоит из кастомных блоков</p>
						</Card>
						<Card hover padding="medium">
							<h4>Совместная работа</h4>
							<p>Поддерживает совместную работу по WebSocket</p>
						</Card>
						<Card hover padding="medium">
							<h4>Микросервисная архитектура</h4>
							<p>Проект разделен на связанные мало-зависимые сервисы</p>
						</Card>
						<Card hover padding="medium">
							<h4>Next.JS</h4>
							<p>Фронтенд написан целиком на Next.JS и TypeScript</p>
						</Card>
						<Card hover padding="medium">
							<h4>ASP.Net</h4>
							<p>Основной бэкенд фреймворк</p>
						</Card>
						<Card hover padding="medium">
							<h4>Next.JS</h4>
							<p>Фронтенд написан целиком на Next.JS и TypeScript</p>
						</Card>
						<Card hover padding="medium">
							<h4>Хранение данных</h4>
							<p>Данные хранятся в разных бд по необходимости</p>
							<ul>
								<li>
									<p>MongoDB - данные страниц, гибкая JSON структура с большим объемом данных</p>
								</li>
								<li>
									<p>PostgreSQL - данные требующие четкой структуры и высокой надежности</p>
								</li>
								<li>
									<p>Redis - кэш для токенов авторизации, прав доступа пользователей</p>
								</li>
								<li>
									<p>ElasticSearch - быстрый поиск по контенту страниц</p>
								</li>
							</ul>
						</Card>
					</div>
				</section>
			</main>

			<footer className={styles.footer}>
				<p>© 2025 Luna. Все права защищены.</p>
			</footer>
		</div>
	);
}