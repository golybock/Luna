import { workspacesSlice } from "@/store/slices/workspaceSlice";
import { authSlice } from "@/store/slices/authSlice";
import { pagesSlice } from "@/store/slices/pagesSlice";

export const rootActions = {
	...workspacesSlice.actions,
	...authSlice.actions,
	...pagesSlice.actions,
}