﻿using HQ.Core.BLL.ShareZone;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HQ.ApiWeb.Controllers
{
    /// <summary>
    /// 好卷圈
    /// </summary>
    public class ZoneController : Controller
    {


        /// <summary>
        /// 分类
        /// </summary>
        /// <returns></returns>
        public ActionResult category()
        {
            String json = JsonConvert.SerializeObject(ShareZoneCatBLL.Instance.getZoneCatList());
            return Content(json, "application/json");
        }


        public ActionResult zoneList(int categoryId, int pageIndex, int pageSize)
        {
            String json = JsonConvert.SerializeObject(ShareZoneArticleBLL.Instance.listByCategoryId(categoryId, pageIndex, pageSize));
            return Content(json, "application/json");
        }
    }
}