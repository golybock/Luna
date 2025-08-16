import { IAuthView } from "@/types/auth/IAuthView";
import { HttpProviderBase } from "./httpProviderBase";

class AuthHttpProvider extends HttpProviderBase {

	constructor() {
		super()
	}

	async requestVerificationCode(email: string): Promise<void> {
		return this.post('/auth/requestverificationcode', { email: email });
	}

	async signIn(email: string, code: string): Promise<IAuthView> {
		return this.post<IAuthView>('/auth/signin', { email: email, code: code });
	}

	async loginGoogle(): Promise<void> {
		window.location.replace("http://localhost:7000/api/v1/Auth/LoginOauth2?provider=google");
	}

	async logout(): Promise<void> {
		return this.post('/auth/logout');
	}

	async checkAuth(): Promise<IAuthView | null> {
		try {
			return await this.get<IAuthView>('/auth/validate');
		} catch {
			return null;
		}
	}

}

export const authHttpProvider = new AuthHttpProvider();