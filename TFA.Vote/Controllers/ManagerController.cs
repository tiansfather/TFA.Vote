using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TF.Common;
using TF.Common.ValueInjecter;
using TFA.Vote.Models;

namespace TFA.Vote.Controllers
{
    [User(Models.UserType.Manager)]
    public class ManagerController : BaseController
    {
        //
        // GET: /Manager/

        public ActionResult Index()
        {
            return View();
        }
        #region 专家账号管理
        public ActionResult ExpertManager_List()
        {
            return View();
        }
        public ActionResult ExpertManager_Add()
        {
            return View();
        }
        public ActionResult CheckName(string name)
        {
            var exist = Config.Helper.Exists<User>("where realname=@0 and isdelete=0", name);
            var result = new { exist = exist };
            return Json(result);
        }
        public ActionResult ExpertManager_Edit(int id)
        {
            var user = Config.Helper.SingleById<User>(id);
            return View(user);
        }
        [HttpPost]
        public ActionResult ExpertManager_Submit(Models.User model)
        {
            User expert = model;
            if (expert.ID > 0)
            {                
                expert = Config.Helper.SingleById<User>(expert.ID);
                var oripassword = expert.Password;
                expert.InjectFrom<RequestInjection>(Request);
                if (expert.Password.IsNullOrEmpty())
                {
                    expert.Password = oripassword;
                }
                else
                {
                    expert.Password= expert.Password.ToMd5Hash();
                }
            }
            else
            {
                expert.Password = expert.Password.ToMd5Hash();
            }
            
            Models.User.Save(expert);
            return SuccessCallback("提交成功", "top.layer.closeAll();top.loadData();");
        }
        public ActionResult ExpertManager_Del(int id)
        {
            Config.Helper.Update<User>("set isdelete=1 where id=@0", id);
            return SuccessCallback("提交成功", "top.layer.closeAll();top.loadData();");
        }
        #endregion

        #region 项目数据管理
        public ActionResult ProjectManager_List()
        {
            var designYears = Config.Helper.Select<string>("select designyear from project where isdelete=0 group by designyear");
            ViewData["designYears"] = designYears;
            return View();
        }
        /// <summary>
        /// 检测导入特征是否已存在
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public ActionResult ProjectManager_CheckTag(string tag)
        {
            var count = Config.Helper.Count<Models.Project>("where tag=@0", tag);
            if (count > 0)
            {
                return Error("error");
            }
            else
            {
                return Success();
            }
        }
        public ActionResult ProjectManager_Import(string tag,string path)
        {
            Models.Project.ImportFrom(tag, Server.MapPath(path));
            return SuccessCallback("导入成功","loadData();");
        }
        /// <summary>
        /// 项目详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProjectManager_View(int id)
        {
            var project = Config.Helper.SingleById<Models.Project>(id);
            return View(project);
        }
        public ActionResult ProjectManager_ViewFrame(int id)
        {
            var project = Config.Helper.SingleById<Models.Project>(id);
            return View(project);
        }
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProjectManager_Del(int id)
        {
            Config.Helper.Update<Project>("set isdelete=1 where id=@0", id);
            return SuccessCallback("提交成功", "top.layer.closeAll();top.loadData();");
        }
        /// <summary>
        /// 为项目添加附件
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult ProjectManager_Attach(int projectid,string filename,string path)
        {
            var file = new System.IO.FileInfo(Server.MapPath(path));
            var attach = new Models.ProjectAttach()
            {
                FileName = filename,
                FilePath = path,
                ProjectID = projectid,
                FileSize=file.Length
            };
            Config.Helper.Save(attach);
            return SuccessCallback("添加成功", "location.reload()");
        }
        public ActionResult ProjectManager_DelAttach(int id)
        {
            Config.Helper.DeleteById<ProjectAttach>(id);
            return Success("删除成功");
        }
        #endregion

        #region 奖项管理
        public ActionResult AwardManager_List()
        {
            return View();
        }
        public ActionResult AwardManager_Add(int id=0)
        {
            Award award = id == 0 ? null : Config.Helper.SingleById<Award>(id);
            return View(award);
        }
        public ActionResult AwardManager_Del(int id)
        {
            Config.Helper.Update<Award>("set isdelete=1 where id=@0", id);
            return SuccessCallback("提交成功", "top.layer.closeAll();top.loadData();");
        }
        [HttpPost]
        public ActionResult AwardManager_Submit(Models.Award award)
        {            
            if(award.ID==0 && Config.Helper.Count<Award>("where awardname=@0", award.AwardName) > 0)
            {
                return Error("相同奖项名称已经存在");
            }
            else
            {
                var projectids = Request.Form["projectids"];
                var expertids = Request.Form["expertids"];
                Award.Save(award, expertids, projectids);
                return SuccessCallback("保存成功", "location.href='/Manager/AwardManager_List'");
            }
        }
        #endregion

