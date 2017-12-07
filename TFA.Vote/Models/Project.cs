using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo;
using ToolGood.ReadyGo.Attributes;
using System.Linq;
using TF.Common;
using Newtonsoft.Json;

namespace TFA.Vote.Models
{
    public class Project:ModelBase
    {
        /// <summary>
        /// 标签
        /// </summary>
        [FieldLength(50)]
        public string Tag { get; set; }
        [DisplayName("项目名称")]
        [FieldLength(100)]
        public string ProjectName { get; set; }
        [DisplayName("设计时间")]
        [FieldLength(20)]
        public string DesignYear { get; set; }
        [DisplayName("设计单位")]
        [FieldLength(100)]
        public string DesignCompany { get; set; }
        [DisplayName("联系人")]
        [FieldLength(50)]
        public string DesignContact { get; set; }
        [DisplayName("手机")]
        [FieldLength(50)]
        public string DesignMobile { get; set; }
        [DisplayName("Email")]
        [FieldLength(50)]
        public string DesignEmail { get; set; }
        [DisplayName("电话")]
        [FieldLength(50)]
        public string DesignPhone { get; set; }
        [DisplayName("设计类别")]
        [FieldLength(50)]
        public string DesignType { get; set; }
        /// <summary>
        /// 申请奖项
        /// </summary>
        [DisplayName("申报奖项")]
        [FieldLength(50)]
        public string ApplicationAward { get; set; }
        [DisplayName("建设单位")]
        [FieldLength(100)]
        public string BuildingCompany { get; set; }
        [DisplayName("建设地点")]
        [FieldLength(100)]
        public string BuildingAddress { get; set; }

        /// <summary>
        /// 设计团队负责人
        /// </summary>
        [DisplayName("设计总负责人")]
        [FieldLength(50)]
        public string TeamCharger { get; set; }
        /// <summary>
        /// 设计师
        /// </summary>
        [DisplayName("设计师")]
        [FieldLength(50)]
        public string TeamDesigner { get; set; }
        /// <summary>
        /// 主创设计
        /// </summary>
        [DisplayName("主创设计师")]
        [FieldLength(50)]
        public string TeamLeader { get; set; }
        [DisplayName("合作设计单位")]
        [FieldLength(100)]
        public string DesignCorperater { get; set; }

        [DisplayName("建筑类别")]
        [FieldLength(50)]
        public string BuildingType { get; set; }
        /// <summary>
        /// 总用地面积
        /// </summary>
        [DisplayName("总用地面积")]
        [FieldLength(50)]
        public string UsedArea { get; set; }
        /// <summary>
        /// 建筑面积
        /// </summary>
        [DisplayName("总建筑面积")]
        [FieldLength(50)]
        public string BuildingArea { get; set; }
        /// <summary>
        /// 建筑面积(地上)
        /// </summary>
        [DisplayName("地上建筑面积")]
        [FieldLength(50)]
        public string BuildingAreaOverGround { get; set; }
        /// <summary>
        /// 建筑面积(地下)
        /// </summary>
        [DisplayName("地下建筑面积")]
        [FieldLength(50)]
        public string BuildingAreaUnderGround { get; set; }
        /// <summary>
        /// 建筑高度
        /// </summary>
        [DisplayName("建筑高度")]
        [FieldLength(50)]
        public string BuildingHeight { get; set; }
        /// <summary>
        /// 容积率
        /// </summary>
        [DisplayName("容积率")]
        [FieldLength(50)]
        public string PlotRatio { get; set; }
        /// <summary>
        /// 层数(地上)
        /// </summary>
        [DisplayName("地上层数")]
        [FieldLength(50)]
        public string BuildingPlyOverGround { get; set; }
        /// <summary>
        /// 层数(地下)
        /// </summary>
        [DisplayName("地下层数")]
        [FieldLength(50)]
        public string BuildingPlyUnderGround { get; set; }

        [Ignore]
        public int AttachNumbers
        {
            get
            {
                return Attaches.Count;
            }
        }
        [Ignore,JsonIgnore]
        public List<ProjectAttach> Attaches
        {
            get
            {
                return Config.Helper.CreateWhere<ProjectAttach>()
                    .Where(o => o.ProjectID == ID)
                    .Select();
            }
        }
        #region 方法
        public static void ImportFrom(string tag,string path)
        {
            var helper = new ExcelHelper(path);
            var dt=helper.ExcelToDataTable(null, true);
            var projects = Config.Dt2List<Project>(dt);
            projects.ForEach(o => {
                
                o.Tag = tag; Config.Helper.Save(o);
            });
        }
        #endregion
    }
    /// <summary>
    /// 项目附件
    /// </summary>
    public class ProjectAttach:ModelBase
    {
        public int ProjectID { get; set; }
        [FieldLength(100)]
        public string FileName { get; set; }
        [FieldLength(100)]
        public string FilePath { get; set; }
        public long FileSize { get; set; }

        [Ignore]
        public string FileSizeStr
        {
            get
            {
                if (FileSize > 1024 * 1024*1024)
                {
                    return Math.Round(Convert.ToDecimal(FileSize) / 1024 / 1024/1024, 2) + "G";
                }else if (FileSize > 1024 * 1024)
                {
                    return Math.Round(Convert.ToDecimal(FileSize) / 1024 / 1024, 2) + "M";
                }
                else
                {
                    return Math.Round(Convert.ToDecimal(FileSize) / 1024 , 2) + "K";
                }
            }
        }

    }
}