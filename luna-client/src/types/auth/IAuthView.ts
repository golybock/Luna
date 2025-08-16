import IUserView from "@/types/auth/IUserView";

export interface IAuthView {
	userId: string;
	email: string;
	user: IUserView;
}