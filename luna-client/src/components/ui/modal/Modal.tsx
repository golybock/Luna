import React, {useMemo, useRef, useState} from "react";
import styles from "./Modal.module.scss";
import Swiper from "@/components/ui/modal/swiper/Swiper";
import Image from "next/image";

interface ModalProps {
	children?: React.ReactNode;
	overlayClassName?: string;
	containerClassName?: string;
	crossClassName?: string;
	showSwiper?: boolean; // По умолчанию true, для обычных окон можно не писать
	showCloseButton?: boolean;
	closeModal: () => void;
}

const Modal: React.FC<ModalProps> = ({
	children,
	overlayClassName,
	containerClassName,
	crossClassName,
	showSwiper = true,
	showCloseButton = true,
	closeModal,
}) => {
	const [modalPosition, setModalPosition] = useState<number>(0);
	const [opacity, setOpacity] = useState<number>(1);

	const wrapperRef = useRef<HTMLDivElement>(null);

	// Функция для обновления позиции модального окна при свайпе
	const updateModalPosition = (position: number) => {
		setModalPosition(position);

		// Вычисляем прозрачность фона в зависимости от позиции
		// Начинаем уменьшать прозрачность, когда смещение достигает 50px
		const maxOpacityOffset = 150; // При каком смещении прозрачность станет 0
		const newOpacity = Math.max(0, 1 - (position / maxOpacityOffset));
		setOpacity(newOpacity);
	};

	// Стили для модального окна с учетом текущей позиции
	const containerStyle = {
		transform: `translateY(${modalPosition}px)`,
		transition: modalPosition === 0 ? "transform 0.5s ease-out" : "none"
	};

	// Стили для оверлея с учетом текущей прозрачности
	const overlayStyle = {
		backgroundColor: `rgba(0, 0, 0, ${opacity * 0.5})`,
		transition: modalPosition === 0 ? "background-color 0.5s ease-out" : "none"
	};

	return (
		<div className={`${styles.overlay} ${overlayClassName || ""}`} style={overlayStyle}>
			<div
				className={`${styles.container} ${containerClassName || ""}`}
				style={containerStyle}
				ref={wrapperRef}
			>
				{showSwiper && (
					<div className={styles.swiper}>
						<Swiper
							closeModal={closeModal}
							updateModalPosition={updateModalPosition}
						/>
					</div>
				)}
				{children}
				{showCloseButton && (
					<Image
						src="/icons/close_24.svg"
						className={styles.cross}
						onClick={closeModal}
						width={24}
						height={24}
						alt="close"
					/>
				)}
			</div>
		</div>
	);
};

export default Modal;