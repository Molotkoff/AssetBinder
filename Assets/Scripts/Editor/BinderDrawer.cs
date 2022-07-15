using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace AMolotkoff.Binder
{
    [CustomPropertyDrawer(typeof(ScriptableObject), true)]
    internal sealed class BinderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = property.serializedObject.targetObject;
            var targetType = target.GetType();
            var fieldName = property.name;
            var field = targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var binderAttributes = field.GetCustomAttributes(typeof(BinderAttribute), true);

            if (binderAttributes.Length > 0)
            {
                var fieldType = field.FieldType;
                var assets = FindAssets(fieldType);
                var assetsNames = assets.Select(asset => asset.name).ToArray();
                var scriptableProperty = (ScriptableObject)property.objectReferenceValue;
                var scriptableName = scriptableProperty != null ? scriptableProperty.name
                                                                : string.Empty;

                var popupPosition = EditorGUI.PrefixLabel(position, label);
                var scriptableIndex = Array.IndexOf(assetsNames, scriptableName);
                var newIndex = EditorGUI.Popup(popupPosition, scriptableIndex , assetsNames);

                if (newIndex != scriptableIndex)
                {
                    property.objectReferenceValue = assets[newIndex];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
                EditorGUI.PropertyField(position, property, label);
        }

        private static ScriptableObject[] FindAssets(Type onType)
        {
            var assets = new List<ScriptableObject>();
            var onFormat = onType.ToString().Replace("UnityEngine.", "");
            var guids = AssetDatabase.FindAssets($"t:{onFormat}");

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

                if (asset != null)
                    assets.Add(asset);
            }

            return assets.ToArray();
        }
    }
}