        #region 评审活动管理
        public ActionResult ReviewManager_List()
        {
            var awardlist = Config.Helper.CreateWhere<Award>()
                .Where(o => !o.IsDelete)
                .Select().ToList();
            ViewData["awardlist"] = awardlist;
            return View();
        }
        public ActionResult ReviewManager_Add()
        {
            var awardlist = Config.Helper.CreateWhere<Award>()
                .Where(o => !o.IsDelete)
                .Select().ToList();
            ViewData["awardlist"] = awardlist;
            return View();
        }        
        public ActionResult ReviewManager_Del(int id)
        {
            Config.Helper.Update<Review>("set isdelete=1 where id=@0", id);
            return SuccessCallback("提交成功", "top.layer.closeAll();top.loadData();");
        }
        [HttpPost]
        public ActionResult ReviewManager_Add(FormCollection collection)
        {
            var review = new Review();
            var reviewdetail = new ReviewDetail();
            var reviewmethodsetting = new ReviewMethodSetting();
            review.InjectFrom<RequestInjection>(Request);
            reviewdetail.InjectFrom<RequestInjection>(Request);
            reviewmethodsetting.InjectFrom<RequestInjection>(Request);
            reviewdetail.ReviewMethod = (ReviewMethod)Convert.ToInt32(Request.Form["reviewmethod"]);
            if (review.AwardID==0)
            {
                return Error("请选择奖项");
            }            
            if (review.ReviewName.IsNullOrEmpty())
            {
                return Error("请输入评审活动说明");
            }
            if (Config.Helper.Count<Review>("where awardid=@0 and isdelete=0 and reviewname=@1", review.AwardID, review.ReviewName) > 0)
            {
                return Error("相同评审活动名称已存在");
            }
            if (reviewdetail.TargetNumber == 0)
            {
                return Error("请输入目标数量");
            }
            if(reviewdetail.ReviewMethod==ReviewMethod.Average && (reviewmethodsetting.MaxScore==0 || reviewmethodsetting.MinScore==0 || reviewmethodsetting.ScoreStep==0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Weighting && (reviewmethodsetting.MaxScore == 0 || reviewmethodsetting.MinScore == 0 || reviewmethodsetting.ScoreStep == 0 || reviewmethodsetting.WeightLast==0 || reviewmethodsetting.WeightNow==0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Vote && (reviewmethodsetting.VoteNumber == 0 ))
            {
                return Error("请正确设置参数");
            }
            if (review.Award.ProjectCount < reviewdetail.TargetNumber)
            {
                return Error("目标数量不能大于参选项目总数");
            }
            var ispublish = collection["publish"] == "1";
            Review.AddReview(review, reviewdetail, reviewmethodsetting, ispublish);
            return SuccessCallback("提交成功", "location.href='/Manager/ReviewManager_List'");
        }
        /// <summary>
        /// 建立下轮次评审
        /// </summary>
        /// <param name="fromdetailid"></param>
        /// <param name="projectids"></param>
        /// <param name="reviewtype"></param>
        /// <returns></returns>
        public ActionResult ReviewManager_AddNext(int ReviewID, string projectids, string reviewtype)
        {
            var review = Config.Helper.SingleById<Review>(ReviewID);
            return View(review);
        }
        [HttpPost]
        public ActionResult ReviewManager_AddNext(FormCollection collection)
        {
            var reviewdetail = new ReviewDetail();            
            var reviewmethodsetting = new ReviewMethodSetting();
            reviewdetail.InjectFrom<RequestInjection>(Request);
            reviewmethodsetting.InjectFrom<RequestInjection>(Request);
            reviewdetail.ReviewMethod = (ReviewMethod)Convert.ToInt32(Request.Form["reviewmethod"]);
            var review = Config.Helper.SingleById<Review>(reviewdetail.ReviewID);
            if (reviewdetail.TargetNumber == 0)
            {
                return Error("请输入目标数量");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Average && (reviewmethodsetting.MaxScore <= 0 || reviewmethodsetting.MinScore <= 0 || reviewmethodsetting.ScoreStep <= 0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Weighting && (reviewmethodsetting.MaxScore <= 0 || reviewmethodsetting.MinScore <= 0 || reviewmethodsetting.ScoreStep <= 0 || reviewmethodsetting.WeightLast <= 0 || reviewmethodsetting.WeightNow <= 0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Weighting && ( reviewmethodsetting.WeightLast + reviewmethodsetting.WeightNow !=100))
            {
                return Error("本轮权重与上轮权重之和必须为100");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Vote && (reviewmethodsetting.VoteNumber <= 0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.SourceProjectIDs.Split(',').Length < reviewdetail.TargetNumber)
            {
                return Error("目标数量不能大于参选项目总数");
            }
            var ispublish = collection["publish"] == "1";
            Review.AddReview(review, reviewdetail, reviewmethodsetting, ispublish);
            return SuccessCallback("提交成功", "location.href='/Manager/ReviewManager_Summary?id="+review.ID+"'");
        }
        [HttpPost]
        public ActionResult ReviewManager_Edit(FormCollection collection)
        {
            var detailid = Convert.ToInt64(collection["id"]);
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            var reviewmethodsetting = new ReviewMethodSetting();
            reviewdetail.InjectFrom<RequestInjection>(Request);
            reviewmethodsetting.InjectFrom<RequestInjection>(Request);
            reviewdetail.ReviewMethod = (ReviewMethod)Convert.ToInt32(Request.Form["reviewmethod"]);

            if (reviewdetail.TargetNumber == 0)
            {
                return Error("请输入目标数量");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Average && (reviewmethodsetting.MaxScore <= 0 || reviewmethodsetting.MinScore <= 0 || reviewmethodsetting.ScoreStep <= 0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Weighting && (reviewmethodsetting.MaxScore <= 0 || reviewmethodsetting.MinScore <= 0 || reviewmethodsetting.ScoreStep <= 0 || reviewmethodsetting.WeightLast <= 0 || reviewmethodsetting.WeightNow <= 0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Weighting && (reviewmethodsetting.WeightLast + reviewmethodsetting.WeightNow != 100))
            {
                return Error("本轮权重与上轮权重之和必须为100");
            }
            if (reviewdetail.ReviewMethod == ReviewMethod.Vote && (reviewmethodsetting.VoteNumber <= 0))
            {
                return Error("请正确设置参数");
            }
            if (reviewdetail.SourceProjectIDs.Split(',').Count() < reviewdetail.TargetNumber)
            {
                return Error("目标数量不能小于参选项目总数");
            }
            var ispublish = collection["publish"] == "1";
            ReviewDetail.EditDetail(reviewdetail, reviewmethodsetting, ispublish);
            return SuccessCallback("提交成功", "location.href='/Manager/ReviewManager_Summary?id="+reviewdetail.ReviewID+"'");
        }
        /// <summary>
        /// 撤回评审
        /// </summary>
        /// <param name="detailid"></param>
        /// <returns></returns>
        public ActionResult ReviewManager_WithDraw(int detailid)
        {
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            if (reviewdetail.ReviewStatus == ReviewStatus.Reviewed)
            {
                return Error("此次评审已经全部结束,无法撤回");
            }
            //撤回
            ReviewDetail.WithDraw(reviewdetail);
            return SuccessRedirect("提交成功", "/Manager/ReviewManager_Summary?id=" + reviewdetail.ReviewID);
        }
        /// <summary>
        /// 评审总览
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReviewManager_Summary(int id)
        {
            var review = Config.Helper.SingleById<Review>(id);
            return View(review);
        }
        /// <summary>
        /// 查看单次评审详情，不同状态不同展示
        /// </summary>
        /// <param name="detailid"></param>
        /// <returns></returns>
        public ActionResult ReviewManager_View(int detailid)
        {
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            var viewname = "";
            switch (reviewdetail.ReviewStatus)
            {
                case ReviewStatus.BeforePublish:
                    viewname = "ReviewManager_ViewEdit";
                    break;
                case ReviewStatus.Reviewing:
                    viewname = "ReviewManager_ViewIng";
                    break;
                case ReviewStatus.Reviewed:
                    viewname = "ReviewManager_ViewFinish";
                    break;
            }
            return View(viewname, reviewdetail);
        }
        /// <summary>
        /// 全屏公示
        /// </summary>
        /// <param name="detailid"></param>
        /// <returns></returns>
        public ActionResult ReviewManager_FullView(int detailid)
        {
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            return View(reviewdetail);
        }
        #endregion

    }
}
