export class HttpProviderBase {
	private readonly baseUrl: string;

	constructor() {
		this.baseUrl = process.env.NEXT_PUBLIC_API_HOST ?? "http://localhost:8000/api/v1";
	}

	private async request<T>(
		endpoint: string,
		options: RequestInit = {}
	): Promise<T> {
		const url = `${this.baseUrl}${endpoint}`;

		const config: RequestInit = {
			headers: {
				'Content-Type': 'application/json',
				...options.headers,
			},
			credentials: 'include',
			...options,
		};

		const response = await fetch(url, config);

		if (response.status === 401) {
			// Сброс флага авторизации при 401
			window.dispatchEvent(new CustomEvent('auth:unauthorized'));
			throw new Error('Unauthorized');
		}

		if (!response.ok) {
			throw new Error(`HTTP error! status: ${response.status}`);
		}

		const text = await response.text();

		return text ? JSON.parse(text) : "" as T;
	}

	private async requestDirect<T>(
		endpoint: string,
		options: RequestInit = {}
	): Promise<T> {
		const url = `${endpoint}`;

		const config: RequestInit = {
			headers: {
				'Content-Type': 'application/json',
				...options.headers,
			},
			credentials: 'include',
			...options,
		};

		const response = await fetch(url, config);

		if (response.status === 401) {
			// Сброс флага авторизации при 401
			window.dispatchEvent(new CustomEvent('auth:unauthorized'));
			throw new Error('Unauthorized');
		}

		if (!response.ok) {
			throw new Error(`HTTP error! status: ${response.status}`);
		}

		return await response.json();
	}

	async get<T>(endpoint: string): Promise<T> {
		return this.request<T>(endpoint, {method: 'GET'});
	}

	async getDirect<T>(endpoint: string): Promise<T> {
		return this.requestDirect<T>(endpoint, {method: 'GET'});
	}

	async post<T>(endpoint: string, data?: any): Promise<T> {
		return this.request<T>(endpoint, {
			method: 'POST',
			body: data ? JSON.stringify(data) : undefined,
		});
	}

	async put<T>(endpoint: string, data?: any): Promise<T> {
		return this.request<T>(endpoint, {
			method: 'PUT',
			body: data ? JSON.stringify(data) : undefined,
		});
	}

	async delete<T>(endpoint: string): Promise<T> {
		return this.request<T>(endpoint, {method: 'DELETE'});
	}
}