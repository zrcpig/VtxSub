//TodoItem.cs

using System;

namespace MainApp
{
    public class TodoItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public System.DateTime? DueDate { get; set; }
    }
}
