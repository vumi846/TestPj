using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;

public class HomeController : Controller
{
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Login()
    {
        return View();
    }

    public JsonResult ChangeLanguage(string lang)
    {
        Session["culture"] = lang;
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);

        return Json("OK", JsonRequestBehavior.AllowGet);
    }
}
