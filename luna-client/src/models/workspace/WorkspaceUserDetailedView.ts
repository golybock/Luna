import IUserView from "@/types/auth/IUserView";

export interface WorkspaceUserDetailedView {
	id: string;
	userId: string;
	workspaceId: string;
	permissions: string[];
	invitedBy?: string;
	acceptedAt?: string;
	user: IUserView;
}