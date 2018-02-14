namespace ToDoApp.Common.Repository
{
    using System.Data.Entity;
    using Models;

    public class TodoDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
        public TodoDbContext() : base("name=ToDoAppDbConnectionString")
        {
        }

        public DbSet<Todo> Todoes { get; set; }

        public DbSet<ApplicationFeatureFlag> ApplicationFeaturesFlags { get; set; }
    }
}
