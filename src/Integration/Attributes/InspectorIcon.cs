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
}
