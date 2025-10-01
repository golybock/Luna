namespace Luna.Tools.SharedModels.Models;

public static class WorkspacePermissions
{
	public const string View = "view";
	public const string Comment = "comment";
	public const string Edit = "edit";
	public const string Admin = "admin";

	public static readonly string[] AllPermissions = { View, Comment, Edit, Admin };

	// Иерархия разрешений: admin включает все остальные
	public static readonly Dictionary<string, string[]> PermissionHierarchy = new()
	{
		[Admin] = [View, Comment, Edit, Admin],
		[Edit] = [View, Comment, Edit],
		[Comment] = [View, Comment],
		[View] = [View]
	};
}