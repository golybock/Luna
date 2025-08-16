import { useTypedSelector } from "./useTypedSelector";

export const useWorkspaces = () => useTypedSelector(state => state.workspaces);