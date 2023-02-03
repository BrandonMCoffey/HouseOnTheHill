#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Coffey_Utils.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property)) return 0;
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool ShouldShow(SerializedProperty property)
        {
            if (attribute is ShowIfAttribute attr)
            {
	            var target = property.serializedObject.targetObject;
	            bool show = ShowIfEditorHelper.ShouldShow(target, attr.Targets);
	            return attr.Reverse ? !show : show;
            }
	        return true;
        }

    }
}
#endif