using System;

namespace Appalachia.CI.Integration.Attributes
{
    public class InspectorIconAttribute : Attribute
    {
        public InspectorIconAttribute(string iconName)
        {
            IconName = iconName;
        }

        #region Fields and Autoproperties

        public string IconName { get; set; }

        #endregion
    }

    public static class Icons
    {
        public static class Squirrel
        {
            public const string Black = "ui_icon_squirrel_BLACK";
            public const string Blue = "ui_icon_squirrel_BLUE";
            public const string Bone = "ui_icon_squirrel_BONE";
            public const string Cream = "ui_icon_squirrel_CREAM";
            public const string DarkBrown = "ui_icon_squirrel_DARK-BROWN";
            public const string DarkYellow = "ui_icon_squirrel_DARK-YELLOW";
            public const string Green = "ui_icon_squirrel_GREEN";
            public const string Grey = "ui_icon_squirrel_GREY";
            public const string LightYellow = "ui_icon_squirrel_LIGHT-YELLOW";
            public const string Normal = "ui_icon_squirrel_NORMAL";
            public const string Orange = "ui_icon_squirrel_ORANGE";
            public const string Outline = "ui_icon_squirrel_OUTLINE";
            public const string Red = "ui_icon_squirrel_RED";
            public const string RichYellow = "ui_icon_squirrel_RICH-YELLOW";
            public const string Tan = "ui_icon_squirrel_TAN";
            public const string Yellow = "ui_icon_squirrel_YELLOW";
        }
    }
}
