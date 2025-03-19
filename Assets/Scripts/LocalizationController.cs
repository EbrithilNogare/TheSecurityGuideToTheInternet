using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationController : MonoBehaviour {
    public void SetCzechLanguage() {
        SetLanguage("cs");
    }

    public void SetEnglishLanguage() {
        SetLanguage("en");
    }
    private void SetLanguage(string localeCode) {
        LoggingService.Log(LoggingService.LogCategory.Settings, "Language changed to: " + localeCode);

        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales) {
            if (locale.Identifier.Code == localeCode) {
                LocalizationSettings.SelectedLocale = locale;
                PlayerPrefs.SetString("selected-locale", localeCode);
                PlayerPrefs.Save();
                break;
            }
        }
    }
}