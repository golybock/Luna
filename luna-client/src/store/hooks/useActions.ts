import { rootActions } from "@/store/rootActions";
import { bindActionCreators } from "@reduxjs/toolkit";
import { useMemo } from "react";
import { useDispatch } from "react-redux";
import { AppDispatch } from "@/store/store";


export const useActions = () => {
	const dispatch = useDispatch<AppDispatch>();

	return useMemo(() => bindActionCreators(rootActions, dispatch), [dispatch]);
};
