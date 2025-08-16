import { workspacesSlice } from "@/store/slices/workspaceSlice";
import { authSlice } from "@/store/slices/authSlice";

export const rootActions = {
	...workspacesSlice.actions,
	...authSlice.actions,
}