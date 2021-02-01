/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEditor;
using UnityEngine;

using MudBun;

public class CustomCreatoinMenu : CreationMenu
{
  [MenuItem("GameObject/Mud Bun/Custom/Custom Solid", priority = 4)]
  public static GameObject CreateCustomSolid()
  {
    var go = CreateGameObject("Mud Custom Solid");
    go.AddComponent<CustomSolid>();

    return OnBrushCreated(go);
  }

  [MenuItem("GameObject/Mud Bun/Custom/Custom Distortion", priority = 4)]
  public static GameObject CreateCustomDistortion()
  {
    var go = CreateGameObject("Mud Custom Distortion");
    go.AddComponent<CustomDistortion>();

    return OnBrushCreated(go, true);
  }

  [MenuItem("GameObject/Mud Bun/Custom/Custom Modifier", priority = 4)]
  public static GameObject CreateCustomModifier()
  {
    var go = CreateGameObject("Mud Custom Modifier");
    go.AddComponent<CustomModifier>();

    return OnBrushCreated(go);
  }
}

