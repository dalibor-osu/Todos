using Microsoft.AspNetCore.Mvc;
using Todos.Data.TodoAction;
using Todos.Database;
using Todos.Models;
using Todos.Utils;

namespace Todos.Controllers;

[Route("todo/")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly DatabaseHandler _databaseHandler = new();

    [HttpGet]
    public ActionResult GetItems([FromQuery] string state = "all")
    {
        List<TodoItem?> items = new();
        switch (state)
        {
            case "created":
            case "Created":
                _databaseHandler.GetItems(ref items, 1);
                return Ok(items);

            case "finished":
            case "Finished":
                _databaseHandler.GetItems(ref items, 2);
                return Ok(items);

            default:
                _databaseHandler.GetItems(ref items);
                return Ok(items);
        }
    }

    [HttpGet("{id}")]
    public ActionResult GetSingleItem(string id)
    {
        if (!Guid.TryParse(id, out Guid guid)) return BadRequest(new TodoAction(TodoActionType.InvalidId));

        TodoItem? item = null;
        switch (_databaseHandler.GetSingleItem(guid, ref item))
        {
            case DatabaseActionResult.NotFound:
                return NotFound(new TodoAction(TodoActionType.NotFound));

            case DatabaseActionResult.Success:
                return Ok(item);

            default:
                return NotFound(new TodoAction(TodoActionType.NotFound));
        }
    }

    [HttpPost]
    public async Task<ActionResult> PostNewItem()
    {
        TodoItem? item = null;
        TodoAction result = Parser.TryParseItem(await new StreamReader(Request.Body).ReadToEndAsync(), ref item);

        if (result.IsError) return BadRequest(result);

        switch (_databaseHandler.InsertNewItem(item))
        {
            case DatabaseActionResult.Success:
                return Ok(item);

            case DatabaseActionResult.AlreadyExists:
                return Conflict(new TodoAction(TodoActionType.AlreadyExists));

            default:
                return BadRequest();
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateItem(string id)
    {
        if (!Guid.TryParse(id, out Guid guid)) return BadRequest(new TodoAction(TodoActionType.InvalidId));

        TodoItem? item = null;
        TodoAction result = Parser.TryParseItem(await new StreamReader(Request.Body).ReadToEndAsync(), ref item);

        if (result.IsError) return BadRequest(result);

        if (item == null || item.Id != guid) return BadRequest(new TodoAction(TodoActionType.IdsNotMatch));

        switch (_databaseHandler.UpdateItem(item))
        {
            case DatabaseActionResult.Success:
                return Ok(item);

            case DatabaseActionResult.NotFound:
                return NotFound(new TodoAction(TodoActionType.NotFound));

            default:
                return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteItem(string id)
    {
        if (!Guid.TryParse(id, out Guid guid)) return BadRequest(new TodoAction(TodoActionType.InvalidId));

        switch (_databaseHandler.DeleteItem(guid))
        {
            case DatabaseActionResult.NotFound:
                return NotFound(new TodoAction(TodoActionType.NotFound));

            default:
                return Ok();
        }
    }
}