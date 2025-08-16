export interface IWorkspaceUserView {
	id: string;
	userId: string;
	workspaceId: string;
	roles: string[];
	permissions: string[];
	invitedBy?: string;
	acceptedAt?: string;
}