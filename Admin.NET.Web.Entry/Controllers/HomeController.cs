﻿using Admin.NET.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin.NET.Web.Entry.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        //private readonly ISystemService _systemService;

        //public HomeController(ISystemService systemService)
        //{
        //    _systemService = systemService;
        //}

        public IActionResult About()
        {
            ViewBag.Description = "11111111111";
         
            return View();
        }
		public IActionResult Index()
		{
			ViewBag.Description = "11111111111";
            //var rep = new SqlSugarRepository<HouseReport>();
            //var t=rep.AsQueryable().Where(i => i.UpdateTime == null).Take(3).ToList();
            return View();
		}
	}
}