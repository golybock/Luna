import { HttpProviderBase } from "./httpProviderBase";
import IUserView from "@/types/auth/IUserView";
import { IWorkspaceUserBlank } from "@/types/workspace/IWorkspaceUserBlank";

class UserHttpProvider extends HttpProviderBase {

	constructor() {
		super()
	}

	async getUser(userId: string): Promise<IUserView | null> {
		return this.get<IUserView | null>(`/user/getUser?userId=${userId}`);
	}

	async getUsers(userIds: string[]): Promise<IUserView[]> {

		let url = `/user/getUsers?`;

		userIds.forEach((userId) => {
			url += `userIds=${userId}&`;
		})

		url = url.slice(0, -1);

		return this.get<IUserView[]>(url);
	}

	async updateUser(userBlank: IWorkspaceUserBlank) {
		return this.post(`/user/updateUser`, userBlank);
	}
}

export const userHttpProvider = new UserHttpProvider();