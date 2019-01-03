namespace MySecrets.App.Domains
{
    public static class Utils
    {
        public static string AddLastCharIfNotExists(this string src, char c)
        {
            if (string.IsNullOrEmpty(src))
                return string.Empty + c;

            if (src[src.Length - 1] == c)
                return src;

            return src + c;
        }


        public static int GetPositionOffset(this string src)
        {
            if (src == null)
                return 0;

            var i = 0;
            foreach (var c in src)
            {
                if (c > ' ')
                    return i;

                i++;
            }

            return 0;
        }

    }

}