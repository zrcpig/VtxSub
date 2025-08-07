//TodoWindow.xaml.cs
using MainApp.Todos;
using System.Windows;
using MainApp.Data;
using VtxSub.Scripts.TodoWd.Services;


namespace MainApp
{
    public partial class TodoWindow : Window
    {
        private readonly ITodoService _todoService;

        public TodoWindow(ITodoService todoService)
        {
            InitializeComponent();
            _todoService = todoService;
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                StatusText.Text = "正在加载数据...";
                var rootItems = await _todoService.GetRootItemsAsync();
                TodoTreeView.ItemsSource = rootItems;
                StatusText.Text = $"已加载 {rootItems.Count()} 个任务";
            }
            catch (Exception ex)
            {
                StatusText.Text = "加载失败";
                System.Windows.MessageBox.Show($"加载数据时出错: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newItem = new TodoItem
                {
                    Title = $"新任务 {DateTime.Now:HH:mm:ss}",
                    Status = CompletionStatus.NotStarted,
                    Priority = PriorityLevel.Medium,
                    CreatedDate = DateTime.Now
                };

                await _todoService.CreateItemAsync(newItem);
                StatusText.Text = "已添加新任务";

                // 刷新数据
                await RefreshDataAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"添加任务失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDataAsync();
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                StatusText.Text = "正在刷新数据...";
                var rootItems = await _todoService.GetRootItemsAsync();
                TodoTreeView.ItemsSource = null;
                TodoTreeView.ItemsSource = rootItems;
                StatusText.Text = $"已刷新 {rootItems.Count()} 个任务";
            }
            catch (Exception ex)
            {
                StatusText.Text = "刷新失败";
                System.Windows.MessageBox.Show($"刷新数据时出错: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
