export interface Status {
	label: string;
	tone: string;
}

export class Statuses {
	public static Paused: Status = { label: "paused", tone: "paused" };
	public static Leaving: Status = { label: "leaving", tone: "info" };
	public static Reconnecting: Status = { label: "reconnecting", tone: "connecting" };
	public static Connecting: Status = { label: "connecting", tone: "connecting" };
	public static Disconnecting: Status = { label: "disconnecting", tone: "info" };
	public static Fetching: Status = { label: "fetching", tone: "info" };
	public static PageDataReceived: Status = { label: "data received", tone: "info" };
	public static PageDataUpdated: Status = { label: "data updated", tone: "info" };
	public static Connected: Status = { label: "connected", tone: "ok" };
	public static UserJoined: Status = { label: "user joined", tone: "info" };
	public static UserLeft: Status = { label: "user left", tone: "info" };
	public static Error: Status = { label: "error", tone: "error" };
	public static Empty: Status = { label: null, tone: "info" };
}