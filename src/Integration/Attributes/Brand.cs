using static Appalachia.CI.Constants.APPASTR;

namespace Appalachia.CI.Integration.Attributes
{
    public static class Brand
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

        private const string BACKGROUND_BEHAVIOUR = Utility.Colors.Colors.Appalachia.HEX.Black;
        private const string BACKGROUND_OBJECT = Utility.Colors.Colors.Appalachia.HEX.DeepBrown;
        private const string BACKGROUND_BASE = Utility.Colors.Colors.Appalachia.HEX.DarkBrown;
        private const string BACKGROUND_PLAYABLE = Utility.Colors.Colors.Appalachia.HEX.Tan;
        private const string BACKGROUND_REPOSITORY = Utility.Colors.Colors.Appalachia.HEX.Blue;

        public static class AppalachiaRepository
        {
            public const string Icon = Squirrel.Bone;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Bone;
            public const string Text = REPOSITORY;
            public const string Fallback = REPO;
            public const string Banner = BACKGROUND_REPOSITORY;
        }

        public static class AppalachiaPlayable
        {
            public const string Icon = Squirrel.Black;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Black;
            public const string Text = PLAYABLE;
            public const string Fallback = PLAY;
            public const string Banner = BACKGROUND_PLAYABLE;
        }

        public static class AppalachiaBase
        {
            public const string Icon = Squirrel.Cream;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Cream;
            public const string Text = BASE;
            public const string Fallback = BASE;
            public const string Banner = BACKGROUND_BASE;
        }

        public static class AppalachiaObject
        {
            public const string Icon = Squirrel.LightYellow;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Yellow;
            public const string Text = SCRIPTABLE;
            public const string Fallback = SCRPT;
            public const string Banner = BACKGROUND_OBJECT;
        }

        public static class AppalachiaBehaviour
        {
            public const string Icon = Squirrel.Yellow;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Yellow;
            public const string Text = BEHAVIOUR;
            public const string Fallback = BHVR;
            public const string Banner = BACKGROUND_BEHAVIOUR;
        }

        public static class SingletonAppalachiaBehaviour
        {
            public const string Icon = Squirrel.DarkYellow;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.DarkYellow;
            public const string Text = SINGLETON_BEHAVIOUR;
            public const string Fallback = SNGT_BHVR;
            public const string Banner = BACKGROUND_BEHAVIOUR;
        }

        public static class InstancedAppalachiaBehaviour
        {
            public const string Icon = Squirrel.Tan;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Tan;
            public const string Text = INSTANCED_BEHAVIOUR;
            public const string Fallback = INST_BHVR;
            public const string Banner = BACKGROUND_BEHAVIOUR;
        }

        public static class AppalachiaObjectLookupCollection
        {
            public const string Icon = Squirrel.Orange;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Orange;
            public const string Text = LOOKUP;
            public const string Fallback = LOOKUP;
            public const string Banner = BACKGROUND_OBJECT;
        }

        public static class SingletonAppalachiaObject
        {
            public const string Icon = Squirrel.RichYellow;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.DarkYellow;
            public const string Text = SINGLETON_SCRIPTABLE;
            public const string Fallback = SNGT_SCRPT;
            public const string Banner = BACKGROUND_OBJECT;
        }

        public static class SingletonAppalachiaObjectLookupCollection
        {
            public const string Icon = Squirrel.Red;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Red;
            public const string Text = SINGLETON_LOOKUP;
            public const string Fallback = SNGT_LOOKUP;
            public const string Banner = BACKGROUND_OBJECT;
        }

        public static class GlobalSingletonAppalachiaBehaviour
        {
            public const string Icon = Squirrel.Red;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Red;
            public const string Text = GLOBAL_SINGLETON;
            public const string Fallback = GLB_SNGT;
            public const string Banner = BACKGROUND_BEHAVIOUR;
        }

        public static class AppalachiaMetadataCollection
        {
            public const string Icon = Squirrel.Blue;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Blue;
            public const string Text = METADATA_COLLECTION;
            public const string Fallback = MDATA_COLL;
            public const string Banner = BACKGROUND_OBJECT;
        }

        public static class AreaMetadata
        {
            public const string Icon = Squirrel.Green;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Green;
            public const string Text = AREA_METADATA;
            public const string Fallback = AREA_DATA;
            public const string Banner = BACKGROUND_OBJECT;
        }

        public static class AreaManager
        {
            public const string Icon = Squirrel.Green;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Green;
            public const string Text = AREA_MANAGER;
            public const string Fallback = AREA_MAN;
            public const string Banner = BACKGROUND_BEHAVIOUR;
        }

        public static class EditorOnlyAppalachiaBehaviour
        {
            public const string Icon = Squirrel.Normal;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Normal;
            public const string Text = EDITOR_ONLY_BEHAVIOUR;
            public const string Fallback = EDT_ONLY;
            public const string Banner = BACKGROUND_BEHAVIOUR;
        }

        public static class SingletonEditorOnlyAppalachiaBehaviour
        {
            public const string Icon = Squirrel.Normal;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Normal;
            public const string Text = SINGLETON_EDITOR_ONLY;
            public const string Fallback = SNGT_EDT_ONLY;
            public const string Banner = BACKGROUND_BEHAVIOUR;
        }

        public static class Title
        {
            public const bool IsBold = false;
            public const int Size = 13;
            public const int Height = 24;
        }

        public static class Subtitle
        {
            public const int Size = 13;
            public const string Text = APPALACHIA_INTERACTIVE;
            public const string Fallback = APPA;
        }

        public static class Font
        {
            public const string ObjectFont = Fonts.Montserrat.Medium;
            public const string BehaviourFont = Fonts.Montserrat.Medium;
        }

        public static class Groups
        {
            public const string LabelColor = Utility.Colors.Colors.Appalachia.HEX.Tan;
            public const string BackgroundColor = Utility.Colors.Colors.Appalachia.HEX.Tan;
            public const string ChildColor = Utility.Colors.Colors.Appalachia.HEX.LightYellow;
        }
    }
}
