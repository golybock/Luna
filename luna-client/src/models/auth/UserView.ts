export default interface UserView {
	id: string;
	username: string;
	displayName?: string;
	image?: string;
	bio?: string;
	lastActive: Date;
}