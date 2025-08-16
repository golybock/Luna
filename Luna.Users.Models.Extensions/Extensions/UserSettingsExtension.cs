using System.Reflection;
using System.Text.Json;
using Luna.Tools.SharedModels.Models;
using Luna.Users.Models.Blank.Models;
using Luna.Users.Models.Database.Models;
using Luna.Users.Models.Domain.Models;
using Luna.Users.Models.View.Models;

namespace Luna.Users.Services.Extensions;

public static class UserSettingsExtension
{
	private static readonly PropertyInfo[] BaseUserSettingsProperties =
		typeof(BaseUserSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);

	public static UserSettingsView ToView(this UserSettingsDatabase userSettings)
	{
		UserSettingsView view = new UserSettingsView()
		{
			Timezone = userSettings.Timezone,
			Language = userSettings.Language
		};

		if (!string.IsNullOrEmpty(userSettings.Settings))
		{
			try
			{
				Dictionary<string, JsonElement>? settingsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userSettings.Settings);
				if (settingsDict != null)
				{
					MapDictionaryToBaseSettings(settingsDict, view);
				}
			}
			catch (JsonException)
			{
				// Игнорируем ошибки десериализации
			}
		}


		return view;
	}

	public static UserSettingsView ToView(this UserSettingsDomain userSettings)
	{
		UserSettingsView view = new UserSettingsView()
		{
			Timezone = userSettings.Timezone,
			Language = userSettings.Language
		};

		MapBaseSettingsToBaseSettings(userSettings, view);
		return view;

	}

	public static UserSettingsDomain ToDomain(this UserSettingsDatabase userSettings)
	{
		UserSettingsDomain domain = new UserSettingsDomain()
		{
			Id = userSettings.Id,
			UserId = userSettings.UserId,
			Timezone = userSettings.Timezone,
			Language = userSettings.Language
		};

		if (!string.IsNullOrEmpty(userSettings.Settings))
		{
			try
			{
				Dictionary<string, JsonElement>? settingsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userSettings.Settings);
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

	public static UserSettingsDomain ToDomain(this UserSettingsBlank userSettings)
	{
		return new UserSettingsDomain()
		{
			Timezone = userSettings.Timezone,
			Language = userSettings.Language,
		};
	}

	public static UserSettingsDatabase ToDatabase(this UserSettingsDomain userSettings)
	{
		Dictionary<string, object?> settingsDict = new Dictionary<string, object?>();

		foreach (PropertyInfo property in BaseUserSettingsProperties)
		{
			object? value = property.GetValue(userSettings);
			settingsDict[property.Name] = value;
		}

		return new UserSettingsDatabase()
		{
			Id = userSettings.Id,
			UserId = userSettings.UserId,
			Settings = JsonSerializer.Serialize(settingsDict),
			Timezone = userSettings.Timezone,
			Language = userSettings.Language
		};
	}

	private static void MapBaseSettingsToBaseSettings(BaseUserSettings source, BaseUserSettings target)
	{
		foreach (var property in BaseUserSettingsProperties)
		{
			var value = property.GetValue(source);
			property.SetValue(target, value);
		}
	}

	private static void MapDictionaryToBaseSettings(Dictionary<string, JsonElement> settingsDict, BaseUserSettings target)
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
			Type t when t == typeof(string) => jsonElement.GetString(),
			Type t when t == typeof(bool) => jsonElement.GetBoolean(),
			Type t when t == typeof(int) => jsonElement.GetInt32(),
			Type t when t == typeof(long) => jsonElement.GetInt64(),
			Type t when t == typeof(double) => jsonElement.GetDouble(),
			Type t when t == typeof(decimal) => jsonElement.GetDecimal(),
			Type t when t == typeof(DateTime) => jsonElement.GetDateTime(),
			Type t when t == typeof(Guid) => jsonElement.GetGuid(),
			_ => JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType)
		};
	}
}