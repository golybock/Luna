using System.Reflection;
using System.Text.Json;
using Luna.Tools.SharedModels.Models;
using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Models.Domain.Models;

public class UserSettingsDomain : BaseUserSettings
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string? Timezone { get; set; }
	public string? Language { get; set; }

	public static UserSettingsDomain FromDatabase(UserSettingsDatabase userSettingsDatabase)
	{
		UserSettingsDomain domain = new UserSettingsDomain()
		{
			Id = userSettingsDatabase.Id,
			UserId = userSettingsDatabase.UserId,
			Timezone = userSettingsDatabase.Timezone,
			Language = userSettingsDatabase.Language
		};

		if (!string.IsNullOrEmpty(userSettingsDatabase.Settings))
		{
			try
			{
				Dictionary<string, JsonElement>? settingsDict =
					JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userSettingsDatabase.Settings);
				if (settingsDict != null)
				{
					MapDictionaryToBaseSettings(settingsDict, domain);
				}
			}
			catch (JsonException)
			{
				// Игнорируем ошибки десериализации
			}
		}

		return domain;
	}

	public static UserSettingsDomain FromBlank(UserSettingsBlank userSettingsBlank)
	{
		return new UserSettingsDomain()
		{
			Timezone = userSettingsBlank.Timezone,
			Language = userSettingsBlank.Language,
		};
	}

	public UserSettingsDatabase ToDatabase()
	{
		Dictionary<string, object?> settingsDict = new Dictionary<string, object?>();

		foreach (PropertyInfo property in BaseUserSettingsProperties)
		{
			object? value = property.GetValue(this);
			settingsDict[property.Name] = value;
		}

		return new UserSettingsDatabase()
		{
			Id = Id,
			UserId = UserId,
			Settings = JsonSerializer.Serialize(settingsDict),
			Timezone = Timezone,
			Language = Language
		};
	}

	public UserSettingsView ToView()
	{
		UserSettingsView view = new UserSettingsView()
		{
			Timezone = Timezone,
			Language = Language
		};

		MapBaseSettingsToBaseSettings(this, view);
		return view;
	}

	private static readonly PropertyInfo[] BaseUserSettingsProperties =
		typeof(BaseUserSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);

	private static void MapBaseSettingsToBaseSettings(BaseUserSettings source, BaseUserSettings target)
	{
		foreach (PropertyInfo property in BaseUserSettingsProperties)
		{
			object? value = property.GetValue(source);
			property.SetValue(target, value);
		}
	}

	private static void MapDictionaryToBaseSettings(Dictionary<string, JsonElement> settingsDict,
		BaseUserSettings target)
	{
		foreach (PropertyInfo property in BaseUserSettingsProperties)
		{
			if (settingsDict.TryGetValue(property.Name, out JsonElement jsonElement))
			{
				try
				{
					object? value = ConvertJsonElementToPropertyType(jsonElement, property.PropertyType);
					property.SetValue(target, value);
				}
				catch
				{
					// Игнорируем ошибки конвертации отдельных свойств
				}
			}
		}
	}

	private static object? ConvertJsonElementToPropertyType(JsonElement jsonElement, Type targetType)
	{
		if (jsonElement.ValueKind == JsonValueKind.Null)
			return null;

		Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

		return underlyingType switch
		{
			{ } t when t == typeof(string) => jsonElement.GetString(),
			{ } t when t == typeof(bool) => jsonElement.GetBoolean(),
			{ } t when t == typeof(int) => jsonElement.GetInt32(),
			{ } t when t == typeof(long) => jsonElement.GetInt64(),
			{ } t when t == typeof(double) => jsonElement.GetDouble(),
			{ } t when t == typeof(decimal) => jsonElement.GetDecimal(),
			{ } t when t == typeof(DateTime) => jsonElement.GetDateTime(),
			{ } t when t == typeof(Guid) => jsonElement.GetGuid(),
			_ => JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType)
		};
	}
}