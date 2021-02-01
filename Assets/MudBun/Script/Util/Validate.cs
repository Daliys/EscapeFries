/******************************************************************************/
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
  public class Validate
  {
    // float
    //-------------------------------------------------------------------------

    public static float AtLeast(float min, ref float v)
    {
      return v = Mathf.Max(min, v);
    }

    public static float Range(float min, float max, ref float v)
    {
      return v = Mathf.Clamp(v, min, max);
    }

    public static float NonNegative(ref float v)
    {
      return AtLeast(0.0f, ref v);
    }

    public static float Positive(ref float v)
    {
      return AtLeast(MathUtil.Epsilon, ref v);
    }

    public static float Saturate(ref float v)
    {
      return Range(0.0f, 1.0f, ref v);
    }

    //-------------------------------------------------------------------------
    // end: float


    // int
    //-------------------------------------------------------------------------

    public static int AtLeast(int min, ref int v)
    {
      return v = Mathf.Max(min, v);
    }

    public static int Range(int min, int max, ref int v)
    {
      return v = Mathf.Clamp(v, min, max);
    }

    public static int NonNegative(ref int v)
    {
      return AtLeast(0, ref v);
    }

    public static int Positive(ref int v)
    {
      return AtLeast(1, ref v);
    }

    //-------------------------------------------------------------------------
    // end: int


    // Vector2
    //-------------------------------------------------------------------------

    public static Vector2 AtLeast(float min, ref Vector2 v)
    {
      return v =
        new Vector2
        (
          Mathf.Max(min, v.x), 
          Mathf.Max(min, v.y)
        );
    }

    public static Vector2 AtLeast(Vector2 min, ref Vector2 v)
    {
      return v =
        new Vector2
        (
          Mathf.Max(min.x, v.x), 
          Mathf.Max(min.y, v.y)
        );
    }

    public static Vector2 Range(float min, float max, ref Vector2 v)
    {
      return v = 
        new Vector2
        (
          Mathf.Clamp(v.x, min, max), 
          Mathf.Clamp(v.y, min, max)
        );
    }

    public static Vector2 Range(Vector2 min, Vector2 max, ref Vector2 v)
    {
      return v =
        new Vector2
        (
          Mathf.Clamp(v.x, min.x, max.x), 
          Mathf.Clamp(v.y, min.y, max.y)
        );
    }

    public static Vector2 NonNegative(ref Vector2 v)
    {
      return AtLeast(0.0f, ref v);
    }

    public static Vector2 Positive(ref Vector2 v)
    {
      return AtLeast(Mathf.Epsilon, ref v);
    }

    public static Vector2 Saturate(ref Vector2 v)
    {
      return Range(0.0f, 1.0f, ref v);
    }

    //-------------------------------------------------------------------------
    // end: Vector2


    // Vector3
    //-------------------------------------------------------------------------

    public static Vector3 AtLeast(float min, ref Vector3 v)
    {
      return v =
        new Vector3
        (
          Mathf.Max(min, v.x), 
          Mathf.Max(min, v.y), 
          Mathf.Max(min, v.z)
        );
    }

    public static Vector3 AtLeast(Vector3 min, ref Vector3 v)
    {
      return v =
        new Vector3
        (
          Mathf.Max(min.x, v.x), 
          Mathf.Max(min.y, v.y), 
          Mathf.Max(min.z, v.z)
        );
    }

    public static Vector3 Range(float min, float max, ref Vector3 v)
    {
      return v = 
        new Vector3
        (
          Mathf.Clamp(v.x, min, max), 
          Mathf.Clamp(v.y, min, max), 
          Mathf.Clamp(v.z, min, max)
        );
    }

    public static Vector3 Range(Vector3 min, Vector3 max, ref Vector3 v)
    {
      return v =
        new Vector3
        (
          Mathf.Clamp(v.x, min.x, max.x), 
          Mathf.Clamp(v.y, min.y, max.y), 
          Mathf.Clamp(v.z, min.z, max.z)
        );
    }

    public static Vector3 NonNegative(ref Vector3 v)
    {
      return AtLeast(0.0f, ref v);
    }

    public static Vector3 Positive(ref Vector3 v)
    {
      return AtLeast(Mathf.Epsilon, ref v);
    }

    public static Vector3 Saturate(ref Vector3 v)
    {
      return Range(0.0f, 1.0f, ref v);
    }

    //-------------------------------------------------------------------------
    // end: Vector3


    // Vector3Int
    //-------------------------------------------------------------------------

    public static Vector3Int AtLeast(int min, ref Vector3Int v)
    {
      return v =
        new Vector3Int
        (
          Mathf.Max(min, v.x), 
          Mathf.Max(min, v.y), 
          Mathf.Max(min, v.z)
        );
    }

    public static Vector3Int AtLeast(Vector3Int min, ref Vector3Int v)
    {
      return v =
        new Vector3Int
        (
          Mathf.Max(min.x, v.x), 
          Mathf.Max(min.y, v.y), 
          Mathf.Max(min.z, v.z)
        );
    }

    public static Vector3Int Range(int min, int max, ref Vector3Int v)
    {
      return v =
        new Vector3Int
        (
          Mathf.Clamp(v.x, min, max), 
          Mathf.Clamp(v.y, min, max), 
          Mathf.Clamp(v.z, min, max)
        );
    }

    public static Vector3Int Range(Vector3Int min, Vector3Int max, ref Vector3Int v)
    {
      return v =
        new Vector3Int
        (
          Mathf.Clamp(v.x, min.x, max.x), 
          Mathf.Clamp(v.y, min.y, max.y), 
          Mathf.Clamp(v.z, min.z, max.z)
        );
    }

    public static Vector3Int NonNegative(ref Vector3Int v)
    {
      return AtLeast(0, ref v);
    }

    public static Vector3Int Positive(ref Vector3Int v)
    {
      return AtLeast(1, ref v);
    }

    //-------------------------------------------------------------------------
    // end: Vector3Int


    // Vector3
    //-------------------------------------------------------------------------

    public static Vector4 AtLeast(float min, ref Vector4 v)
    {
      return v =
        new Vector4
        (
          Mathf.Max(min, v.x), 
          Mathf.Max(min, v.y), 
          Mathf.Max(min, v.z), 
          Mathf.Max(min, v.w)
        );
    }

    public static Vector4 AtLeast(Vector4 min, ref Vector4 v)
    {
      return v =
        new Vector4
        (
          Mathf.Max(min.x, v.x), 
          Mathf.Max(min.y, v.y), 
          Mathf.Max(min.z, v.z),
          Mathf.Max(min.w, v.w)
        );
    }

    public static Vector4 Range(float min, float max, ref Vector4 v)
    {
      return v =
        new Vector4
        (
          Mathf.Clamp(v.x, min, max), 
          Mathf.Clamp(v.y, min, max), 
          Mathf.Clamp(v.z, min, max), 
          Mathf.Clamp(v.w, min, max)
        );
    }

    public static Vector4 Range(Vector4 min, Vector4 max, ref Vector4 v)
    {
      return v =
        new Vector4
        (
          Mathf.Clamp(v.x, min.x, max.x), 
          Mathf.Clamp(v.y, min.y, max.y), 
          Mathf.Clamp(v.z, min.z, max.z), 
          Mathf.Clamp(v.w, min.w, max.w)
        );
    }

    public static Vector4 NonNegative(ref Vector4 v)
    {
      return AtLeast(0.0f, ref v);
    }

    public static Vector4 Positive(ref Vector4 v)
    {
      return AtLeast(Mathf.Epsilon, ref v);
    }

    public static Vector4 Saturate(ref Vector4 v)
    {
      return Range(0.0f, 1.0f, ref v);
    }

    //-------------------------------------------------------------------------
    // end: Vector3
  }
}

