import IUserView from "@/types/auth/IUserView";

export interface IWorkspaceUserDetailedView {
	id: string;
	userId: string;
	workspaceId: string;
	roles: string[];
	permissions: string[];
	invitedBy?: string;
	acceptedAt?: string;
	user: IUserView;
}