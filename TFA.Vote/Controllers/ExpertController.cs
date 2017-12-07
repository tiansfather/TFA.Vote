using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TFA.Vote.Models;
using ToolGood.ReadyGo;

namespace TFA.Vote.Controllers
{
    [User(Models.UserType.Expert)]
    public class ExpertController : BaseController
    {
        #region 首页  
        public ActionResult Index()
        {
            //获取当前用户的进行中评审
            var expert = Config.CurrentUser;
            var reviews = Config.Helper.CreateWhere<Review>()
                .Where(o => !o.IsDelete)
                .Where(o => o.ReviewStatus == ReviewStatus.Reviewing)
                .AddWhereSql(" awardid in (select awardid from awardexpert where expertid=@0)", expert.ID)
                .Select();

            return View(reviews);
        }
        /// <summary>
        /// 当前用户对某个评审是否已经提交
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CheckIfReviewed(int id)
        {
            var expert = Config.CurrentUser;
            var review = Config.Helper.SingleById<Review>(id);
            var currentreviewdetail = review.CurrentReviewDetail;
            if (currentreviewdetail == null)
            {
                return Error("不存在对应进行中评审");
            }
            if (currentreviewdetail.VoteDetails.Exists(o => o.ExpertID == expert.ID && o.FinishTime!=null))
            {
                return Error("您已提交过评审");
            }
            return SuccessRedirect("", "/Expert/Review?detailid=" + currentreviewdetail.ID);
        }
        #endregion

        #region 评审
        public ActionResult Review(int detailid)
        {
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            return View(reviewdetail);
        }
        /// <summary>
        /// 提交评审
        /// </summary>
        /// <param name="detailid"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Review(int detailid,FormCollection collection)
        {
            
            var publish = collection["publish"] == "1" ? true : false;
            var projectdetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReviewVoteExpertProjectDetail>>(collection["data"]);
            ReviewDetail.Review(detailid,projectdetails, publish);
            if (publish)
            {
                return SuccessRedirect("提交成功", "/Expert/");
            }
            else
            {
                return Success("暂存成功");
            }
        }
        #endregion
    }
}
