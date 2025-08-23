using Game.Cheats;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(AdjustableBoolean))]
public class AdjustableBooleanPropertyDrawer : PropertyDrawer {
	public override VisualElement CreatePropertyGUI(SerializedProperty property) {
		return new PropertyField(property.FindPropertyRelative("_value"), property.displayName);
	}
}
