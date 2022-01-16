using static Appalachia.CI.Constants.APPASTR;

namespace Appalachia.CI.Integration.Attributes
{
    public static class Brand
    {
        #region Constants and Static Readonly

        private const string BACKGROUND_BASE = Utility.Colors.Colors.Appalachia.HEX.DarkBrown;

        private const string BACKGROUND_BEHAVIOUR = Utility.Colors.Colors.Appalachia.HEX.Black;
        private const string BACKGROUND_SELECTABLE = Utility.Colors.Colors.Appalachia.HEX.Black;
        private const string BACKGROUND_OBJECT = Utility.Colors.Colors.Appalachia.HEX.DeepBrown;
        private const string BACKGROUND_PLAYABLE = Utility.Colors.Colors.Appalachia.HEX.Tan;
        private const string BACKGROUND_REPOSITORY = Utility.Colors.Colors.Appalachia.HEX.Blue;

        #endregion

        #region Nested type: AppalachiaBase

        public static class AppalachiaBase
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BASE;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Cream;
            public const string Fallback = BASE;
            public const string Icon = Squirrel.Cream;
            public const string Label = BASE;
            public const string Text = BASE;

            #endregion
        }

        #endregion

        #region Nested type: AppalachiaBehaviour

        public static class AppalachiaBehaviour
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BEHAVIOUR;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Yellow;
            public const string Fallback = BHVR;
            public const string Icon = Squirrel.Yellow;
            public const string Label = BEHAVIOUR;
            public const string Text = BEHAVIOUR;

