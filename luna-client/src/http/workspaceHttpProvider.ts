import { HttpProviderBase} from "./httpProviderBase";
import { WorkspaceView } from "@/models/workspace/WorkspaceView";
import { WorkspaceDetailedView } from "@/models/workspace/WorkspaceDetailedView";
import { WorkspaceUserBlank } from "@/models/workspace/WorkspaceUserBlank";
import { WorkspaceBlank } from "@/models/workspace/WorkspaceBlank";
import { InviteUserBlank } from "@/models/invite/InviteUserBlank";
import { InviteUserView } from "@/models/invite/InviteUserView";

class WorkspaceHttpProvider extends HttpProviderBase {

	constructor() {
		super()
	}

	async getWorkspace(workspaceId: string): Promise<WorkspaceView> {
		return this.get<WorkspaceView>(`/workspace/getWorkspace?workspaceId=${workspaceId}`);
	}

	async getWorkspaceDetailed(workspaceId: string): Promise<WorkspaceDetailedView> {
		return this.get<WorkspaceDetailedView>(`/workspace/getWorkspaceDetailed?workspaceId=${workspaceId}`);
	}

	async getAvailableWorkspaces(): Promise<WorkspaceView[]> {
		return this.get<WorkspaceView[]>("/Workspace/GetAvailableWorkspaces");
	}

	async createWorkspace(workspaceBlank: WorkspaceBlank): Promise<string> {
		return this.post(`/workspace/createWorkspace`, workspaceBlank);
	}

	async updateWorkspace(workspaceId: string, workspaceBlank: WorkspaceBlank): Promise<void> {
		return this.put(`/workspace/updateWorkspace?workspaceId=${workspaceId}`, workspaceBlank);
	}

	async deleteWorkspace(workspaceId: string): Promise<void> {
		return this.delete(`/workspace/deleteWorkspace?workspaceId=${workspaceId}`);
	}

	async inviteUserToWorkspace(inviteUserBlank: InviteUserBlank): Promise<InviteUserView> {
		return this.post<InviteUserView>(`/workspace/inviteUserToWorkspace`, inviteUserBlank);
	}

	async getWorkspaceByInvite(inviteId: string): Promise<WorkspaceView> {
		return this.get(`/workspace/GetWorkspaceByInvite?inviteId=${inviteId}`);
	}

	async acceptInvite(inviteId: string): Promise<void> {
		return this.post(`/workspace/AcceptInvite?inviteId=${inviteId}`);
	}

	async updateWorkspaceUser(workspaceUserId: string, workspaceUserBlank: WorkspaceUserBlank): Promise<void> {
		return this.put(`/workspace/updateWorkspaceUser?workspaceUserId=${workspaceUserId}`, workspaceUserBlank);
	}

	async deleteWorkspaceUser(workspaceUserId: string): Promise<void> {
		return this.delete(`/workspace/deleteWorkspaceUser?workspaceUserId=${workspaceUserId}`);
	}

	async getAvailableWorkspacePermissions()  : Promise<string[]> {
		return this.get(`/workspace/GetAvailableWorkspacePermissions`);
	}
}

export const workspaceHttpProvider = new WorkspaceHttpProvider();