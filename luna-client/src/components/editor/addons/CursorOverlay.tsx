import React, { useEffect, useState } from 'react';
import { UserCursorView } from '@/models/cursor/UserCursorView';
import styles from './CursorOverlay.module.scss';
import { useAuth } from "@/hooks/useAuth";

interface CursorOverlayProps {
	editorElement: HTMLDivElement;
	cursors: UserCursorView[];
}

interface CursorPosition {
	x: number;
	y: number;
	userName: string;
	userId: string;
	color: string;
}

// Генерируем стабильный цвет для пользователя на основе его ID
const getUserColor = (userId: string): string => {
	const colors = [
		'#FF6B6B',
		'#4ECDC4',
		'#45B7D1',
		'#FFA07A',
		'#98D8C8',
		'#F7DC6F',
		'#BB8FCE',
		'#85C1E2',
		'#F8B739',
		'#52B788',
	];

	// Создаем хеш из userId
	let hash = 0;
	for (let i = 0; i < userId.length; i++) {
		hash = userId.charCodeAt(i) + ((hash << 5) - hash);
	}

	return colors[Math.abs(hash) % colors.length];
};

export const CursorOverlay: React.FC<CursorOverlayProps> = ({ editorElement, cursors }) => {
	const [cursorPositions, setCursorPositions] = useState<CursorPosition[]>([]);

	const { user } = useAuth();

	useEffect(() => {
		const updateCursorPositions = () => {
			const positions: CursorPosition[] = [];

			cursors.forEach((cursor) => {
				try {
					const blockElement = editorElement.querySelector(
						`[data-id="${cursor.blockId}"]`
					) as HTMLElement;

					if (!blockElement) return;

					const textNode = getTextNodeFromBlock(blockElement);
					if (!textNode) {
						// Если нет текста, показываем курсор в начале блока
						const rect = blockElement.getBoundingClientRect();
						const editorRect = editorElement.getBoundingClientRect();

						positions.push({
							x: rect.left - editorRect.left,
							y: rect.top - editorRect.top,
							userName: cursor.user?.displayName || cursor.user?.username || 'Unknown',
							userId: cursor.userId,
							color: getUserColor(cursor.userId),
						});
						return;
					}

					const range = document.createRange();
					const textLength = textNode.textContent?.length || 0;
					const position = Math.min(cursor.position, textLength);

					range.setStart(textNode, position);
					range.setEnd(textNode, position);

					const rect = range.getBoundingClientRect();
					const editorRect = editorElement.getBoundingClientRect();

					positions.push({
						x: rect.left - editorRect.left,
						y: rect.top - editorRect.top,
						userName: cursor.user?.displayName || cursor.user?.username || 'Unknown',
						userId: cursor.userId,
						color: getUserColor(cursor.userId),
					});
				} catch (err) {
					console.error('[CursorOverlay] Error calculating cursor position:', err);
				}
			});

			setCursorPositions(positions);
		};

		const getTextNodeFromBlock = (blockElement: HTMLElement): Text | null => {
			const textContainer = blockElement.querySelector(
				'.ce-paragraph, .ce-header, .cdx-block'
			);

			if (!textContainer) return null;

			const findTextNode = (node: Node): Text | null => {
				if (node.nodeType === Node.TEXT_NODE) {
					return node as Text;
				}

				for (let i = 0; i < node.childNodes.length; i++) {
					const result = findTextNode(node.childNodes[i]);
					if (result) return result;
				}

				return null;
			};

			return findTextNode(textContainer);
		};

		updateCursorPositions();

		const handleUpdate = () => {
			requestAnimationFrame(updateCursorPositions);
		};

		window.addEventListener('scroll', handleUpdate, true);
		window.addEventListener('resize', handleUpdate);

		const interval = setInterval(updateCursorPositions, 500);

		return () => {
			window.removeEventListener('scroll', handleUpdate, true);
			window.removeEventListener('resize', handleUpdate);
			clearInterval(interval);
		};
	}, [cursors, editorElement]);

	return (
		<>
			{cursorPositions.filter(cur => cur.userId != user.userId).map((pos) => (
				<div
					key={pos.userId}
					className={styles.cursor}
					style={{
						left: `${pos.x}px`,
						top: `${pos.y}px`,
						borderColor: pos.color,
					}}
				>
					<div
						className={styles.cursorLabel}
						style={{ backgroundColor: pos.color }}
					>
						{pos.userName}
					</div>
				</div>
			))}
		</>
	);
};