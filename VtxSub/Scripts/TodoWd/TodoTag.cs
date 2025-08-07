//TodoTag.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Todos
{
    public class TodoTag
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // 新增反向导航属性
        public List<TodoItem> TodoItems { get; set; } = new();
    }
}
