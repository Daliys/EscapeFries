/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Collections.Generic;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class MudEditorWindowBase : EditorWindow
{
  internal static void Header(string label)
  {
    EditorGUILayout.LabelField
    (
      new GUIContent() { text = label }, 
      new GUIStyle("label") { fontStyle = FontStyle.Bold }
    );
  }

  internal static void Space()
  {
    EditorGUILayout.Space();
  }

  private int m_numIndents = 0;
  internal void Indent() { ++m_numIndents; }
  internal void Unindent() { --m_numIndents; m_numIndents = Mathf.Max(0, m_numIndents); }
  internal string LeadingSpaces
  {
    get
    {
      string leadingSpaces = "  ";
      for (int i = 0; i < m_numIndents; ++i)
        leadingSpaces += "  ";
      return leadingSpaces;
    }
  }

  internal void Property(SerializedProperty prop, string label, string tooltip = "")
  {
    EditorGUILayout.PropertyField
    (
      prop, 
      new GUIContent() { text = LeadingSpaces + label, tooltip = tooltip }, 
      true
    );
  }

  internal void Text(string text, FontStyle style = FontStyle.Normal)
  {
    EditorGUILayout.LabelField
    (
      new GUIContent() { text = LeadingSpaces + text }, 
      new GUIStyle("label") { fontStyle = style }
    );
  }

  private Dictionary<SerializedProperty, ReorderableList> m_listMap = new Dictionary<SerializedProperty, ReorderableList>();

  internal void Array(SerializedProperty prop, string label)
  {
    ReorderableList list = null;
    if (!m_listMap.TryGetValue(prop, out list))
    {
      list = new ReorderableList(prop.serializedObject, prop, true, true, true, true);

      if (label.Length > 0)
      {
        list.drawHeaderCallback = (Rect rect) =>
        {
          EditorGUI.LabelField(rect, label);
        };
      }
      else
      {
        list.headerHeight = 3.0f;
      }

      list.elementHeightCallback = (int index) =>
      {
        var elementProp = prop.GetArrayElementAtIndex(index);
        return EditorGUI.GetPropertyHeight(elementProp, new GUIContent() { text = "" });
      };

      list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
      {
        var elementProp = prop.GetArrayElementAtIndex(index);
        string elementLabel = " [" + index + "]";
        EditorGUI.LabelField(rect, elementLabel);
        rect.x += 30.0f;
        rect.width -= 30.0f;

        EditorGUI.PropertyField(rect, elementProp, new GUIContent() { text = "" });
      };

      m_listMap.Add(prop, list);
    }

    list.DoLayoutList();
  }
}
