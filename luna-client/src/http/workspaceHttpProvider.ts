import { HttpProviderBase} from "./httpProviderBase";
import { IWorkspaceView } from "@/types/workspace/IWorkspaceView";
import { IWorkspaceBlank } from "@/types/workspace/IWorkspaceBlank";
import { IWorkspaceUserBlank } from "@/types/workspace/IWorkspaceUserBlank";
import { IWorkspaceDetailedView } from "@/types/workspace/IWorkspaceDetailedView";

class WorkspaceHttpProvider extends HttpProviderBase {

	constructor() {
		super()
	}

	async getWorkspace(workspaceId: string): Promise<IWorkspaceView> {
		return this.get<IWorkspaceView>(`/workspace/getWorkspace?workspaceId=${workspaceId}`);
	}

	async getWorkspaceDetailed(workspaceId: string): Promise<IWorkspaceDetailedView> {
		return this.get<IWorkspaceDetailedView>(`/workspace/getWorkspaceDetailed?workspaceId=${workspaceId}`);
	}

	async getAvailableWorkspaces(): Promise<IWorkspaceView[]> {
		return this.get<IWorkspaceView[]>("/Workspace/GetAvailableWorkspaces");
	}

	async getAvailableWorkspacesDetailed(): Promise<IWorkspaceDetailedView[]> {
		return this.get<IWorkspaceDetailedView[]>("/Workspace/GetAvailableWorkspacesDetailed");
	}


	async createWorkspace(workspaceBlank: IWorkspaceBlank): Promise<string> {
		return this.post(`/workspace/createWorkspace`, workspaceBlank);
	}

	async updateWorkspace(workspaceId: string, workspaceBlank: IWorkspaceBlank): Promise<void> {
		return this.put(`/workspace/updateWorkspace?workspaceId=${workspaceId}`, workspaceBlank);
	}

	async deleteWorkspace(workspaceId: string): Promise<void> {
		return this.delete(`/workspace/deleteWorkspace?workspaceId=${workspaceId}`);
	}

	async inviteUserToWorkspace(workspaceUserBlank: IWorkspaceUserBlank): Promise<string> {
		return this.post(`/workspace/inviteUserToWorkspace`, workspaceUserBlank);
	}

	async getWorkspaceInvite(inviteId: string): Promise<IWorkspaceUserBlank> {
		return this.get(`/workspace/inviteUserToWorkspace?inviteId=${inviteId}`);
	}

	async updateWorkspaceUser(workspaceUserId: string, workspaceUserBlank: IWorkspaceUserBlank): Promise<void> {
		return this.put(`/workspace/updateWorkspaceUser?workspaceUserId=${workspaceUserId}`, workspaceUserBlank);
	}

	async deleteWorkspaceUser(workspaceUserId: string): Promise<void> {
		return this.delete(`/workspace/deleteWorkspaceUser?workspaceUserId=${workspaceUserId}`);
	}
}

export const workspaceHttpProvider = new WorkspaceHttpProvider();