import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

// Публичные пути, доступные без авторизации
const publicPaths = ['/', '/signIn'];

// Пути к ресурсам, всегда доступные
const resourcesPaths = ['/icons', '/resources'];

export function middleware(request: NextRequest) {
	const { pathname } = request.nextUrl;

	const token = request.cookies.get('access_token')?.value;
	const isAuthenticated = !!token;

	// Проверка доступа к ресурсам - всегда разрешен
	if (resourcesPaths.some(path => pathname.startsWith(path))) {
		return NextResponse.next();
	}

	if (!isAuthenticated) {
		// Разрешаем доступ только к публичным путям
		if (publicPaths.includes(pathname)) {
			return NextResponse.next();
		}

		return NextResponse.redirect(new URL('/signIn', request.url));
	}

	if (isAuthenticated) {
		// Запрещаем доступ к странице входа, редирект на /start
		if (pathname === '/signIn') {
			return NextResponse.redirect(new URL('/start', request.url));
		}

		return NextResponse.next();
	}

	return NextResponse.next();
}

export const config = {
	matcher: [
		'/((?!api|_next/static|_next/image|favicon.ico).*)',
	],
};