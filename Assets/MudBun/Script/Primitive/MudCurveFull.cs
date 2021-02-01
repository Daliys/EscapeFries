/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace MudBun
{
  [ExecuteInEditMode]
  public class MudCurveFull : MudSolid
  {
    /*
    [Header("Noise")]

    [SerializeField] private bool m_enableNoise = false;
    [SerializeField] private float m_noiseOffset = 0.0f;
    [SerializeField] private Vector2 m_noiseBaseOctaveSize = 0.5f * Vector2.one;
    [SerializeField] [Range(0.0f, 1.0f)] private float m_noiseThreshold = 0.5f;
    [SerializeField] [Range(1, 3)] private int m_noiseNumOctaves = 2;
    [SerializeField] private float m_noiseOctaveOffsetFactor = 0.5f;
    public bool EnableNoise { get => m_enableNoise; set { m_enableNoise = value; MarkDirty(); } }
    public float NoiseOffset { get => m_noiseOffset; set { m_noiseOffset = value; MarkDirty(); } }
    public Vector2 NoiseBaseOctaveSize { get => m_noiseBaseOctaveSize; set { m_noiseBaseOctaveSize = value; MarkDirty(); } }
    public float NoiseThreshold { get => m_noiseThreshold; set { m_noiseThreshold = value; MarkDirty(); } }
    public int NoiseNumOctaves { get => m_noiseNumOctaves; set { m_noiseNumOctaves = value; MarkDirty(); } }
    public float NoiseOctaveOffsetFactor { get => m_noiseOctaveOffsetFactor; set { m_noiseOctaveOffsetFactor = value; MarkDirty(); } }
    */

    [Serializable]
    public class Point
    {
      public Transform Transform;
      public float Radius;

      public Point(Transform transform = null, float radius = 0.2f)
      {
        Transform = transform;
        Radius = radius;
      }

      public Point(GameObject go, float radius = 0.2f)
      {
        Transform = go?.transform;
        Radius = radius;
      }
    }

    [Header("Shape")]

    [SerializeField] [Range(1, 16)] private int m_precision = 8;
    public int Precision { get => m_precision; set { m_precision = value; MarkDirty(); } }

    public Transform HeadControlPoint;
    public Transform TailControlPoint;
    [SerializeField] private List<Point> m_points = new List<Point>();
    public ICollection<Point> Points
    {
      get => m_points;
      set
      {
        m_points.Clear();
        foreach (var p in value)
          m_points.Add(p);
        
        MarkDirty();
      }
    }

    public MudCurveFull()
    {
      m_points.Add(new Point());
    }

    public override Aabb Bounds
    {
      get
      {
        Aabb bounds = Aabb.Empty;

        foreach (var p in m_points)
        {
          if (p == null || p.Transform == null)
            continue;

          Vector3 posRs = PointRs(p.Transform.position);
          Vector3 r = 1.5f * p.Radius * Vector3.one;
          bounds.Include(new Aabb(posRs - r, posRs + r));
        }

        return bounds;
      }
    }

    public override void SanitizeParameters()
    {
      base.SanitizeParameters();

      //Validate.NonNegative(ref m_noiseBaseOctaveSize);

      if (m_points != null)
      {
        foreach(var p in m_points)
        {
          if (p == null || p.Transform == null)
            continue;

          Validate.NonNegative(ref p.Radius);
        }
      }
    }

    private void Update()
    {
      foreach (var p in m_points)
      {
        if (p == null || p.Transform == null)
          continue;

        if (!p.Transform.hasChanged)
          continue;

        MarkDirty();
        p.Transform.hasChanged = false;
      }

      if (HeadControlPoint != null && HeadControlPoint.hasChanged)
      {
        MarkDirty();
        HeadControlPoint.hasChanged = false;
      }

      if (TailControlPoint != null && TailControlPoint.hasChanged)
      {
        MarkDirty();
        TailControlPoint.hasChanged = false;
      }
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      if (m_points == null || m_points.Count == 0)
        return 0;

      if (m_points.Any(p => p == null || p.Transform == null))
        return 0;

      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) SdfBrush.TypeEnum.CurveFull;

      if (m_points.Count == 1)
      {
        return 0;
      }

      int iBrush = iStart;

      brush.Data0.x = m_points.Count + 2;
      brush.Data0.y = Precision;
      brush.Data0.z = 0.0f;//m_enableNoise ? 1.0f : 0.0f;

      if (aBone != null)
      {
        brush.BoneIndex = aBone.Count;
        foreach (var p in m_points)
          aBone.Add(p.Transform);
      }

      aBrush[iBrush++] = brush;
      brush.Type = (int) SdfBrush.TypeEnum.Nop;

      /*
      if (m_enableNoise)
      {
        brush.Data0 = new Vector4(m_noiseBaseOctaveSize.x, m_noiseBaseOctaveSize.y, m_noiseBaseOctaveSize.y, m_noiseThreshold);
        brush.Data1 = new Vector4(m_noiseOffset, 0.0f, 0.0f, m_noiseNumOctaves);
        brush.Data2 = new Vector4(m_noiseOctaveOffsetFactor, 0.0f, 0.0f, 0.0f);
        aBrush[iBrush++] = brush;
      }
      */

      int iPreHead = iBrush;
      var head = m_points[0];
      var postHead = m_points[1];
      Vector3 preHeadPosRs = 
        HeadControlPoint == null 
          ? 2.0f * head.Transform.position - postHead.Transform.position 
          : HeadControlPoint.position;
      preHeadPosRs = PointRs(preHeadPosRs);
      brush.Data0 = new Vector4(preHeadPosRs.x, preHeadPosRs.y, preHeadPosRs.z, head.Radius);
      aBrush[iBrush++] = brush;

      for (int i = 0; i < m_points.Count; ++i)
      {
        var p = m_points[i];
        Vector3 pointPosRs = PointRs(p.Transform.position);
        brush.Data0 = new Vector4(pointPosRs.x, pointPosRs.y, pointPosRs.z, p.Radius);
        aBrush[iBrush++] = brush;
      }

      int iPostTail = iBrush;
      var tail = m_points[m_points.Count - 1];
      var preTail = m_points[m_points.Count - 2];
      Vector3 postTailPosRs = 
        TailControlPoint == null 
          ? 2.0f * tail.Transform.position - preTail.Transform.position 
          : TailControlPoint.position;
      postTailPosRs = PointRs(postTailPosRs);
      brush.Data0 = new Vector4(postTailPosRs.x, postTailPosRs.y, postTailPosRs.z, tail.Radius);
      aBrush[iBrush++] = brush;

      if (HeadControlPoint == null)
      {
        Vector3 headControlPosRs = 
          2.0f * head.Transform.position 
          - VectorUtil.CatmullRom
            (
              preHeadPosRs, 
              head.Transform.position, 
              postHead.Transform.position, 
              aBrush[iPreHead + 3].Data0, 
              0.75f
            );
        headControlPosRs = PointRs(headControlPosRs);
        aBrush[iPreHead].Data0 = new Vector4(headControlPosRs.x, headControlPosRs.y, headControlPosRs.z, head.Radius);
      }

      if (TailControlPoint == null)
      {
        Vector3 tailControlPosRs = 
          2.0f * tail.Transform.position 
          - VectorUtil.CatmullRom
            (
              postTailPosRs, 
              tail.Transform.position, 
              preTail.Transform.position, 
              aBrush[iPostTail - 3].Data0, 
              0.75f
            );
        tailControlPosRs = PointRs(tailControlPosRs);
        aBrush[iPostTail].Data0 = new Vector4(tailControlPosRs.x, tailControlPosRs.y, tailControlPosRs.z, tail.Radius);
      }

      return iBrush - iStart;
    }

    public override void DrawSelectionGizmosRs()
    {
      base.DrawSelectionGizmosRs();

      if (m_points == null)
        return;

      if (m_points.Any(p => p == null || p.Transform == null))
        return;

      GizmosUtil.DrawInvisibleCatmullRom
      (
        m_points.Select(p => PointRs(p.Transform.position)).ToArray(), 
        m_points.Select(p => p.Radius).ToArray(), 
        HeadControlPoint != null ? PointRs(HeadControlPoint.position) : VectorUtil.Invalid, 
        TailControlPoint != null ? PointRs(TailControlPoint.position) : VectorUtil.Invalid
      );
    }

    public override void DrawOutlineGizmosRs()
    {
      base.DrawOutlineGizmosRs();

      if (m_points == null)
        return;

      if (m_points.Any(p => p == null || p.Transform == null))
        return;

      GizmosUtil.DrawWireCatmullRom
      (
        m_points.Select(p => PointRs(p.Transform.position)).ToArray(), 
        m_points.Select(p => p.Radius).ToArray(), 
        HeadControlPoint != null ? PointRs(HeadControlPoint.position) : VectorUtil.Invalid, 
        TailControlPoint != null ? PointRs(TailControlPoint.position) : VectorUtil.Invalid
      );
    }
  }
}

