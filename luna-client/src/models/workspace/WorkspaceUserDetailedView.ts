import UserView from "@/models/auth/UserView";

export interface WorkspaceUserDetailedView {
	id: string;
	userId: string;
	workspaceId: string;
	permissions: string[];
	invitedBy?: string;
	acceptedAt?: string;
	user: UserView;
}