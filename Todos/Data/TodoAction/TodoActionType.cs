namespace Todos.Data.TodoAction;

public enum TodoActionType
{
    InvalidId,
    InvalidTitle,
    InvalidState,
    InvalidContent,
    AlreadyExists,
    NotFound,
    InvalidBody,
    IdsNotMatch,
    Success
}