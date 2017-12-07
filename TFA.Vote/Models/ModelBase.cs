using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo.Attributes;

namespace TFA.Vote.Models
{
    public class ModelBase
    {
        public long ID { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public Boolean IsDelete { get; set; } = false;

        [Ignore,JsonIgnore]
        public object CustomData { get; set; }
    }
}