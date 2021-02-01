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
  public class MudFishEye : MudDistortion
  {
    [SerializeField] private float m_radius = 0.5f;
    public float Radius { get => m_radius; set { m_radius = value; MarkDirty(); } }

    [Range(0.0f, 10.0f)] [SerializeField] private float m_strength = 1.0f;
    public float Amount { get => m_strength; set { m_strength = value; MarkDirty(); } }

    public override float MaxDistortion => m_radius;

    public override Aabb Bounds
    {
      get
      {
        Vector3 posRs = PointRs(transform.position);
        Vector3 r = m_radius * Vector3.one;
        Aabb bounds = new Aabb(-r, r);
        bounds.Min += posRs;
        bounds.Max += posRs;
        return bounds;
      }
    }

    public override void SanitizeParameters()
    {
      base.SanitizeParameters();

      Validate.Positive(ref m_radius);
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) SdfBrush.TypeEnum.FishEye;
      brush.Blend = m_radius;
      brush.Radius = m_radius;
      brush.Data0.x = m_strength;
      aBrush[iStart] = brush;

      return 1;
    }

    public override void DrawSelectionGizmosRs()
    {
      base.DrawSelectionGizmosRs();

      GizmosUtil.DrawInvisibleSphere(PointRs(transform.position), m_radius, Vector3.one, RotationRs(transform.rotation));
    }

    public override void DrawOutlineGizmosRs()
    {
      base.DrawOutlineGizmosRs();

      GizmosUtil.DrawWireSphere(PointRs(transform.position), m_radius, Vector3.one, RotationRs(transform.rotation));
    }
  }
}

