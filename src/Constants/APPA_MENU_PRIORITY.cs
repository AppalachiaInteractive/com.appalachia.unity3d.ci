namespace Appalachia.Core.Constants
{
    public static partial class APPA_MENU
    {
        public static class ASSETS
        {
        }

        public static class GAME_OBJ
        {
            public static class LOD
            {
                public const int PRIORITY = -100;

                public const int LOD_COUNT = PRIORITY + 5;
                public const int LOD_COUNT_MIN = LOD_COUNT + 1;
                public const int LOD_COUNT_MAX = LOD_COUNT + 2;
                public const int LOD_COUNT_SPECTRUM = LOD_COUNT + 3;

                public const int THRESHOLD = PRIORITY + 10;
                public const int THRESHOLD_RELATIVE = THRESHOLD + 1;
                public const int THRESHOLD_LINEAR = THRESHOLD + 2;
                public const int THRESHOLD_ADAPTIVE = THRESHOLD + 3;

                public const int FADE = PRIORITY + 15;

                public const int LAST_LEVEL = PRIORITY + 16;
                public const int SET_CULL = PRIORITY + 17;
                public const int UTILITY = PRIORITY + 18;
            }

            public static class QUIXEL
            {
                public const int PRIORITY = -100;
            }

            public static class SELECT
            {
                public const int PRIORITY = -100;
            }

            public static class COLLIDERS
            {
                public const int PRIORITY = -100;

                public const int COLLIDER_ADD = PRIORITY + 1;

                public const int RIGIDBODY_ADD = PRIORITY + 2;
                public const int RIGIDBODY_REMOVE = PRIORITY + 3;

                public const int COLLIDER_BAKE = PRIORITY + 4;
            }
        }

        public static class TOOLS
        {
            public static class GENERAL
            {
                public const int PRIORITY = -100;
            }

            public static class PREFAB_RENDER
            {
                public const int PRIORITY = GENERAL.PRIORITY;
            }

            public static class MESH_BURY
            {
                public const int PRIORITY = GENERAL.PRIORITY;

                public const int ENABLE = PRIORITY - 1;
                public const int TOOLS = PRIORITY + 1;
                public const int CLEAR_QUEUES = PRIORITY + 3;
                public const int FORCE_SAVE = PRIORITY + 4;
                public const int REFRESH = PRIORITY + 5;
                public const int RESET = PRIORITY + 6;
                public const int ENABLE_VSP = PRIORITY + 10;
                public const int DISABLE_VSP = PRIORITY + 10;
                public const int OPERATIONS = PRIORITY + 11;
            }

            public static class PHYSICS
            {
                public const int PRIORITY = GENERAL.PRIORITY;
            }
        }
    }
}
