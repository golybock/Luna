import UserView from "@/models/auth/UserView";

export interface AuthView {
	userId: string;
	email: string;
	user: UserView;
}