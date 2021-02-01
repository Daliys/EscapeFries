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
  public class MudTwist : MudDistortion
  {
    [SerializeField] private float m_radius = 0.5f;
    public float Radius { get => m_radius; set { m_radius = value; MarkDirty(); } }

    [SerializeField] private float m_angle = 90.0f;
    public float Angle { get => m_angle; set { m_angle = value; MarkDirty(); } }

    [SerializeField] [Range(1.0f, 5.0f)] private float m_strength = 1.0f;
    public float Strength { get => m_strength; set { m_strength = value; MarkDirty(); } }

    public override float MaxDistortion => 
      2.0f * Mathf.Sin(0.5f * Mathf.Min(MathUtil.Pi, Mathf.Abs(m_angle * MathUtil.Deg2Rad))) * m_radius;

    public override Aabb Bounds
    {
      get
      {
        Vector3 posRs = PointRs(transform.position);
        Vector3 size = VectorUtil.Abs(transform.localScale);
        Vector3 r = new Vector3(m_radius, 0.5f * size.y, m_radius);
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
      brush.Type = (int) SdfBrush.TypeEnum.Twist;
      brush.Radius = m_radius;
      brush.Data0.x = m_angle * MathUtil.Deg2Rad;
      brush.Data0.y = m_strength;
      aBrush[iStart] = brush;

      return 1;
    }

    public override void DrawSelectionGizmosRs()
    {
      base.DrawSelectionGizmosRs();

      GizmosUtil.DrawInvisibleCylinder(PointRs(transform.position), m_radius, transform.localScale.y, RotationRs(transform.rotation));
    }

    public override void DrawOutlineGizmosRs()
    {
      base.DrawOutlineGizmosRs();

      GizmosUtil.DrawWireCylinder(PointRs(transform.position), m_radius, 0.0f, transform.localScale.y, RotationRs(transform.rotation));
    }
  }
}

