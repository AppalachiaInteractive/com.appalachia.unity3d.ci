using System;

namespace Appalachia.CI.Constants
{
    public static class APPAINT
    {
        #region Nested type: POWERS_OF

        public static class POWERS_OF
        {
            private static int[] Calculate(int root, int startIndex, int length)
            {
                var result = new int[length];

                for (var i = startIndex; i < length; i++)
                {
                    result[i] = (int)Math.Pow(root, i);
                }

                return result;
            }

            #region Nested type: TWO

            public static class TWO
            {
                #region Static Fields and Autoproperties

                private static int[] _hdTextureSizes;

                private static int[] _previewTextureSizes;
                private static int[] _textureSizes;

                #endregion

                public static int[] HDTextureSizes
                {
                    get
                    {
                        if (_hdTextureSizes == null)
                        {
                            _hdTextureSizes = Get(11, 14);
                        }

                        return _hdTextureSizes;
                    }
                }

                public static int[] PreviewTextureSizes
                {
                    get
                    {
                        if (_previewTextureSizes == null)
                        {
                            _previewTextureSizes = Get(4, 9);
                        }

                        return _previewTextureSizes;
                    }
                }

                public static int[] TextureSizes
                {
                    get
                    {
                        if (_textureSizes == null)
                        {
                            _textureSizes = Get(5, 12);
                        }

                        return _textureSizes;
                    }
                }

                public static int[] Get(int startIndex, int length)
                {
                    return Calculate(2, startIndex, length);
                }
            }

            #endregion
        }

        #endregion
    }
}
