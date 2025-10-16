import { WorkspaceUserDetailedView } from "@/models/workspace/WorkspaceUserDetailedView";

export interface WorkspaceDetailedView {
	id: string;
	name: string;
	ownerId: string;
	icon?: string;
	description?: string;
	defaultPermission: string;
	settings? : object;
	deletedAt?: string;
	users: WorkspaceUserDetailedView[];
}