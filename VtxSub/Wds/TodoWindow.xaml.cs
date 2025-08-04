//TodoWindow.xaml.cs

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls; // 添加命名空间以访问控件

namespace MainApp
{
    public partial class TodoWindow : Window
    {
        public ObservableCollection<TodoItem> TodoItems { get; set; } = new();

        private bool _isExpanded = false;

        public TodoWindow()
        {
            InitializeComponent();
            TodoListView.ItemsSource = TodoItems;
            // 示例数据
            TodoItems.Add(new TodoItem { Title = "学习WPF", Description = "完成WPF基础教程", DueDate = DateTime.Today.AddDays(1) });
            TodoItems.Add(new TodoItem { Title = "写日报", Description = "提交日报", DueDate = DateTime.Today });
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _isExpanded = !_isExpanded;
            // 通过 FindName 获取控件引用，避免命名空间或生成问题
            var rightPanel = this.FindName("RightPanel") as Border;
            var toggleButton = this.FindName("ToggleButton") as System.Windows.Controls.Button;
            if (_isExpanded)
            {
                this.Width = 500;
                if (rightPanel != null) rightPanel.Visibility = Visibility.Visible;
                if (toggleButton != null) toggleButton.Content = "缩小";
            }
            else
            {
                this.Width = 200;
                if (rightPanel != null) rightPanel.Visibility = Visibility.Collapsed;
                if (toggleButton != null) toggleButton.Content = "放大";
            }
        }
    }
}
