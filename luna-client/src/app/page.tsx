'use client';

import styles from "./Page.module.scss"
import Button from "@/ui/button/Button";
import Card from "@/ui/card/Card";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";

export default function LandingPage() {

	const router = useRouter();
	const containerRef = useRef<HTMLDivElement>(null);
	const [isScrolled, setIsScrolled] = useState(false);
	const [zoomedScheme, setZoomedScheme] = useState<string | null>(null);
	const [zoomLevel, setZoomLevel] = useState(1);

	const handleAuthClick = () => {
		router.push("/signIn");
	}

	useEffect(() => {
		const container = containerRef.current;
		if (!container) return;
		setIsScrolled(container.scrollTop > 24);
	}, []);

	useEffect(() => {
		if (!zoomedScheme) return;

		const handleKeyDown = (event: KeyboardEvent) => {
			if (event.key === "Escape") {
				setZoomedScheme(null);
			}
		};

		window.addEventListener("keydown", handleKeyDown);
		return () => window.removeEventListener("keydown", handleKeyDown);
	}, [zoomedScheme]);

	return (
		<div className={styles.container} ref={containerRef} onScroll={(event) => {
			const scrollTop = event.currentTarget.scrollTop;
			setIsScrolled(scrollTop > 24);
		}}>
			<header className={`${styles.header} ${isScrolled ? styles.headerScrolled : ""}`}>
				<div className={styles.nav}>
					<div className={styles.logo}>
						<Image
							src={"/icons/logo.png"}
							alt="logo"
							width={64}
							height={64}
						/>
						<h3>Luna</h3>
					</div>
					<div className={`${styles.headerActions} ${isScrolled ? styles.headerActionsVisible : ""}`}>
						<Button
							variant="ghost"
							size="small"
							className={styles.headerButton}
							href="https://github.com/golybock/Luna"
							target="_blank"
							rel="noreferrer"
						>
							Подробнее на github
						</Button>
						<Button
							variant="ghost"
							size="small"
							className={styles.headerButton}
							href="https://github.com/golybock"
							target="_blank"
							rel="noreferrer"
						>
							Профиль на github
						</Button>
						<Button
							variant="primary"
							size="small"
							className={styles.headerButton}
							onClick={handleAuthClick}
						>
							Войти
						</Button>
					</div>
				</div>
			</header>

			<main className={styles.main}>
				<section className={styles.hero}>
					<div className={styles.heroContent}>
						<span className={styles.eyebrow}>Notion-like платформа</span>
						<h1>Луна для команд: страницы, блоки, права, поиск</h1>
						<p>
							Проект в формате портфолио: микросервисы, CQRS, event-driven, gRPC и
							несколько хранилищ под разные задачи. UI вдохновлен Notion — большие
							блоки, чистая типографика, быстрый поиск.
						</p>
						<div className={styles.heroActions}>
							<Button variant="primary" size="medium" onClick={handleAuthClick}>
								Войти
							</Button>
							<Button
								variant="ghost"
								size="medium"
								href="https://github.com/golybock/Luna"
								target="_blank"
								rel="noreferrer"
							>
								Подробнее на github
							</Button>
							<Button
								variant="ghost"
								size="medium"
								href="https://github.com/golybock"
								target="_blank"
								rel="noreferrer"
							>
								Профиль на github
							</Button>
						</div>
						<div className={styles.heroMeta}>
							<span>Auth · Users · Workspaces · Pages · Notification</span>
							<span>Gateway (Ocelot) · Kafka/RabbitMQ · MongoDB · PostgreSQL · Redis · ElasticSearch</span>
						</div>
					</div>
					<Card elevated padding="large" className={styles.heroCard}>
						<div className={styles.mockupContent}>
							<h4>Notion-like редактор</h4>
							<p>
								Совместная работа, версии и блоки, быстрый поиск по содержимому
								через ElasticSearch.
							</p>
							<div className={styles.mockupHighlights}>
								<span>CQRS</span>
								<span>Outbox</span>
								<span>Search</span>
								<span>Realtime</span>
							</div>
						</div>
					</Card>
				</section>

				<section className={styles.section}>
					<h2>Основные технологии</h2>
					<div className={styles.techGrid}>
						<span>.NET 9</span>
						<span>Next.js</span>
						<span>TypeScript</span>
						<span>Ocelot</span>
						<span>Kafka</span>
						<span>RabbitMQ</span>
						<span>MongoDB</span>
						<span>PostgreSQL</span>
						<span>Redis</span>
						<span>ElasticSearch</span>
						<span>SignalR</span>
						<span>gRPC</span>
						<span>EF Core</span>
						<span>ADO.NET</span>
					</div>
				</section>

				<section className={styles.section}>
					<h2>Хранилища по назначению</h2>
					<p className={styles.sectionLead}>
						Каждая база используется там, где дает максимум пользы по производительности и структуре данных.
					</p>
					<div className={styles.dataGrid}>
						<Card hover padding="large">
							<h3>MongoDB</h3>
							<p>Страницы и TipTap‑документы с гибкой JSON‑структурой и большими объемами контента.</p>
						</Card>
						<Card hover padding="large">
							<h3>PostgreSQL</h3>
							<p>Права доступа, связи и данные, где нужна строгая схема и транзакции.</p>
						</Card>
						<Card hover padding="large">
							<h3>Redis</h3>
							<p>Кэш прав, сессии и refresh‑токены — быстро и дешево на чтение.</p>
						</Card>
						<Card hover padding="large">
							<h3>ElasticSearch</h3>
							<p>Полнотекстовый поиск по страницам и контенту с релевантностью и фильтрами.</p>
						</Card>
					</div>
				</section>

				<section className={styles.section}>
					<h2>Ключевые блоки</h2>
					<div className={styles.featuresGrid}>
						<Card hover padding="large">
							<h3>CQRS в Pages</h3>
							<p>Разделение команд и запросов, отдельные обработчики и репозитории.</p>
						</Card>
						<Card hover padding="large">
							<h3>Event-driven и шины</h3>
							<p>Kafka и RabbitMQ для асинхронных событий и фоновых задач.</p>
						</Card>
						<Card hover padding="large">
							<h3>Микросервисы</h3>
							<p>Auth, Users, Workspaces, Pages, Notification — изолированные домены.</p>
						</Card>
						<Card hover padding="large">
							<h3>Gateway и OAuth2</h3>
							<p>Ocelot Gateway, Google OAuth2, JWT + refresh токены.</p>
						</Card>
						<Card hover padding="large">
							<h3>Next.js + TypeScript</h3>
							<p>Современный интерфейс с упором на UX и читаемость блоков.</p>
						</Card>
						<Card hover padding="large">
							<h3>.NET 9 и gRPC</h3>
							<p>Быстрые сервисы и межсервисные вызовы, строгие контракты.</p>
						</Card>
						<Card hover padding="large">
							<h3>Realtime на SignalR</h3>
							<p>Работа со страницами через WebSocket и SignalR.</p>
						</Card>
						<Card hover padding="large">
							<h3>EF Core + ADO.NET</h3>
							<p>Используются оба подхода: ORM и ручные SQL-запросы.</p>
						</Card>
						<Card hover padding="large">
							<h3>Хранение по назначению</h3>
							<p>Каждая БД отвечает за свою задачу, без компромиссов.</p>
							<ul>
								<li>
									<p>MongoDB — страницы и блоки с гибкой структурой.</p>
								</li>
								<li>
									<p>PostgreSQL — права доступа и строгие связи.</p>
								</li>
								<li>
									<p>Redis — кэш прав и refresh токены.</p>
								</li>
								<li>
									<p>ElasticSearch — быстрый поиск по контенту.</p>
								</li>
							</ul>
						</Card>
					</div>
				</section>

				<section className={styles.section}>
					<h2>Архитектурные схемы</h2>
					<p className={styles.sectionLead}>
						Ключевые потоки и интеграции — от авторизации до индексации и realtime.
					</p>
					<div className={styles.schemeGrid}>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Auth + Refresh Flow</h3>
								<p>JWT/refresh, Redis и Google OAuth2.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/auth.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/auth.svg" alt="Auth flow scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Код авторизации и уведомления</h3>
								<p>Outbox → RabbitMQ → Notification → Email.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/auth-code-email.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/auth-code-email.svg" alt="Auth code email scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Gateway валидирует токены</h3>
								<p>Ocelot ходит в Auth и прокидывает custom headers.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/gateway.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/gateway.svg" alt="Gateway validation scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Permissions sync</h3>
								<p>Workspaces → Kafka → Pages → кэш и БД.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/permissions.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/permissions.svg" alt="Permissions scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Realtime editing</h3>
								<p>SignalR cursors + presence + Redis.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/realtime-edit.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/realtime-edit.svg" alt="Realtime edit scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>CQRS в Pages</h3>
								<p>Разделение команд/запросов + поиск.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/cqrs.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/cqrs.svg" alt="CQRS scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Индексация страниц</h3>
								<p>Outbox → ES: актуализация контента.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/search-index.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/search-index.svg" alt="Search index scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Поиск по контенту</h3>
								<p>ES → Mongo: получение результатов.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/search-result.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/search-result.svg" alt="Search result scheme"/>
							</button>
						</Card>
						<Card hover padding="large">
							<div className={styles.schemeText}>
								<h3>Обновление страницы по WS</h3>
								<p>Gateway → SignalR → CQRS → Outbox.</p>
							</div>
							<button className={styles.schemeButton} type="button"
								onClick={() => {
									setZoomedScheme("/schemes/update-page.svg");
									setZoomLevel(1);
								}}>
								<img src="/schemes/update-page.svg" alt="Update page scheme"/>
							</button>
						</Card>
					</div>
				</section>
			</main>

			{zoomedScheme && (
				<div className={styles.schemeOverlay} role="dialog" aria-modal="true"
					 onClick={() => setZoomedScheme(null)}>
					<div className={styles.schemeControls} onClick={(event) => event.stopPropagation()}>
						<button
							className={styles.schemeControlButton}
							type="button"
							onClick={() => setZoomLevel((value) => Math.max(0.5, Number((value - 0.1).toFixed(2))))}
						>
							–
						</button>
						<button
							className={styles.schemeControlButton}
							type="button"
							onClick={() => setZoomLevel(1)}
						>
							100%
						</button>
						<button
							className={styles.schemeControlButton}
							type="button"
							onClick={() => setZoomLevel((value) => Math.min(3, Number((value + 0.1).toFixed(2))))}
						>
							+
						</button>
					</div>
					<button
						className={styles.schemeClose}
						type="button"
						onClick={(event) => {
							event.stopPropagation();
							setZoomedScheme(null);
						}}
					>
						×
					</button>
					<div className={styles.schemeZoomedWrapper} onClick={(event) => event.stopPropagation()}>
						<img
							className={styles.schemeZoomed}
							style={{ transform: `scale(${zoomLevel})` }}
							src={zoomedScheme}
							alt="Scheme zoom"
						/>
					</div>
				</div>
			)}

			<footer className={styles.footer}>
				<p>© 2025 Luna. Все права защищены.</p>
			</footer>
		</div>
	);
}