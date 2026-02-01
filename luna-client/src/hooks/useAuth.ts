import { useDispatch, useSelector } from 'react-redux';
import { useCallback } from 'react';
import { AppDispatch, TypeRootState } from '@/store/store';
import {
	checkAuthentication,
	requestVerificationCode,
	loginGoogle as loginGoogleAction,
	signIn as signInAction,
	getUser as getUserAction,
	logout as logoutAction
} from '@/store/slices/authSlice';

export const useAuth = () => {
	const dispatch = useDispatch<AppDispatch>();
	const { isAuthenticated, user, isLoading, codeRequested, codeRequestAt, requestedEmail } = useSelector((state: TypeRootState) => state.auth);

	const checkAuth = useCallback(() => {
		dispatch(checkAuthentication());
	}, [dispatch]);

	const requestCode = useCallback(async (email: string) => {
		const result = await dispatch(requestVerificationCode({ email }));
		if (requestVerificationCode.rejected.match(result)) {
			throw new Error('requestVerificationCode failed');
		}
	}, [dispatch]);

	const loginGoogle = useCallback(async () => {
		const result = await dispatch(loginGoogleAction());
		if (loginGoogleAction.rejected.match(result)) {
			throw new Error('Google login failed');
		}
	}, [dispatch]);

	const signIn = useCallback(async (email: string, code: string) => {
		const result = await dispatch(signInAction({ email, code }));
		if (signInAction.rejected.match(result)) {
			throw new Error('signInAction failed');
		}
	}, [dispatch]);

	const getUser = useCallback(async () => {
		const result = await dispatch(getUserAction());
		if (getUserAction.rejected.match(result)) {
			throw new Error('Failed to get user data');
		}
		return result.payload;
	}, [dispatch]);

	const logout = useCallback(async () => {
		await dispatch(logoutAction());
	}, [dispatch]);

	return {
		isAuthenticated,
		user,
		isLoading,
		codeRequested,
		requestedEmail,
		codeRequestAt,
		checkAuth,
		requestCode,
		loginGoogle,
		signIn,
		getUser,
		logout,
	};
};