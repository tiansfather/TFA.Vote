using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TF.Common;
using TFA.Vote.Models;
using ToolGood.ReadyGo;

namespace TFA.Vote.Controllers
{
    public class DataController : BaseController
    {
        public ActionResult Upload()
        {
            var dic = Server.MapPath($"/upload/{DateTime.Now.ToString("yyyy/MM/dd")}/");
            if (!System.IO.Directory.Exists(dic))
            {
                System.IO.Directory.CreateDirectory(dic);
            }
            var file = Request.Files[0];
            var path = System.IO.Path.Combine($"/upload/{DateTime.Now.ToString("yyyy/MM/dd")}/", Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName));
            file.SaveAs(Server.MapPath(path));
            var obj = new {path=path,filename=file.FileName };
            return Success(obj);
        }
        /// <summary>
        /// 专家信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult Experts(int page,int limit,string field="RealName",string order="asc")
        {
            var username = Request.QueryString["username"];
            var realname = Request.QueryString["realname"];
            var enable = Request.QueryString["enable"];
            var exclude = Request.QueryString["exclude"];
            var limitin = Request.QueryString["limitin"];

            var pageData = Config.Helper.CreateWhere<Models.User>()
                .Where(o=>!o.IsDelete)
                .Where(o => o.UserType == Models.UserType.Expert)
                .IfSet(username).Where(o=>o.UserName.Contains(username))
                .IfTrue(enable=="true").Where(o=>o.Enable)
                .IfSet(exclude).AddWhereSql("id not in("+exclude+")")
                .IfSet(limitin).AddWhereSql("id in("+limitin+")")
                .IfSet(realname).Where(o=>o.RealName.Contains(realname))
                .AddOrderBySql(field+" "+ order)
                .Page(page, limit);
            var result = new { code = 0, msg = "", count = pageData.TotalItems, data = pageData.Items };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 项目数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult Projects(int page, int limit, string field = "id", string order = "asc")
        {
            var projectname = Request.QueryString["projectname"];
            var designyear = Request.QueryString["designyear"];
            var designcompany = Request.QueryString["designcompany"];
            var exclude = Request.QueryString["exclude"];
            var tag = Request.QueryString["i_designyear"] + "-" + Request.QueryString["i_tag"];

            var pageData = Config.Helper.CreateWhere<Models.Project>()
                .Where(o => !o.IsDelete)
                .IfSet(projectname).Where(o => o.ProjectName.Contains(projectname))
                .IfSet(designyear).Where(o => o.DesignYear==designyear)
                .IfSet(designcompany).Where(o=>o.DesignCompany.Contains(designcompany))
                .IfSet(exclude).AddWhereSql("id not in(" + exclude + ")")
                .IfTrue(tag!="-").Where(o=>o.Tag.Contains(tag))
                .AddOrderBySql(field + " " + order)
                .Page(page, limit);
            var result = new { code = 0, msg = "", count = pageData.TotalItems, data = pageData.Items };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 奖项信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult Awards(int page, int limit, string field = "id", string order = "desc")
        {
            var awardname = Request.QueryString["awardname"];

            var pageData = Config.Helper.CreateWhere<Models.Award>()
                .Where(o => !o.IsDelete)
                .IfSet(awardname).Where(o => o.AwardName.Contains(awardname))
                .AddOrderBySql(field + " " + order)
                .Page(page, limit);
            var result = new { code = 0, msg = "", count = pageData.TotalItems, data = pageData.Items };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 评审活动
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult Reviews(int page, int limit, string field = "id", string order = "desc")
        {
            var awardid = Request.QueryString["awardid"];
            var reviewname = Request.QueryString["reviewname"];

            var pageData = Config.Helper.CreateWhere<Models.Review>()
                .Where(o => !o.IsDelete)
                .IfSet(awardid).Where(o => o.AwardID==Convert.ToInt64(awardid))
                .IfSet(reviewname).Where(o => o.ReviewName.Contains(reviewname))
                .AddOrderBySql(field + " " + order)
                .Page(page, limit);
            var result = new { code = 0, msg = "", count = pageData.TotalItems, data = pageData.Items.Select(o=> {
                return new {o.ID, AwardName = o.Award.AwardName, o.ReviewName, ProjectCount = o.Award.ProjectCount, StartTime=o.StartTime?.ToString("yyyy-MM-dd HH:mm"), o.CurrentRoundTurn, o.ReviewStatus };
            }) };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 奖项的参选项目
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult AwardProjects(int page, int limit)
        {
            var awardid = Request.QueryString["awardid"];
            var detailid = Request.QueryString["detailid"];
            var sourceprojectids = Request.QueryString["projectids"];
            if (!detailid.IsNullOrEmpty())
            {
                var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
                awardid = Config.Helper.SingleById<Review>(reviewdetail.ReviewID).AwardID.ToString();
                sourceprojectids = reviewdetail.SourceProjectIDs;
            }
            var pageData = Config.Helper.CreateWhere<Models.AwardProject>()
                .Where(o => !o.IsDelete)
                .IfSet(awardid).Where(o => o.AwardID==Convert.ToInt64(awardid))
                .IfSet(sourceprojectids).AddWhereSql("projectid in("+ sourceprojectids + ")")
                .Page(page, limit);
            var result = new { code = 0, msg = "", count = pageData.TotalItems, data = pageData.Items.Select(o=> {
                var project = Config.Helper.SingleById<Project>(o.ProjectID);
                return new {o.BaseScore, o.Sort,project.ProjectName,project.BuildingCompany,project.BuildingType,project.DesignCompany,project.DesignYear,project.ID,o.ExcludeExpertIDs};
            }) };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 评审的所有项目得分详情
        /// </summary>
        /// <param name="detailid"></param>
        /// <returns></returns>
        public ActionResult ReviewProjectDetail(int detailid)
        {
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            var sourceprojectids = reviewdetail.SourceProjectIDs;
            var data = reviewdetail.GetProjectRanks().Where(o => reviewdetail.SourceProjectIDs.Split(',').ToList().Contains(o.ID.ToString())).ToList();
            //进行同分判定
            var linescore = data[reviewdetail.TargetNumber - 1].TotalScore;//界限分
            
            for (var i = 0; i < data.Count; i++)
            {
                var obj = data[i];
                obj.Rank = i + 1;
            }
            if (data.Count(o => o.TotalScore == linescore) > 1 && data.Count(o=>o.TotalScore>=linescore)>reviewdetail.TargetNumber)
            {
                data.Where(o => o.TotalScore == linescore).ForEach(o => o.NeedConfirm = true);
            }
            var result = new
            {
                code = 0,
                msg = "",
                count = data.Count,
                data = data
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
