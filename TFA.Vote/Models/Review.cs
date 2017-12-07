using ClaySharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo;
using ToolGood.ReadyGo.Attributes;

namespace TFA.Vote.Models
{
    /// <summary>
    /// 评审活动
    /// </summary>
    public class Review:ModelBase
    {
        public long AwardID { get; set; }
        [FieldLength(200)]
        public string ReviewName { get; set; }
        /// <summary>
        /// 评审状态
        /// </summary>
        public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.BeforePublish;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        [FieldLength(4000)]
        public string Data { get; set; }

        [Ignore,JsonIgnore]
        public Award Award
        {
            get
            {
                return Config.Helper.SingleById<Award>(AwardID);
            }
        }
        [Ignore,JsonIgnore]
        public List<ReviewDetail> ReviewDetails
        {
            get
            {
                return Config.Helper.CreateWhere<ReviewDetail>()
                    .Where(o=>!o.IsDelete)
                    .Where(o => o.ReviewID == ID)
                    .Select();
            }
        }
        [Ignore]
        public string CurrentRoundTurn
        {
            get
            {
                var currentReviewDetail = Config.Helper.CreateWhere<ReviewDetail>()
                    .Where(o => !o.IsDelete)
                    .Where(o => o.ReviewID == ID)
                    .OrderBy(o => o.ID, ToolGood.ReadyGo.OrderType.Desc)
                    .First();
                return $"第<span class='bold'>{currentReviewDetail.RoundC}</span>轮第 <span class='bold'>{currentReviewDetail.Turn}</span> 次";
            }
        }
        /// <summary>
        /// 评审活动的进行中轮次
        /// </summary>
        [Ignore,JsonIgnore]
        public ReviewDetail CurrentReviewDetail
        {
            get
            {
                var reviewdetail = Config.Helper.CreateWhere<ReviewDetail>()
                    .Where(o => !o.IsDelete)
                    .Where(o => o.ReviewID == ID)
                    .Where(o=>o.ReviewStatus==ReviewStatus.Reviewing)
                    .OrderBy(o => o.ID, ToolGood.ReadyGo.OrderType.Desc)
                    .FirstOrDefault();
                return reviewdetail;
            }
        }
        #region 方法
        /// <summary>
        /// 添加评审活动，并建立第一轮第一次评审
        /// </summary>
        /// <param name="review"></param>
        /// <param name="reviewdetail"></param>
        /// <param name="reviewmethodsetting"></param>
        /// <param name="isPublish"></param>
        public static void AddReview(Review review,ReviewDetail reviewdetail,ReviewMethodSetting reviewmethodsetting,bool isPublish=false)
        {
            using(var trans = Config.Helper.UseTransaction())
            {
                try
                {
                    if (isPublish)
                    {
                        review.ReviewStatus = ReviewStatus.Reviewing;
                        reviewdetail.ReviewStatus = ReviewStatus.Reviewing;
                        review.StartTime = DateTime.Now;
                    }
                    else
                    {
                        review.ReviewStatus = ReviewStatus.BeforePublish;
                    }
                    Config.Helper.Save(review);
                    reviewdetail.ReviewID = review.ID;
                    reviewdetail.ReviewMethodSetting = reviewmethodsetting;
                    if (reviewdetail.Round == 0) { reviewdetail.Round = 1; }
                    if (reviewdetail.Turn == 0) { reviewdetail.Turn = 1; }
                    if (reviewdetail.SourceProjectIDs.IsNullOrEmpty()) { reviewdetail.SourceProjectIDs = review.Award.Projects.Select(o => o.ID).Join(","); }
                    
                    Config.Helper.Save(reviewdetail);
                }
                catch(Exception ex)
                {
                    trans.Abort();
                    throw new Exception(ex.Message);
                }
            }
            
        }
        #endregion
    }
    /// <summary>
    /// 评审轮次
    /// </summary>
    public class ReviewDetail : ModelBase
    {
        public long ReviewID { get; set; }
        /// <summary>
        /// 轮
        /// </summary>
        public int Round { get; set; }
        /// <summary>
        /// 次
        /// </summary>
        public int Turn { get; set; }
        /// <summary>
        /// 目标数量
        /// </summary>
        public int TargetNumber { get; set; }
        /// <summary>
        /// 评审方式
        /// </summary>
        public ReviewMethod ReviewMethod { get; set; }
        /// <summary>
        /// 评审状态
        /// </summary>
        public ReviewStatus ReviewStatus { get; set; }
        /// <summary>
        /// 参选项目
        /// </summary>
        [FieldLength(4000)]
        public string SourceProjectIDs { get; set; }
        /// <summary>
        /// 评选结果项目
        /// </summary>
        [FieldLength(4000)]
        public string ResultProjectIDs { get; set; }
        /// <summary>
        /// 评审参数设定
        /// </summary>
        [FieldLength(1000)]
        public string ReviewMethodSettingStr { get; set; }
        [Text]
        public string Data { get; set; }
        [Ignore,JsonIgnore]
        public ReviewMethodSetting ReviewMethodSetting
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewMethodSetting>(ReviewMethodSettingStr);
            }
            set
            {
                ReviewMethodSettingStr = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }
        [Ignore,JsonIgnore]
        public List<ReviewVoteExpertDetail> VoteDetails
        {
            get
            {
                List<ReviewVoteExpertDetail> result = new List<ReviewVoteExpertDetail>();
                try
                {
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReviewVoteExpertDetail>>(Data);
                }
                catch
                {

                }
                return result;
            }
            set
            {
                Data = JsonConvert.SerializeObject(value);
            }
        }
        /// <summary>
        /// 是否是最后一轮
        /// </summary>
        [Ignore,JsonIgnore]       
        public Boolean IsLastRoundTurn
        {
            get
            {
                return !Config.Helper.Exists<ReviewDetail>("where reviewid=@0 and (round>@1 or (round=@1 and turn>@2))", ReviewID, Round,Turn);
            }
        }
        [Ignore, JsonIgnore]
        public Boolean IsLastRound
        {
            get
            {
                return !Config.Helper.Exists<ReviewDetail>("where reviewid=@0 and (round>@1 )", ReviewID, Round);
            }
        }
        /// <summary>
        /// 轮的中文显示
        /// </summary>
        [Ignore]
        public string RoundC
        {
            get
            {
                return Config.NumberToChinese(Round);
            }
        }
        #region 方法
        /// <summary>
        /// 提交基础fen
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="data"></param>
        public static void SubmitBaseScore(long reviewId,Dictionary<int,string> data)
        {
            var review = Config.Helper.SingleById<Review>(reviewId);
            foreach(var d in data)
            {
                var projectid = d.Key;
                if (d.Value.IsNullOrEmpty())
                {
                    Config.Helper.Update<AwardProject>("set basescore=null where awardid=@0 and projectid=@1", review.AwardID, projectid);
                }
                else
                {
                    decimal score;
                    decimal.TryParse(d.Value, out score);
                    Config.Helper.Update<AwardProject>("set basescore=@2 where awardid=@0 and projectid=@1", review.AwardID, projectid,score);
                }
                
            }
        }
        /// <summary>
        /// 修改评审轮次
        /// </summary>
        /// <param name="reviewdetail"></param>
        /// <param name="reviewmethodsetting"></param>
        /// <param name="isPublish"></param>
        public static void EditDetail(ReviewDetail reviewdetail, ReviewMethodSetting reviewmethodsetting, bool isPublish = false)
        {
            using (var trans = Config.Helper.UseTransaction())
            {
                try
                {
                    var review = Config.Helper.SingleById<Review>(reviewdetail.ReviewID);
                    if (isPublish)
                    {
                        review.ReviewStatus = ReviewStatus.Reviewing;
                        reviewdetail.ReviewStatus = ReviewStatus.Reviewing;
                        review.StartTime = DateTime.Now;
                    }
                    Config.Helper.Save(review);
                    reviewdetail.ReviewMethodSetting = reviewmethodsetting;
                    Config.Helper.Save(reviewdetail);
                }
                catch (Exception ex)
                {
                    trans.Abort();
                    throw new Exception(ex.Message);
                }
            }

        }
        
