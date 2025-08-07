//TodoItem.cs

using System;
using System.Text.Json.Serialization;

namespace MainApp.Todos
{
    public enum CompletionStatus
    {
        Default=0,
        NotStarted=1,
        InProgress=2,
        Completed=3,
        Cancelled= 4
    }

    public enum PriorityLevel
    {
        Low=0,
        Medium=1,
        High=2,
        Critical=3
    }

    public class TodoItem
    {
        //id：对象标识符
        public int Id { get; set; }
        //标题：动词
        public string Title { get; set; } = string.Empty;
        //任务主题：你是在做什么方面的工作
        public string Subject { get; set; } = string.Empty;
        //描述：详情
        public string Description { get; set; } = string.Empty;
        //创建日期：任务创建时间
        public System.DateTime CreatedDate { get; set; } = System.DateTime.Now;
        //目标日期：任务投递的日期
        public System.DateTime? TargetDate { get; set; }
        //到期日期：任务截止时间
        public System.DateTime? DueDate { get; set; }
        //优先级：任务的优先级
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        //完成状态：任务的完成状态
        public CompletionStatus Status { get; set; } = CompletionStatus.Default;

        // 父子关系
        public int? ParentId { get; set; }

        [JsonIgnore]
        public TodoItem? Parent { get; set; }
        public List<TodoItem> Children { get; set; } = new List<TodoItem>();

        // 标签
        public List<TodoTag> Tags { get; set; } = new();
    }
}
