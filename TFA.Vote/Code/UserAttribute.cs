namespace TFA.Vote
{
    using System;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UserAttribute : FilterAttribute, IAuthorizationFilter
    {
        public const string LoginUrl = "/Account/Login";
        public Models.UserType RequireUserType;

        public UserAttribute(Models.UserType usertype)
        {
            RequireUserType = usertype;
        }

        private void GotoLogin(AuthorizationContext filterContext,string url=LoginUrl)
        {
            if (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ContentResult result = new ContentResult {
                    Content = string.Format("<script>top.location.href = '{0}';</script>", url),
                    ContentType = "text/html"
                };
                filterContext.Result = result;
            }
            else
            {
                JsonResult result2 = new JsonResult {
                    Data = new { 
                        ok = false,
                        forwardUrl = url,
                        errCode = -1,
                        errMsg = "超时，请重新登入."
                    }
                };
                filterContext.Result = result2;
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Config.CurrentUser == null)
            {
                this.GotoLogin(filterContext);
            }else if (Config.CurrentUser.UserType != RequireUserType)
            {
                this.GotoLogin(filterContext,"/");
            }
        }
    }
}

