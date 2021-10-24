using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Appalachia.CI.Integration.Serialization
{
    // Provide simple value get/set methods for SerializedProperty.  Can be used with
// any data types and with arbitrarily deeply-pathed properties.
    public static class SerializedPropertyExtensions
    {
        private static readonly Regex arrayElementRegex =
            new(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);

        /// (Extension) Get the value of the serialized property.
        public static object GetValue(this SerializedProperty property)
        {
            var propertyPath = property.propertyPath;
            object value = property.serializedObject.targetObject;
            var i = 0;
            while (NextPathComponent(propertyPath, ref i, out var token))
            {
                value = GetPathComponentValue(value, token);
            }

            return value;
        }

        /// (Extension) Set the value of the serialized property.
        public static void SetValue(this SerializedProperty property, object value)
        {
            Undo.RecordObject(property.serializedObject.targetObject, $"Set {property.name}");

            SetValueNoRecord(property, value);

            EditorUtility.SetDirty(property.serializedObject.targetObject);
            property.serializedObject.ApplyModifiedProperties();
        }

        /// (Extension) Set the value of the serialized property, but do not record the change.
        /// The change will not be persisted unless you call SetDirty and ApplyModifiedProperties.
        public static void SetValueNoRecord(this SerializedProperty property, object value)
        {
            var propertyPath = property.propertyPath;
            object container = property.serializedObject.targetObject;

            var i = 0;
            NextPathComponent(propertyPath, ref i, out var deferredToken);
            while (NextPathComponent(propertyPath, ref i, out var token))
            {
                container = GetPathComponentValue(container, deferredToken);
                deferredToken = token;
            }

            Debug.Assert(
                !container.GetType().IsValueType,
                $"Cannot use SerializedObject.SetValue on a struct object, as the result will be set on a temporary.  Either change {container.GetType().Name} to a class, or use SetValue with a parent member."
            );
            SetPathComponentValue(container, deferredToken, value);
        }

        private static object GetMemberValue(object container, string name)
        {
            if (container == null)
            {
                return null;
            }

            var type = container.GetType();
            var members = type.GetMember(
                name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
            );
            for (var i = 0; i < members.Length; ++i)
            {
                if (members[i] is FieldInfo field)
                {
                    return field.GetValue(container);
                }

                if (members[i] is PropertyInfo property)
                {
                    return property.GetValue(container);
                }
            }

            return null;
        }

        private static object GetPathComponentValue(object container, PropertyPathComponent component)
        {
            if (component.propertyName == null)
            {
                return ((IList) container)[component.elementIndex];
            }

            return GetMemberValue(container, component.propertyName);
        }

        // Parse the next path component from a SerializedProperty.propertyPath.  For simple field/property access,
        // this is just tokenizing on '.' and returning each field/property name.  Array/list access is via
        // the pseudo-property "Array.data[N]", so this method parses that and returns just the array/list index N.
        //
        // Call this method repeatedly to access all path components.  For example:
        //
        //      string propertyPath = "quests.Array.data[0].goal";
        //      int i = 0;
        //      NextPropertyPathToken(propertyPath, ref i, out var component);
        //          => component = { propertyName = "quests" };
        //      NextPropertyPathToken(propertyPath, ref i, out var component) 
        //          => component = { elementIndex = 0 };
        //      NextPropertyPathToken(propertyPath, ref i, out var component) 
        //          => component = { propertyName = "goal" };
        //      NextPropertyPathToken(propertyPath, ref i, out var component) 
        //          => returns false
        private static bool NextPathComponent(
            string propertyPath,
            ref int index,
            out PropertyPathComponent component)
        {
            component = new PropertyPathComponent();

            if (index >= propertyPath.Length)
            {
                return false;
            }

            var arrayElementMatch = arrayElementRegex.Match(propertyPath, index);
            if (arrayElementMatch.Success)
            {
                index += arrayElementMatch.Length + 1; // Skip past next '.'
                component.elementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
                return true;
            }

            var dot = propertyPath.IndexOf('.', index);
            if (dot == -1)
            {
                component.propertyName = propertyPath.Substring(index);
                index = propertyPath.Length;
            }
            else
            {
                component.propertyName = propertyPath.Substring(index, dot - index);
                index = dot + 1; // Skip past next '.'
            }

            return true;
        }

        private static void SetMemberValue(object container, string name, object value)
        {
            var type = container.GetType();
            var members = type.GetMember(
                name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
            );
            for (var i = 0; i < members.Length; ++i)
            {
                if (members[i] is FieldInfo field)
                {
                    field.SetValue(container, value);
                    return;
                }

                if (members[i] is PropertyInfo property)
                {
                    property.SetValue(container, value);
                    return;
                }
            }

            Debug.Assert(false, $"Failed to set member {container}.{name} via reflection");
        }

        private static void SetPathComponentValue(
            object container,
            PropertyPathComponent component,
            object value)
        {
            if (component.propertyName == null)
            {
                ((IList) container)[component.elementIndex] = value;
            }
            else
            {
                SetMemberValue(container, component.propertyName, value);
            }
        }

        // Union type representing either a property name or array element index.  The element
        // index is valid only if propertyName is null.
        private struct PropertyPathComponent
        {
            public int elementIndex;
            public string propertyName;
        }
    }
}
