import { PropsWithChildren, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { AppDispatch, TypeRootState } from "@/store/store";
import { getAvailableWorkspaces } from "@/store/slices/workspaceSlice";
import { checkAuthentication, getUser } from "@/store/slices/authSlice";
import { Spinner } from "@/components/ui/spinner/Spinner";

export default function DataInitializer({ children }: PropsWithChildren) {

	const dispatch = useDispatch<AppDispatch>();
	const { isAuthenticated, isLoading: authLoading } = useSelector((state: TypeRootState) => state.auth);
	const { isFetchingWorkspaces } = useSelector((state: TypeRootState) => state.workspaces);

	// Проверка авторизации при первой загрузке
	useEffect(() => {
		dispatch(checkAuthentication());
	}, [dispatch]);

	// Загрузка воркспейсов после успешной авторизации
	useEffect(() => {
		if (isAuthenticated) {
			dispatch(getAvailableWorkspaces());
			dispatch(getUser())
		}
	}, [isAuthenticated, dispatch]);

	if (authLoading || isFetchingWorkspaces) {
		return (
			<Spinner/>
		);
	}

	return (
		<>
			{children}
		</>
	);
}
