using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TFA.Vote.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Login/

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username,string password)
        {
            Models.User.Login(username, password);
            return SuccessRedirect("","/");
        }
        [HttpPost]
        public ActionResult ChangePassword(string oripassword,string newpassword,string repassword)
        {
            Models.User.ChangePassword(oripassword, newpassword, repassword);
            Session.Abandon();
            return SuccessRedirect("密码修改成功,请重新登录", "/");
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect("/");
        }
    }
}
