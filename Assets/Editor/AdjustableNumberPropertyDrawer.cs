using Game.Cheats;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(AdjustableNumber))]
public class AdjustableNumberPropertyDrawer : PropertyDrawer {
	public override VisualElement CreatePropertyGUI(SerializedProperty property) {
		return new PropertyField(property.FindPropertyRelative("_value"), property.displayName);
	}
}
