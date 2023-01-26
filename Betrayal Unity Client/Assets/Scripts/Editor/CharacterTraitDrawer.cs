#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CharacterTrait))]
public class CharacterTraitDrawer : PropertyDrawer
{
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var indexProp = property.FindPropertyRelative("Index");
		
		var labelRect = position;
		labelRect.width = 60;
		GUI.Label(labelRect, label);
		
		position.width -= labelRect.width;
		position.x += labelRect.width;
		
		var width = position.width / CharacterTrait.TraitValueCount - 1;
		
		bool showAll = width > 36;
		
		var boolRect = position;
		var valueRect = position;
		
		if (showAll)
		{
			boolRect.width = boolRect.height;
			boolRect.x += width / 2 - boolRect.height + 1;
		
			valueRect.width = width / 2;
			valueRect.x += valueRect.width;
		}
		else 
		{
			valueRect.width = width - 2;
		}
		
		var prevSelected = indexProp.intValue;
		var selected = new List<int>();
		
		for (int i = 1; i < CharacterTrait.TraitValueCount + 1; i++)
		{
			var valueProp = property.FindPropertyRelative("Value" + i);
			
			if (showAll)
			{
				if (EditorGUI.Toggle(boolRect, prevSelected == i))
				{
					selected.Add(i);
				}
				boolRect.x += width + 1;
			}
			EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
			valueRect.x += width + 1;
		}
		selected.Remove(prevSelected);
		if (selected.Count > 0) indexProp.intValue = selected[0];
	}
}
#endif