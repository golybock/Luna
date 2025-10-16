export interface WorkspaceUserView {
	id: string;
	userId: string;
	workspaceId: string;
	permissions: string[];
	invitedBy?: string;
	acceptedAt?: string;
}