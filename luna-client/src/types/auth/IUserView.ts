export default interface IUserView {
	id: string;
	username: string;
	displayName?: string;
	image?: string;
	bio?: string;
	lastActive: Date;
}