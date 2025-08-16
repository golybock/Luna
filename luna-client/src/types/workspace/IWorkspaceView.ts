import { IWorkspaceUserView } from "@/types/workspace/IWorkspaceUserView";

export interface IWorkspaceView {
	id: string;
	name: string;
	ownerId: string;
	icon?: string;
	description?: string;
	visibility: string;
	defaultPermission: string;
	settings? : object;
	deletedAt?: string;
	users: IWorkspaceUserView[];
}