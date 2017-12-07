using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFA.Vote.Models
{
    /// <summary>
    /// 项目的评审详情
    /// </summary>
    public class ProjectReviewDetail
    {
        public long ID { get; set; }
        public string ProjectName { get; set; }
        public string BuildingCompany { get; set; }
        public string BuildingType { get; set; }
        public string DesignType { get; set; }
        public string DesignYear { get; set; }
        public string DesignCompany { get; set; }
        public int Sort { get; set; }
        public Boolean NeedConfirm { get; set; }
        /// <summary>
        /// 本轮实际分，如果是加权的则是加权后的分
        /// </summary>
        public decimal Score { get; set; }
        /// <summary>
        /// 本轮打分
        /// </summary>
        public decimal OriScore { get; set; }
        /// <summary>
        /// 用来排序的混合分
        /// </summary>
        public decimal TotalScore { get; set; }
        public int Rank { get; set; }
        public List<int> SubScores { get; set; } = new List<int>();
    }
}