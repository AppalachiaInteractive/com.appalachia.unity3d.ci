// ReSharper disable MemberHidesStaticFromOuterClass

namespace Appalachia.CI.Constants
{
    public static class APPASTR
    {
        #region Constants and Static Readonly

        public const string _asmdef = ".asmdef";
        public const string _dotSettings = ".dotSettings";
        public const string _git = ".git";
        public const string All = "All";
        public const string Analysis = "Analysis";
        public const string Any = "Any";
        public const string Any_Issues = Any + _ + Issues;
        public const string Appalachia = "Appalachia";
        public const string Interactive = "Interactive";
        public const string APPALACHIA = "APPALACHIA";
        public const string INTERACTIVE = "INTERACTIVE";
        public const string SCRIPTABLE = "SCRIPTABLE";
        public const string SINGLETON = "SINGLETON";
        public const string BEHAVIOUR = "BEHAVIOUR";
        public const string Scriptable = "Scriptable";
        public const string Singleton = "Singleton";
        public const string Behaviour = "Behaviour";
        public const string SINGLETON_SCRIPTABLE = SINGLETON + _ + SCRIPTABLE;
        public const string SINGLETON_BEHAVIOUR = SINGLETON + _ + BEHAVIOUR;
        public const string Singleton_Scriptable = Singleton + _ + Scriptable;
        public const string Singleton_Behaviour = Singleton + _ + Behaviour;
        public const string Appalachia_Interactive = Appalachia + _ + Interactive;
        public const string APPALACHIA_INTERACTIVE = APPALACHIA + _ + INTERACTIVE;
        public const string Appalachia_Managed_Only = Appalachia + _ + Managed + _ + Only;
        public const string Appalachia_Only = Appalachia + _ + Only;
        public const string ASMDEFBUTTON = "ASMDEFBUTTON";
        public const string Assemblies = "Assemblies";
        public const string Assembly = "Assembly";
        public const string Assembly_Current = Assembly + " (" + Current + ")";
        public const string Assembly_Definitions = Assembly + _ + Definitions;
        public const string Assembly_Details = Assembly + " Details";
        public const string Assembly_Ideal = Assembly + " (" + Ideal + ")";
        public const string Assembly_Review = Assembly + _ + Review;
        public const string asset = "asset";
        public const string Asset = "Asset";
        public const string Asset_Generation = "Asset_Generation";
        public const string Asset_Review = Asset + _ + Review;
        public const string Assets = "Assets";
        public const string Assets_Only = Assets + _ + Only;
        public const string Buttons = "Buttons";
        public const string Canvas = "Canvas";
        public const string Canvas_Group = Canvas + _ + Group;
        public const string Canvas_Scaling = Canvas + _ + Scaling;
        public const string CI = "CI";
        public const string Completed = "Completed";
        public const string config = "config";
        public const string Content = "Content";
        public const string Context = "Context";
        public const string Convert = "Convert";
        public const string Convert_All_To_Packages = Convert + _ + All + _ + To + _ + Packages;
        public const string Convert_All_To_Repositories = Convert + _ + All + _ + To + _ + Repositories;
        public const string Convert_To_Package = Convert + _ + To + _ + Package;
        public const string Convert_To_Repository = Convert + _ + To + _ + Repository;
        public const string Count = "Count";
        public const string Current = "Current";
        public const string Current_Game = Current + _ + Game;
        public const string data = "data";
        public const string Definitions = "Definitions";
        public const string Dependencies = "Dependencies";
        public const string Deserialize = "Deserialize";
        public const string Details = "Details";
        public const string Developer = "Developer";
        public const string Directories = "Directories";
        public const string Directory = "Directory";
        public const string Distributable = "Distributable";
        public const string Distributable_Size = Distributable + _ + Size;
        public const string Distributable_Version = Distributable + _ + Version;
        public const string Doozy = "Doozy";
        public const string Doozy_Canvas = Doozy + _ + Canvas;
        public const string Doozy_Graph = Doozy + _ + Graph;
        public const string Doozy_View = Doozy + _ + View;
        public const string Editor = "Editor";
        public const string File = "File";
        public const string File_Current = File + " (" + Ideal + ")";
        public const string File_Ideal = File + " (" + Current + ")";
        public const string Files = "Files";
        public const string Fix = "Fix";
        public const string Flags = "Flags";
        public const string Folder = "Folder";
        public const string Folders = "Folders";
        public const string Game = "Game";
        public const string Game_State = "Game State";
        public const string General = "General";
        public const string Graph = "Graph";
        public const string Graphic = "Graphic";
        public const string Graphic_Raycaster = Graphic + _ + Raycaster;
        public const string Graphics = "Graphics";
        public const string Group = "Group";
        public const string Hide = "Hide";
        public const string Hide_Flags = Hide + _ + Flags;
        public const string ID = "ID";
        public const string Ideal = "Ideal";
        public const string Integration = "Integration";
        public const string Integration_Analysis = Integration + _ + Analysis;
        public const string Internal = "Internal";
        public const string Issue = "Issue";
        public const string Issue_Type = Issue + _ + Type;
        public const string Issue_Type_Assembly = Issue + _ + Type + _ + Assembly;
        public const string Issue_Type_Package = Issue + _ + Type + _ + Package;
        public const string Issue_Type_Repository = Issue + _ + Type + _ + Repository;
        public const string Issues = "Issues";
        public const string Keys = "Keys";
        public const string Level = "Level";
        public const string Maintenance = "Maintenance";
        public const string Maintenance_Tasks = Maintenance + _ + Tasks;
        public const string Managed = "Managed";
        public const string Menu = "Menu";
        public const string Menu_Width = Menu + _ + Width;
        public const string Metadata = "Metadata";
        public const string Name = "Name";
        public const string Namespace = "Namespace";
        public const string Only = "Only";
        public const string Only_Issues = Only + _ + Issues;
        public const string Open = "Open";
        public const string Open__asmdef = Open + _ + _asmdef;
        public const string Open__dotSettings = Open + _ + _dotSettings;
        public const string Open_package_json = Open + _ + package_json;
        public const string Opportunities = "Opportunities";
        public const string Opportunity = "Opportunity";
        public const string Opportunity_Count = Opportunity + _ + Count;
        public const string Overview = "Overview";
        public const string Package = "Package";
        public const string Package_All = Package + _ + All;
        public const string package_json = "package.json";
        public const string Package_Review = Package + _ + Review;
        public const string Package_Search = Package + _ + Search;
        public const string Packages = "Packages";
        public const string Path = "Path";
        public const string Presence = "Presence";
        public const string Productivity = "Productivity";
        public const string Publish = "Publish";
        public const string Publish_All = Publish + _ + All;
        public const string Publish_Status = Publish + _ + Status;
        public const string Published = "Published";
        public const string Published_Version = Published + _ + Version;
        public const string Raycaster = "Raycaster";
        public const string Reanalyze = "Reanalyze";
        public const string Reanalyze_All = Reanalyze + _ + All;
        public const string Ref_ = "Ref.";
        public const string Ref_Assembly_Level = Ref_ + Assembly + Level;
        public const string References = "References";
        public const string Repositories = "Repositories";
        public const string Repository = "Repository";
        public const string Repository_Review = Repository + _ + Review;
        public const string Reserialize = "Reserialize";
        public const string Reserialize_All = Reserialize + _ + All;
        public const string Reset = "Reset";
        public const string Reset_Context = Reset + _ + Context;
        public const string Review = "Review";
        public const string Runtime = "Runtime";
        public const string Scaling = "Scaling";
        public const string Search = "Search";
        public const string Select = "Select";
        public const string Serialize = "Serialize";
        public const string Size = "Size";
        public const string src = "src";
        public const string Status = "Status";
        public const string Tasks = "Tasks";
        public const string Test = "Test";
        public const string Test_Files = Test + _ + Files;
        public const string To = "To";
        public const string Type = "Type";
        public const string url = "url";
        public const string User = "User";
        public const string Validity = "Validity";
        public const string version = "version";
        public const string Version = "Version";
        public const string Version_All = Version + _ + All;
        public const string Versions = "Versions";
        public const string View = "View";
        public const string Width = "Width";

