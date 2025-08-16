import { PropsWithChildren } from "react";
import { persistor, store } from "@/store/store";
import { Provider as ReduxProvider } from "react-redux";
import { PersistGate } from "redux-persist/integration/react";

export default function Providers({ children }: PropsWithChildren) {
	return (
		<ReduxProvider store={store}>
			<PersistGate loading={null} persistor={persistor}>
				{children}
			</PersistGate>
		</ReduxProvider>
	);
}
