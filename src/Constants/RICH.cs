using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.CI.Constants
{
    public static class RICH
    {
        public static string Bold(this string value)
        {
            return $"<b>{value}</b>";
        }

        public static string Color(this string value, Color color)
        {
            var hex = color.ToHexCode(true, true);

            return $"<color=#{hex}>{value}</color>";
        }

        public static string Italic(this string value)
        {
            return $"<i>{value}</i>";
        }

        public static string Size(this string value, int size)
        {
            size = Mathf.Clamp(size, 1, 100);

            return $"<size={size}>{value}</size>";
        }

        public static void SupportRichText(this GUIStyle style)
        {
            style.richText = true;
        }
    }
}
