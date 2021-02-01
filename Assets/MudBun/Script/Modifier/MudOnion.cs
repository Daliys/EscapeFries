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
  public class MudOnion : MudModifier
  {
    [SerializeField] private float m_thickness = 0.1f;
    public float Thickness { get => m_thickness; set { m_thickness = value; MarkDirty(); } }

    public override float MaxModification => m_thickness;

    public override Aabb Bounds
    {
      get
      {
        Vector3 posRs = PointRs(transform.position);
        Vector3 r = 0.5f * VectorUtil.Abs(transform.localScale) + m_thickness * Vector3.one;
        Aabb bounds = new Aabb(-r, r);
        bounds.Rotate(RotationRs(transform.rotation));
        bounds.Min += posRs;
        bounds.Max += posRs;
        return bounds;
      }
    }

    public override void SanitizeParameters()
    {
      base.SanitizeParameters();

      Validate.AtLeast(1e-2f, ref m_thickness);
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) SdfBrush.TypeEnum.Onion;
      brush.Data0.x = m_thickness;
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

