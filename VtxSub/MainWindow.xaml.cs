//MainWindow.xaml.cs

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MainApp.Views;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon();
            InitTimer();
            InitializeTodoWindow();
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Debug.WriteLine(localAppData);

            // 订阅窗口加载完成事件
            Loaded += (s, e) =>
            {
                PositionWindow();
                StartWorkSession(); // 启动工作会话计时
            };
        }

        #region 窗口坐标设置
        private void PositionWindow()
        {
            // 获取屏幕工作区（排除任务栏）
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            // 考虑DPI缩放
            var dpi = VisualTreeHelper.GetDpi(this);
            double dpiScaleX = dpi.DpiScaleX;
            double dpiScaleY = dpi.DpiScaleY;

            // 获取窗口实际尺寸
            double windowWidth = ActualWidth;
            double windowHeight = ActualHeight;

            // 调试输出
            Debug.WriteLine($"屏幕工作区: {screenWidth}x{screenHeight} (DPI: {dpiScaleX}x{dpiScaleY})");
            Debug.WriteLine($"窗口尺寸: {windowWidth}x{windowHeight}");

            // 计算右下角位置（带10像素边距）
            const int margin = 0;
            /*            Left = (screenWidth / dpiScaleX) - windowWidth - margin;
                        Top = (screenHeight / dpiScaleY) - windowHeight - margin;
            */
            Left = screenWidth - windowWidth - margin;
            Top = screenHeight - windowHeight - margin;
            Debug.WriteLine($"设置位置: ({Left}, {Top})");

            // 确保窗口可见
            this.Visibility = Visibility.Visible;
            this.Topmost = true;
        }

        // 拖动整个窗口
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        #endregion




        #region 任务栏代码
        // 添加托盘图标
        private NotifyIcon _notifyIcon = null!;
        private bool _isExiting;


        private void InitializeTrayIcon()
        {
            try
            {
                // 创建托盘图标
                _notifyIcon = new NotifyIcon
                {
                    Icon = LoadCustomTrayIcon("/Assets/Icons/favicon.ico"), // 使用自定义图标
                    Text = "桌面精灵",
                    Visible = true
                };

                // 创建右键菜单
                var contextMenu = new ContextMenuStrip();

                // 添加菜单项
                contextMenu.Items.Add("显示窗口", null, (s, e) => ShowWindow());
                contextMenu.Items.Add(new ToolStripSeparator()); // 分隔线
                contextMenu.Items.Add("退出", null, (s, e) => ExitApplication());

                _notifyIcon.ContextMenuStrip = contextMenu;

                // 双击托盘图标显示窗口
                _notifyIcon.DoubleClick += (s, e) => ShowWindow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"托盘图标初始化失败: {ex.Message}");
            }
        }


        // 添加自定义图标加载方法
        private System.Drawing.Icon LoadCustomTrayIcon(string resourcePath)
        {
            try
            {
                // 构建资源URI（使用相对路径）
                var uri = new Uri(resourcePath, UriKind.Relative);

                // 获取资源流
                var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
                if (streamResourceInfo == null)
                    throw new FileNotFoundException($"图标资源未找到: {resourcePath}");

                // 从流创建图标
                using (var stream = streamResourceInfo.Stream)
                {
                    return new System.Drawing.Icon(stream);
                }
            }
            catch (Exception ex)
            {
                // 失败时使用默认图标并记录错误
                Debug.WriteLine($"加载自定义图标失败: {ex.Message}");
                return SystemIcons.Application;
            }
        }


        private void ShowWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Topmost = true;

            // 恢复待办窗口的原始状态
            if (_todoWindow != null && _todoWindowWasVisible)
            {
                PositionTodoWindowOnMinimized();
                _todoWindow.Show();
            }
        }

        private void ExitApplication()
        {
            _isExiting = true;

            // 关闭主窗口（触发正常关闭流程）
            this.Close();
        }


        #endregion

        #region 计时器

        // 添加休息窗口引用
        private RestWindow _restWindow;

        private DispatcherTimer _timer = null!;
        private DateTime _startTime;
        private const int TIMEOUT_SECONDS = 2700;


        private void InitTimer()
        {
            // 初始化计时器
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000); // 每1秒更新一次
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _startTime;
            TimerText.Text = elapsed.ToString(@"hh\:mm\:ss");

            if (elapsed.TotalSeconds >= TIMEOUT_SECONDS)
            {
                _timer.Stop();
                ShowRestWindow();
                ResetTimer();
            }
        }

        private void StartWorkSession()
        {
            if (!_timer.IsEnabled)
            {
                _startTime = DateTime.Now;
                _timer.Start();
            }
        }

        // 开始计时按钮点击
        private void RestButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                ShowRestWindow();
                ResetTimer();
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // 重置计时器
            _timer.Stop();
            ResetTimer();
            StartWorkSession(); // 重新开始工作会话
        }


        // 显示休息窗口
        private void ShowRestWindow()
        {
            // 确保只有一个休息窗口实例
            if (_restWindow == null || !_restWindow.IsActive)
            {
                _restWindow = new RestWindow();

                // 设置窗口位置（屏幕居中）
                _restWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // 订阅关闭事件以清理资源
                _restWindow.Closed += (s, e) => {
                    _restWindow = null;
                    this.IsEnabled = true; // 重新启用主窗口
                    StartWorkSession(); // 重新开始工作会话
                };

                // 临时禁用主窗口
                this.IsEnabled = false;

                // 显示休息窗口
                _restWindow.Show();
            }
        }


        // 重置计时器
        private void ResetTimer()
        {
            TimerText.Text = "00:00:00";

        }

        #endregion

        #region 待办事项窗口
        private TodoWindow _todoWindow = null!;
        private bool _todoWindowWasVisible = false;

        private void InitializeTodoWindow()
        {
            _todoWindow = new TodoWindow();
            //_todoWindow.Owner = this; // 设置主窗口为所有者
            // 修改此行以避免将 null 赋值给非 null 引用类型
            _todoWindow.Closed += (s, e) => _todoWindow = null!;// 窗口关闭时清空引用（可能有乍todo）
            _todoWindow.Hide();

            // 订阅位置变化事件
            LocationChanged += MainWindow_LocationChanged;

            // 添加主窗口渲染完成事件
            ContentRendered += MainWindow_ContentRendered;

        }


        // 主窗口内容渲染完成后设置Owner
        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            if (_todoWindow != null && _todoWindow.Owner == null)
            {
                _todoWindow.Owner = this;
            }
        }

        // 位置变化时同步待办窗口位置
        private void MainWindow_LocationChanged(object? sender, EventArgs e)
        {
            if (_todoWindow != null && _todoWindow.IsVisible)
            {
                PositionTodoWindowOnMinimized();
            }
        }

        private void PositionTodoWindowOnMinimized()
        {
            if (_todoWindow == null) return;

            // MainWindow的右边界
            double mainRight = this.Left + this.ActualWidth;
            // MainWindow的上边界
            double mainTop = this.Top;

            // TodoWindow的宽高
            double todoWidth = _todoWindow.ActualWidth;
            double todoHeight = _todoWindow.ActualHeight;

            // 设置TodoWindow的位置
            _todoWindow.Left = mainRight - todoWidth;
            _todoWindow.Top = mainTop - todoHeight;
        }

        // 显示/隐藏待办窗口
        private void ShowTodoWindow_Click(object sender, RoutedEventArgs e)
        {
            if (_todoWindow.IsVisible)
            {
                _todoWindow.Hide();
            }
            else
            {
                _todoWindow.Show();
                // 用 Dispatcher 延迟定位，确保窗口尺寸已更新
                _todoWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PositionTodoWindowOnMinimized();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }


        #endregion


        #region 全局方法
        // 退出应用
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        // 添加窗口隐藏功能
        private void HideWindow_Click(object sender, RoutedEventArgs e)
        {
            // 记录待办窗口当前状态
            if (_todoWindow != null)
            {
                _todoWindowWasVisible = _todoWindow.IsVisible;
                _todoWindow.Hide(); // 同时隐藏待办窗口
            }
            // 隐藏主窗口
            this.Hide();
        }

        // 关闭事件处理
        protected override void OnClosed(EventArgs e)
        {
            // 1. 确保关闭待办窗口
            _todoWindow?.Close();

            // 2. 释放托盘图标资源
            if (_notifyIcon != null)
            {
                _notifyIcon.Dispose();
                _notifyIcon = null!;
            }

            // 3. 停止计时器
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
                _timer = null!;
            }

            // 4. 调用基类方法

            base.OnClosed(e);

            // 5. 强制结束应用程序（确保完全退出）
            System.Windows.Application.Current.Shutdown();

        }

        #endregion
    }
}