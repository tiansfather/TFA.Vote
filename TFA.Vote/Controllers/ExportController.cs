using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TFA.Vote.Models;
using ToolGood.ReadyGo;

namespace TFA.Vote.Controllers
{
    public class ExportController : Controller
    {
        public ActionResult File(string path)
        {
            return File(path, "application/octet-stream", System.IO.Path.GetFileName(path));
        }

        /// <summary>
        /// 项目导出
        /// </summary>
        /// <returns></returns>

        public ActionResult Project(int awardid,string projectids,int round=1,int turn=1)
        {
            var award = Config.Helper.SingleById<Award>(awardid);
            var awardprojects = Config.Helper.CreateWhere<AwardProject>()
                .Where(o => o.AwardID == awardid).Select();
            var projects = Config.Helper.CreateWhere<Project>()
                .AddWhereSql("id in(select projectid from awardproject where awardid=@0)", awardid)
                .IfSet(projectids).AddWhereSql("id in(" + projectids + ")")
                .Select().Select(o=> {
                    var awardproject = awardprojects.Single(p => p.ProjectID == o.ID);
                    return new { o.ProjectName, o.BuildingCompany, o.BuildingType, o.DesignCompany, o.DesignYear, awardproject.Sort };
                }).OrderBy(o=>o.Sort);

            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("序号"),
                new DataColumn("项目名称"),
                new DataColumn("建设单位"),
                new DataColumn("建筑类别"),
                new DataColumn("设计单位"),
                new DataColumn("设计时间")
            });
            foreach(var p in projects)
            {
                var row = dt.NewRow();
                row["序号"] = p.Sort;
                row["项目名称"] = p.ProjectName;
                row["建设单位"] = p.BuildingCompany;
                row["建筑类别"] = p.BuildingType;
                row["设计单位"] = p.DesignCompany;
                row["设计时间"] = p.DesignYear;
                dt.Rows.Add(row);
            }
            var dic = Server.MapPath($"/upload/{DateTime.Now.ToString("yyyy/MM/dd")}/");
            if (!System.IO.Directory.Exists(dic))
            {
                System.IO.Directory.CreateDirectory(dic);
            }
            var filename = $"{award.AwardName}-{round}-{turn}-待审项目-{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            var filepath = System.IO.Path.Combine(dic, filename);

            var excelHelper = new ExcelHelper(filepath);
            excelHelper.DataTableToExcel(dt, "待审项目",true);

