// Scripts/TodoWd/Services/ITodoService.cs
using MainApp.Todos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VtxSub.Scripts.TodoWd.Services
{
    public interface ITodoService
    {
        // 任务管理
        Task<TodoItem> GetItemAsync(int id);
        Task<IEnumerable<TodoItem>> GetItemsByParentAsync(int? parentId);
        Task<int> CreateItemAsync(TodoItem item);
        Task UpdateItemAsync(TodoItem item);
        Task DeleteItemAsync(int id);
        Task MoveItemAsync(int itemId, int? newParentId);

        // 标签管理
        Task<IEnumerable<TodoTag>> GetAllTagsAsync();
        Task<TodoTag> GetOrCreateTagAsync(string title);
        Task AssignTagAsync(int itemId, string tagTitle);
        Task RemoveTagAsync(int itemId, int tagId);

        // 树形操作
        Task<TodoItem> GetItemTreeAsync(int itemId);
        Task<IEnumerable<TodoItem>> GetRootItemsAsync();
    }
}