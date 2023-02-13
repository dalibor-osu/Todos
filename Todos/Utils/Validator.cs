using Newtonsoft.Json;

namespace Todos.Utils;

public static class Validator
{
    /// <summary>
    /// Validates if string is a valid JSON.
    /// </summary>
    /// <param name="jsonString">string to validate</param>
    /// <returns>true if string is a valid JSON</returns>
    public static bool IsValidJson(this string jsonString)
    {
        bool success = true;
        var settings = new JsonSerializerSettings
        {
            Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
            MissingMemberHandling = MissingMemberHandling.Error
        };
        JsonConvert.DeserializeObject(jsonString, settings);
        return success;
    }
}