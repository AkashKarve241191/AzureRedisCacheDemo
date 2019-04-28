using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisCacheDemo.Models;

namespace RedisCacheDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedCache distributedCache;

        public HomeController(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public IActionResult Index()
        {
            var cacheValue = distributedCache.GetString("CacheTime");

            if (cacheValue == null)
            {
                cacheValue = DateTime.Now.ToString();

                // CACHE INVALIDATION POICY
                // CACHE WILL BE REMOVED, IF IT ISN'T TOUCH FOR 2 MINUTES

                var distributedCacheEntryOptions = new DistributedCacheEntryOptions();
                distributedCacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(2));

                distributedCache.SetString("CacheTime", cacheValue, distributedCacheEntryOptions);
            }

            ViewData["CacheTime"] = cacheValue;
            ViewData["CurrentTime"] = DateTime.Now.ToString();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
