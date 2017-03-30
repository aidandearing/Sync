using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlackboardValue))]
[CanEditMultipleObjects]
public class BlackboardValueEditor : Editor
{
    bool foldout = false;

    SerializedProperty type;
    SerializedProperty value;

    void OnEnable()
    {
        type = serializedObject.FindProperty(Literals.Strings.Blackboard.Type);
        value = serializedObject.FindProperty(Literals.Strings.Blackboard.Value);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        foldout = EditorGUILayout.Foldout(foldout, new GUIContent("Blackboard Value"));

        EditorGUILayout.PropertyField(type);

        Debug.Log(type.enumValueIndex);

        switch((BlackboardValue.ValueType)type.enumValueIndex)
        {
            case BlackboardValue.ValueType.Integer:
                value.intValue = EditorGUILayout.IntField(value.intValue);
                break;
            case BlackboardValue.ValueType.Float:
                value.floatValue = EditorGUILayout.FloatField(value.floatValue);
                break;
            case BlackboardValue.ValueType.Double:
                value.doubleValue = EditorGUILayout.DoubleField(value.doubleValue);
                break;
            case BlackboardValue.ValueType.Vector2:
                value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Vector 2"), value.vector2Value);
                break;
            case BlackboardValue.ValueType.Vector3:
                value.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("Vector 2"), value.vector3Value);
                break;
            case BlackboardValue.ValueType.Vector4:
                value.vector4Value = EditorGUILayout.Vector4Field(new GUIContent("Vector 2"), value.vector4Value);
                break;
            case BlackboardValue.ValueType.String:
                value.stringValue = EditorGUILayout.TextField(value.stringValue);
                break;
            default:
                EditorGUILayout.ObjectField(value);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
