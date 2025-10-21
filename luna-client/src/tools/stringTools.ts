export function getInitials(displayName?: string, username?: string) {
	const name = displayName || username || '';
	return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
}

export function formatLastActive(lastActive: Date) {
	const now = new Date();
	const diff = now.getTime() - new Date(lastActive).getTime();
	const days = Math.floor(diff / (1000 * 60 * 60 * 24));

	if (days === 0) return 'Сегодня';
	if (days === 1) return 'Вчера';
	if (days < 7) return `${days} дн. назад`;
	return new Date(lastActive).toLocaleDateString('ru-RU');
}

export function formatDate (dateString?: string) {
	if (!dateString) return 'Never';
	return new Date(dateString).toLocaleDateString('ru-RU', {
		day: '2-digit',
		month: '2-digit',
		year: 'numeric'
	});
}