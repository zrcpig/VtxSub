//TodoService.cs

using MainApp.Todos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VtxSub.Scripts.TodoWd.Repositories;

namespace VtxSub.Scripts.TodoWd.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoItemRepository _itemRepo;
        private readonly ITodoTagRepository _tagRepo;

        public TodoService(
            ITodoItemRepository itemRepo,
            ITodoTagRepository tagRepo)
        {
            _itemRepo = itemRepo;
            _tagRepo = tagRepo;
        }

        public async Task<TodoItem> GetItemAsync(int id)
        {
            return await _itemRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TodoItem>> GetItemsByParentAsync(int? parentId)
        {
            return await _itemRepo.GetByParentIdAsync(parentId);
        }

        public async Task<int> CreateItemAsync(TodoItem item)
        {
            await _itemRepo.AddAsync(item);
            return item.Id;
        }

        public async Task UpdateItemAsync(TodoItem item)
        {
            await _itemRepo.UpdateAsync(item);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _itemRepo.DeleteAsync(id);
        }

        public async Task MoveItemAsync(int itemId, int? newParentId)
        {
            var item = await _itemRepo.GetByIdAsync(itemId);
            if (item != null)
            {
                item.ParentId = newParentId;
                await _itemRepo.UpdateAsync(item);
            }
        }

        public async Task<IEnumerable<TodoTag>> GetAllTagsAsync()
        {
            return await _tagRepo.GetAllAsync();
        }

        public async Task<TodoTag> GetOrCreateTagAsync(string title)
        {
            var tag = await _tagRepo.GetByTitleAsync(title);
            if (tag == null)
            {
                tag = new TodoTag { Title = title };
                await _tagRepo.AddAsync(tag);
            }
            return tag;
        }

        public async Task AssignTagAsync(int itemId, string tagTitle)
        {
            var item = await _itemRepo.GetByIdAsync(itemId);
            var tag = await GetOrCreateTagAsync(tagTitle);

            if (item != null && !item.Tags.Any(t => t.Id == tag.Id))
            {
                item.Tags.Add(tag);
                await _itemRepo.UpdateAsync(item);
            }
        }

        public async Task RemoveTagAsync(int itemId, int tagId)
        {
            var item = await _itemRepo.GetByIdAsync(itemId);
            if (item != null)
            {
                var tagToRemove = item.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tagToRemove != null)
                {
                    item.Tags.Remove(tagToRemove);
                    await _itemRepo.UpdateAsync(item);
                }
            }
        }

        public async Task<TodoItem> GetItemTreeAsync(int itemId)
        {
            var item = await _itemRepo.GetByIdAsync(itemId);
            if (item != null)
            {
                await LoadChildrenRecursive(item);
            }
            return item;
        }

        private async Task LoadChildrenRecursive(TodoItem parent)
        {
            parent.Children = (await _itemRepo.GetByParentIdAsync(parent.Id)).ToList();
            foreach (var child in parent.Children)
            {
                await LoadChildrenRecursive(child);
            }
        }

        public async Task<IEnumerable<TodoItem>> GetRootItemsAsync()
        {
            return await _itemRepo.GetByParentIdAsync(null);
        }
    }
}