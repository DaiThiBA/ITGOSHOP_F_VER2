﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITGoShop_F_Ver2.Models;
using PagedList;
using PagedList.Mvc;

namespace ITGoShop_F_Ver2.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult my_order()
        {
            return View();
        }
        public IActionResult order_detail()
        {
            return View();
        }
    }
}
