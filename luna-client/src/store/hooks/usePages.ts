import { useTypedSelector } from "./useTypedSelector";

export const usePages = () => useTypedSelector(state => state.pages);