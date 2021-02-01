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
  public struct SdfBrushMaterial
  {
    public static readonly int Stride = 16 * sizeof(float);

    public Color Color;
    public Color Emission;
    public Vector4 MetallicSmoothnessSizeTightness;
    public Vector4 IntWeight;

    public static SdfBrushMaterial Empty
    {
      get
      {
        SdfBrushMaterial mat;
        mat.Color = Color.white;
        mat.Emission = Color.black;
        mat.MetallicSmoothnessSizeTightness = Vector4.zero;
        mat.IntWeight = Vector4.zero;

        return mat;
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct SdfBrushMaterialCompressed
  {
    public static readonly int Stride = 4 * sizeof(uint) + 4 * sizeof(float);

    public uint Color;
    public uint EmissionTightness;
    public uint IntWeight;
    public uint Padding0;

    public float MetallicSmoothness;
    public float Size;
    public float Hash;
    public float Padding1;
  }


  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct SdfBrush
  {
    public static readonly int Stride = 7 * sizeof(int) + 29 * sizeof(float);

    public enum TypeEnum
    {
      Nop = -1, 

      // groups
      GroupStart = -2, 
      GroupEnd = -3, 

      // primitives
      Box = 0, 
      Sphere, 
      Cylinder, 
      Torus, 
      SolidAngle, 

      // effects
      Particle = 100, 
      ParticleSystem, 
      UniformNoise, 
      CurveSimple, 
      CurveFull, 

      // distortion
      FishEye = 200, 
      Pinch, 
      Twist, 
      Quantize, 

      // modifiers
      Onion = 300, 
    }

    public enum OperatorEnum
    {
      Union, 
      Subtract, 
      Intersect, 
      Dye, 
      NoOp = -1, 
    }

    public enum BoundaryShapeEnum
    {
      Box, 
      Sphere, 
      Cylinder, 
      Torus, 
      SolidAngle, 
    }

    public enum NoiseTypeEnum
    {
      CachedPerlin, 
      Triangle, 
    }

    public enum FlagBit
    {
      Hidden, 
      MirrorX, 
      CountAsBone, 
      CreateMirroredBone, 
      ContributeMaterial, 
      LockNoisePosition, 
      SphericalNoiseCoordinates, 
    }

    public int Type;
    public int Operator;
    public int Proxy;
    public int Index;

    public Vector3 Position;
    public float Blend;

    public Quaternion Rotation;

    public Vector3 Size;
    public float Radius;

    public Vector4 Data0;
    public Vector4 Data1;
    public Vector4 Data2;
    public Vector4 Data3;

    public Bits32 Flags;
    public int MaterialIndex;
    public int BoneIndex;
    public float Hash;

    public static SdfBrush New
    {
      get
      {
        SdfBrush brush;
        brush.Type = -1;
        brush.Operator = 0;
        brush.Proxy = -1;
        brush.Index = -1;

        brush.Position = Vector3.zero;
        brush.Blend = 0.0f;

        brush.Rotation = Quaternion.identity;

        brush.Size = Vector3.one;
        brush.Radius = 0.0f;

        brush.Data0 = Vector4.zero;
        brush.Data1 = Vector4.zero;
        brush.Data2 = Vector4.zero;
        brush.Data3 = Vector4.zero;

        brush.Flags = new Bits32(0);
        brush.MaterialIndex = -1;
        brush.BoneIndex = -1;
        brush.Hash = 0.0f;

        return brush;
      }
    }
  }
}