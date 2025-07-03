using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public static class LoggingService
{
    private const string NewRelicLogUrl = "https://log-api.eu.newrelic.com/log/v1";
    private static string ApiKey;
    private static int quotaLimiter = 10000;

    private static string sessionId;
    private static Environment environment;
    private static readonly Dictionary<string, int> errorCounts = new Dictionary<string, int>();
    private static readonly Dictionary<string, int> warningCounts = new Dictionary<string, int>();
    private const int MaxRepeats = 10;

    private enum LogLevel { Info, Warning, Error }
    private enum Environment { Development, Production }
    public enum LogCategory { LogMessageReceived, Navigation, Presentation, Minigame, Quiz, Level, PersonalDetails, Settings, Store }

    static LoggingService()
    {
        ApiKey = Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXUwMXh4MDlkNWJjMWJiZmIzYjAxNDA2NGM3ZmMyMDNGRkZGTlJBTA==")); // key is visible in request anyway
        sessionId = System.Guid.NewGuid().ToString();
        environment = Debug.isDebugBuild ? Environment.Development : Environment.Production;

        Application.logMessageReceived += (message, stackTrace, type) =>
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                if (!errorCounts.TryGetValue(message, out var count) || count < MaxRepeats)
                {
                    errorCounts[message] = count + 1;
                    SendLogsAsync(LogLevel.Error, LogCategory.LogMessageReceived, message, new Dictionary<string, string> { { "stackTrace", stackTrace } });
                }
                return;
            }

            if (type == LogType.Warning || type == LogType.Assert)
            {
                if (!warningCounts.TryGetValue(message, out var count) || count < MaxRepeats)
                {
                    warningCounts[message] = count + 1;
                    SendLogsAsync(LogLevel.Warning, LogCategory.LogMessageReceived, message, new Dictionary<string, string> { { "stackTrace", stackTrace } });
                }
            }
        };
    }

    private static string Sanitize(string message)
    {
        return message.Replace("'", "\"").Replace("\"", "\\\"");
    }

    private static async void SendLogsAsync(LogLevel logLevel, LogCategory category, string description, Dictionary<string, string> attributes = null)
    {
        if (quotaLimiter-- <= 0)
            return;

        if (environment == Environment.Development)
        {
            Debug.Log($"[LoggingService] {logLevel} {category} {description}");
            return;
        }

        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }

        attributes.Add("environment", environment.ToString());
        attributes.Add("level", logLevel.ToString());
        attributes.Add("sessionId", sessionId);
        attributes.Add("activeScene", SceneManager.GetActiveScene().name);
        attributes.Add("category", category.ToString());
        attributes.Add("platform", Application.platform.ToString());
        attributes.Add("version", Application.version);

        var attributesJson = new StringBuilder();
        bool first = true;
        foreach (var kvp in attributes)
        {
            if (first)
                first = false;
            else
                attributesJson.Append(",");

            attributesJson.Append($"\"{Sanitize(kvp.Key)}\":\"{Sanitize(kvp.Value)}\"");
        }

        var logData = $@"
{{
    ""message"": ""{Sanitize(description)}"",
    ""attributes"": {{ {attributesJson.ToString()} }}
}}";

        using (UnityWebRequest request = new UnityWebRequest(NewRelicLogUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(logData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Api-Key", ApiKey);

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();
        }
    }

    public static void Log(LogCategory category, string description)
    {
        SendLogsAsync(LogLevel.Info, category, description);
    }

    public async static void LogStartGame()
    {
        var moreData = new Dictionary<string, string>();

        if (Store.Instance.PersonalDataConcent)
        {
            // personal data, that need permisions (GDPR)
            moreData.Add("personalDataName", Store.Instance.PersonalDataName);
            moreData.Add("personalDataAge", Store.Instance.PersonalDataAge);
            moreData.Add("personalDataGender", Store.Instance.PersonalDataGender == 1 ? "Man" : Store.Instance.PersonalDataGender == 2 ? "Woman" : "");
            moreData.Add("personalDataClass", Store.Instance.PersonalDataClass);
            moreData.Add("personalDataRegion", Store.Instance.PersonalDataRegion);
        }

        var languageHandle = LocalizationSettings.SelectedLocaleAsync;
        var language = await languageHandle.Task;
        moreData.Add("language", language.LocaleName);

        SendLogsAsync(LogLevel.Info, LogCategory.Navigation, "Game Opened", moreData);
    }
}
