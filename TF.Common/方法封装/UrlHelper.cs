using System;

namespace TF.Common
{
    public class UrlHelper
    {
        public static void Normalize(string baseUri, ref string strUri)
        {
            baseUri = baseUri.Split('#')[0];
            baseUri = baseUri.Split('?')[0];

            // 相对URL转换成绝对URL
            if (strUri.StartsWith("/"))
            {
                strUri = baseUri + strUri.Substring(1);
            }

            // 当查询字符串为空时去掉问号"?"
            if (strUri.EndsWith("?"))
                strUri = strUri.Substring(0, strUri.Length - 1);

            // 转换为小写
            strUri = strUri.ToLower();

            // 删除默认端口号
            // 解析路径
            // 解码转义字符
            Uri tempUri = new Uri(strUri);
            strUri = tempUri.ToString();

            // 根目录添加斜杠
            int posTailingSlash = strUri.IndexOf("/", 8);
            if (posTailingSlash == -1)
                strUri += '/';

            // 猜测的目录添加尾部斜杠
            if (posTailingSlash != -1 && !strUri.EndsWith("/") && strUri.IndexOf(".", posTailingSlash) == -1)
                strUri += '/';

            // 删除分块
            int posFragment = strUri.IndexOf("#");
            if (posFragment != -1)
            {
                strUri = strUri.Substring(0, posFragment);
            }

            // 删除缺省名字
            string[] DefaultDirectoryIndexes =
            {
                "index.html",
                "default.asp",
                "default.aspx",
            };
            foreach (string index in DefaultDirectoryIndexes)
            {
                if (strUri.EndsWith(index))
                {
                    strUri = strUri.Substring(0, (strUri.Length - index.Length));
                    break;
                }
            }
        }

        public static void Normalize(ref string strUri)
        {
            Normalize(string.Empty, ref strUri);
        }

        public static string GetBaseUri(string strUri)
        {
            string baseUri;
            Uri uri = new Uri(strUri);
            string port = string.Empty;
            if (!uri.IsDefaultPort)
                port = ":" + uri.Port;
            baseUri = uri.Scheme + "://" + uri.Host + port + "/";

            return baseUri;
        }

        public static string GetExtensionByMimeType(string mimeType)
        {
            int pos;
            if ((pos = mimeType.IndexOf('/')) != -1)
            {
                return mimeType.Substring(pos + 1);
            }
            return string.Empty;
        }
    }
}