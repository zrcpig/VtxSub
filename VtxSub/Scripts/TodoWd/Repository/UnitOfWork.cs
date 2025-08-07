using MainApp.Data;
using System.Threading.Tasks;


namespace VtxSub.Scripts.TodoWd.Repositories
{
    public interface IUnitOfWork
    {
        ITodoItemRepository TodoItems { get; }
        ITodoTagRepository TodoTags { get; }
        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            TodoItems = new TodoItemRepository(context);
            TodoTags = new TodoTagRepository(context);
        }

        public ITodoItemRepository TodoItems { get; }
        public ITodoTagRepository TodoTags { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}