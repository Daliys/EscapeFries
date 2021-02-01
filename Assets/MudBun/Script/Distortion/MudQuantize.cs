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
  public class MudQuantize : MudDistortion
  {
    [SerializeField] private float m_cellSize = 0.25f;
    public float CellSize { get => m_cellSize; set { m_cellSize = value; MarkDirty(); } }

    [SerializeField] [Range(0.0f, 10.0f)] private float m_strength = 5.0f;
    public float Strength { get => m_strength; set { m_strength = value; MarkDirty(); } }

    [SerializeField] [Range(0.0f, 1.0f)] private float m_fade = 1.0f;
    public float Fade { get => m_fade; set { m_fade = value; MarkDirty(); } }

    public override float MaxDistortion => CellSize;

    public override Aabb Bounds
    {
      get
      {
        Vector3 posRs = PointRs(transform.position);
        Vector3 r = 0.5f * VectorUtil.Abs(transform.localScale) + m_fade * m_cellSize * Vector3.one;
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

      Validate.AtLeast(1e-2f, ref m_cellSize);
      Validate.NonNegative(ref m_strength);
      Validate.NonNegative(ref m_fade);
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) SdfBrush.TypeEnum.Quantize;
      brush.Data0.x = m_cellSize;
      brush.Data0.y = m_strength;
      brush.Data0.z = m_fade;
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

