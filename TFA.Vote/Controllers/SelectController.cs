using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TFA.Vote.Controllers
{
    public class SelectController :BaseController
    {
        /// <summary>
        /// 选择专家
        /// </summary>
        /// <returns></returns>

        public ActionResult Experts()
        {
            return View();
        }
        /// <summary>
        /// 选择项目
        /// </summary>
        /// <returns></returns>
        public ActionResult Projects()
        {
            var designYears = Config.Helper.Select<string>("select designyear from project where isdelete=0 group by designyear");
            ViewData["designYears"] = designYears;
            return View();
        }
    }
}
