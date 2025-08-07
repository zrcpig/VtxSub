//AppDbContext.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MainApp.Todos;
using System.IO;

namespace MainApp.Data
{
    class AppDbContext : DbContext
    {
        // 数据库表集合
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoTag> TodoTags { get; set; }

        // 数据库文件路径
        private readonly string _dbPath;

        public AppDbContext()
        {
            // 获取本地应用数据目录 (LocalApplicationData)
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // 创建应用专属文件夹
            var appFolder = Path.Combine(localAppData, "TodoAssistant");
            Directory.CreateDirectory(appFolder);

            // 设置数据库文件路径
            _dbPath = Path.Combine(appFolder, "todos.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 配置SQLite数据库
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置TodoItem实体
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id); // 主键配置

                // 父子关系配置（自引用）
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Cascade); // 级联删除

                // 多对多标签关系
                entity.HasMany(e => e.Tags)
                      .WithMany(t => t.TodoItems) // 需在TodoTag添加反向导航
                      .UsingEntity(j => j.ToTable("TodoItemTags"));

                // 枚举映射为整数（更高效）
                entity.Property(e => e.Priority)
                      .HasConversion<int>(); // 存储为0,1,2,3

                entity.Property(e => e.Status)
                      .HasConversion<int>(); // 存储为0,1,2,3,4

                // 默认值配置
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 配置TodoTag实体
            modelBuilder.Entity<TodoTag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);

                // 添加反向导航（支持双向关系）
                entity.HasMany(t => t.TodoItems)
                      .WithMany(i => i.Tags);
            });
        }

    }
}
