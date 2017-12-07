namespace TFA.Vote
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using ToolGood.ReadyGo;

    public class BaseController : Controller
    {
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            Config.DisposeHelper();
        }

        public void ClearCookie(string name)
        {
            Response.Cookies.Remove(name);
        }

        protected ActionResult Error(string msg)
        {
            var data = new {
                errCode = -1,
                errMsg = msg
            };
            return base.Json(data);
        }

        protected ActionResult Error404() => 
            new HttpStatusCodeResult(0x194);

        protected ActionResult Error404(string msg) => 
            new HttpStatusCodeResult(0x194, msg);

        [NonAction]
        public ActionResult File(string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                [".323"] = "text/h323",
                [".acx"] = "application/internet-property-stream",
                [".ai"] = "application/postscript",
                [".aif"] = "audio/x-aiff",
                [".aifc"] = "audio/x-aiff",
                [".aiff"] = "audio/x-aiff",
                [".asf"] = "video/x-ms-asf",
                [".asr"] = "video/x-ms-asf",
                [".asx"] = "video/x-ms-asf",
                [".au"] = "audio/basic",
                [".avi"] = "video/x-msvideo",
                [".axs"] = "application/olescript",
                [".bas"] = "text/plain",
                [".bcpio"] = "application/x-bcpio",
                [".bin"] = "application/octet-stream",
                [".bmp"] = "image/bmp",
                [".c"] = "text/plain",
                [".cat"] = "application/vnd.ms-pkiseccat",
                [".cdf"] = "application/x-cdf",
                [".cer"] = "application/x-x509-ca-cert",
                [".class"] = "application/octet-stream",
                [".clp"] = "application/x-msclip",
                [".cmx"] = "image/x-cmx",
                [".cod"] = "image/cis-cod",
                [".cpio"] = "application/x-cpio",
                [".crd"] = "application/x-mscardfile",
                [".crl"] = "application/pkix-crl",
                [".crt"] = "application/x-x509-ca-cert",
                [".csh"] = "application/x-csh",
                [".css"] = "text/css",
                [".dcr"] = "application/x-director",
                [".der"] = "application/x-x509-ca-cert",
                [".dir"] = "application/x-director",
                [".dll"] = "application/x-msdownload",
                [".dms"] = "application/octet-stream",
                [".doc"] = "application/msword",
                [".dot"] = "application/msword",
                [".dvi"] = "application/x-dvi",
                [".dxr"] = "application/x-director",
                [".eps"] = "application/postscript",
                [".etx"] = "text/x-setext",
                [".evy"] = "application/envoy",
                [".exe"] = "application/octet-stream",
                [".fif"] = "application/fractals",
                [".flr"] = "x-world/x-vrml",
                [".gif"] = "image/gif",
                [".gtar"] = "application/x-gtar",
                [".gz"] = "application/x-gzip",
                [".h"] = "text/plain",
                [".hdf"] = "application/x-hdf",
                [".hlp"] = "application/winhlp",
                [".hqx"] = "application/mac-binhex40",
                [".hta"] = "application/hta",
                [".htc"] = "text/x-component",
                [".htm"] = "text/html",
                [".html"] = "text/html",
                [".htt"] = "text/webviewhtml",
                [".ico"] = "image/x-icon",
                [".ief"] = "image/ief",
                [".iii"] = "application/x-iphone",
                [".ins"] = "application/x-internet-signup",
                [".isp"] = "application/x-internet-signup",
                [".jfif"] = "image/pipeg",
                [".jpe"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".jpg"] = "image/jpeg",
                [".js"] = "application/x-javascript",
                [".latex"] = "application/x-latex",
                [".lha"] = "application/octet-stream",
                [".lsf"] = "video/x-la-asf",
                [".lsx"] = "video/x-la-asf",
                [".lzh"] = "application/octet-stream",
                [".m13"] = "application/x-msmediaview",
                [".m14"] = "application/x-msmediaview",
                [".m3u"] = "audio/x-mpegurl",
                [".man"] = "application/x-troff-man",
                [".mdb"] = "application/x-msaccess",
                [".me"] = "application/x-troff-me",
                [".mht"] = "message/rfc822",
                [".mhtml"] = "message/rfc822",
                [".mid"] = "audio/mid",
                [".mny"] = "application/x-msmoney",
                [".mov"] = "video/quicktime",
                [".movie"] = "video/x-sgi-movie",
                [".mp2"] = "video/mpeg",
                [".mp3"] = "audio/mpeg",
                [".mpa"] = "video/mpeg",
                [".mpe"] = "video/mpeg",
                [".mpeg"] = "video/mpeg",
                [".mpg"] = "video/mpeg",
                [".mpp"] = "application/vnd.ms-project",
                [".mpv2"] = "video/mpeg",
                [".ms"] = "application/x-troff-ms",
                [".mvb"] = "application/x-msmediaview",
                [".nws"] = "message/rfc822",
                [".oda"] = "application/oda",
                [".p10"] = "application/pkcs10",
                [".p12"] = "application/x-pkcs12",
                [".p7b"] = "application/x-pkcs7-certificates",
                [".p7c"] = "application/x-pkcs7-mime",
                [".p7m"] = "application/x-pkcs7-mime",
                [".p7r"] = "application/x-pkcs7-certreqresp",
                [".p7s"] = "application/x-pkcs7-signature",
                [".pbm"] = "image/x-portable-bitmap",
                [".pdf"] = "application/pdf",
                [".pfx"] = "application/x-pkcs12",
                [".pgm"] = "image/x-portable-graymap",
                [".pko"] = "application/ynd.ms-pkipko",
                [".pma"] = "application/x-perfmon",
                [".pmc"] = "application/x-perfmon",
                [".pml"] = "application/x-perfmon",
                [".pmr"] = "application/x-perfmon",
                [".pmw"] = "application/x-perfmon",
                [".pnm"] = "image/x-portable-anymap",
                [".pot,"] = "application/vnd.ms-powerpoint",
                [".ppm"] = "image/x-portable-pixmap",
                [".pps"] = "application/vnd.ms-powerpoint",
                [".ppt"] = "application/vnd.ms-powerpoint",
                [".prf"] = "application/pics-rules",
                [".ps"] = "application/postscript",
                [".pub"] = "application/x-mspublisher",
                [".qt"] = "video/quicktime",
                [".ra"] = "audio/x-pn-realaudio",
                [".ram"] = "audio/x-pn-realaudio",
                [".ras"] = "image/x-cmu-raster",
                [".rgb"] = "image/x-rgb",
                [".rmi"] = "audio/mid",
                [".roff"] = "application/x-troff",
                [".rtf"] = "application/rtf",
                [".rtx"] = "text/richtext",
                [".scd"] = "application/x-msschedule",
                [".sct"] = "text/scriptlet",
                [".setpay"] = "application/set-payment-initiation",
                [".setreg"] = "application/set-registration-initiation",
                [".sh"] = "application/x-sh",
                [".shar"] = "application/x-shar",
                [".sit"] = "application/x-stuffit",
                [".snd"] = "audio/basic",
                [".spc"] = "application/x-pkcs7-certificates",
                [".spl"] = "application/futuresplash",
                [".src"] = "application/x-wais-source",
                [".sst"] = "application/vnd.ms-pkicertstore",
                [".stl"] = "application/vnd.ms-pkistl",
                [".stm"] = "text/html",
                [".svg"] = "image/svg+xml",
                [".sv4cpio"] = "application/x-sv4cpio",
                [".sv4crc"] = "application/x-sv4crc",
                [".swf"] = "application/x-shockwave-flash",
                [".t"] = "application/x-troff",
                [".tar"] = "application/x-tar",
                [".tcl"] = "application/x-tcl",
                [".tex"] = "application/x-tex",
                [".texi"] = "application/x-texinfo",
                [".texinfo"] = "application/x-texinfo",
                [".tgz"] = "application/x-compressed",
                [".tif"] = "image/tiff",
                [".tiff"] = "image/tiff",
                [".tr"] = "application/x-troff",
                [".trm"] = "application/x-msterminal",
                [".tsv"] = "text/tab-separated-values",
                [".txt"] = "text/plain",
                [".uls"] = "text/iuls",
                [".ustar"] = "application/x-ustar",
                [".vcf"] = "text/x-vcard",
                [".vrml"] = "x-world/x-vrml",
                [".wav"] = "audio/x-wav",
                [".wcm"] = "application/vnd.ms-works",
                [".wdb"] = "application/vnd.ms-works",
                [".wks"] = "application/vnd.ms-works",
                [".wmf"] = "application/x-msmetafile",
                [".wps"] = "application/vnd.ms-works",
                [".wri"] = "application/x-mswrite",
                [".wrl"] = "x-world/x-vrml",
                [".wrz"] = "x-world/x-vrml",
                [".xaf"] = "x-world/x-vrml",
                [".xbm"] = "image/x-xbitmap",
                [".xla"] = "application/vnd.ms-excel",
                [".xlc"] = "application/vnd.ms-excel",
                [".xlm"] = "application/vnd.ms-excel",
                [".xls"] = "application/vnd.ms-excel",
                [".xlt"] = "application/vnd.ms-excel",
                [".xlw"] = "application/vnd.ms-excel",
                [".xof"] = "x-world/x-vrml",
                [".xpm"] = "image/x-xpixmap",
                [".xwd"] = "image/x-xwindowdump",
                [".z"] = "application/x-compress",
                [".zip"] = "application/zip",
                [".mp4"] = "video/mp4",
                [".ogg"] = "video/ogg",
                [".webm"] = "video/webm",
                [".doc"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                [".woff"] = "application/x-font-woff",
                [".svg"] = "image/svg+xml"
            };
            string fileDownloadName = Path.GetFileName(fileName);
            string extension = Path.GetExtension(fileName);
            string str3 = "";
            if (dictionary.TryGetValue(extension, out str3))
            {
                return this.File(fileName, str3, fileDownloadName);
            }
            return this.File(fileName, "application/octet-stream", fileDownloadName);
        }

        protected string GetSession(string key) => 
            base.Session[key]?.ToString();

        protected ActionResult GridJson<T>(Page<T> objs)
        {
            var data = new {
                rows = objs.Items,
                results = objs.TotalItems
            };
            return base.Json(data);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            if (exception is ArgumentException)
            {
                ArgumentException exception2 = exception as ArgumentException;
                var type = new {
                    errCode = -1,
                    errMsg = exception2.Message
                };
                JsonResult result = new JsonResult {
                    Data = type
                };
                filterContext.Result = result;
                filterContext.ExceptionHandled = true;
            }
            else
            {
                var type2 = new {
                    errCode = -1,
                    errMsg = exception.Message
                };
                JsonResult result2 = new JsonResult {
                    Data = type2
                };
                filterContext.Result = result2;
                filterContext.ExceptionHandled = true;
            }
        }

        protected void SetCookie(string cookiename, string cookievalue)
        {
            this.SetCookie(cookiename, cookievalue, DateTime.Now.AddDays(30.0));
        }

        protected void SetCookie(string cookiename, string cookievalue, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(cookiename) {
                Value = cookievalue,
                Expires = expires
            };
            Response.Cookies.Add(cookie);
        }

        protected ActionResult Success()
        {
            var data = new {
                errCode = 0,
                errMsg = ""
            };
            return base.Json(data);
        }
        protected ActionResult SuccessRedirect(string msg,string url)
        {
            var data = new
            {
                errCode = 0,
                errMsg = msg,
                forwardUrl=url
            };
            return base.Json(data);
        }
        protected ActionResult SuccessCallback(string msg,string callback)
        {
            var data = new
            {
                errCode = 0,
                errMsg = msg,
                callback = callback
            };
            return base.Json(data);
        }
        protected ActionResult Success(object obj)
        {
            var data = new {
                errCode = 0,
                errMsg = obj.ToString(),
                item = obj
            };
            return base.Json(data);
        }

        protected void VerifyDateTime(string name, DateTime num, DateTime min, DateTime max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min.ToString("yyyy-MM-dd HH:mm"));
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max.ToString("yyyy-MM-dd HH:mm"));
            }
        }

        protected void VerifyEmail(string name, string str)
        {
            Regex regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!regex.IsMatch(str))
            {
                throw new ArgumentException(name + "非Eamil格式");
            }
        }

        protected void VerifyInt(string name, string num)
        {
            Regex regex = new Regex(@"^-?(\d+)$");
            if (!regex.IsMatch(num))
            {
                throw new ArgumentException(name + "不能小于不是整数");
            }
        }

        protected void VerifyInt(string name, string num, int min, int max)
        {
            Regex regex = new Regex(@"^-?(\d+)$");
            if (!regex.IsMatch(num))
            {
                throw new ArgumentException(name + "不能小于不是整数");
            }
            int num2 = int.Parse(num);
            if (num2 < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num2 > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyLength(string name, string str, int max)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Length > max)
            {
                throw new ArgumentException(name + "字符大于" + max);
            }
        }

        protected void VerifyLength(string name, string str, int min, int max)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Length < min)
            {
                throw new ArgumentException(name + "字符小于" + min);
            }
            if (str.Length > max)
            {
                throw new ArgumentException(name + "字符大于" + max);
            }
        }

        protected void VerifyNotNull<T>(string name, List<T> str)
        {
            if (str == null)
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Count == 0)
            {
                throw new ArgumentException(name + "不能为空");
            }
        }

        protected void VerifyNotNull(string name, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(name + "不能为空");
            }
        }

        protected void VerifyNotNull<T>(string name, T[] str)
        {
            if (str == null)
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Length == 0)
            {
                throw new ArgumentException(name + "不能为空");
            }
        }

        protected void VerifyNumber(string name, string num)
        {
            Regex regex = new Regex(@"^-?(\d+|\d+\.\d+)$");
            if (!regex.IsMatch(num))
            {
                throw new ArgumentException(name + "不能小于不是数字");
            }
        }

        protected void VerifyNumber(string name, decimal num, decimal min, decimal max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, double num, double min, double max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, short num, short min, short max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, int num, int min, int max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, long num, long min, long max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, float num, float min, float max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, ushort num, ushort min, ushort max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, uint num, uint min, uint max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, ulong num, ulong min, ulong max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyUserName(string name, string str)
        {
            Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9_]{4,20}$");
            if (!regex.IsMatch(name))
            {
                throw new ArgumentException(name + "不符合要求");
            }
        }
    }
}

