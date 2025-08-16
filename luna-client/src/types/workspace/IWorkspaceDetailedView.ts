import { IWorkspaceUserDetailedView } from "@/types/workspace/IWorkspaceUserDetailedView";

export interface IWorkspaceDetailedView {
	id: string;
	name: string;
	ownerId: string;
	icon?: string;
	description?: string;
	visibility: string;
	defaultPermission: string;
	settings? : object;
	deletedAt?: string;
	users: IWorkspaceUserDetailedView[];
}