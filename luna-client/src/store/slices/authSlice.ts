import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { authHttpProvider } from "@/http/authHttpProvider";
import { userHttpProvider } from "@/http/userHttpProvider";
import { AuthView } from "@/models/auth/AuthView";
import UserView from "@/models/auth/UserView";

interface AuthState {
	isAuthenticated: boolean;
	codeRequested: boolean;
	codeRequestAt: number | null;
	requestedEmail: string | null;
	user: AuthView | null;
	isLoading: boolean;
}

const initialState: AuthState = {
	isAuthenticated: false,
	codeRequested: false,
	codeRequestAt: null,
	requestedEmail: null,
	user: null,
	isLoading: false,
};

export const checkAuthentication = createAsyncThunk(
	"auth/checkAuthentication",
	async (): Promise<AuthView | null> => {
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
	async ({ email, code }: { email: string; code: string }): Promise<AuthView> => {
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
	async (_, { getState }): Promise<UserView> => {
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
			state.requestedEmail = null;
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
			.addCase(requestVerificationCode.fulfilled, (state, action) => {
				state.codeRequestAt = Date.now();
				state.codeRequested = true;
				state.requestedEmail = action.meta.arg.email;
				state.isLoading = false;
			})
			.addCase(signIn.fulfilled, (state, action) => {
				state.user = action.payload;
				state.isAuthenticated = true;
				state.codeRequestAt = null;
				state.codeRequested = false;
				state.requestedEmail = null;
				state.isLoading = false;
			})
			.addCase(signIn.rejected, (state) => {
				state.user = null;
				state.isAuthenticated = false;
				state.codeRequestAt = null;
				state.codeRequested = false;
				state.requestedEmail = null;
				state.isLoading = false;
			})
			.addCase(loginGoogle.pending, (state) => {
				state.isLoading = true;
			})
			.addCase(loginGoogle.fulfilled, (state) => {
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
			.addCase(getUser.pending, () => {
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