            #endregion
        }

        #endregion

        #region Nested type: AppalachiaMetadataCollection

        public static class AppalachiaMetadataCollection
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_OBJECT;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Blue;
            public const string Fallback = MDATA_COLL;
            public const string Icon = Squirrel.Blue;
            public const string Label = METADATA;
            public const string Text = METADATA_COLLECTION;

            #endregion
        }

        #endregion

        #region Nested type: AppalachiaObject

        public static class AppalachiaObject
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_OBJECT;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Yellow;
            public const string Fallback = SCRPT;
            public const string Icon = Squirrel.LightYellow;
            public const string Label = SCRIPTABLE;
            public const string Text = SCRIPTABLE;

            #endregion
        }

        #endregion

        #region Nested type: AppalachiaObjectLookupCollection

        public static class AppalachiaObjectLookupCollection
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_OBJECT;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Orange;
            public const string Fallback = LOOKUP;
            public const string Icon = Squirrel.Orange;
            public const string Label = LOOKUP;
            public const string Text = LOOKUP;

            #endregion
        }

        #endregion

        #region Nested type: AppalachiaPlayable

        public static class AppalachiaPlayable
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_PLAYABLE;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Black;
            public const string Fallback = PLAY;
            public const string Icon = Squirrel.Black;
            public const string Label = PLAYABLE;
            public const string Text = PLAYABLE;

            #endregion
        }

        #endregion

        #region Nested type: AppalachiaRepository

        public static class AppalachiaRepository
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_REPOSITORY;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Bone;
            public const string Fallback = REPO;
            public const string Icon = Squirrel.Bone;
            public const string Label = REPOSITORY;
            public const string Text = REPOSITORY;

            #endregion
        }

        #endregion

        #region Nested type: AppalachiaBehaviour

        public static class AppalachiaSelectable
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_SELECTABLE;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Orange;
            public const string Fallback = SLCT;
            public const string Icon = Squirrel.Orange;
            public const string Label = SELECTABLE;
            public const string Text = SELECTABLE;

            #endregion
        }

        #endregion
        
        #region Nested type: AreaManager

        public static class AreaManager
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BEHAVIOUR;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Green;
            public const string Fallback = AREA_MAN;
            public const string Icon = Squirrel.Green;
            public const string Label = AREA;
            public const string Text = AREA_MANAGER;

            #endregion
        }

        #endregion

        #region Nested type: AreaMetadata

        public static class AreaMetadata
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_OBJECT;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Green;
            public const string Fallback = AREA_DATA;
            public const string Icon = Squirrel.Green;
            public const string Label = AREA;
            public const string Text = AREA_METADATA;

            #endregion
        }

        #endregion

        #region Nested type: EditorOnlyAppalachiaBehaviour

        public static class EditorOnlyAppalachiaBehaviour
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BEHAVIOUR;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Normal;
            public const string Fallback = EDT_ONLY;
            public const string Icon = Squirrel.Normal;
            public const string Label = EDITOR;
            public const string Text = EDITOR_ONLY_BEHAVIOUR;

            #endregion
        }

        #endregion

        #region Nested type: Font

        public static class Font
        {
            #region Constants and Static Readonly

            public const string BehaviourFont = Fonts.Montserrat.Medium;
            public const string ObjectFont = Fonts.Montserrat.Medium;

            #endregion
        }

        #endregion

        #region Nested type: GlobalSingletonAppalachiaBehaviour

        public static class GlobalSingletonAppalachiaBehaviour
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BEHAVIOUR;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Red;
            public const string Fallback = GLB_SNGT;
            public const string Icon = Squirrel.Red;
            public const string Label = GLOBAL;
            public const string Text = GLOBAL_SINGLETON;

            #endregion
        }

        #endregion

        #region Nested type: Groups

        public static class Groups
        {
            #region Constants and Static Readonly

            public const string BackgroundColor = Utility.Colors.Colors.Appalachia.HEX.Tan;
            public const string ChildColor = Utility.Colors.Colors.Appalachia.HEX.LightYellow;
            public const string LabelColor = Utility.Colors.Colors.Appalachia.HEX.Tan;

            #endregion
        }

        #endregion

        #region Nested type: InstancedAppalachiaBehaviour

        public static class InstancedAppalachiaBehaviour
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BEHAVIOUR;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Tan;
            public const string Fallback = INST_BHVR;
            public const string Icon = Squirrel.Tan;
            public const string Label = INSTANCED;
            public const string Text = INSTANCED_BEHAVIOUR;

            #endregion
        }

        #endregion

        #region Nested type: SingletonAppalachiaBehaviour

        public static class SingletonAppalachiaBehaviour
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BEHAVIOUR;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.DarkYellow;
            public const string Fallback = SNGT_BHVR;
            public const string Icon = Squirrel.DarkYellow;
            public const string Label = SINGLETON;
            public const string Text = SINGLETON_BEHAVIOUR;

            #endregion
        }

        #endregion

        #region Nested type: SingletonAppalachiaObject

        public static class SingletonAppalachiaObject
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_OBJECT;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.DarkYellow;
            public const string Fallback = SNGT_SCRPT;
            public const string Icon = Squirrel.RichYellow;
            public const string Label = SINGLETON;
            public const string Text = SINGLETON_SCRIPTABLE;

            #endregion
        }

        #endregion

        #region Nested type: SingletonAppalachiaObjectLookupCollection

        public static class SingletonAppalachiaObjectLookupCollection
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_OBJECT;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Red;
            public const string Fallback = SNGT_LOOKUP;
            public const string Icon = Squirrel.Red;
            public const string Label = LOOKUP;
            public const string Text = SINGLETON_LOOKUP;

            #endregion
        }

        #endregion

        #region Nested type: SingletonEditorOnlyAppalachiaBehaviour

        public static class SingletonEditorOnlyAppalachiaBehaviour
        {
            #region Constants and Static Readonly

            public const string Banner = BACKGROUND_BEHAVIOUR;
            public const string Color = Utility.Colors.Colors.Appalachia.HEX.Normal;
            public const string Fallback = SNGT_EDT_ONLY;
            public const string Icon = Squirrel.Normal;
            public const string Label = EDITOR;
            public const string Text = SINGLETON_EDITOR_ONLY;

            #endregion
        }

        #endregion

        #region Nested type: Squirrel

        public static class Squirrel
        {
            #region Constants and Static Readonly

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

            #endregion
        }

        #endregion

        #region Nested type: Subtitle

        public static class Subtitle
        {
            #region Constants and Static Readonly

            public const int Size = 13;
            public const string Fallback = APPA;
            public const string Text = APPALACHIA_INTERACTIVE;

            #endregion
        }

        #endregion

        #region Nested type: Title

        public static class Title
        {
            #region Constants and Static Readonly

            public const bool IsBold = false;
            public const int Height = 24;
            public const int Size = 13;

            #endregion
        }

        #endregion
    }
}
