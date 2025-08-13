using CaseBattleBackend.Enums;

namespace CaseBattleBackend.Helpers;

public static class LanguageHelper
{
    public static string GetLocalizedMessage(string resourceKey)
    {
        var resourceManager = Properties.Localization_ru.ResourceManager;

        var messageTemplate = resourceManager.GetString(resourceKey) ?? throw new Exception("Message not found.");

        return messageTemplate;
    }
}