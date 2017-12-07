using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo;
using ToolGood.ReadyGo.Attributes;

namespace TFA.Vote.Models
{
    [Table("Users")]
    public class User:ModelBase
    {
        [FieldLength(50)]
        public string UserName { get; set; }
        [FieldLength(50)]
        public string Password { get; set; }
        [FieldLength(50)]
        public string RealName { get; set; }
        [FieldLength(100)]
        public string Company { get; set; }
        public Boolean Enable { get; set; } = true;
        [FieldLength(200)]
        public string Memo { get; set; }
        public UserType UserType { get; set; } = UserType.Expert;

        #region 方法
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void Login(string username,string password)
        {
            var user = Config.Helper.CreateWhere<User>()
                .Where(o => o.UserName == username && o.Password == password.ToMd5Hash())
                .SingleOrDefault();
            if (user == null)
            {
                throw new Exception("用户名或密码不正确");
            }
            else if (!user.Enable || user.IsDelete)
            {
                throw new Exception("用户处于无效状态，无法登录");
            }
            else
            {
                Config.CurrentUser = user;
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oripassword"></param>
        /// <param name="newpassword"></param>
        /// <param name="repassword"></param>
        public static void ChangePassword(string oripassword,string newpassword,string repassword)
        {
            var user = Config.CurrentUser;
            if (string.IsNullOrEmpty(oripassword) || string.IsNullOrEmpty(newpassword) || string.IsNullOrEmpty(repassword))
            {
                throw new Exception("请输入原密码和新密码");
            }
            else if (repassword != newpassword)
            {
                throw new Exception("两次密码输入不一致");
            }
            else if (user.Password != oripassword.ToMd5Hash())
            {
                throw new Exception("原密码输入不正确");
            }
            else
            {
                user.Password = newpassword.ToMd5Hash();
                Config.Helper.Save(user);
            }
        }
        public static void Save(User user)
        {
            if (user.ID == 0)
            {
                
                if (Config.Helper.Count<User>("where username=@0", user.UserName) > 0)
                {
                    throw new Exception("相同帐号已存在");
                }
            }else if (user.ID > 0)
            {
                if (Config.Helper.Count<User>("where username=@0 and id<>@1",user.UserName,user.ID) > 0)
                {
                    throw new Exception("相同帐号已存在");
                }
            }
            Config.Helper.Save(user);
        }
        #endregion
    }
    public enum UserType
    {
        Manager,
        Expert
    }
}