using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TFA.Vote.Controllers
{
    public class InitController : Controller
    {
        //
        // GET: /Init/

        public ActionResult Index()
        {
            Config.Helper.TableHelper.TryCreateTable<Models.User>();
            var user = Config.Helper.CreateWhere<Models.User>()
                .Where(o => o.UserName == "admin" && o.UserType == Models.UserType.Manager)
                .FirstOrDefault();
            if (user == null)
            {
                user = new Models.User()
                {
                    UserName="admin",
                    Password="123456".ToMd5Hash(),
                    UserType=Models.UserType.Manager,
                    RealName="科管"                     
                };
                Config.Helper.Save(user);
            }

            Config.Helper.TableHelper.TryCreateTable<Models.Project>();
            Config.Helper.TableHelper.TryCreateTable<Models.ProjectAttach>();

            Config.Helper.TableHelper.TryCreateTable<Models.Award>();
            Config.Helper.TableHelper.TryCreateTable<Models.AwardProject>();
            Config.Helper.TableHelper.TryCreateTable<Models.AwardExpert>();

            Config.Helper.TableHelper.TryCreateTable<Models.Review>();
            Config.Helper.TableHelper.TryCreateTable<Models.ReviewDetail>();
            return Content("OK");
        }

    }
}
