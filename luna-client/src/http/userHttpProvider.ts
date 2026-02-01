import { HttpProviderBase } from "./httpProviderBase";
import UserView from "@/models/auth/UserView";
import { WorkspaceUserBlank } from "@/models/workspace/WorkspaceUserBlank";

class UserHttpProvider extends HttpProviderBase {

	constructor() {
		super()
	}

	async getUser(userId: string): Promise<UserView | null> {
		return this.get<UserView | null>(`/user/getUser?userId=${userId}`);
	}

	async getUsers(userIds: string[]): Promise<UserView[]> {

		let url = `/user/getUsers?`;

		userIds.forEach((userId) => {
			url += `userIds=${userId}&`;
		})

		url = url.slice(0, -1);

		return this.get<UserView[]>(url);
	}

	async updateUser(userBlank: WorkspaceUserBlank) {
		return this.post(`/user/updateUser`, userBlank);
	}
}

export const userHttpProvider = new UserHttpProvider();