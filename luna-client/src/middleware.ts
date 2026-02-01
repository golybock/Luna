import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

// Публичные пути, доступные без авторизации
const publicPaths = new Set(['/', '/signIn']);

// Пути к ресурсам, всегда доступные
const resourcesPaths = ['/icons', '/resources', '/schemes', '/favicon.ico'];

export function middleware(request: NextRequest) {
	const { pathname } = request.nextUrl;

	// Проверка доступа к ресурсам - всегда разрешен
	if (resourcesPaths.some(path => pathname.startsWith(path))) {
		return NextResponse.next();
	}

	const isPublic = publicPaths.has(pathname);
	const isAuthenticated = Boolean(request.cookies.get('access_token')?.value);

	if (!isAuthenticated && !isPublic) {
		return NextResponse.redirect(new URL('/signIn', request.url));
	}

	// Запрещаем доступ к странице входа, редирект на /start
	if (isAuthenticated && pathname === '/signIn') {
		return NextResponse.redirect(new URL('/start', request.url));
	}

	return NextResponse.next();
}

export const config = {
	matcher: [
		'/((?!api|_next/static|_next/image|favicon.ico).*)',
	],
};