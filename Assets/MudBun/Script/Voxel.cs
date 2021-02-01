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
  public struct VoxelNode
  {
    public static readonly int Stride = 3 * sizeof(float) + 2 * sizeof(int);

    public Vector3 Center;
    public int ParentId;
    public int BrushMaskId;

    public static VoxelNode New(Vector3 center)
    {
      VoxelNode node;
      node.Center = float.MaxValue * Vector3.one;
      node.ParentId = -1;
      node.BrushMaskId = -1;

      return node;
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct VoxelHashEntry
  {
    public static readonly int Stride = sizeof(uint);

    public uint Id;

    public static VoxelHashEntry Null
    {
      get
      {
        VoxelHashEntry v;
        v.Id = 0u;
        return v;
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct VoxelCacheTableEntry
  {
    public static readonly int Stride = sizeof(uint);

    public uint Id;

    public static VoxelHashEntry Null
    {
      get
      {
        VoxelHashEntry v;
        v.Id = 0u;
        return v;
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct VoxelCacheDataEntry
  {
    public static readonly int Stride = 4 * sizeof(float) + SdfBrushMaterial.Stride;

    public Vector4 Data;
    public SdfBrushMaterial Material;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct GenPoint
  {
    public static readonly int Stride = 8 * sizeof(float) + 8 * sizeof(int) + SdfBrushMaterialCompressed.Stride;

    public Vector4 PosNorm;

    // Unity y u no have Vector4Int?
    public int BoneIndex0;
    public int BoneIndex1;
    public int BoneIndex2;
    public int BoneIndex3;

    public uint BoneWeight;
    public int iBrushMask;
    public uint VertId;
    public uint AtSmoothEdge;

    public float SdfValue;
    public float Norm2d;
    public float Padding0;
    public float Padding1;

    public SdfBrushMaterialCompressed Material;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct AutoSmoothVertData
  {
    public static readonly int Stride = 2 * sizeof(int) + 24 * sizeof(float);

    public uint Id;
    public uint NumNormals;
    public Vector4 VertNormalPacked0123;
    public Vector4 VertNormalPacked4567;
    public Vector4 VertNormalPacked89AB;
    public Vector4 Area0123;
    public Vector4 Area4567;
    public Vector4 Area89AB;
  }
}
