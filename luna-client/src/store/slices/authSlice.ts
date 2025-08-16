import { IWorkspaceView } from "@/types/workspace/IWorkspaceView";
import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { workspaceHttpProvider } from "@/http/workspaceHttpProvider";
import { IAuthView } from "@/types/auth/IAuthView";
import { authHttpProvider } from "@/http/authHttpProvider";
import { userHttpProvider } from "@/http/userHttpProvider";
import IUserView from "@/types/auth/IUserView";

interface AuthState {
	isAuthenticated: boolean;
	codeRequested: boolean;
	codeRequestAt: number | null;
	user: IAuthView | null;
	isLoading: boolean;
}

const initialState: AuthState = {
	isAuthenticated: false,
	codeRequested: false,
	codeRequestAt: null,
	user: null,
	isLoading: false,
};

export const checkAuthentication = createAsyncThunk(
	"auth/checkAuthentication",
	async (): Promise<IAuthView | null> => {
		return await authHttpProvider.checkAuth();
	}
);

export const requestVerificationCode = createAsyncThunk(
	"auth/requestVerificationCode",
	async ({ email }: { email: string; }): Promise<void> => {
		return await authHttpProvider.requestVerificationCode(email);
	}
);

export const signIn = createAsyncThunk(
	"auth/signIn",
	async ({ email, code }: { email: string; code: string }): Promise<IAuthView> => {
		return await authHttpProvider.signIn(email, code);
	}
);


export const loginGoogle = createAsyncThunk(
	"auth/loginGoogle",
	async (): Promise<void> => {
		await authHttpProvider.loginGoogle();
	}
);

export const getUser = createAsyncThunk(
	"auth/getUser",
	async (_, { getState }): Promise<IUserView> => {
		const state = getState() as { auth: AuthState };
		if (!state.auth.user?.userId) {
			throw new Error("User ID not found");
		}
		return await userHttpProvider.getUser(state.auth.user.userId);
	}
);

export const logout = createAsyncThunk(
	"auth/logout",
	async (): Promise<void> => {
		try {
			await authHttpProvider.logout();
		} catch {
			// Игнорируем ошибки при выходе
		}
	}
);

export const authSlice = createSlice({
	name: "auth",
	initialState,
	reducers: {
		setLoading: (state, action: PayloadAction<boolean>) => {
			state.isLoading = action.payload;
		},
		handleUnauthorized: (state) => {
			state.user = null;
			state.isAuthenticated = false;
			state.codeRequested = false;
			state.codeRequestAt = null;
			state.isLoading = false;
		},
		resetCodeRequest: (state) => {
			state.codeRequested = false;
			state.codeRequestAt = null;
		}
	},
	extraReducers: (builder) => {
		builder
			.addCase(checkAuthentication.pending, (state) => {
				state.isLoading = true;
			})
			.addCase(checkAuthentication.fulfilled, (state, action) => {
				state.user = action.payload;
				state.isAuthenticated = !!action.payload;
				state.isLoading = false;
			})
			.addCase(checkAuthentication.rejected, (state) => {
				state.user = null;
				state.isAuthenticated = false;
				state.isLoading = false;
			})
			.addCase(requestVerificationCode.pending, (state) => {
				state.isLoading = true;
			})
			.addCase(requestVerificationCode.fulfilled, (state) => {
				state.codeRequestAt = Date.now();
				state.codeRequested = true;
				state.isLoading = false;
			})
			.addCase(requestVerificationCode.rejected, (state) => {
				state.user = null;
				state.isAuthenticated = false;
				state.codeRequested = false;
				state.codeRequestAt = null;
				state.isLoading = false;
			})
			.addCase(loginGoogle.pending, (state) => {
				state.isLoading = true;
			})
			.addCase(loginGoogle.fulfilled, (state, action) => {
				state.isAuthenticated = true;
				state.isLoading = false;
			})
			.addCase(loginGoogle.rejected, (state) => {
				state.user = null;
				state.isAuthenticated = false;
				state.isLoading = false;
			})
			.addCase(signIn.pending, (state) => {
				state.isLoading = true;
			})
			.addCase(signIn.fulfilled, (state, action) => {
				state.user = action.payload;
				state.isAuthenticated = true;
				state.codeRequestAt = null;
				state.codeRequested = false;
				state.isLoading = false;
			})
			.addCase(signIn.rejected, (state) => {
				state.user = null;
				state.isAuthenticated = false;
				state.codeRequestAt = null;
				state.codeRequested = false;
				state.isLoading = false;
			})
			.addCase(getUser.pending, (state) => {
			})
			.addCase(getUser.fulfilled, (state, action) => {
				if (state.user) {
					state.user.user = action.payload;
				}
				// state.isLoading = false;
			})
			.addCase(getUser.rejected, (state) => {
				state.user = null;
				// state.isAuthenticated = false;
				// state.isLoading = false;
			})

			// Logout
			.addCase(logout.pending, (state) => {
				state.isLoading = true;
			})
			.addCase(logout.fulfilled, (state) => {
				state.user = null;
				state.isAuthenticated = false;
				state.isLoading = false;
			})
			.addCase(logout.rejected, (state) => {
				// Даже при ошибке выходим
				state.user = null;
				state.isAuthenticated = false;
				state.isLoading = false;
			});
	}
});