        public const string Workflow = "Workflow";

        private const string _ = " ";

        #endregion

        #region Nested type: EncryptionKeys

        public static class EncryptionKeys
        {
            #region Constants and Static Readonly

            public const string _00 = "GyrJwR879q4m9ge0XZJcOr3TI2RDNcIdZrwdJkk7";
            public const string _01 = "q1XwQ2t76nQyAtKbnbIYZdEcFfOyQRAqGW2MYLYX";
            public const string _02 = "GoDs63oHtTyrCzl4AfRqR2HMaql8YAAMCwXpv31n";
            public const string _03 = "akxjyxwtUcapozJnnyE3TJlYqa7EyodHJiAAT5rz";
            public const string _04 = "jJjE32bl59f1ObjIkoCsQLGYUipx5UXApCVcekpD";
            public const string _05 = "T4ZUDirykSRG6sbjuKqjVA2JyPUINoeQQ2hknyXs";
            public const string _06 = "O4vdy4fXiBT7AeE6I4m8v3kH4voGg4NJt8o9Qtc4";
            public const string _07 = "IrgphmW4kSHGw07x4GrfVjuPKXTyE4GWvpg5YTOZ";
            public const string _08 = "VAh9upSxrIPKmTazBdCjuxyfAVENUxTM5Uz8uoU6";
            public const string _09 = "x6q5y8qbI9hAGzEEw2OSDCARgOcWwSIzzDIQIQSc";
            public const string _10 = "jvKpi7r3i2NCaZRAzV53Gz0if5P8MERIYwR73pvn";
            public const string _11 = "Ivg8ihxDADTPLerIbWnsm23a4iJQdys6LrTCvMI2";
            public const string _12 = "l4dCw7y2OpmfEpaWTBdK0L2yyJ10zajot4vI9TLf";
            public const string _13 = "qmgvZnKGL0ck82jbulHVsdAJFbKcpmenWiwEqycM";
            public const string _14 = "zJiMH9x8M7kUv3pBJ7pBoHp5bOc51Wy4C75t8YS5";
            public const string _15 = "TfrYyouQvxbhb3bj1x2ckYOfpoycjd0HBSvXDWso";
            public const string _16 = "U7QGaNPU2eEPWQfJuTDIxaNE9j17NzbP9w243zUD";
            public const string _17 = "OUMLaiVXIGlDxUTvKz8KG7wnI0pLsB4VscsDvQz9";
            public const string _18 = "Lg7wXPXWqHiFNdiaYL9WhZW7h7IzLxMSN1ub4c0X";
            public const string _19 = "hI6csSa22vXACGH1NUiEtxMKmzYaRPhuYYxhTeQK";
            public const string _20 = "oy2qJ57pK8wVNBFWGPTTsI0xkTHZ6OnxfBnlcQj2";
            public const string _21 = "SW9EtAQiPWf7QmqU6c407A8WS79Vgzq69idHAGwh";
            public const string _22 = "ZM14W5cLRJ2NbD2EgbgPpzEz8pTrJwLEJSDkZWxB";
            public const string _23 = "mJ00XxAhzJ9e4DtoDU36gteEQCmL0oZpocQcbUFf";
            public const string _24 = "FegKTHClUokzfBXCfRASVqZFUcMknla7sehgnowc";
            public const string _25 = "YeRxMNPtBp86QQusQwLf9eyLku1WpXu5YMMy2SQE";
            public const string _26 = "fybyvLWQo858vx75GquQML5KiPSlTlG30FZEjQgT";
            public const string _27 = "gvL5knwz2yuFR7vjwtQm1YxHf6MZOQ45rqcmDsGy";
            public const string _28 = "ld0St6DTmch2znX5NL1F2HVLv9OV0FdVTtozZdNO";
            public const string _29 = "iODfQDZaPQnteEgexxdbExrOhXJFnMCe0BrOQZcR";
            public const string _30 = "RiheQ4QbDm3lLvqLRHGLSnRIqAC9F0qPs6YYySRh";
            public const string _31 = "l9zDFSU5AeSApbWBpwrzKQevb3FSNP5VlyIemxvc";
            public const string _42 = "dXKHHHMD9gXWNxdZuNJPGo7MvU3m7fO1ywQ6xiKP";
            public const string _43 = "mpQENB9iKxUEmMlTOgSOitVwaJEnIfiAVdQZVtq2";
            public const string _44 = "xw3p3VpFFDiw78CWSBEPXJjCp9heI2OAJkC1mWvr";
            public const string _45 = "UtbPXBnbztnofuMbKvTAwZCiI0fwH270k0xtC5vn";
            public const string DB_CONFIG_SETTINGS = "bzOBB4wSkxHrEVXTyyujp6cm8YdL0ZMidayjQGOO";
            public const string DB_DEVELOPER_GAMESTATE = "0eao9fuO1dOshjNUsjAJLO26v3UCZnrJFDjR0HGm";
            public const string DB_DEVELOPER_METADATA = "aQjpNXj6DpXW1dzjai321S0wvebBpbNKM6Eum1Yj";
            public const string DB_DEVELOPER_USER = "U0Y82EvJM156xaCW3PBLGyi2RsqohwSddCDoTxxl";
            public const string DB_DEVELOPER2_GAMESTATE = "gMoR0nAQHh5VYxBKhmz0dsC1RoFfY2ptaIVTdx4P";
            public const string DB_DEVELOPER2_METADATA = "EFrw9s5pWFTr41abSAodNx3aCTu15a2tPyyp27dJ";
            public const string DB_DEVELOPER2_USER = "JJuP6WjKgdV2FSFn3gHg0mODqfr1XZBF8UIB6vrb";
            public const string DB_EDITOR_GAMESTATE = "mnGuZM1zvVb78wMmTczg2HG1UVoWmOqSGeSo5TfQ";
            public const string DB_EDITOR_METADATA = "hToQ4835vDpEZ7hiOFFE8ZwER3MKV7OfqG3ehE4q";
            public const string DB_EDITOR_USER = "nhtODRB0MrKGk67ZtPRERtQAqQsQ2NLLvcxOuYyf";
            public const string DB_RUNTIME_GAMESTATE = "wr2fB9TD1DrsyK3rycK2dCoP01EFaR5Nrq5XzRVv";
            public const string DB_RUNTIME_METADATA = "6IMCBrWYUzpfcz8vE6uTXHgT4ffbE25j2rAbVW5x";
            public const string DB_RUNTIME_USER = "qs0zdOEHeQsp5gUJrQy7TkHAhrZuZVb2p4okbycV";
            public const string GENERIC_JSON = "PX4IApAhOmVjodVIiOd1WjsrNgLYUBduXq8lsvcP";

            #endregion
        }

        public static class Fonts
        {
            private const string _black = "Black";
            private const string _bold = "Bold";
            private const string _extra = "Extra";
            private const string _italic = "Italic";
            private const string _light = "Light";
            private const string _medium = "Medium";
            private const string _regular = "Regular";
            private const string _semi = "Semi";
            private const string _thin = "Thin";
            private const string _blackItalic = _black + _italic;
            private const string _boldItalic = _bold + _italic;
            private const string _extraBold = _extra + _bold;
            private const string _extraBoldItalic = _bold + _italic;
            private const string _extraLight = _extra + _light;
            private const string _lightItalic = _light + _italic;
            private const string _mediumItalic = _medium + _italic;
            private const string _semiBold = _semi + _bold;
            private const string _thinItalic = _thin + _italic;
            private const string _extraLightItalic = _extra + _light + _italic;
            private const string _semiBoldItalic = _semi + _bold + _italic;

            public static class Montserrat
            {
                private const string FAMILY = "Montserrat-";

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
            }
        }

        #endregion
    }
}