        /// <summary>
        /// 撤回评审
        /// </summary>
        /// <param name="reviewDetail"></param>
        public static void WithDraw(ReviewDetail reviewdetail)
        {
            reviewdetail.ReviewStatus = ReviewStatus.BeforePublish;
            reviewdetail.VoteDetails = new List<ReviewVoteExpertDetail>();//清空投票明细
            var review = Config.Helper.SingleById<Review>(reviewdetail.ReviewID);
            review.ReviewStatus = ReviewStatus.BeforePublish;
            Config.Helper.Save(review);
            Config.Helper.Save(reviewdetail);
        }
        /// <summary>
        /// 专家提交评审
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="isPublish"></param>
        public static void Review(int detailid,List<ReviewVoteExpertProjectDetail> projectdetails,bool isPublish=false)
        {
            lock (Config.lockObj)
            {
                var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
                var review = Config.Helper.SingleById<Review>(reviewdetail.ReviewID);
                var votedetails=reviewdetail.VoteDetails;
                var expert = Config.CurrentUser;
                votedetails.RemoveAll(o => o.ExpertID == expert.ID);//移除旧评分
                var expertDetail = new ReviewVoteExpertDetail()
                {
                    ExpertID = expert.ID,
                    VoteProjectDetail = projectdetails
                };
                if (isPublish) { expertDetail.FinishTime = DateTime.Now; }
                votedetails.Add(expertDetail);
                reviewdetail.VoteDetails = votedetails;
                //如果专家都已投票，则设置评审活动状态为已结束
                if(review.Award.ExpertCount == votedetails.Count(o => o.FinishTime != null))
                {
                    reviewdetail.ReviewStatus = ReviewStatus.Reviewed;
                    review.ReviewStatus = ReviewStatus.Reviewed;
                    Config.Helper.Save(review);
                }
                Config.Helper.Save(reviewdetail);
            }
        }
        /// <summary>
        /// 获取项目得分详情列表
        /// </summary>
        /// <returns></returns>
        public List<ProjectReviewDetail> GetProjectRanks(int r=0)
        {
            
            //dynamic New = new ClayFactory();
            var review = Config.Helper.SingleById<Review>(ReviewID);
            //本轮首次得分
            var mainreviewdetail = Config.Helper.Single<ReviewDetail>("where reviewid=@0 and round=@1 and turn=1",ReviewID,r==0?Round:r);
            var otherreviewdetails = Config.Helper.CreateWhere<ReviewDetail>()
                .Where(p => p.ReviewID == ReviewID && p.Round==mainreviewdetail.Round && p.Turn > 1 && p.ReviewStatus == ReviewStatus.Reviewed).Select();
            //上轮得分
            List<ProjectReviewDetail> lastProjectRanks = null;
            //参数
            var methodsetting = mainreviewdetail.ReviewMethodSetting;

            if (mainreviewdetail.ReviewMethod == ReviewMethod.Weighting)
            {
                lastProjectRanks = GetProjectRanks(mainreviewdetail.Round - 1);
            }
            var sourceProjects = Config.Helper.CreateWhere<Project>()
                .AddWhereSql("id in(" + mainreviewdetail.SourceProjectIDs + ")")
                .Select();

            var projectdetails = mainreviewdetail.VoteDetails.SelectMany(o => o.VoteProjectDetail);
            var result = sourceProjects.Select(o =>
            {
                var awardproject = Config.Helper.CreateWhere<AwardProject>()
                .Where(a => a.AwardID == review.AwardID && a.ProjectID == o.ID)
                .SingleOrDefault();
                var obj = new ProjectReviewDetail();
                obj.ID = o.ID;
                obj.ProjectName = o.ProjectName;
                obj.BuildingCompany = o.BuildingCompany;
                obj.BuildingType = o.BuildingType;
                obj.DesignCompany = o.DesignCompany;
                obj.DesignType = o.DesignType;
                obj.DesignYear = o.DesignYear;
                obj.Sort = awardproject==null?0:awardproject.Sort;//排序
                obj.NeedConfirm = false;//同分标记
                //得分
                if (mainreviewdetail.ReviewMethod == ReviewMethod.Vote)
                {
                    obj.Score = projectdetails.Where(p => p.ProjectID == o.ID).Sum(p =>
                    {
                        if (p.IsAvoid) { return 0; }
                        return p.VoteFlag ? 1 : 0;
                    });
                }
                else
                {
                    var scores = projectdetails.Where(p => p.ProjectID == o.ID && !p.IsAvoid).Select(p => p.Score).ToList();
                    //如果是第一轮第一次的平均分计算，且存在基础分，则将基础分加入
                    if (mainreviewdetail.Round==1 && awardproject.BaseScore != null)
                    {
                        scores.Add(Convert.ToDecimal(awardproject.BaseScore));
                    }
                    obj.Score =Math.Round( CalculateScore(scores, mainreviewdetail.ReviewMethodSetting),4);
                    //obj.Score =Math.Round( projectdetails.Where(p => p.ProjectID == o.ID && !p.IsAvoid).Average(p =>
                    //{                        
                    //    return p.Score;
                    //}),4);
                    obj.OriScore = obj.Score;
                    //与上轮加权的还需要再计算
                    if (mainreviewdetail.ReviewMethod == ReviewMethod.Weighting && mainreviewdetail.Round > 1)
                    {
                        var lastroundscore = lastProjectRanks.Single(p => p.ID == o.ID).Score;
                        obj.Score =Math.Round( lastroundscore * methodsetting.WeightLast / 100 + obj.Score * methodsetting.WeightNow / 100,4);
                    }
                }
                obj.TotalScore = obj.Score*1000;//TotalScore用来存本轮的总分用来排序
                //需要将同轮下面几次的分数也列出
                
                otherreviewdetails.ForEach(p => {
                    var subscore=p.VoteDetails.SelectMany(v => v.VoteProjectDetail).Where(v=>v.ProjectID==o.ID).Sum(d =>
                    {
                        if (d.IsAvoid) { return 0; }
                        return d.VoteFlag ? 1 : 0;
                    });
                    obj.SubScores.Add(subscore);
                    obj.TotalScore += subscore;
                });

                return obj;
            }).OrderByDescending(o => o.TotalScore).ToList();
            
            return result;
        }

