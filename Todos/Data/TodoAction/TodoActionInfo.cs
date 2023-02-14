using System.Text.Json.Serialization;

namespace Todos.Data.TodoAction;

public record TodoActionInfo
{
    [JsonPropertyName("Code")] public int Code { get; init; }

    [JsonPropertyName("Message")] public string Message { get; init; }
}