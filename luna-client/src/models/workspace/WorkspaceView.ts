import { WorkspaceUserView } from "@/models/workspace/WorkspaceUserView";

export interface WorkspaceView {
	id: string;
	name: string;
	ownerId: string;
	icon?: string;
	description?: string;
	defaultPermission: string;
	settings? : object;
	deletedAt?: string;
	users: WorkspaceUserView[];
}