import React, { useState } from "react";
import styles from "./Swiper.module.scss";

interface SwiperProps {
	closeModal: () => void;
	updateModalPosition?: (position: number) => void;
}

const Swiper: React.FC<SwiperProps> = ({ closeModal, updateModalPosition }) => {
	const [touchStart, setTouchStart] = useState<null | number>(null);
	const [touchEnd, setTouchEnd] = useState<null | number>(null);
	const [isDragging, setIsDragging] = useState<boolean>(false);

	const minSwipeDistance = 150;

	const onTouchStart = (e: React.TouchEvent<HTMLDivElement>) => {
		setTouchEnd(null);
		setTouchStart(e.targetTouches[0].clientY);
		setIsDragging(true);
	};

	const onTouchMove = (e: React.TouchEvent<HTMLDivElement>) => {
		if (!touchStart || !isDragging) return;

		const currentY = e.targetTouches[0].clientY;
		setTouchEnd(currentY);

		// Вычисляем смещение (отрицательное при свайпе вверх, положительное при свайпе вниз)
		const offset = currentY - touchStart;

		// Ограничиваем максимальное смещение вниз
		const newPosition = Math.max(0, offset);

		// Передаем позицию родительскому компоненту
		if (updateModalPosition) {
			updateModalPosition(newPosition);
		}
	};

	const onTouchEnd = () => {
		setIsDragging(false);

		if (!touchStart || !touchEnd) {
			// Возвращаем модальное окно в исходное положение
			if (updateModalPosition) {
				updateModalPosition(0);
			}
			return;
		}

		const distance = touchStart - touchEnd;
		const isDownSwipe = distance < 0 && Math.abs(distance) > minSwipeDistance;

		if (isDownSwipe) {
			// Закрываем модальное окно при свайпе вниз
			closeModal();
		} else {
			// Возвращаем модальное окно в исходное положение
			if (updateModalPosition) {
				updateModalPosition(0);
			}
		}
	};

	return (
		<div
			className={styles.swiper}
			onTouchStart={onTouchStart}
			onTouchMove={onTouchMove}
			onTouchEnd={onTouchEnd}
		>
		</div>
	);
};

export default Swiper;