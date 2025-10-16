import {
	FLUSH,
	PAUSE,
	PERSIST,
	PURGE,
	REGISTER,
	REHYDRATE,
	persistReducer,
	persistStore
} from "redux-persist";
import { combineReducers, configureStore } from "@reduxjs/toolkit";

import storage from "redux-persist/lib/storage";
import { workspacesSlice } from "@/store/slices/workspaceSlice";
import { authSlice } from "@/store/slices/authSlice";
import { pagesSlice } from "@/store/slices/pagesSlice";

const persistConfig = {
	key: "luna",
	storage
};

const rootReducer = combineReducers({
	workspaces: workspacesSlice.reducer,
	auth: authSlice.reducer,
	pages: pagesSlice.reducer,
});

const persistedReducer = persistReducer(persistConfig, rootReducer);

export const store = configureStore({
	reducer: persistedReducer,
	middleware: getDefaultMiddleware =>
		getDefaultMiddleware({
			serializableCheck: {
				ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER]
			}
		}),
	devTools: true
});

export const persistor = persistStore(store);

export type AppDispatch = typeof store.dispatch;
export type TypeRootState = ReturnType<typeof store.getState>;