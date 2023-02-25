using System;
using Microsoft.EntityFrameworkCore;

namespace RefreshCache.Models
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }
    }
}

