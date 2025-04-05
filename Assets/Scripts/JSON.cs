using System;
using System.Text;

public static class JSON
{
    public static string ArrayToJson<T>(T[] array)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                sb.Append("null");
            else if (typeof(T) == typeof(string))
                sb.Append("\"").Append(array[i]).Append("\"");
            else if (typeof(T) == typeof(bool))
                sb.Append(array[i].ToString().ToLower());
            else
                sb.Append(array[i]);

            if (i < array.Length - 1)
                sb.Append(",");
        }

        sb.Append("]");
        return sb.ToString();
    }

    public static T[] FromJson<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new T[0];

        json = json.Trim('[', ']');
        string[] parts = json.Split(',');

        T[] result = new T[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            string value = parts[i].Trim();
            try
            {
                if (typeof(T) == typeof(string))
                    result[i] = (T)(object)value.Trim('"');
                else if (typeof(T) == typeof(bool))
                    result[i] = (T)(object)(value.ToLower() == "true");
                else
                    result[i] = (T)Convert.ChangeType(value, typeof(T));
            }
            catch (FormatException)
            {
                throw new InvalidCastException($"Failed to convert value '{value}' to type {typeof(T)} at index {i}.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while parsing JSON at index {i}: {ex.Message}");
            }
        }

        return result;
    }
}
