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

namespace MudBun
{
  public class MudBunConfig
  {
    //[MenuItem("MudBun/Configure MudBun")]
    public static void SelectConfigFile()
    {
      var config = Resources.Load("Config");
      if (config == null)
        return;

      Selection.activeObject = config;
    }
  }
}