// RestWindow.xaml.cs
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MainApp.Views
{
    public partial class RestWindow : Window
    {
        private readonly DispatcherTimer _timer = new();
        private DateTime _startTime;
        private const int REQUIRED_REST_SECONDS = 60; // 需要休息60秒

        // 添加旋转动画实例引用
        private RotateTransform _rotateTransform;

        public RestWindow()
        {
            InitializeComponent();

            // 获取旋转动画实例
            _rotateTransform = (RotateTransform)FindName("RotateTransform");

            SetupEventHandlers();
            InitializeTimer();
        }

        private void SetupEventHandlers()
        {
            // 设置按钮点击事件
            ReturnButton.Click += ReturnButton_Click;

            // 窗口加载完成时初始化计时
            Loaded += (s, e) => StartRestTimer();
        }

        private void InitializeTimer()
        {
            // 创建计时器，每0.1秒更新一次
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
        }

        private void StartRestTimer()
        {
            _startTime = DateTime.Now;
            _timer.Start();

            // 初始状态设置
            ReturnButton.IsEnabled = false;
            TimerText.Text = "休息时间: 00:00";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 计算经过的时间
            TimeSpan elapsed = DateTime.Now - _startTime;

            // 更新计时器文本
            TimerText.Text = $"休息时间: {elapsed:mm\\:ss}";

            // 检查是否达到60秒
            if (!ReturnButton.IsEnabled && elapsed.TotalSeconds >= REQUIRED_REST_SECONDS)
            {
                EnableReturnButton();
            }
        }

        private void EnableReturnButton()
        {
            // 启用返回按钮
            ReturnButton.IsEnabled = true;

            // 启动装饰圆环动画
            StartRingAnimation();
        }

        private void StartRingAnimation()
        {
            if (_rotateTransform == null) return;

            // 创建圆环旋转动画
            DoubleAnimation rotationAnimation = new DoubleAnimation
            {
                To = 360,
                Duration = TimeSpan.FromSeconds(8),
                RepeatBehavior = RepeatBehavior.Forever
            };

            // 应用动画
            _rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            // 停止计时器
            _timer.Stop();

            // 停止旋转动画
            if (_rotateTransform != null)
            {
                _rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);
            }

            // 关闭窗口
            this.Close();
        }
    }
}