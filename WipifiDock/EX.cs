using System;

namespace WipifiDock
{
    public static class EX
    {
        public static int ToID(this string text)
        {
            int z = 0;
            if (!string.IsNullOrEmpty(text))
            {
                char[] textArray = text.ToCharArray();
                for (int i = 0; i < textArray.Length; i++)
                {
                    z += textArray[i] * i;
                }
            }
            return z;
        }
    }
}
