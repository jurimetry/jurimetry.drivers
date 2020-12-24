using System.Text.RegularExpressions;

namespace GladosSearcher.Helper
{
    public static class TextUtils
    {
        public static string GetStringWihoutSpace(this string text) 
        {
            var regex = new Regex(@"\s+", RegexOptions.Compiled);

            if (string.IsNullOrEmpty(text)) return text;

            return regex.Replace(text, " ").Trim();
        }
    }
}
