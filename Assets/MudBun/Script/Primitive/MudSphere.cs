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
  public class MudSphere : MudSolid
  {
    [SerializeField] private float m_radius = 0.5f;
    public float Radius { get => m_radius; set { m_radius = value; MarkDirty(); } }

    [Range(-1.0f, 1.0f)] public float PivotShift = 0.0f;
    public Vector3 PivotShiftOffset => -0.5f * transform.up * PivotShift * transform.localScale.y;

    public override Aabb Bounds
    {
      get
      {
        Vector3 r = m_radius * VectorUtil.Abs(transform.localScale);
        Vector3 posRs = PointRs(transform.position) + VectorRs(PivotShiftOffset);
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

      Validate.NonNegative(ref m_radius);
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) SdfBrush.TypeEnum.Sphere;
      brush.Radius = m_radius;
      brush.Data0.x = PivotShift;

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

      GizmosUtil.DrawInvisibleSphere(PointRs(transform.position) + VectorRs(PivotShiftOffset), m_radius, transform.localScale, RotationRs(transform.rotation));
    }

    public override void DrawOutlineGizmosRs()
    {
      base.DrawOutlineGizmosRs();
      
      GizmosUtil.DrawWireSphere(PointRs(transform.position) + VectorRs(PivotShiftOffset), m_radius, transform.localScale, RotationRs(transform.rotation));
    }
  }
}

