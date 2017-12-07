using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TF.Common.ValueInjecter;
using ToolGood.ReadyGo;
using ToolGood.ReadyGo.Attributes;

namespace TFA.Vote.Models
{
    public class Award:ModelBase
    {
        /// <summary>
        /// 奖项名称
        /// </summary>
        [FieldLength(100)]
        public string AwardName { get; set; }
        [FieldLength(200)]
        public string Memo { get; set; }

        [Ignore]
        public int ExpertCount
        {
            get
            {
                return Config.Helper.CreateWhere<AwardExpert>()
                    .Where(o => !o.IsDelete && o.AwardID == ID)
                    .Select().Count;
            }
        }
        [Ignore]
        public int ProjectCount
        {
            get
            {
                return Config.Helper.CreateWhere<AwardProject>()
                    .Where(o => !o.IsDelete && o.AwardID == ID)
                    .Select().Count;
            }
        }
        [Ignore,JsonIgnore]        
        public List<User> Experts
        {
            get
            {
                return Config.Helper.CreateWhere<User>()
                    .AddWhereSql("id in(select expertid from awardexpert where awardid=@0)", ID)
                    .Select();
            }
        }
        [Ignore, JsonIgnore]
        public List<Project> Projects
        {
            get
            {
                return Config.Helper.CreateWhere<Project>()
                    .AddWhereSql("id in(select projectid from awardproject where awardid=@0)", ID)
                    .Select();
            }
        }
        #region 方法
        public static void Save(Award postaward,string expertids,string projectids)
        {
            using(var trans = Config.Helper.UseTransaction())
            {
                try
                {
                    Award award = null;
                    if (postaward.ID == 0)
                    {
                        award = postaward;
                    }
                    else
                    {
                        award = Config.Helper.SingleById<Award>(postaward.ID);
                        award.InjectFrom(postaward);
                    }
                    Config.Helper.Save(award);
                    //移除原有专家及项目
                    Config.Helper.Delete<AwardExpert>("where awardid=@0", award.ID);
                    Config.Helper.Delete<AwardProject>("where awardid=@0", award.ID);

                    var expertids_arr = expertids.Split(',').ToList();
                    
                    expertids_arr.ForEach(o => {
                        var awardExpert = new AwardExpert()
                        {
                            AwardID = award.ID,
                            ExpertID = Convert.ToInt64(o),
                            
                        };
                        Config.Helper.Save(awardExpert);
                    });
                    var projectids_arr = projectids.Split('#').ToList();
                    var sort = 1;
                    projectids_arr.ForEach(o => {
                        var awardProject = new AwardProject()
                        {
                            AwardID = award.ID,
                            ProjectID = Convert.ToInt64(o.Split('|')[0]),
                            ExcludeExpertIDs = o.Split('|')[1],
                            Sort = int.Parse(o.Split('|')[2])
                        };
                        var score = o.Split('|')[3];
                        if (!score.IsNullOrEmpty())
                        {
                            decimal basescore;
                            if(decimal.TryParse(score, out basescore))
                            {
                                awardProject.BaseScore = basescore;
                            }

                        }
                        Config.Helper.Save(awardProject);
                    });
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

    public class AwardProject : ModelBase
    {
        public long AwardID { get; set; }
        public long ProjectID { get; set; }
        /// <summary>
        /// 回避专家ids
        /// </summary>
        [FieldLength(2000)]
        public string ExcludeExpertIDs { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 基础分
        /// </summary>
        public decimal? BaseScore { get; set; }
    }

    public class AwardExpert : ModelBase
    {
        public long AwardID { get; set; }
        public long ExpertID { get; set; }
    }
}