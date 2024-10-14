using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoggingService
{
    private const string NewRelicLogUrl = "https://log-api.eu.newrelic.com/log/v1?Api-Key=";
    private static string ApiKey;
    private static HttpClient client;

    private static string sessionId;
    private static Environment environment;
    private static bool permissionToLog;

    private enum LogLevel { INFO, ERROR }
    private enum Environment { DEVELOPMENT, PRODUCTION }
    public enum LogCategory { ERROR, NAVIGATION, MINIGAME, LEVEL }

    static LoggingService()
    {
        ApiKey = Encoding.UTF8.GetString(Convert.FromBase64String("ZXUwMXh4MDlkNWJjMWJiZmIzYjAxNDA2NGM3ZmMyMDNGRkZGTlJBTA=="));
        client = new HttpClient();
        sessionId = Guid.NewGuid().ToString();
        permissionToLog = true; // todo make it false and ask for it

#if UNITY_EDITOR
        environment = Environment.DEVELOPMENT;
#else
        environment = Environment.PRODUCTION;
#endif

        Application.logMessageReceived += (message, stackTrace, type) =>
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                _ = SendLogsAsync(LogLevel.ERROR, LogCategory.ERROR, message, new Dictionary<string, string> { { "stackTrace", stackTrace } });
            }
        };

    }

    private static string Sanitize(string message)
    {
        return message.Replace("'", "\"").Replace("\"", "\\\"");
    }

    private static async Task SendLogsAsync(LogLevel logLevel, LogCategory category, string description, Dictionary<string, string> attributes = null)
    {
        if (!permissionToLog) { return; }

        if (attributes == null)
        {
            attributes = new Dictionary<string, string>();
        }

        attributes.Add("environment", environment.ToString());
        attributes.Add("level", logLevel.ToString());
        attributes.Add("sessionId", sessionId);
        attributes.Add("activeScene", SceneManager.GetActiveScene().name);
        attributes.Add("category", category.ToString());

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

        var content = new StringContent(logData, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(NewRelicLogUrl + ApiKey, content);
        response.EnsureSuccessStatusCode();
        //Debug.Log("Logs: " + response.StatusCode);
    }
    public static void GrandPermissionToTrack(bool permission) { permissionToLog = permission; }

    public static void Log(LogCategory category, string description)
    {
        _ = SendLogsAsync(LogLevel.INFO, category, description);
    }


}
