/******** **********************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;

namespace MudBun
{
  [ExecuteInEditMode]
  public class MudBrush : MudBrushBase
  {
    protected override void ScanRenderer()
    {
      base.ScanRenderer();

      var t = transform.parent;
      MudRenderer r = null;
      for (int i = 0; i < 128; ++i)
      {
        if (t == null)
          break;

        r = t.GetComponent<MudRenderer>();
        if (r != null)
          break;

        t = t.parent;
      }

      if (r != null)
        r.RescanBrushes();
    }
  }
}

