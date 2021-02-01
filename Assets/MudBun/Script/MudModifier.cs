/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace MudBun
{
  public class MudModifier : MudBrush
  {
    public enum OperatorEnum
    {
      Modify = 100, 
    }

    public override bool IsPredecessorModifier => true;

    public virtual float MaxModification => 0.0f;

    public override void FillBrushData(ref SdfBrush brush, int iBrush)
    {
      base.FillBrushData(ref brush, iBrush);

      brush.Operator = (int) OperatorEnum.Modify;
      brush.Blend = MaxModification;
    }

    public override void DrawGizmosRs()
    {
      base.DrawGizmosRs();

      #if UNITY_EDITOR
      bool selected = Selection.Contains(gameObject);
      #else
      bool selected = false;
      #endif

      Color prevColor = Gizmos.color;
        
      Gizmos.color = 
        selected 
          ? GizmosUtil.OutlineSelected 
          : GizmosUtil.OutlineDefault;

      DrawOutlineGizmosRs();

      Gizmos.color = prevColor;
    }
  }
}