            return Json(new { filepath = filepath }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 评审结果导出
        /// </summary>
        /// <param name="detailid"></param>
        /// <returns></returns>
        public ActionResult ReviewDetail(int detailid)
        {
            var reviewdetail = Config.Helper.SingleById<ReviewDetail>(detailid);
            var review = Config.Helper.SingleById<Review>(reviewdetail.ReviewID);
            var award = review.Award;
            var sourceprojectids = reviewdetail.SourceProjectIDs;
            //对应项目的打分情况
            var projectreviewdetails = reviewdetail.GetProjectRanks().Where(o => reviewdetail.SourceProjectIDs.Split(',').ToList().Contains(o.ID.ToString())).ToList();
            for (var i = 0; i < projectreviewdetails.Count; i++)
            {
                var obj = projectreviewdetails[i];
                obj.Rank = i + 1;
            }
            var dt = BuildProjectTable(review, reviewdetail,award);
            BuildTableData(dt, review, reviewdetail, award, projectreviewdetails);
            var dic = Server.MapPath($"/upload/{DateTime.Now.ToString("yyyy/MM/dd")}/");
            if (!System.IO.Directory.Exists(dic))
            {
                System.IO.Directory.CreateDirectory(dic);
            }
            var filename = $"{award.AwardName}-{reviewdetail.Round}-{reviewdetail.Turn}-评审结果-{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            var filepath = System.IO.Path.Combine(dic, filename);

            var excelHelper = new ExcelHelper(filepath);
            excelHelper.DataTableToExcel(dt, "待审项目", true);

            return Json(new { filepath = filepath }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 构建表格结构
        /// </summary>
        /// <param name="review"></param>
        /// <param name="reviewdetail"></param>
        /// <param name="award"></param>
        /// <returns></returns>
        private DataTable BuildProjectTable(Review review, ReviewDetail reviewdetail,Award award)
        {
            var experts = award.Experts;
            var dt = new DataTable();
            var c = 53;//起始颜色
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("排名"),
                new DataColumn("序号"),
                new DataColumn("项目名称")
            });
            if (reviewdetail.ReviewMethod == ReviewMethod.Weighting)
            {
                //如果是与上轮加权的，需要增加一列
                dt.Columns.Add("加权得分").Prefix="c"+c--.ToString();
            }
            dt.Columns.Add("本轮得分").Prefix = "c" + c--.ToString(); 
            if(reviewdetail.Round==1 && reviewdetail.Turn==1 && reviewdetail.ReviewMethod != ReviewMethod.Vote)
            {
                dt.Columns.Add("基础分").Prefix = "c" + c.ToString();
            }
            //补投列
            //显示的最大轮数
            var maxshowturn = reviewdetail.Turn == 1 ? review.ReviewDetails.Where(o => o.ReviewStatus == TFA.Vote.Models.ReviewStatus.Reviewed && o.Round == reviewdetail.Round).Max(o => o.Turn) : reviewdetail.Turn;
            for (var i = 2; i <= maxshowturn; i++)
            {
                dt.Columns.Add("补投" + (i - 1)).Prefix = "c" + c--.ToString();
            }
            //专家打分列
            for (var i = 1; i <= maxshowturn; i++)
            {
                experts.ForEach(o => {
                    dt.Columns.Add(o.RealName + i).Prefix = "c" + (c+i-1).ToString();
                });
            }
            return dt;
        }

        private void BuildTableData(DataTable dt, Review review, ReviewDetail reviewdetail, Award award,List<ProjectReviewDetail> projectreviewdetails)
        {
            var experts = award.Experts;
            //显示的最大轮数
            var maxshowturn = reviewdetail.Turn == 1 ? review.ReviewDetails.Where(o => o.ReviewStatus == TFA.Vote.Models.ReviewStatus.Reviewed && o.Round == reviewdetail.Round).Max(o => o.Turn) : reviewdetail.Turn;
            var allreviewdetails = review.ReviewDetails.Where(o=>o.Round==reviewdetail.Round).ToList();
            foreach (var p in projectreviewdetails)
            {
                var awardproject = Config.Helper.CreateWhere<AwardProject>()
                    .Where(o => o.AwardID == award.ID && o.ProjectID == p.ID).SingleOrDefault();
                var row = dt.NewRow();
                row["排名"] = p.Rank;
                row["序号"] = p.Sort;
                row["项目名称"] = p.ProjectName;
                if (reviewdetail.ReviewMethod == ReviewMethod.Weighting)
                {
                    row["加权得分"] = p.Score;
                }
                row["本轮得分"] = p.OriScore;
                if (reviewdetail.Round == 1 && reviewdetail.Turn == 1 && reviewdetail.ReviewMethod != ReviewMethod.Vote)
                {
                    row["基础分"] = awardproject?.BaseScore;
                }
                //补投列

                for (var i = 2; i <= maxshowturn; i++)
                {
                    row["补投" + (i - 1)]=p.SubScores[i-2];
                }
                //专家打分列
                for (var i = 1; i <= maxshowturn; i++)
                {
                    var expertprojectreviewdetail = allreviewdetails[i - 1].VoteDetails;
                    experts.ForEach(o =>
                    {
                        var projectdetail=expertprojectreviewdetail.Single(e => e.ExpertID == o.ID).VoteProjectDetail.SingleOrDefault(e => e.ProjectID == p.ID);
                        if (projectdetail != null)
                        {
                            row[o.RealName + i] = projectdetail.IsAvoid ? "" : (allreviewdetails[i - 1].ReviewMethod == ReviewMethod.Vote ? (projectdetail.VoteFlag?"Y":"") : projectdetail.Score.ToString());
                        }
                        
                    });
                }
                dt.Rows.Add(row);
            }
        }
    }
}
