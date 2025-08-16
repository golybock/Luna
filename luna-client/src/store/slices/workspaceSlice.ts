import { IWorkspaceView } from "@/types/workspace/IWorkspaceView";
import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import { logout } from "@/store/slices/authSlice";

interface WorkspaceState {
	selectedWorkspaceId: string | null;
	workspaces: IWorkspaceView[],
	isFetchingWorkspaces: boolean;
}

const initialState: WorkspaceState = {
	selectedWorkspaceId: null,
	workspaces: [],
	isFetchingWorkspaces: false,
};

export const workspacesSlice = createSlice({
	name: "workspaces",
	initialState,
	reducers: {
		setSelectedWorkspace: (state, action: PayloadAction<string | null>) => {
			state.selectedWorkspaceId = action.payload;
		},
		setWorkspaces: (state, action: PayloadAction<IWorkspaceView[]>) => {
			state.workspaces = action.payload;
		},
		clearWorkspaces: (state) => {
			state.workspaces = [];
			state.selectedWorkspaceId = null;
		}
	},
	extraReducers: (builder) => {
		builder
			.addCase(getAvailableWorkspaces.pending, (state, action) => {
				state.isFetchingWorkspaces = true;
			})
			.addCase(getAvailableWorkspaces.fulfilled, (state, action: PayloadAction<IWorkspaceView[]>) => {
				state.workspaces = action.payload;
				state.isFetchingWorkspaces = false;
			})
			.addCase(getAvailableWorkspaces.rejected, (state, action) => {
				state.isFetchingWorkspaces = false;
			})
			// Очистка при logout
			.addCase(logout.fulfilled, (state) => {
				state.workspaces = [];
				state.selectedWorkspaceId = null;
			})
			.addCase(logout.rejected, (state) => {
				// Очищаем даже при ошибке logout
				state.workspaces = [];
				state.selectedWorkspaceId = null;
			});
	}
});

export const getAvailableWorkspaces = createAsyncThunk(
	"workspacesSlice/getAvailableWorkspaces",
	async (): Promise<IWorkspaceView[]> => {
		console.log("load workspaces");
		return await workspaceHttpProvider.getAvailableWorkspaces();
	}
);
