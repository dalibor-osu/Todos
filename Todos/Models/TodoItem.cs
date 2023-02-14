using System.Text.Json.Serialization;

namespace Todos.Models;

public record TodoItem
{
    public TodoItem(Guid id, string title, int state, string content)
    {
        Id = id;
        Title = title;
        State = state;
        Content = content;
    }

    [JsonPropertyName("Id")] public Guid Id { get; }
    [JsonPropertyName("Title")] public string Title { get; }
    [JsonPropertyName("State")] public int State { get; }
    [JsonPropertyName("Content")] public string Content { get; }

    public override string ToString()
    {
        return "{\n\t\"Id\": \"" + Id + "\",\n\t\"Title\": \"" + Title + "\",\n\t\"State\": " + State +
               ",\n\t\"Content\": \"" +
               Content + "\"\n}";
    }
}