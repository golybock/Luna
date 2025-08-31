'use client';

import styles from "../styles/global.scss"
import Button from "@/ui/button/Button";
import Card from "@/ui/card/Card";

export default function LandingPage() {
	return (
		<div className={styles.container}>
			{/* Header */}
			<header className={styles.header}>
				<div className={styles.nav}>
					<div className={styles.logo}>
						<h2>Luna</h2>
					</div>
					<div className={styles.navActions}>
						<Button variant="ghost" size="medium">Войти</Button>
					</div>
				</div>
			</header>

			{/* Hero Section */}
			<main className={styles.main}>
				<section className={styles.hero}>
					<div className={styles.heroContent}>
						<h1>Ваш цифровой рабочий стол для заметок и проектов</h1>
						<p>Создавайте, организуйте и делитесь знаниями с командой. Все в одном месте.</p>
						<div className={styles.heroActions}>
							<Button variant="primary" size="large">Начать бесплатно</Button>
							<Button variant="secondary" size="large">Посмотреть демо</Button>
						</div>
					</div>
					<div className={styles.heroImage}>
						<Card elevated padding="large">
							<div className={styles.mockupContent}>
								<h3>📝 Мои заметки</h3>
								<p>Быстрые заметки и идеи</p>
							</div>
						</Card>
					</div>
				</section>

				{/* Features */}
				<section className={styles.features}>
					<h2>Возможности</h2>
					<div className={styles.featuresGrid}>
						<Card hover padding="large">
							<h4>📄 Блочный редактор</h4>
							<p>Создавайте контент с помощью блоков</p>
						</Card>
						<Card hover padding="large">
							<h4>🚀 Быстрая работа</h4>
							<p>Молниеносная скорость загрузки</p>
						</Card>
						<Card hover padding="large">
							<h4>👥 Совместная работа</h4>
							<p>Работайте в команде над проектами</p>
						</Card>
					</div>
				</section>
			</main>

			{/* Footer */}
			<footer className={styles.footer}>
				<p>© 2024 Luna. Все права защищены.</p>
			</footer>
		</div>
	);
}