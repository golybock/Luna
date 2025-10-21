import styles from "./Loading.module.scss";

export const Loading = () => {
	return (
		<div className={styles.container}>
			<div className={styles.loading}>
				{/*<loader>*/}
				<h2>Loading...</h2>
			</div>
		</div>
	)
}