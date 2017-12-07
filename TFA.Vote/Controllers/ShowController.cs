using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TFA.Vote.Models;
using ToolGood.ReadyGo;

namespace TFA.Vote.Controllers
{
    public class ShowController : Controller
    {
        //
        // GET: /Show/

        public ActionResult Project(int id)
        {
            var project = Config.Helper.SingleById<Project>(id);
            return View(project);
        }
        /// <summary>
        /// 项目打分详情
        /// </summary>
        /// <param name="detailid"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public ActionResult ProjectReviewDetail(int detailid,int projectid)
        {
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            var review = Config.Helper.SingleById<Review>(reviewdetail.ReviewID);
            var awardproject = Config.Helper.CreateWhere<AwardProject>()
                .Where(a => a.AwardID == review.AwardID && a.ProjectID == projectid)
                .Single();
            var projectdetails=reviewdetail.VoteDetails.SelectMany(o => o.VoteProjectDetail).Where(o => o.ProjectID == projectid).ToList();
            //如果是第一轮则加上基础分
            if(awardproject.BaseScore!=null && reviewdetail.Round==1 && reviewdetail.Turn==1 && reviewdetail.ReviewMethod != ReviewMethod.Vote)
            {
                ViewData["basescore"] = awardproject.BaseScore;
            }
            else
            {
                ViewData["basescore"] = "";
            }
            ViewData["reviewdetail"] = reviewdetail;
            return View(projectdetails);
        }

        public ActionResult PDF(string path)
        {
            ViewData["path"] = path;
            return View();
        }
        /// <summary>
        /// 查看回避专家
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult ExcludeExperts(string ids)
        {
            var experts = Config.Helper.CreateWhere<User>()
                .IfSet(ids).AddWhereSql("id in (" + ids + ")")
                .IfTrue(ids.IsNullOrEmpty()).Where(o=>o.ID==0)
                .Select();
            return View(experts);
        }
    }
}
