// ReSharper disable MemberHidesStaticFromOuterClass

namespace Appalachia.CI.Integration.Core
{
    public static class APPASTR
    {
        public const string _asmdef = ".asmdef";
        public const string _dotSettings = ".dotSettings";
        public const string _git = ".git";
        public const string All = "All";
        public const string Analysis = "Analysis";
        public const string Any = "Any";
        public const string Any_Issues = Any + " " + Issues;
        public const string Appalachia = "Appalachia";
        public const string Appalachia_Only = Appalachia + " " + Only;
        public const string ASMDEFBUTTON = "ASMDEFBUTTON";
        public const string Assemblies = "Assemblies";
        public const string Assembly = "Assembly";
        public const string Assembly_Current = Assembly + " (" + Current + ")";
        public const string Assembly_Definitions = Assembly + " " + Definitions;
        public const string Assembly_Details = Assembly + " Details";
        public const string Assembly_Ideal = Assembly + " (" + Ideal + ")";
        public const string Assembly_Review = Assembly + " " + Review;
        public const string asset = "asset";
        public const string Asset = "Asset";

        public const string Asset_Review = Asset + " " + Review;
        public const string Assets = "Assets";
        public const string Assets_Only = Assets + " " + Only;
        public const string CI = "CI";
        public const string Completed = "Completed";
        public const string config = "config";
        public const string Count = "Count";
        public const string Current = "Current";
        public const string data = "data";
        public const string Definitions = "Definitions";
        public const string Dependencies = "Dependencies";
        public const string Details = "Details";
        public const string Directories = "Directories";
        public const string Directory = "Directory";
        public const string File = "File";
        public const string File_Current = File + " (" + Ideal + ")";
        public const string File_Ideal = File + " (" + Current + ")";
        public const string Files = "Files";
        public const string Fix = "Fix";
        public const string Folder = "Folder";
        public const string Folders = "Folders";
        public const string Ideal = "Ideal";
        public const string Integration = "Integration";
        public const string Integration_Analysis = Integration + " " + Analysis;
        public const string Issue = "Issue";
        public const string Issue_Type = Issue + " " + Type;
        public const string Issue_Type_Assembly = Issue + " " + Type + " " + Assembly;
        public const string Issue_Type_Package = Issue + " " + Type + " " + Package;
        public const string Issue_Type_Repository = Issue + " " + Type + " " + Repository;
        public const string Issues = "Issues";
        public const string Level = "Level";
        public const string Metadata = "Metadata";
        public const string Name = "Name";
        public const string Namespace = "Namespace";
        public const string Only = "Only";
        public const string Only_Issues = Only + " " + Issues;
        public const string Open = "Open";
        public const string Open__asmdef = Open + " " + _asmdef;
        public const string Open__dotSettings = Open + " " + _dotSettings;
        public const string Open_package_json = Open + " " + package_json;
        public const string Opportunities = "Opportunities";
        public const string Opportunity = "Opportunity";
        public const string Opportunity_Count = Opportunity + " " + Count;
        public const string Overview = "Overview";
        public const string Package = "Package";
        public const string package_json = "package.json";
        public const string Package_Review = Package + " " + Review;
        public const string Package_Search = Package + " " + Search;
        public const string Packages = "Packages";
        public const string Path = "Path";
        public const string Presence = "Presence";
        public const string Reanalyze = "Reanalyze";
        public const string Ref_ = "Ref.";
        public const string Ref_Assembly_Level = Ref_ + Assembly + Level;
        public const string References = "References";
        public const string Repositories = "Repositories";
        public const string Repository = "Repository";
        public const string Repository_Review = Repository + " " + Review;
        public const string Review = "Review";
        public const string Search = "Search";
        public const string Select = "Select";
        public const string src = "src";
        public const string Status = "Status";
        public const string Test = "Test";
        public const string Test_Files = Test + " " + Files;
        public const string Type = "Type";
        public const string url = "url";
        public const string Validity = "Validity";
        public const string version = "version";
        public const string Version = "Version";
        public const string Versions = "Versions";

        public static class PREF
        {
            private const string PREFIX = Appalachia + "/";

            public static class CI
            {
                public const string Asset_Review = PREFIX + APPASTR.Asset_Review;
                public const string Package_Review = PREFIX + APPASTR.Package_Review;
            }
        }
    }
}
