using System.Text.Json;
using Todos.Data.TodoAction;
using Todos.Models;

namespace Todos.Utils;

public static class Parser
{
    private static readonly int[] ValidStates = {1, 2};
    
    public static TodoAction TryParseItem(string data, ref TodoItem? item)
    {
        if (!data.IsValidJson()) return new TodoAction(TodoActionType.InvalidBody);
        
        JsonDocument doc = JsonDocument.Parse(data);
        JsonElement root = doc.RootElement;
        
        if (!root.TryGetProperty("Id", out JsonElement idElement) || !Guid.TryParse(idElement.ToString(), out Guid id))
        {
            return new TodoAction(TodoActionType.InvalidId);
        }

        if (!root.TryGetProperty("Title", out JsonElement titleElement))
        {
            return new TodoAction(TodoActionType.InvalidTitle);
        }

        string title = titleElement.ToString();
        
        if (!root.TryGetProperty("State", out JsonElement stateElement) ||
            !int.TryParse(stateElement.ToString(), out int state) || 
            !ValidStates.Contains(state))
        {
            return new TodoAction(TodoActionType.InvalidState);
        }
        
        if (!root.TryGetProperty("Content", out JsonElement contentElement))
        {
            return new TodoAction(TodoActionType.InvalidContent);
        }
        
        string content = contentElement.ToString();
        
        item = new TodoItem(id, title, state, content);
        return new TodoAction(TodoActionType.Success);
    }
}