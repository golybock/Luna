import UserView from "@/models/auth/UserView";

export interface UserCursorView {
	blockId: string;
	position: number;
	userId: string;
	user?: UserView;
}