        private decimal CalculateScore(IEnumerable<decimal> scores,ReviewMethodSetting setting)
        {
            var score = scores.Average();
            //仅当去掉最高最低分且打分数大于2
            if (setting.CutOff && scores.Count()>2)
            {
                var max = scores.Max();
                var min = scores.Min();
                score = (scores.Sum() - max - min) / (scores.Count() - 2);
            }
            return score;
        }
        #endregion
    }
    /// <summary>
    /// 评审状态
    /// </summary>
    public enum ReviewStatus
    {
        /// <summary>
        /// 未发布
        /// </summary>
        [Description("未发布")]
        BeforePublish =0,
        /// <summary>
        /// 评审中
        /// </summary>
        [Description("评审中")]
        Reviewing =1,
        /// <summary>
        /// 已评审
        /// </summary>
        [Description("已评审")]
        Reviewed =2
    }
    /// <summary>
    /// 评审方法
    /// </summary>
    public enum ReviewMethod
    {
        /// <summary>
        /// 本轮平均
        /// </summary>
        [Description("本轮平均")]
        Average =1,
        /// <summary>
        /// 与上轮加权
        /// </summary>
        [Description("与上轮加权")]
        Weighting =2,
        /// <summary>
        /// 投票
        /// </summary>
        [Description("投票")]
        Vote =3
    }
    /// <summary>
    /// 评审参数设置
    /// </summary>
    public class ReviewMethodSetting
    {
       
        public int MaxScore { get; set; }
        public int MinScore { get; set; }
        public decimal ScoreStep { get; set; }
        /// <summary>
        /// 上轮权重
        /// </summary>
        public decimal WeightLast { get; set; }
        /// <summary>
        /// 本轮权重
        /// </summary>
        public decimal WeightNow { get; set; }
        /// <summary>
        /// 票数
        /// </summary>
        public int VoteNumber { get; set; }
        /// <summary>
        /// 去掉最高最低分后的平均
        /// </summary>
        public Boolean CutOff { get; set; }
    }
    /// <summary>
    /// 投票投分明细
    /// </summary>
    public class ReviewVoteExpertProjectDetail
    {
        public long ExpertID { get; set; }
        public long ProjectID { get; set; }
        /// <summary>
        /// 打分
        /// </summary>
        public decimal Score { get; set; }
        /// <summary>
        /// 投票标记
        /// </summary>
        public Boolean VoteFlag { get; set; } = false;
        /// <summary>
        /// 回避标记
        /// </summary>
        public Boolean IsAvoid { get; set; } = false;
    }
    /// <summary>
    /// 专家投票明细
    /// </summary>
    public class ReviewVoteExpertDetail
    {
        public long ExpertID { get; set; }
        /// <summary>
        /// 投票时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        public List<ReviewVoteExpertProjectDetail> VoteProjectDetail { get; set; }
    }
}