import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import { logout } from "@/store/slices/authSlice";
import { WorkspaceView } from "@/models/workspace/WorkspaceView";
import { PageStatisticView } from "@/models/page/PageStatisticView";
import { pageHttpProvider } from "@/http/pageHttpProvider";

interface WorkspaceState {
	selectedWorkspaceId: string | null;
	selectedWorkspacePageStatistic: PageStatisticView | null;
	workspaces: WorkspaceView[],
}

const initialState: WorkspaceState = {
	selectedWorkspaceId: null,
	selectedWorkspacePageStatistic: null,
	workspaces: [],
};

export const workspacesSlice = createSlice({
	name: "workspaces",
	initialState,
	reducers: {
		setSelectedWorkspace: (state, action: PayloadAction<string | null>) => {
			state.selectedWorkspaceId = action.payload;
		},
		setWorkspaces: (state, action: PayloadAction<WorkspaceView[]>) => {
			state.workspaces = action.payload;
		},
		clearWorkspaces: (state) => {
			state.workspaces = [];
			state.selectedWorkspacePageStatistic = null;
			state.selectedWorkspaceId = null;
		}
	},
	extraReducers: (builder) => {
		builder
			.addCase(getAvailableWorkspaces.pending, (state, action) => {
			})
			.addCase(getAvailableWorkspaces.fulfilled, (state, action: PayloadAction<WorkspaceView[]>) => {
				state.workspaces = action.payload;
			})
			.addCase(getAvailableWorkspaces.rejected, (state, action) => {
			})
			.addCase(getPageStatistic.pending, (state, action) => {
				state.selectedWorkspacePageStatistic = null;
			})
			.addCase(getPageStatistic.fulfilled, (state, action: PayloadAction<PageStatisticView>) => {
				state.selectedWorkspacePageStatistic = action.payload;
			})
			.addCase(getPageStatistic.rejected, (state, action) => {
				state.selectedWorkspacePageStatistic = null;
			})
			.addCase(logout.fulfilled, (state) => {
				state.workspaces = [];
				state.selectedWorkspacePageStatistic = null;
				state.selectedWorkspaceId = null;
			})
			.addCase(logout.rejected, (state) => {
				state.workspaces = [];
				state.selectedWorkspacePageStatistic = null;
				state.selectedWorkspaceId = null;
			});
	}
});

export const getAvailableWorkspaces = createAsyncThunk(
	"workspacesSlice/getAvailableWorkspaces",
	async (): Promise<WorkspaceView[]> => {
		return await workspaceHttpProvider.getAvailableWorkspaces();
	}
);

export const getPageStatistic = createAsyncThunk(
	"workspacesSlice/getPageStatistic",
	async (workspaceId: string): Promise<PageStatisticView> => {
		return await pageHttpProvider.getPageStatistic(workspaceId);
	}
);