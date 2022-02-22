namespace Appalachia.CI.Constants
{
    public static partial class APPASTR
    {
        public static class Fonts
        {
            #region Constants and Static Readonly

            private const string _black = "Black";
            private const string _blackItalic = _black + _italic;
            private const string _bold = "Bold";
            private const string _boldItalic = _bold + _italic;
            private const string _extra = "Extra";
            private const string _extraBold = _extra + _bold;
            private const string _extraBoldItalic = _bold + _italic;
            private const string _extraLight = _extra + _light;
            private const string _extraLightItalic = _extra + _light + _italic;
            private const string _italic = "Italic";
            private const string _light = "Light";
            private const string _lightItalic = _light + _italic;
            private const string _medium = "Medium";
            private const string _mediumItalic = _medium + _italic;
            private const string _regular = "Regular";
            private const string _semi = "Semi";
            private const string _semiBold = _semi + _bold;
            private const string _semiBoldItalic = _semi + _bold + _italic;
            private const string _thin = "Thin";
            private const string _thinItalic = _thin + _italic;

            #endregion

            #region Nested type: Montserrat

            public static class Montserrat
            {
                #region Constants and Static Readonly

                public const string Black = FAMILY + _black;
                public const string BlackItalic = FAMILY + _blackItalic;
                public const string Bold = FAMILY + _bold;
                public const string BoldItalic = FAMILY + _boldItalic;
                public const string ExtraBold = FAMILY + _extraBold;
                public const string ExtraBoldItalic = _extraBoldItalic;
                public const string ExtraLight = FAMILY + _extraLight;
                public const string ExtraLightItalic = FAMILY + _extraLightItalic;
                public const string Italic = FAMILY + _italic;
                public const string Light = FAMILY + _light;
                public const string LightItalic = FAMILY + _lightItalic;
                public const string Medium = FAMILY + _medium;
                public const string MediumItalic = FAMILY + _mediumItalic;
                public const string Regular = FAMILY + _regular;
                public const string SemiBold = FAMILY + _semiBold;
                public const string SemiBoldItalic = FAMILY + _semiBoldItalic;
                public const string Thin = FAMILY + _thin;
                public const string ThinItalic = FAMILY + _thinItalic;
                private const string FAMILY = "Montserrat-";

                #endregion
            }

            #endregion
        }
    }
}
