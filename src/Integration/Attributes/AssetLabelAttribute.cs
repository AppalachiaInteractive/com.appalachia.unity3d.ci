using System;

namespace Appalachia.CI.Integration.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AssetLabelAttribute : Attribute
    {
        public AssetLabelAttribute(string label)
        {
            Label = label;
        }

        #region Fields and Autoproperties

        public string Label { get; set; }

        #endregion
    }
}
