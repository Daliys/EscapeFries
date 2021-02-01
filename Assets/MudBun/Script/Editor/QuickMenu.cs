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
using UnityEngine;

namespace MudBun
{
  public class QuickMenu : EditorWindow
  {
    private static readonly float ButtonSize = 40.0f;

    private static Dictionary<string, Texture2D> s_textures = new Dictionary<string, Texture2D>();
    private static Texture2D GetTexture(string guid)
    {
      Texture2D texture;

      if (!s_textures.TryGetValue(guid, out texture) || texture == null)
      {
        texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid));
        s_textures[guid] = texture;
      }

      return texture;
    }

    public static Texture2D BoxIcon => GetTexture("00670ac6c2f92b4439e88bf26075763f");

    //[MenuItem("Window/MudBun/Quick Menu")]
    static void Init()
    {
      var window = GetWindow(typeof(QuickMenu));
      window.Show();
    }

    void OnGUI()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Button(BoxIcon, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize));
      GUILayout.Button(BoxIcon, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize));
      GUILayout.Button(BoxIcon, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize));
      GUILayout.Button(BoxIcon, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize));
      GUILayout.Button(BoxIcon, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize));
      GUILayout.EndHorizontal();
    }
  }
}
