using MainApp.Data;
using MainApp.Todos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VtxSub.Scripts.TodoWd.Repositories
{
    public interface ITodoItemRepository : IRepository<TodoItem>
    {
        Task<IEnumerable<TodoItem>> GetByParentIdAsync(int? parentId);
        Task<IEnumerable<TodoItem>> GetByTagAsync(string tagName);
        Task AddChildAsync(int parentId, TodoItem child);
    }

    public class TodoItemRepository : BaseRepository<TodoItem>, ITodoItemRepository
    {
        public TodoItemRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TodoItem>> GetByParentIdAsync(int? parentId)
        {
            return await _context.TodoItems
                .Include(i => i.Children)
                .Include(i => i.Tags)
                .Where(i => i.ParentId == parentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoItem>> GetByTagAsync(string tagName)
        {
            return await _context.TodoItems
                .Include(i => i.Tags)
                .Where(i => i.Tags.Any(t => t.Title == tagName))
                .ToListAsync();
        }

        public async Task AddChildAsync(int parentId, TodoItem child)
        {
            var parent = await GetByIdAsync(parentId);
            if (parent != null)
            {
                child.ParentId = parentId;
                await AddAsync(child);
            }
        }
    }
}
