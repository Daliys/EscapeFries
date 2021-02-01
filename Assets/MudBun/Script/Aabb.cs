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
using System.Runtime.InteropServices;

using UnityEngine;

namespace MudBun
{
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct Aabb
  {
    public static readonly int Stride = 6 * sizeof(float);

    public Vector3 Min;
    public Vector3 Max;

    public static Aabb Union(Aabb a, Aabb b)
    {
      return 
        new Aabb
        (
          new Vector3
          (
            Mathf.Min(a.Min.x, b.Min.x), 
            Mathf.Min(a.Min.y, b.Min.y), 
            Mathf.Min(a.Min.z, b.Min.z)
          ), 
          new Vector3
          (
            Mathf.Max(a.Max.x, b.Max.x), 
            Mathf.Max(a.Max.y, b.Max.y), 
            Mathf.Max(a.Max.z, b.Max.z)
          )
        );
    }

    public static bool Intersects(Aabb a, Aabb b)
    {
      return 
           a.Min.x <= b.Max.x && a.Max.x >= b.Min.x 
        && a.Min.y <= b.Max.y && a.Max.y >= b.Min.y 
        && a.Min.z <= b.Max.z && a.Max.z >= b.Min.z;
    }

    private static Aabb s_empty = new Aabb(float.MaxValue * Vector3.one, float.MinValue * Vector3.one);
    public static Aabb Empty => s_empty;

    public bool IsEmpty => (Min.x >= Max.x || Min.y >= Max.y || Min.z >= Max.z);

    public float HalfArea { get { Vector3 e = Max - Min; return e.x * e.y + e.y * e.z + e.z * e.x; } }

    public Vector3 Center => (0.5f * (Min + Max));
    public Vector3 Size => (Max - Min);
    public Vector3 Extents => (0.5f * (Max - Min));

    public Aabb(Vector3 min, Vector3 max)
    {
      Min = min;
      Max = max;
    }

    public void Include(Vector3 p)
    {
      Min.x = Mathf.Min(Min.x, p.x);
      Min.y = Mathf.Min(Min.y, p.y);
      Min.z = Mathf.Min(Min.z, p.z);

      Max.x = Mathf.Max(Max.x, p.x);
      Max.y = Mathf.Max(Max.y, p.y);
      Max.z = Mathf.Max(Max.z, p.z);
    }

    public void Include(Aabb aabb)
    {
      Min.x = Mathf.Min(Min.x, aabb.Min.x);
      Min.y = Mathf.Min(Min.y, aabb.Min.y);
      Min.z = Mathf.Min(Min.z, aabb.Min.z);

      Max.x = Mathf.Max(Max.x, aabb.Max.x);
      Max.y = Mathf.Max(Max.y, aabb.Max.y);
      Max.z = Mathf.Max(Max.z, aabb.Max.z);
   }

    public void Expand(float r)
    {
      Min.x -= r;
      Min.y -= r;
      Min.z -= r;

      Max.x += r;
      Max.y += r;
      Max.z += r;
    }

    public void Expand(Vector3 r)
    {
      Min.x -= r.x;
      Min.y -= r.y;
      Min.z -= r.z;

      Max.x += r.x;
      Max.y += r.y;
      Max.z += r.z;
    }

    public void Rotate(Quaternion q)
    {
      Vector3 oldExtentsA = Extents;
      Vector3 newExtents = oldExtentsA;
      newExtents = VectorUtil.Max(newExtents, VectorUtil.Abs(q * new Vector3( oldExtentsA.x,  oldExtentsA.y,  oldExtentsA.z)));
      newExtents = VectorUtil.Max(newExtents, VectorUtil.Abs(q * new Vector3(-oldExtentsA.x,  oldExtentsA.y,  oldExtentsA.z)));
      newExtents = VectorUtil.Max(newExtents, VectorUtil.Abs(q * new Vector3( oldExtentsA.x, -oldExtentsA.y,  oldExtentsA.z)));
      newExtents = VectorUtil.Max(newExtents, VectorUtil.Abs(q * new Vector3( oldExtentsA.x,  oldExtentsA.y, -oldExtentsA.z)));

      Vector3 newCenter = q * Center;
      Min = newCenter - newExtents;
      Max = newCenter + newExtents;
    }

    public void Transform(Transform transform)
    {
      Vector3 center = transform.TransformPoint(Center);
      Vector3 extents = VectorUtil.CompMul(Extents, transform.localScale);
      if (transform.parent != null)
        extents = VectorUtil.CompMul(extents, transform.parent.lossyScale);
      Min = -extents;
      Max = extents;
      Rotate(transform.rotation);
      Min += center;
      Max += center;
    }

    public bool Contains(Aabb rhs)
    {
      return 
           Min.x <= rhs.Min.x 
        && Min.y <= rhs.Min.y 
        && Min.z <= rhs.Min.z 
        && Max.x >= rhs.Max.x 
        && Max.y >= rhs.Max.y 
        && Max.z >= rhs.Max.z;
    }

    // Real-time Collision Detection, p179.
    public float RayCast(Vector3 from, Vector3 to, float maxFraction = 1.0f)
    {
      float tMin = float.MinValue;
      float tMax = float.MaxValue;

      Vector3 d = to - from;
      Vector3 absD = VectorUtil.Abs(d);

      for (int i = 0; i < 3; ++i)
      {
        float dComp = VectorUtil.GetComopnent(d, i);
        float absDComp = VectorUtil.GetComopnent(absD, i);
        float fromComp = VectorUtil.GetComopnent(from, i);
        float minComp = VectorUtil.GetComopnent(Min, i);
        float maxComp = VectorUtil.GetComopnent(Max, i);

        if (absDComp < float.Epsilon)
        {
          // parallel?
          if (fromComp < minComp || maxComp < fromComp)
            return float.MinValue;
        }
        else
        {
          float invD = 1.0f / dComp;
          float t1 = (minComp - fromComp) * invD;
          float t2 = (maxComp - fromComp) * invD;

          if (t1 > t2)
          {
            float temp = t1;
            t1 = t2;
            t2 = temp;
          }

          tMin = Mathf.Max(tMin, t1);
          tMax = Mathf.Min(tMax, t2);

          if (tMin > tMax)
            return float.MinValue;
        }
      }

      // does the ray start inside the box?
      // does the ray intersect beyond the max fraction?
      if (tMin < 0.0f || maxFraction < tMin)
        return float.MinValue;

      // intersection detected
      return tMin;
    }
  }
}

