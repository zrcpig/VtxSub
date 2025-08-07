//App.xaml.cs

using MainApp;
using MainApp.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using VtxSub.Scripts.TodoWd.Repositories;
using VtxSub.Scripts.TodoWd.Services;

namespace VtxSub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        // 添加 Startup 事件处理
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 空实现（仅用于解决 XAML 解析问题）
        }

        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. 配置依赖注入容器
            var services = new ServiceCollection();
            ConfigureServices(services);

            // 2. 构建服务提供程序
            ServiceProvider = services.BuildServiceProvider();

            // 3. 初始化数据库
            InitializeDatabase();

            // 4. 显示主窗口
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 配置数据库上下文
            services.AddDbContext<AppDbContext>();

            // 注册仓储
            services.AddScoped<ITodoItemRepository, TodoItemRepository>();
            services.AddScoped<ITodoTagRepository, TodoTagRepository>();

            // 注册服务
            services.AddScoped<ITodoService, TodoService>();

            // 注册窗口
            services.AddTransient<MainWindow>();
            services.AddTransient<TodoWindow>();
        }

        private void InitializeDatabase()
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 确保数据库已创建并应用迁移
            dbContext.Database.EnsureCreated();
        }
    }

}
