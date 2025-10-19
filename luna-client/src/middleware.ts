import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

const publicPaths = ['', '/signIn'];
const resourcesPaths = ['/icons', '/resources']

export function middleware(request: NextRequest) {
	const { pathname } = request.nextUrl;

	// const token = request.cookies.get('access_token')?.value;
	// const isAuthenticated = !!token;
	//
	// // Защищенные пути
	// if (!isAuthenticated && (!publicPaths.some(path => pathname == path)) && !resourcesPaths.some(path => pathname.startsWith(path))) {
	// 	return NextResponse.redirect(new URL('/signIn', request.url));
	// }
	//
	// // Редирект авторизованных пользователей
	// if (isAuthenticated && publicPaths.some(item => item === pathname)) {
	// 	return NextResponse.redirect(new URL('/start', request.url));
	// }

	return NextResponse.next();
}

export const config = {
	matcher: [
		'/((?!api|_next/static|_next/image|favicon.ico).*)',
	],
};