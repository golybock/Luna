import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { LightPageView } from "@/models/page/view/LightPageView";
import { pageHttpProvider } from "@/http/pageHttpProvider";

interface PagesState {
	selectedPageId: string | null;
	pages: LightPageView[],
	isFetchingPages: boolean;
}

const initialState: PagesState = {
	selectedPageId: null,
	pages: [],
	isFetchingPages: false,
};

export const pagesSlice = createSlice({
	name: "pages",
	initialState,
	reducers: {
		setSelectedPage: (state, action: PayloadAction<string | null>) => {
			state.selectedPageId = action.payload;
		},
		setPages: (state, action: PayloadAction<LightPageView[]>) => {
			state.pages = action.payload;
		},
		clearPages: (state) => {
			state.pages = [];
			state.selectedPageId = null;
		}
	},
	extraReducers: (builder) => {
		builder
			.addCase(getWorkspacePages.pending, (state) => {
				state.isFetchingPages = true;
			})
			.addCase(getWorkspacePages.fulfilled, (state, action: PayloadAction<LightPageView[]>) => {
				state.pages = action.payload;
				state.isFetchingPages = false;
			})
			.addCase(getWorkspacePages.rejected, (state) => {
				state.isFetchingPages = false;
			})
	}
});

export const getWorkspacePages = createAsyncThunk(
	"pagesSlice/getWorkspacePages",
	async (workspaceId: string): Promise<LightPageView[]> => {
		return await pageHttpProvider.getWorkspacePages(workspaceId);
	}
);