using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TFA.Vote.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var user = Config.CurrentUser;
            if (user == null)
            {
                return Redirect("/Account/Login");
            }else if (user.UserType == Models.UserType.Expert)
            {
                return Redirect("/Expert");
            }
            else if(user.UserType==Models.UserType.Manager)
            {
                return Redirect("/Manager");
            }
            return null;
        }

    }
}
