// ReSharper disable MemberHidesStaticFromOuterClass

namespace Appalachia.CI.Constants
{
    public static partial class APPASTR
    {
        public static class PREF
        {
            private const string PREFIX = Appalachia + "/";

            public static class CI
            {
                private const string PREFIX = PREF.PREFIX + APPASTR.CI + "/";
                
                public const string Asset_Review = PREFIX + APPASTR.Asset_Review;
                public const string Maintenance_Tasks = PREFIX + APPASTR.Maintenance_Tasks;
                public const string Package_Review = PREFIX + APPASTR.Package_Review;
            }

            public static class EDITOR
            {
                private const string PREFIX = PREF.PREFIX + APPASTR.Editor + "/";

                public const string Asset_Generation = PREFIX + APPASTR.Asset_Generation;
            }
        }
    }
}
