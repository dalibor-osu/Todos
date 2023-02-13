using System.Text.Json.Serialization;

namespace Todos.Data.TodoAction;

public class TodoAction
{
    [JsonPropertyName("IsError")] public bool IsError { get; }
    [JsonPropertyName("Error")] public TodoActionInfo Info { get; private set; }
    
    public TodoAction(TodoActionType type)
    {
        IsError = true;
        switch (type)
        {
            case TodoActionType.InvalidId:
                SetInfo(1, "Invalid Id");
                break;
            
            case TodoActionType.InvalidTitle:
                SetInfo(2, "Invalid Title");
                break;
            
            case TodoActionType.InvalidState:
                SetInfo(3, "Invalid State");
                break;
            
            case TodoActionType.InvalidContent:
                SetInfo(4, "Invalid Content");
                break;
            
            case TodoActionType.InvalidBody:
                SetInfo(5, "Invalid Body");
                break;
            
            case TodoActionType.NotFound:
                SetInfo(6, "Invalid State");
                break;
            
            case TodoActionType.IdsNotMatch:
                SetInfo(7, "Invalid State");
                break;
            
            case TodoActionType.AlreadyExists:
                SetInfo(8, "Invalid State");
                break;
            
            case TodoActionType.Success:
                IsError = false;
                break;
        }
    }

    private void SetInfo(int code, string message)
    {
        Info = new TodoActionInfo
        {
            Code = code,
            Message = message
        };
    }
}