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

using UnityEngine;

namespace MudBun
{
  public class CustomSolid : MudSolid
  {
    // this value matches kCustomSolid in CustomBrush.cginc
    public static readonly int TypeId = 900;

    [SerializeField] private float m_round = 0.05f;
    public float Round { get => m_round; set { m_round = value; MarkDirty(); } }

    public override Aabb Bounds
    {
      get
      {
        Vector3 posRs = PointRs(transform.position);
        Vector3 r = 0.5f * VectorUtil.Abs(transform.localScale);
        Aabb bounds = new Aabb(-r, r);
        bounds.Rotate(RotationRs(transform.rotation));
        Vector3 round = m_round * Vector3.one;
        bounds.Min += posRs - round;
        bounds.Max += posRs + round;
        return bounds;
      }
    }

    public override void SanitizeParameters()
    {
      base.SanitizeParameters();

      Validate.NonNegative(ref m_round);
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = TypeId;
      brush.Radius = m_round;

      if (aBone != null)
      {
        brush.BoneIndex = aBone.Count;
        aBone.Add(gameObject.transform);
      }

      aBrush[iStart] = brush;

      return 1;
    }

    public override void DrawSelectionGizmosRs()
    {
      base.DrawSelectionGizmosRs();

      GizmosUtil.DrawInvisibleBox(PointRs(transform.position), transform.localScale, RotationRs(transform.rotation));
    }

    public override void DrawOutlineGizmosRs()
    {
      base.DrawOutlineGizmosRs();

      GizmosUtil.DrawWireBox(PointRs(transform.position), transform.localScale, RotationRs(transform.rotation));
    }
  }
}

