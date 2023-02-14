using System.Text.Json;
using Todos.Data.TodoAction;
using Todos.Models;

namespace Todos.Utils;

public static class Parser
{
    private static readonly int[] ValidStates = { 1, 2 };

    public static TodoAction TryParseItem(string data, ref TodoItem? item)
    {
        if (!data.IsValidJson()) return new TodoAction(TodoActionType.InvalidBody);

        JsonDocument doc = JsonDocument.Parse(data);
        JsonElement root = doc.RootElement;

        if (!root.TryGetIdProperty(out Guid id))
            return new TodoAction(TodoActionType.InvalidId);

        if (!root.TryGetStringProperty("Title", out string title))
            return new TodoAction(TodoActionType.InvalidTitle);
        
        if (!root.TryGetStateProperty(out int state))
            return new TodoAction(TodoActionType.InvalidState);

        if (!root.TryGetStringProperty("Content", out string content))
            return new TodoAction(TodoActionType.InvalidContent);
            
        item = new TodoItem(id, title, state, content);
        return new TodoAction(TodoActionType.Success);
    }

    private static bool TryGetIdProperty(this JsonElement rootElement, out Guid id)
    {
        if (rootElement.TryGetProperty("Id", out JsonElement idElement) &&
            Guid.TryParse(idElement.ToString(), out id)) return true;
        
        id = Guid.Empty;
        return false;
    }
    private static bool TryGetStateProperty(this JsonElement rootElement, out int state)
    {
        if (rootElement.TryGetProperty("State", out JsonElement stateElement) &&
            int.TryParse(stateElement.ToString(), out state) &&
            ValidStates.Contains(state)) return true;
        
        state = 0;
        return false;

    }
    private static bool TryGetStringProperty(this JsonElement rootElement, string propertyName, out string value)
    {
        if (!rootElement.TryGetProperty(propertyName, out JsonElement element))
        {
            value = "";
            return false;
        }

        value = element.ToString();
        return true;
    }
}