using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Stupid.Test.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            ViewBag.inip = Common.WebApplyCommon.GetIPInfo();
            ViewBag.outip = Common.WebApplyCommon.GetOutIP();
            ViewBag.info = HttpClientHelper.HttpGet("http://1212.ip138.com/ic.asp", "");
            ViewBag.outuserip = Common.WebApplyCommon.GetOutUserIP();

            var test = "123456";
            ViewBag.endes = Common.EncipherCommon.EnDes(test, "");
            ViewBag.des = Common.EncipherCommon.DeDes(ViewBag.endes, "");
            return View();
        }

        public ActionResult Openlayer()
        {

            return View();
        }
    }
}