using System.ComponentModel.DataAnnotations;
using Luna.Tools.SharedModels.Models;

namespace Luna.Tools.Validation;

public class PermissionsValidationAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is not string[] permissions)
		{
			return new ValidationResult("Разрешения должны быть массивом строк");
		}

		if (permissions.Length == 0)
		{
			return new ValidationResult("Пользователь должен иметь хотя бы одно разрешение");
		}

		// Проверяем, что все разрешения валидны
		string[] invalidPermissions = permissions.Where(p => !WorkspacePermissions.AllPermissions.Contains(p)).ToArray();
		if (invalidPermissions.Any())
		{
			return new ValidationResult($"Недопустимые разрешения: {string.Join(", ", invalidPermissions)}");
		}

		// Проверяем уникальность
		if (permissions.Length != permissions.Distinct().Count())
		{
			return new ValidationResult("Разрешения не должны дублироваться");
		}

		return ValidationResult.Success;
	}
}
