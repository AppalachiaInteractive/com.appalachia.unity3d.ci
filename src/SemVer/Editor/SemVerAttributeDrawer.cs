using Appalachia.Utility.Strings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.CI.SemVer
{
    [CustomPropertyDrawer(typeof(SemVerAttribute))]
    internal class SemVerAttributeDrawer : SemVerDrawer
    {
        public SemVerAttributeDrawer()
        {
            DrawAutoBuildPopup = false;
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type == "string")
            {
                Target = SemVer.Parse(property.stringValue);
                var corrected = DrawSemVer(position, property, label);
                property.stringValue = corrected.ToString();
                return;
            }

            Context.Log.Warn(ZString.Format("{0} is not supported by {1}", property.type, this));
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property);
            EditorGUI.EndProperty();
        }
    }
}
