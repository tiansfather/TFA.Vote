using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using ToolGood.ReadyGo;

namespace TFA.Vote
{
    public class Config
    {
        public static object lockObj = new object();
        [ThreadStatic]
        private static SqlHelper _sqlHelper = null;
        public static SqlHelper Helper
        {
            get
            {
                if (_sqlHelper == null)
                {
                    _sqlHelper= SqlHelperFactory.OpenFormConnStr("conn");
                }
                return _sqlHelper;
            }
        }
        public static void DisposeHelper()
        {
            if (_sqlHelper != null)
            {
                _sqlHelper.Dispose();
                _sqlHelper = null;
            }
        }
        public static string SoftTitle
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SoftTitle"];
            }
        }

        public static Models.User CurrentUser
        {
            get
            {
                //return Config.Helper.Single<Models.User>("where username=@0", "admin");
                return HttpContext.Current.Session["user"] as Models.User;
            }
            set
            {
                HttpContext.Current.Session["user"] = value;
            }
        }

        public static IList<T> Dt2List<T>(DataTable dt) where T:new()
        {
            IList<T> list = new List<T>();
            Type type = typeof(T);
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    T t = new T();
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object value = null;
                        //是否有自定义属性
                        var ca = pi.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                        string displayname = "";
                        if (ca.Length > 0)
                        {
                            displayname = (ca[0] as DisplayNameAttribute).DisplayName;
                        }
                        if (!string.IsNullOrEmpty(displayname) && dt.Columns.Contains(displayname))
                        {
                            value = dr[displayname];
                        }
                        else if (dt.Columns.Contains(pi.Name))
                        {
                            value = dr[pi.Name];
                        }
                        if (value != null && value != DBNull.Value)
                        {
                            /*
                            string dbname = dr[pi.Name].GetType().Name.ToString().ToLower();
                            //
                            if (dbname == "boolean" ||  dbname == "dbnull")
                                pi.SetValue(t, value.ToString(), null);
                            else
                                pi.SetValue(t, value, null);
                             */
                            if (pi.PropertyType.Name.ToLower() == "decimal" && string.IsNullOrEmpty(value.ToString()))
                            {
                                value = 0;
                            }
                            try
                            {
                                pi.SetValue(t, Convert.ChangeType(value, pi.PropertyType), null);
                            }
                            catch
                            {

                            }

                        }
                    }
                    list.Add(t);
                }
                catch (Exception ex) { }
            }
            return list;
        }

        /// <summary>
        /// 数字转中文
        /// </summary>
        /// <param name="number">eg: 22</param>
        /// <returns></returns>
        public static string NumberToChinese(int number)
        {
            string res = string.Empty;
            string str = number.ToString();
            string schar = str.Substring(0, 1);
            switch (schar)
            {
                case "1":
                    res = "一";
                    break;
                case "2":
                    res = "二";
                    break;
                case "3":
                    res = "三";
                    break;
                case "4":
                    res = "四";
                    break;
                case "5":
                    res = "五";
                    break;
                case "6":
                    res = "六";
                    break;
                case "7":
                    res = "七";
                    break;
                case "8":
                    res = "八";
                    break;
                case "9":
                    res = "九";
                    break;
                default:
                    res = "零";
                    break;
            }
            if (str.Length > 1)
            {
                switch (str.Length)
                {
                    case 2:
                    case 6:
                        res += "十";
                        break;
                    case 3:
                    case 7:
                        res += "百";
                        break;
                    case 4:
                        res += "千";
                        break;
                    case 5:
                        res += "万";
                        break;
                    default:
                        res += "";
                        break;
                }
                res += NumberToChinese(int.Parse(str.Substring(1, str.Length - 1)));
            }
            return res;
        }
    }
}