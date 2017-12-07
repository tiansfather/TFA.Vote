using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using TF.Common.ValueInjecter.Injections;
namespace TF.Common
{
    public class RequestInjection : KnownSourceInjection<HttpRequestBase>
    {
        protected override void Inject(HttpRequestBase source, object target)
        {
            Type t=target.GetType();
            var targetPros = t.GetProperties();//取得实体对象的所有属性
            foreach (var targetPro in targetPros)
            {
                try
                {
                    var name = targetPro.Name;
                    var value = source[name];
                    if (value == null) continue;
                    targetPro.SetValue(target, Convert.ChangeType(value, targetPro.PropertyType), null);
                }
                catch { }
                
            }
        }
    }
    public class JObjectInjection : KnownSourceInjection<JObject>
    {
        protected override void Inject(JObject source, object target)
        {
            Type t = target.GetType();
            var targetPros = t.GetProperties();//取得实体对象的所有属性
            foreach (var targetPro in targetPros)
            {
                var name = targetPro.Name;
                var value=source.GetValue(name, StringComparison.CurrentCultureIgnoreCase);
                //var value = source[name];
                if (value == null) continue;
                try
                {
                    targetPro.SetValue(target, Convert.ChangeType(value, targetPro.PropertyType), null);
                }
                catch { }
                
            }
        }
    }
}
