using System;
using RefreshCache.Models;

namespace RefreshCache.Services
{
    public class TodoService
    {
        private readonly TodoDbContext _context;

        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        public List<Todo> GetAllTodoItems()
        {
            try
            {
                var todoItems = _context.Todos.ToList();
                return todoItems;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

