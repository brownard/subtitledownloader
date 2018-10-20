
namespace SubtitleDownloader.Util
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string str)
        {
            for (int i = 0; i < str.Length; i++ )
            {
                if (!char.IsDigit(str[i]))
                    return false;
            }
            return true;
        }
    }
}
