using MainApp.Data;
using MainApp.Todos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace VtxSub.Scripts.TodoWd.Repositories
{
    public interface ITodoTagRepository : IRepository<TodoTag>
    {
        Task<TodoTag> GetByTitleAsync(string title);
    }

    public class TodoTagRepository : BaseRepository<TodoTag>, ITodoTagRepository
    {
        public TodoTagRepository(AppDbContext context) : base(context) { }

        public async Task<TodoTag> GetByTitleAsync(string title)
        {
            return await _context.TodoTags
                .FirstOrDefaultAsync(t => t.Title == title);
        }
    }
}

