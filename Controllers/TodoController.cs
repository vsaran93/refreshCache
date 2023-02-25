using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RefreshCache.Models;
using RefreshCache.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RefreshCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : Controller
    {
        public readonly IMemoryCache _memoryCache;
        private readonly TodoService _todoService;

        public TodoController(IMemoryCache memoryCache, TodoService todoService)
        {
            _memoryCache = memoryCache;
            _todoService = todoService;
        }

        [HttpGet("all")]
        public IActionResult GetAllTodos()
        {
            try
            {
                var cachedItems = GetOrCreateRefreshCache<List<Todo>>("GET_TODO_ITEMS");
                return Ok(cachedItems);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T GetOrCreateRefreshCache<T>(string key)
        {
            try
            {
                T result = (T)_memoryCache.Get(key);
                if (result == null)
                {
                    result = RefreshCache<T>(key);
                }
                return result;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public T RefreshCache<T>(string key)
        {
            T results = default(T);
            
            if (key == "GET_TODO_ITEMS")
            {
                var dbResults = _todoService.GetAllTodoItems();
                results = (T)Convert.ChangeType(dbResults, typeof(T));
            }

            Timer timer = new Timer((object state) => TimeTick<T>(key, results), null, 2 * 60, 3 * 60);
            return results;
        }

        private void TimeTick<T>(string key, T data)
        {
            if (data != null)
            {
                var cacheEntryOptions = GetMemoryCacheEntryOptions(3 * 60);
                _memoryCache.Remove(key);
                _memoryCache.Set(key, data, cacheEntryOptions);
            }
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(int expiryTime)
        {
            return new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(expiryTime));
        }
    }
}

