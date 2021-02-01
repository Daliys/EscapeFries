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

using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace MudBun
{
  [ExecuteInEditMode]
  public abstract class MudRendererBase : MonoBehaviour
  {
    #region Enums & Structs

    public enum RenderModeEnum
    {
      FlatMesh, 
      SmoothMesh, 
      CircleSplats, 
      QuadSplats, 
    }

    public enum RenderModeCatergoryEnum
    {
      Mesh, 
      Splats, 
    }

    public enum RenderPipelineEnum
    {
      Invalid = -1, 
      BuiltIn, 
      URP, 
      HDRP, 
    }

    public enum MeshingModeEnum
    {
      MarchingCubes, 
      DualQuads, 
      SurfaceNets, 
      DualContouring, 
    }

    public struct Const
    {
      public struct KernelIndex
      {
        public int ClearVoxelHashTable;
        public int ClearAutoSmoothVertDataTable;
        public int ClearVoxelCache;
        public int RegisterTopNodes;
        public int UpdateBranchingIndirectDispatchArgs;
        public int AllocateChildNodes;
        public int UpdateVoxelIndirectDispatchArgs;

        public int GenerateFlatMarchingCubesMesh;
        public int GenerateSmoothMarchingCubesMesh;
        public int GenerateMarchingCubesSplats;
        public int GenerateFlatMarchingCubesMesh2d;
        public int GenerateSmoothMarchingCubesMesh2d;
        public int GenerateMarchingCubesSplats2d;
        public int UpdateMarchingCubesAutoSmoothIndirectDispatchArgs;
        public int MarchingCubesUpdateAutoSmooth;
        public int MarchingCubesComputeAutoSmooth;

        public int GenerateDualQuads;
        public int GenerateDualQuads2d;
        public int UpdateDualMeshingIndirectDispatchArgs;
        public int DualMeshingFlatMeshNormal;
        public int DualMeshingSmoothMeshNormal;
        public int DualMeshingFlatMeshNormal2d;
        public int DualMeshingSmoothMeshNormal2d;
        public int DualMeshingUpdateAutoSmooth;
        public int DualMeshingComputeAutoSmooth;
        public int DualMeshingUpdateSmoothCornerIndirectDispatchArgs;
        public int DualMeshingSmoothCorner;
        public int UpdateDualMeshingSplatsIndirectArgs;
        public int ConvertDualMeshingSplats;

        public int SurfaceNetsMovePoint;
        public int SurfaceNetsMovePoint2d;

        public int DualContouringMovePoint;
        public int DualContouringMovePoint2d;

        public int GenerateNoiseCache;

        public int RigBones;
      }
      public static KernelIndex Kernel;
      public static Dictionary<ComputeShader, int[]> Kernels;

      public static int TriTable;
      public static int VertTable;
      public static int TriTable2d;

      public static int Brushes;
      public static int BrushMaterials;
      public static int NumBrushes;

      public static int SurfaceShift;

      public static int RenderMode;
      public static int MeshingMode;

      public static int NormalDifferentiationStep;
      public static int NormalQuantization;
      public static int Normal2dFadeDist;
      public static int Normal2dStrength;
      public static int EnableAutoSmooth;
      public static int AutoSmoothMaxAngle;
      public static int AutoSmoothVertDataTable;
      public static int AutoSmoothVertDataPoolSize;
      public static int EnableSmoothCorner;
      public static int SmoothCornerSubdivision;
      public static int SmoothCornerNormalBlur;
      public static int SmoothCornerFade;

      public static int SplatSize;
      public static int SplatSizeJitter;
      public static int SplatNormalShift;
      public static int SplatNormalShiftJitter;
      public static int SplatColorJitter;
      public static int SplatPositionJitter;
      public static int SplatRotationJitter;
      public static int SplatOrientationJitter;
      public static int SplatJitterNoisiness;
      public static int SplatCameraFacing;
      public static int SplatScreenSpaceFlattening;
      public static int SurfaceNetsDualQuadsBlend;
      public static int SurfaceNetsBinarySearchIterations;
      public static int SurfaceNetsGradientDescentIterations;
      public static int SurfaceNetsGradientDescentFactor;
      public static int DualContouringDualQuadsBlend;
      public static int DualContouringRelaxation;
      public static int DualContouringBinarySearchIterations;
      public static int DualContouringGradientDescentIterations;
      public static int DualContouringGradientDescentFactor;

      public static int AabbTree;
      public static int AabbRoot;

      public static int Enable2dMode;
      public static int ForceAllBrushes;
      public static int NumAllocations; // general allocation counters
      public static int NodeHashTable;
      public static int NodeHashTableSize;
      public static int NodePool;
      public static int NodePoolSize;
      public static int NumNodesAllocated;
      public static int UseVoxelCache;
      public static int VoxelCacheIdTable;
      public static int VoxelCache;
      public static int VoxelCacheSize;
      public static int BrushMaskPool;
      public static int BrushMaskPoolSize;
      public static int IndirectDispatchArgs;
      public static int CurrentNodeDepth;
      public static int CurrentNodeBranchingFactor;
      public static int CurrentNodeSize;
      public static int VoxelSize;
      public static int MaxNodeDepth;
      public static int ChunkVoxelDensity;
      public static int GenPoints;
      public static int MaxGenPoints;
      public static int IndirectDrawArgs;

      public static int MasterColor;
      public static int MasterEmission;
      public static int MasterMetallic;
      public static int MasterSmoothness;

      public static int LocalToWorld;
      public static int LocalToWorldIt;
      public static int LocalToWorldScale;
      public static int WorldToLocal;

      public static int NoiseCache;
      public static int NoiseCacheDimension;
      public static int NoiseCacheDensity;
      public static int NoiseCachePeriod;
    }

    #endregion // end: Enums & Structs

    //-------------------------------------------------------------------------

    #region Global Consts

    public static readonly int ThreadGroupExtent = 4;
    public static readonly int ThreadGroupSize = ThreadGroupExtent * ThreadGroupExtent * ThreadGroupExtent;
    public static readonly int ClearThreadGroupSize = 256;
    private static int[] s_voxelTreeBranchingFactors = new int[] { 8, 8, 4 };
    public static int[] VoxelTreeBranchingFactors => s_voxelTreeBranchingFactors; 
    public static int VoxelNodeDepth => VoxelTreeBranchingFactors.Length;
    private static int s_chunkVoxelDensity = -1;
    public static int ChunkVoxelDensity
    {
      get
      {
        if (s_chunkVoxelDensity < 0)
          s_chunkVoxelDensity = VoxelTreeBranchingFactors.Aggregate((x, y) => x * y);
        return s_chunkVoxelDensity;
      }
    }

    // maximum allowed number of brushes per renderer
    // NOTE: MaxBrushes must be a multiple of 32 (8 * sizeof(int))
    //       (MaxBrushes / 32) must be less than or equal to kMaxBrushMaskInts in Brushes.cginc 
    //       MaxBrushes must *NOT* be larger than 2^kAabbTreeNodeStackSize in AabbTree.cginc
    public static readonly int MaxBrushes = 1024;
    public static int MaxBrushMaskInts => MaxBrushes / 32;
    public static int MaxBrushGroupDepth = 8;

    private static int[] s_noiseCacheDimensionInts = new int[] { 256, 128, 256 };
    public static int[] NoiseCacheDimensionInts => s_noiseCacheDimensionInts;
    private static float[] s_noiseCacheDimensionFloats;
    public static float[] NoiseCacheDimensionFloats
    {
      get
      {
        if (s_noiseCacheDimensionFloats == null)
          s_noiseCacheDimensionFloats = NoiseCacheDimensionInts.Select(x => (float)x).ToArray();
        return s_noiseCacheDimensionFloats;
      }
    }
    public static readonly float NoiseCacheDensity = 32.0f;
    private static float[] s_noiseCachePeriod;
    public static float[] NoiseCachePeriod
    {
      get
      {
        if (s_noiseCachePeriod == null)
          s_noiseCachePeriod = NoiseCacheDimensionInts.Select(x => x / NoiseCacheDensity).ToArray();
        return s_noiseCachePeriod;
      }
    }

    public int VoxelToVertexFactor
    {
      get
      {
        switch (RenderMode)
        {
          case RenderModeEnum.FlatMesh:
          case RenderModeEnum.SmoothMesh:
            return 3;

          case RenderModeEnum.CircleSplats:
            return 2;

          case RenderModeEnum.QuadSplats:
            return 3;
        }
        return 3;
      }
    }

    #endregion // end: Global Consts

    //-------------------------------------------------------------------------

    #region Global Resources

    private static bool s_globalResourcesValid = false;

    protected static HashSet<MudRendererBase> s_renderers = new HashSet<MudRendererBase>();
    private static SdfBrush[] s_aSdfBrush = new SdfBrush[MaxBrushes];
    private static SdfBrushMaterial[] s_aSdfBrushMaterial = new SdfBrushMaterial[MaxBrushes];
    private static Dictionary<int, int> s_sdfBrushMaterialIndexMap = new Dictionary<int, int>();
    private static ComputeShader s_computeVoxelGen;
    private static ComputeShader s_computeMarchingCubes;
    private static ComputeShader s_computeDualMeshing;
    private static ComputeShader s_computeSurfaceNets;
    private static ComputeShader s_computeDualContouring;
    private static ComputeShader s_computeNoiseCache;
    private static ComputeShader s_computeMeshLock;
    private static ComputeBuffer s_triTableBuffer;
    private static ComputeBuffer s_vertTableBuffer;
    private static ComputeBuffer s_triTable2dBuffer;
    private static ComputeBuffer s_brushesBuffer;
    private static ComputeBuffer s_brushMaterialBuffer;
    private static ComputeBuffer s_aabbTreeBuffer;
    private static ComputeBuffer s_dummyBuffer;
    private static RenderTexture s_noiseCache;
    protected static RenderPipelineEnum s_renderPipeline = (RenderPipelineEnum)(-1);
    public static RenderPipelineEnum RenderPipeline => DetermineRenderPipeline();

    public static int GlobalResourceGpuMemoryAllocated
    {
      get
      {
        int bytes = 0;

        if (s_triTableBuffer != null)
          bytes += s_triTableBuffer.stride * s_triTableBuffer.count;

        if (s_vertTableBuffer != null)
          bytes += s_vertTableBuffer.stride * s_vertTableBuffer.count;

        if (s_triTable2dBuffer != null)
          bytes += s_triTable2dBuffer.stride * s_triTable2dBuffer.count;

        if (s_brushesBuffer != null)
          bytes += s_brushesBuffer.stride * s_brushesBuffer.count;

        if (s_brushMaterialBuffer != null)
          bytes += s_brushMaterialBuffer.stride * s_brushMaterialBuffer.count;

        if (s_aabbTreeBuffer != null)
          bytes += s_aabbTreeBuffer.stride * s_aabbTreeBuffer.count;

        if (s_noiseCache != null)
          bytes += s_noiseCache.width * s_noiseCache.height * s_noiseCache.volumeDepth * sizeof(float);

        if (s_dummyBuffer != null)
          bytes += s_dummyBuffer.stride * s_dummyBuffer.count;

        return bytes;
      }
    }

    #endregion // end: Global Resources

    //-------------------------------------------------------------------------

    #region Local Resources

    private bool m_localResourcesValid = false;

    //[Header("Budgets")]

    // base budgets
    [Range(1, 2048)] public int MaxVoxelsK = 256;
    [Range(16, 1024)] public int MaxChunks = 64;
    private bool UseVoxelCache = false; // profiler says this doesn't really help with performance

    // derived budgets
    public int MaxVoxels => 1024 * MaxVoxelsK;
    public int MaxVoxelNodes => 1024 * (MaxChunks + MaxVoxelsK);
    public int MaxBrushMasks => MaxChunks;
    public int MaxGenPoints => VoxelToVertexFactor * MaxVoxelNodes;

    // only works in editor
    public bool ShowGpuMemoryUsage = false;
    public bool AutoAdjustBudgetsToHighWaterMarks = false;
    [Range(0, 100)] public int AutoAdjustBudgetsToHighWaterMarksMarginPercent = 16;
    private bool m_autoAdjustBudgetsToHighWaterMarks = false;

    //[Header("Render")]

    // local shader resources
    [Range(0.1f, 64.0f)] public float VoxelDensity = 8.0f;
    public float TopVoxelNodeSize
    {
      get
      {
        float size = ChunkVoxelDensity / VoxelDensity;
        if (LockMeshIntermediateState != LockMeshIntermediateStateEnum.Idle)
          size = ChunkVoxelDensity / MeshGenerationVoxelDensity;
        return size * 1.0001f;
      }
    }
    private float[] m_aNodeSize;
    public float[] NodeSizes
    {
      get
      {
        if (m_aNodeSize == null || m_aNodeSize.Length != VoxelNodeDepth + 1)
          m_aNodeSize = new float[VoxelNodeDepth + 1];
        float nodeSize = TopVoxelNodeSize;
        for (int depth = 0; depth < m_aNodeSize.Length; ++depth)
        {
          m_aNodeSize[depth] = nodeSize;
          if (depth < VoxelNodeDepth)
            nodeSize /= VoxelTreeBranchingFactors[depth];
        }
        return m_aNodeSize;
      }
    }

    private static bool AllowSharedRWBuffers => SystemInfo.graphicsDeviceType != GraphicsDeviceType.Metal; // Metal is weird
    private ComputeBuffer m_brushesBuffer;
    private ComputeBuffer m_brushMaterialBuffer;
    private ComputeBuffer m_aabbTreeBuffer;

    public float VoxelSize => NodeSizes[VoxelNodeDepth];
    private ComputeBuffer m_nodeHashTableBuffer;
    protected ComputeBuffer m_nodePoolBuffer;
    protected ComputeBuffer m_numNodesAllocatedBuffer;
    private ComputeBuffer m_numAllocationsBuffer;
    private ComputeBuffer m_voxelCacheIdTableBuffer;
    private ComputeBuffer m_voxelCacheBuffer;
    private ComputeBuffer m_brushMaskPoolBuffer;
    private ComputeBuffer m_indirectDispatchArgsBuffer;
    private ComputeBuffer m_autoSmoothVertDataTableBuffer;
    protected ComputeBuffer m_genPointsBufferDefault;
    protected ComputeBuffer m_indirectDrawArgsBufferDefault;
    protected ComputeBuffer m_genPointsBufferOverride;
    protected ComputeBuffer m_indirectDrawArgsBufferOverride;
    protected ComputeBuffer m_genPointsBufferUsedForCompute;
    protected ComputeBuffer m_indirectDrawArgsBufferUsedForCompute;

    private static NativeArray<int> s_indirectDrawArgsInitData;
    private static NativeArray<int> IndirectDrawArgsInitData
    {
      get
      {
        bool usingXr = false;
        var aXrDisplaySubsystem = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(aXrDisplaySubsystem);
        foreach (var xrDisplay in aXrDisplaySubsystem)
        {
          if (xrDisplay.running)
          {
            usingXr = true;
            break;
          }
        }
        int numInstances = usingXr ? 2 : 1;

        if (s_indirectDrawArgsInitData.Length > 0)
        {
          s_indirectDrawArgsInitData[1] = numInstances;
          return s_indirectDrawArgsInitData;
        }

        s_indirectDrawArgsInitData = new NativeArray<int>(new int[] { 0, numInstances, 0, 0 }, Allocator.Persistent);
        return s_indirectDrawArgsInitData;
      }
    }

    private static NativeArray<int> s_unitIndirectDispatchArgsInitData;
    private static NativeArray<int> UnitIndirectDispatchArgsInitData
    {
      get
      {
        if (s_unitIndirectDispatchArgsInitData.Length > 0)
          return s_unitIndirectDispatchArgsInitData;

        s_unitIndirectDispatchArgsInitData = new NativeArray<int>(new int[] { 1, 1, 1 }, Allocator.Persistent);
        return s_unitIndirectDispatchArgsInitData;
      }
    }

    private int[] m_numAllocationsBufferInitData;

    private static readonly int NodeHashTableAllocationMultiplier = 2;
    public int NodeHashTableSize => MaxChunks* NodeHashTableAllocationMultiplier;

    private static readonly int AutoSmoothVertDataAllocationMultiplier = 2;
    public int AutoSmoothVertDataTableSize => MaxVoxels * AutoSmoothVertDataAllocationMultiplier;

    public enum NumAllcationIndex
    {
      BrushMask, 
      VoxelCache, 
      VoxelHash, 
      AutoSmoothVertData, 
    }

    private static bool s_warnedResourceAccessPerformanceImpact = false;

    public long LocalResourceGpuMemoryAllocated
    {
      get
      {
        long bytes = 0;

        if (!AllowSharedRWBuffers)
        {
          if (m_brushesBuffer != null)
            bytes += m_brushesBuffer.stride * m_brushesBuffer.count;

          if (m_brushMaterialBuffer != null)
            bytes += m_brushMaterialBuffer.stride * m_brushMaterialBuffer.count;

          if (m_aabbTreeBuffer != null)
            bytes += m_aabbTreeBuffer.stride * m_aabbTreeBuffer.count;
        }

        if (m_nodePoolBuffer != null)
          bytes += m_nodePoolBuffer.stride * ((long) m_nodePoolBuffer.count);

        if (m_nodeHashTableBuffer != null)
          bytes += m_nodeHashTableBuffer.stride * m_nodeHashTableBuffer.count;

        if (m_numNodesAllocatedBuffer != null)
          bytes += m_numNodesAllocatedBuffer.stride * m_numNodesAllocatedBuffer.count;

        if (m_numAllocationsBuffer != null)
          bytes += m_numAllocationsBuffer.stride * m_numAllocationsBuffer.count;

        if (m_voxelCacheIdTableBuffer != null)
          bytes += m_voxelCacheIdTableBuffer.stride * m_voxelCacheIdTableBuffer.count;

        if (m_voxelCacheBuffer != null)
          bytes += m_voxelCacheBuffer.stride * m_voxelCacheBuffer.count;

        if (m_brushMaskPoolBuffer != null)
          bytes += m_brushMaskPoolBuffer.stride * m_brushMaskPoolBuffer.count;

        if (m_indirectDispatchArgsBuffer != null)
          bytes += m_indirectDispatchArgsBuffer.stride * m_indirectDispatchArgsBuffer.count;

        if (m_genPointsBufferDefault != null)
          bytes += m_genPointsBufferDefault.stride * ((long)m_genPointsBufferDefault.count);

        if (m_indirectDrawArgsBufferDefault != null)
          bytes += m_indirectDrawArgsBufferDefault.stride * m_indirectDrawArgsBufferDefault.count;

        if (m_autoSmoothVertDataTableBuffer != null)
          bytes += m_autoSmoothVertDataTableBuffer.stride * m_autoSmoothVertDataTableBuffer.count;

        if (!s_warnedResourceAccessPerformanceImpact && Application.isPlaying)
        {
          Debug.LogWarning("MudBun: Accessing resource usage impacts performance!");
          s_warnedResourceAccessPerformanceImpact = true;
        }

        return bytes;
      }
    }

    public long LocalResourceGpuMemoryUsed
    {
      get
      {
        long bytes = 0;

        if (!AllowSharedRWBuffers)
        {
          if (m_brushesBuffer != null)
            bytes += m_brushesBuffer.stride * m_brushesBuffer.count;

          if (m_brushMaterialBuffer != null)
            bytes += m_brushMaterialBuffer.stride * m_brushMaterialBuffer.count;

          if (m_aabbTreeBuffer != null)
            bytes += m_aabbTreeBuffer.stride * m_aabbTreeBuffer.count;
        }

        if (m_numNodesAllocatedBuffer != null)
        {
          bytes += m_numNodesAllocatedBuffer.stride * ((long)m_numNodesAllocatedBuffer.count);

          var aNumNodesAllocated = new int[m_numNodesAllocatedBuffer.count];
          m_numNodesAllocatedBuffer.GetData(aNumNodesAllocated);
          int numTotalNodes = aNumNodesAllocated[0];
          bytes += numTotalNodes * m_nodePoolBuffer.stride;

          if (m_genPointsBufferDefault != null)
            bytes += 6 * numTotalNodes * m_genPointsBufferDefault.stride;
        }

        int [] aNumAllocated = null;
        if (m_numAllocationsBuffer != null)
        {
          aNumAllocated = new int[m_numAllocationsBuffer.count];
          m_numAllocationsBuffer.GetData(aNumAllocated);
        }
        if (aNumAllocated != null)
        {
          if (m_nodeHashTableBuffer != null)
          {
            int numTotalHashes = aNumAllocated[(int) NumAllcationIndex.VoxelHash];
            bytes += NodeHashTableAllocationMultiplier * numTotalHashes * m_nodeHashTableBuffer.stride;
          }

          if (m_autoSmoothVertDataTableBuffer != null)
          {
            int numSmoothVertData = aNumAllocated[(int) NumAllcationIndex.AutoSmoothVertData];
            bytes += numSmoothVertData * m_autoSmoothVertDataTableBuffer.count;
          }
        }

        if (m_numAllocationsBuffer != null)
          bytes += m_numAllocationsBuffer.stride * m_numAllocationsBuffer.count;

        if (m_voxelCacheIdTableBuffer != null)
          bytes += m_voxelCacheIdTableBuffer.stride * m_voxelCacheIdTableBuffer.count;

        if (m_voxelCacheBuffer != null)
          bytes += m_voxelCacheBuffer.stride * m_voxelCacheBuffer.count;

        if (m_brushMaskPoolBuffer != null)
          bytes += m_brushMaskPoolBuffer.stride * m_brushMaskPoolBuffer.count;

        if (m_indirectDispatchArgsBuffer != null)
          bytes += m_indirectDispatchArgsBuffer.stride * m_indirectDispatchArgsBuffer.count;

        if (m_indirectDrawArgsBufferDefault != null)
          bytes += m_indirectDrawArgsBufferDefault.stride * m_indirectDrawArgsBufferDefault.count;

        if (!s_warnedResourceAccessPerformanceImpact && Application.isPlaying)
        {
          Debug.LogWarning("MudBun: Accessing resource usage impacts performance!");
          s_warnedResourceAccessPerformanceImpact = true;
        }

        return bytes;
      }
    }

    public int NumVerticesAllocated => VoxelToVertexFactor * NumVoxelsAllocated;
    public int NumVerticesGenerated
    {
      get
      {
        if (m_indirectDispatchArgsBuffer == null)
          return 0;

        int[] aIndirectDrawArgs = new int[4];
        m_indirectDrawArgsBufferDefault.GetData(aIndirectDrawArgs);

        if (!s_warnedResourceAccessPerformanceImpact && Application.isPlaying)
        {
          Debug.LogWarning("MudBun: Accessing resource usage impacts performance!");
          s_warnedResourceAccessPerformanceImpact = true;
        }

        return aIndirectDrawArgs[0];
      }
    }

    public int NumVoxelsAllocated => (m_nodePoolBuffer != null) ? MaxVoxels : 0;
    public int NumChunksAllocated => (m_nodeHashTableBuffer != null) ? MaxChunks : 0;
    public int NumVoxelsUsed
    {
      get
      {
        if (m_numNodesAllocatedBuffer == null)
          return 0;

        var aNumAllocated = new int[m_numNodesAllocatedBuffer.count];
        m_numNodesAllocatedBuffer.GetData(aNumAllocated);

        if (!s_warnedResourceAccessPerformanceImpact && Application.isPlaying)
        {
          Debug.LogWarning("MudBun: Accessing resource usage impacts performance!");
          s_warnedResourceAccessPerformanceImpact = true;
        }

        return aNumAllocated[0];
      }
    }
    public int NumChunksUsed
    {
      get
      {
        if (m_numAllocationsBuffer == null)
          return 0;

        var aNumAllocated = new int[m_numAllocationsBuffer.count];
        m_numAllocationsBuffer.GetData(aNumAllocated);
        int numTotalHashes = aNumAllocated[(int)NumAllcationIndex.VoxelHash];

        if (!s_warnedResourceAccessPerformanceImpact && Application.isPlaying)
        {
          Debug.LogWarning("MudBun: Accessing resource usage impacts performance!");
          s_warnedResourceAccessPerformanceImpact = true;
        }

        return numTotalHashes;
      }
    }

    public bool ForceEvaluateAllBrushes = false;

    private bool ShouldForceAllBrushes()
    {
      if (ForceEvaluateAllBrushes)
        return true;

      // trial version can't reference MudSolid
      // should probably manually force this with the manual option anyway
      /*
      if (Enable2dMode)
      {
        if (Normal2dFade > MathUtil.Epsilon)
        {
          foreach (var b in m_aBrush)
          {
            var sb = (MudSolid) b;
            if (sb.Operator == SdfBrush.OperatorEnum.Subtract)
              return true;
          }
        }
      }
      */

      return false;
    }

    public bool Enable2dMode = false;
    [Range(-1.0f, 1.0f)] public float SurfaceShift = 0.0f;

    public RenderModeEnum RenderMode = RenderModeEnum.SmoothMesh;
    public RenderModeCatergoryEnum RenderModeCategory
    {
      get
      {
        switch (RenderMode)
        {
          case RenderModeEnum.FlatMesh:
          case RenderModeEnum.SmoothMesh:
            return RenderModeCatergoryEnum.Mesh;

          case RenderModeEnum.CircleSplats:
          case RenderModeEnum.QuadSplats:
            return RenderModeCatergoryEnum.Splats;
        }
        return RenderModeCatergoryEnum.Mesh;
      }
    }

    public MeshingModeEnum MeshingMode = MeshingModeEnum.MarchingCubes;

    public bool ShowAdvancedNormalOptions = false;
    [Range(0.0f, 1.0f)] public float SmoothNormalBlurRelative = 0.05f;
    [Range(0.0f, 0.2f)] public float SmoothNormalBlurAbsolute = 0.0f;
    [Range(0.0f, 1.0f)] public float NormalQuantization = 0.0f;
    [Range(0.0f, 1.0f)] public float Normal2dFade = 0.0f;
    [Range(0.0f, 1.0f)] public float Normal2dStrength = 1.0f;
    public bool EnableAutoSmoothing = false;
    [Range(0.0f, 180.0f)] public float AutoSmoothingMaxAngle = 30.0f;
    public bool EnableSmoothCorner = false;
    [Range(1, 4)] public int SmoothCornerSubdivision = 2;
    [Range(0.001f, 0.1f)] public float SmoothCornerNormalBlur = 0.02f;
    [Range(0.0f, 1.0f)] public float SmoothCornerFade = 0.0f;

    private bool ShouldDoAutoSmoothing
    {
      get
      {
        if (!EnableAutoSmoothing)
          return false;

        if (Enable2dMode)
          return false;

        switch (MeshingMode)
        {
          case MeshingModeEnum.MarchingCubes:
          case MeshingModeEnum.SurfaceNets:
          case MeshingModeEnum.DualContouring:
            switch (RenderMode)
            {
              case RenderModeEnum.FlatMesh:
              case RenderModeEnum.SmoothMesh:
                return true;
            }
            break;
        }
        return false;
      }
    }

    public bool ShowAdvancedSplatOptions = false;
    [Range(0.0f, 5.0f)] public float SplatSize = 1.0f;
    [Range(0.0f, 1.0f)] public float SplatSizeJitter = 0.0f;
    [Range(0.0f, 1.0f)] public float SplatNormalShift = 0.0f;
    [Range(0.0f, 1.0f)] public float SplatNormalShiftJitter = 0.0f;
    [Range(0.0f, 1.0f)] public float SplatColorJitter = 0.0f;
    [Range(0.0f, 1.0f)] public float SplatPositionJitter = 0.0f;
    [Range(0.0f, 1.0f)] public float SplatRotationJitter = 0.0f;
    [Range(0.0f, 1.0f)] public float SplatOrientationJitter = 0.0f;
    [Range(0.01f, 1.0f)] public float SplatJitterNoisiness = 1.0f;
    [Range(0.0f, 1.0f)] public float SplatCameraFacing = 0.0f;
    [Range(0.0f, 1.0f)] public float SplatScreenSpaceFlattening = 1.0f;

    [Range(0.0f, 1.0f)] public float SurfaceNetsDualQuadsBlend = 0.0f;
    public bool ShowAdvancedMeshingOptions = false;
    //[Range(0, 10)] public int SurfaceNetsBinarySearchIterations = 0;
    //[Range(0, 5)] public int SurfaceNetsGradientDescentIterations = 0;
    //[Range(0.0f, 1.0f)] public float SurfaceNetsGradientDescentFactor = 1.0f;
    public bool SurfaceNetsHighAccuracyMode = false;
    [Range(0.0f, 1.0f)] public float DualContouringDualQuadsBlend = 0.0f;
    [Range(0.0f, 1.0f)] public float DualContouringRelaxation = 0.0f;
    //[Range(0, 10)] public int DualContouringBinarySearchIterations = 0;
    //[Range(0, 5)] public int DualContouringGradientDescentIterations = 0;
    //[Range(0.0f, 1.0f)] public float DualContouringGradientDescentFactor = 1.0f;
    public bool DualContouringHighAccuracyMode = false;

    public ShadowCastingMode CastShadows = ShadowCastingMode.On;
    public bool ReceiveShadows = true;

    public ICollection<MudBrushBase> Brushes => m_aBrush;
    private List<MudBrushBase> m_aBrush = new List<MudBrushBase>();
    private List<MudBrushBase> m_aBrushToProcess = new List<MudBrushBase>();
    private bool m_needRescanBrushes = false;
    private static readonly float AabbTreeFatBoundsRadius = 0.25f;
    protected AabbTree<MudBrushBase> m_aabbTree;
    private void ValidateAabbTree()
    {
      if (m_aabbTree != null)
        return;

      m_aabbTree = new AabbTree<MudBrushBase>(AabbTreeFatBoundsRadius);
    }

    //[Header("Material")]

    private MudSharedMaterialBase m_usedSharedMaterial;
    public MudSharedMaterialBase SharedMaterial;
    [SerializeField] private Color m_masterColor = Color.white;
    [SerializeField] private Color m_masterEmission = Color.white;
    [SerializeField] [Range(0.0f, 1.0f)] private float m_masterMetallic = 1.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float m_masterSmoothness = 1.0f;
    public Color MasterColor => m_usedSharedMaterial ? m_usedSharedMaterial.Color : m_masterColor;
    public Color MasterEmission => m_usedSharedMaterial ? m_usedSharedMaterial.Emission : m_masterEmission;
    public float MasterMetallic => m_usedSharedMaterial ? m_usedSharedMaterial.Metallic : m_masterMetallic;
    public float MasterSmoothness => m_usedSharedMaterial ? m_usedSharedMaterial.Smoothness : m_masterSmoothness;

    public Material RenderMaterialMesh;
    public Material RenderMaterialSplats;
    public Material RenderMaterial => (RenderModeCategory == RenderModeCatergoryEnum.Splats) ? RenderMaterialSplats : RenderMaterialMesh;
    private Material m_materialCloned;
    private Material m_materialUsed;
    private MaterialPropertyBlock m_materialProps;

    //[Header("Editor")]

    public bool EnableClickSelection = true;

    //[Header("Debug")]

    public bool AlwaysDrawGizmos = false;
    public bool DrawRawBrushBounds = false;
    public bool DrawComputeBrushBounds = false;
    public bool DrawRenderBounds = false;
    public bool DrawVoxelNodes = false;
    [Range(-1, 3)] public int DrawVoxelNodesDepth = -1;
    [Range(0.0f, 1.0f)] public float DrawVoxelNodesScale = 1.0f;

    public Aabb RenderBounds
    {
      get
      {
        var bounds = RenderBoundsCs;
        if (!bounds.IsEmpty) // don't expand if empty, or it might crash the GPU!
          bounds.Expand(SurfaceShift);
        bounds.Transform(transform);
        return bounds;
      }
    }

    public Aabb RenderBoundsCs => m_aabbTree.Bounds;

    [SerializeField] [HideInInspector] private string m_firstTrackedVersion = "";
    [SerializeField] [HideInInspector] private string m_previousTrackedVersion = "";
    [SerializeField] [HideInInspector] private string m_currentTrackedVersion = "";
    public string FirstTrackedVersion => m_firstTrackedVersion;
    public string PreviousTrackedVersion => m_previousTrackedVersion;
    public string CurrentTrackedVersion => m_currentTrackedVersion;

    #endregion // end: Local Resources

    //-------------------------------------------------------------------------

    #region Mesh Utilities

    public static readonly float MaxMeshGenerationVoxelDensityFreeVersion = 8.0f;
    public static readonly int MaxMeshGenerationTrianglesFreeVersion = 4096;

    public float MeshGenerationVoxelDensity
    {
      get
      {
        #if MUDBUN_FREE
        return Mathf.Min(MaxMeshGenerationVoxelDensityFreeVersion, VoxelDensity);
        #else
        return VoxelDensity;
        #endif
      }
    }

    public enum RenderableMeshMode
    {
      None, 
      Procedural, 
      MeshRenderer, 
    }

    public enum UVGenerationMode
    {
      None, 
      PerTriangleEditorOnly, 
      //Spherical, 
    }

    public bool MeshGenerationCreateNewObject = false;
    public bool MeshGenerationCreateCollider = false;
    public bool MeshGenerationCreateRigidBody = false;
    #if MUDBUN_FREE
    [Range(0.0f, 8.0f)] public float MeshGenerationColliderVoxelDensity = 4.0f;
    #else
    [Range(0.0f, 128.0f)] public float MeshGenerationColliderVoxelDensity = 4.0f;
    #endif
    public RenderableMeshMode MeshGenerationRenderableMeshMode = RenderableMeshMode.Procedural;
    public bool MeshGenerationAutoRigging = false;
    public UVGenerationMode MeshGenerationUVGeneration = UVGenerationMode.None;
    public bool MeshGenerationLockOnStart = false;
    public bool GenerateMeshAssetByEditor = true; // editor only
    public string GenerateMeshAssetByEditorName = ""; // editor only
    public bool RecursiveLockMeshByEditor = true; // editor only
    [SerializeField] [HideInInspector] public bool MeshGenerationLockOnStartByEditor = false; // editor only

    [Serializable]
    private class TransformCache
    {
      public Transform Transform;
      public Transform Parent;
      public Vector3 GlobalPosition;
      public Quaternion GlobalRotation;
      public Vector3 LocalPosition;
      public Quaternion LocalRotation;
      public Vector3 LocalScale;
      public bool HasBrushDescendants;
    }

    // order: parent->child
    [SerializeField] [HideInInspector] private List<TransformCache> m_aBrushTransformCache;
    [SerializeField] [HideInInspector] private List<TransformCache> m_aNestedRendereTransformCache;

    private bool HashNonMudBunObjectInHierarchy(Transform t)
    {
      if (t == null)
        return false;

      if (t.GetComponent<Collider>() != null || t.GetComponent<Collider2D>() != null)
        return true;

      if (t.GetComponent<MeshRenderer>() != null || t.GetComponent<SkinnedMeshRenderer>() != null)
      {
        var r = t.GetComponent<MudRendererBase>();
        if (r == null || !r.MeshLocked)
        {
          if (t.GetComponent<MudRendererBase>() != null)
            return true;
        }
      }

      for (int i = 0; i < t.childCount; ++i)
      {
        if (HashNonMudBunObjectInHierarchy(t.GetChild(i)))
          return true;
      }

      return false;
    }

    private bool HasBrushInHierarchy(Transform t)
    {
      if (t == null)
        return false;

      if (t.GetComponent<MudBrushBase>() != null)
        return true;

      for (int i = 0; i < t.childCount; ++i)
      {
        if (HasBrushInHierarchy(t.GetChild(i)))
          return true;
      }

      return false;
    }

    private void DetectMixedHierarchy(Transform t)
    {
      if (!HasBrushInHierarchy(t) || !HashNonMudBunObjectInHierarchy(t))
        return;

      Debug.LogWarning
      (
           "WARNING: Mixed MudBun objects and non-MudBun objects in renderer hierarchy detected during auto-rigging. "
         + "This can cause issues when normalizing bones to a unit scale of (1, 1, 1).\n" 
         + "Please follow this guideline: " 
         + "Objects with mixed MudBun objects and non-MudBun objects in their hierarchy should have a unit scale of (1, 1, 1)."
      );
    }

    private void CacheBoneTransforms()
    {
      m_aBrushTransformCache = new List<TransformCache>();
      m_aNestedRendereTransformCache = new List<TransformCache>();
      CacheBoneTransformsRecursive(transform);
      DetectMixedHierarchy(transform);
    }

    private void NormalizeBoneTransforms()
    {
      if (m_aBrushTransformCache == null)
        return;

      // move nested renderers out of the way
      foreach (var cache in m_aNestedRendereTransformCache)
      {
        cache.Transform.SetParent(null, true);
      }

      // set each bone's local scale to unit scale to avoid shearing in child bones
      foreach (var cache in m_aBrushTransformCache)
      {
        if (cache.HasBrushDescendants)
        {
          cache.Transform.localScale = Vector3.one;
          cache.Transform.position = cache.GlobalPosition;
          cache.Transform.rotation = cache.GlobalRotation;
        }
        else
        {
          cache.Transform.position = cache.GlobalPosition;
          cache.Transform.rotation = cache.GlobalRotation;
        }
      }

      // put nested renderers back in hierarchy
      foreach (var cache in m_aNestedRendereTransformCache)
      {
        cache.Transform.SetParent(cache.Parent, true);
        cache.Transform.position = cache.GlobalPosition;
        cache.Transform.rotation = cache.GlobalRotation;
      }
    }

    private void CacheBoneTransformsRecursive(Transform t)
    {
      if (t == null)
        return;

      var cache = 
        new TransformCache()
        {
          Transform = t, 
          Parent = t.parent, 
          GlobalPosition = t.position, 
          GlobalRotation = t.rotation, 
          LocalPosition = t.localPosition, 
          LocalRotation = t.localRotation, 
          LocalScale = t.localScale, 
          HasBrushDescendants = HasBrushInHierarchy(t), 
        };

      if (t != transform)
      {
        if (t.GetComponent<MudRendererBase>() == null)
        {
          m_aBrushTransformCache.Add(cache);
        }
        else
        {
          // renderer blocks recursion
          m_aNestedRendereTransformCache.Add(cache);
          return;
        }
      }

      for (int i = 0; i < t.childCount; ++i)
      {
        var child = t.GetChild(i);

        CacheBoneTransformsRecursive(t.GetChild(i));
      }
    }

    private void RestoreBoneTransforms()
    {
      if (m_aBrushTransformCache == null)
        return;

      // move nested renderers out of the way
      foreach (var cache in m_aNestedRendereTransformCache)
      {
        try
        {
          cache.Transform.SetParent(null, true);
        }
        catch (Exception)
        { }
      }

      // restore brush transforms
      foreach (var cache in m_aBrushTransformCache)
      {
        try
        {
          cache.Transform.localScale = cache.LocalScale;
          cache.Transform.localPosition = cache.LocalPosition;
          cache.Transform.localRotation = cache.LocalRotation;
        }
        catch (Exception)
        { }
      }

      // restore nested renderer transforms
      foreach (var cache in m_aNestedRendereTransformCache)
      {
        try
        {
          cache.Transform.SetParent(cache.Parent, true);
          cache.Transform.localScale = cache.LocalScale;
          cache.Transform.localPosition = cache.LocalPosition;
          cache.Transform.localRotation = cache.LocalRotation;
        }
        catch (Exception)
        { }
      }

      m_aBrushTransformCache = null;
      m_aNestedRendereTransformCache = null;
    }

    public enum GeneratedMeshType
    {
      Standard, 
      Compute, 
      Collider, 
    }

    private class PendingMeshData
    {
      public Mesh Mesh;
      public GeneratedMeshType MeshType;
      public Transform RootBone;
      public List<Transform> Bones;
      public bool DoRigging;
      public UVGenerationMode UVGeneration;
      public bool Async;
      public ComputeBuffer IndirectDrawArgsBuffer;
      public ComputeBuffer GenPointsBuffer;

      public void Dispose()
      {
        if (IndirectDrawArgsBuffer != null)
        {
          IndirectDrawArgsBuffer.Release();
          IndirectDrawArgsBuffer = null;
        }

        if (GenPointsBuffer != null)
        {
          GenPointsBuffer.Release();
          GenPointsBuffer = null;
        }
      }
    }

    private Dictionary<AsyncGPUReadbackRequest, PendingMeshData> m_pendingMeshTable = new Dictionary<AsyncGPUReadbackRequest, PendingMeshData>();
    public bool IsAnyMeshGenerationPending => m_pendingMeshTable != null && m_pendingMeshTable.Count > 0;
    public bool IsMeshGenerationPending(Mesh mesh)
    {
      foreach (var pair in m_pendingMeshTable)
        if (pair.Value.Mesh == mesh)
          return true;

      return false;
    }
    public void WaitForMeshGeneration(Mesh mesh)
    {
      foreach (var pair in m_pendingMeshTable)
      {
        if (pair.Value.Mesh != mesh)
          continue;

        pair.Key.WaitForCompletion();
        break;
      }
    }

    public Mesh GenerateMesh
    (
      GeneratedMeshType meshType, 
      bool async, 
      Mesh mesh = null, 
      UVGenerationMode uvGeneration = UVGenerationMode.None
    )
    {
      Transform [] aBone;
      return GenerateMesh(meshType, null, out aBone, async, mesh, uvGeneration);
    }

    public Mesh GenerateMesh
    (
      GeneratedMeshType meshType, 
      Transform rootBone, 
      out Transform [] aBone, 
      bool async, 
      Mesh mesh = null, 
      UVGenerationMode uvGeneration = UVGenerationMode.None
    )
    {
      aBone = null;

      if (meshType == GeneratedMeshType.Compute)
      {
        MarkNeedsCompute();
        return null;
      }

      ValidateLocalResources();

      var prevRenderMode = RenderMode;
      float prevVoxelDensity = VoxelDensity;
      int prevMaxVoxelsK = MaxVoxelsK;
      int prevMaxChunks = MaxChunks;
      if (meshType == GeneratedMeshType.Collider)
      {
        RenderMode = RenderModeEnum.FlatMesh;
        VoxelDensity = MeshGenerationColliderVoxelDensity;
      }
      else
      {
        VoxelDensity = MeshGenerationVoxelDensity;
      }
      if (RenderModeCategory == RenderModeCatergoryEnum.Splats)
      {
        RenderMode = RenderModeEnum.FlatMesh;
      }

      // always override buffers to work in Metal
      m_indirectDrawArgsBufferOverride = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
      m_genPointsBufferOverride = new ComputeBuffer(MaxGenPoints, GenPoint.Stride);
      var indirectDrawArgsBuffer = m_indirectDrawArgsBufferOverride;
      var genPointsBuffer = m_genPointsBufferOverride;

      indirectDrawArgsBuffer.SetData(IndirectDrawArgsInitData);
      ForceCompute();

      RenderMode = prevRenderMode;
      VoxelDensity = prevVoxelDensity;
      MaxVoxelsK = prevMaxVoxelsK;
      MaxChunks = prevMaxChunks;
      MarkNeedsCompute();

      if (indirectDrawArgsBuffer == null)
        return null;

      if (genPointsBuffer == null)
        return null;

      if (mesh == null)
        mesh = new Mesh();

      var pendingMeshData = 
        new PendingMeshData()
        {
          Mesh = mesh, 
          MeshType = meshType, 
          RootBone = rootBone, 
          Bones = m_aBone, 
          DoRigging = m_doRigging, 
          UVGeneration = uvGeneration, 
          Async = async, 
          IndirectDrawArgsBuffer = indirectDrawArgsBuffer, 
          GenPointsBuffer = genPointsBuffer, 
        };

      if (m_aBone != null)
      {
        aBone = m_aBone.ToArray();
        m_aBone = null;
      }

      var request = AsyncGPUReadback.Request(pendingMeshData.IndirectDrawArgsBuffer, OnIndirectDrawArgsBufferRead);
      m_pendingMeshTable.Add(request, pendingMeshData);
      if (!async)
        request.WaitForCompletion();

      return mesh;
    }

    private void OnIndirectDrawArgsBufferRead(AsyncGPUReadbackRequest request)
    {
      PendingMeshData pendingMeshData;
      bool dataFound = m_pendingMeshTable.TryGetValue(request, out pendingMeshData);
      m_pendingMeshTable.Remove(request);
      var aDrawArgs = request.GetData<int>();
      if (!dataFound 
          || aDrawArgs.Length <= 0 
          || request.hasError)
      {
        if (pendingMeshData != null)
          pendingMeshData.Dispose();
        return;
      }

      int numVerts = aDrawArgs[0];
      if (MudBun.IsFreeVersion)
        numVerts = Mathf.Min(numVerts, 3 * MaxMeshGenerationTrianglesFreeVersion);

      if (numVerts <= 0)
      {
        pendingMeshData.Dispose();
        return;
      }

      var newRequest = AsyncGPUReadback.Request(pendingMeshData.GenPointsBuffer, numVerts * pendingMeshData.GenPointsBuffer.stride, 0, OnGenPointsBufferRead);
      m_pendingMeshTable.Add(newRequest, pendingMeshData);
      if (!pendingMeshData.Async)
        newRequest.WaitForCompletion();
    }

    private void OnGenPointsBufferRead(AsyncGPUReadbackRequest request)
    {
      PendingMeshData pendingMeshData;
      bool dataFound = m_pendingMeshTable.TryGetValue(request, out pendingMeshData);
      m_pendingMeshTable.Remove(request);
      if (!dataFound 
          || request.hasError)
      {
        if (pendingMeshData != null)
          pendingMeshData.Dispose();
        return;
      }

      var aGenPoint = request.GetData<GenPoint>();
      BuildMesh(pendingMeshData.Mesh, pendingMeshData.MeshType, aGenPoint, pendingMeshData.RootBone, pendingMeshData.Bones, pendingMeshData.DoRigging, pendingMeshData.UVGeneration);

      if (pendingMeshData.Async 
          && pendingMeshData.IndirectDrawArgsBuffer == m_indirectDrawArgsBufferDefault 
          && pendingMeshData.GenPointsBuffer == m_genPointsBufferDefault 
          && m_isMeshLocked)
      {
        DisposeLocalResources();
      }

      pendingMeshData.Dispose();
    }

    private void BuildMesh
    (
      Mesh mesh, 
      GeneratedMeshType meshType, 
      NativeArray<GenPoint> aGenPoint, 
      Transform rootBone, 
      List<Transform> 
      bones, bool doRigging, 
      UVGenerationMode uvGeneration
    )
    {
      int numVerts = aGenPoint.Length;
      var aVertex = new NativeArray<Vector3>(numVerts, Allocator.Temp);
      var aNormal = new NativeArray<Vector3>(numVerts, Allocator.Temp);
      var aVertIndex = new NativeArray<int>(numVerts, Allocator.Temp);
      for (int i = 0; i < numVerts; ++i)
      {
        aVertex[i] = aGenPoint[i].PosNorm;
        aNormal[i] = Codec.UnpackNormal(aGenPoint[i].PosNorm.w);
        aVertIndex[i] = i;
      }

      mesh.Clear();
      mesh.SetVertices(aVertex);
      mesh.SetNormals(aNormal);
      mesh.indexFormat = numVerts > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16;
      mesh.SetIndices(aVertIndex, MeshTopology.Triangles, 0);

      if (meshType == GeneratedMeshType.Standard)
      {
        Color [] aColor = new Color[numVerts];
        Vector4 [] aEmissionHash = new Vector4[numVerts];
        Vector2 [] aMetallicSmoothness = new Vector2[numVerts];

        BoneWeight [] aBoneWeight = null;
        if (doRigging)
        {
          CacheBoneTransforms();
          aBoneWeight = new BoneWeight[numVerts];
        }
        
        for (int i = 0; i < numVerts; ++i)
        {
          Color c = Codec.UnpackRgba(aGenPoint[i].Material.Color);
          Color et = Codec.UnpackRgba(aGenPoint[i].Material.EmissionTightness);

          if (QualitySettings.activeColorSpace == ColorSpace.Linear)
          {
            float gamma = 2.2f;
            aColor[i] = 
              new Color
              (
                c.r * Mathf.Pow(MasterColor.r, gamma), 
                c.g * Mathf.Pow(MasterColor.g, gamma), 
                c.b * Mathf.Pow(MasterColor.b, gamma), 
                c.a * MasterColor.a
              );
            aEmissionHash[i] = 
              new Vector4
              (
                et.r * Mathf.Pow(MasterEmission.r, gamma), 
                et.g * Mathf.Pow(MasterEmission.g, gamma), 
                et.b * Mathf.Pow(MasterEmission.b, gamma), 
                aGenPoint[i].Material.Hash
              );
          }
          else
          {
            aColor[i] = c * MasterColor;
            aEmissionHash[i] = 
              new Vector4
              (
                et.r * MasterEmission.r, 
                et.g * MasterEmission.g, 
                et.b * MasterEmission.b, 
                aGenPoint[i].Material.Hash
              );
          }

          Vector2 ms = Codec.UnpackSaturated(aGenPoint[i].Material.MetallicSmoothness);
          aMetallicSmoothness[i] = new Vector2(ms.x * MasterMetallic, ms.y * MasterSmoothness);

          if (aBoneWeight != null)
          {
            aBoneWeight[i].boneIndex0 = aGenPoint[i].BoneIndex0;
            aBoneWeight[i].boneIndex1 = aGenPoint[i].BoneIndex1;
            aBoneWeight[i].boneIndex2 = aGenPoint[i].BoneIndex2;
            aBoneWeight[i].boneIndex3 = aGenPoint[i].BoneIndex3;

            Vector4 boneWeight = Codec.UnpackRgba(aGenPoint[i].BoneWeight);
            boneWeight /= Mathf.Max(MathUtil.Epsilon, Vector4.Dot(boneWeight, Vector4.one));
            aBoneWeight[i].weight0 = boneWeight.x;
            aBoneWeight[i].weight1 = boneWeight.y;
            aBoneWeight[i].weight2 = boneWeight.z;
            aBoneWeight[i].weight3 = boneWeight.w;

            if (aBoneWeight[i].boneIndex0 < 0)
            {
              aBoneWeight[i].boneIndex0 = 0;
              aBoneWeight[i].weight0 = 0.0f;
            }

            if (aBoneWeight[i].boneIndex1 < 0)
            {
              aBoneWeight[i].boneIndex1 = 0;
              aBoneWeight[i].weight1 = 0.0f;
            }

            if (aBoneWeight[i].boneIndex2 < 0)
            {
              aBoneWeight[i].boneIndex2 = 0;
              aBoneWeight[i].weight2 = 0.0f;
            }

            if (aBoneWeight[i].boneIndex3 < 0)
            {
              aBoneWeight[i].boneIndex3 = 0;
              aBoneWeight[i].weight3 = 0.0f;
            }
          }
        }
        mesh.SetColors(aColor);
        mesh.SetUVs(6, aEmissionHash);
        mesh.SetUVs(7, aMetallicSmoothness);
        if (aBoneWeight != null 
            && rootBone != null)
        {
          NormalizeBoneTransforms();
          mesh.boneWeights = aBoneWeight;
          mesh.bindposes = bones.Select(x => x.worldToLocalMatrix * rootBone.localToWorldMatrix).ToArray();
        }

        GenerateUV(mesh, uvGeneration);
      }
    }

    virtual protected void GenerateUV(Mesh mesh, UVGenerationMode mode) { }

    [SerializeField] [HideInInspector] private bool m_isMeshLocked = false;
    public bool MeshLocked => m_isMeshLocked;

    protected enum LockMeshIntermediateStateEnum
    {
      Idle, 
      PreLock, 
      PostLock, 
      PreUnlock, 
    }
    protected virtual LockMeshIntermediateStateEnum LockMeshIntermediateState => LockMeshIntermediateStateEnum.Idle;

    protected bool m_doRigging = false;

    public virtual Mesh AddCollider
    (
      GameObject go, 
      bool async, 
      Mesh mesh = null, 
      bool makeRigidBody = false
    )
    {
      return null;
    }

    public virtual Mesh AddLockedStandardMesh
    (
      GameObject go, 
      bool autoRigging, 
      bool async, 
      Mesh mesh = null, 
      UVGenerationMode uvGeneration = UVGenerationMode.None
    )
    {
      return null;
    }

    public virtual void LockMesh
    (
      bool autoRigging, 
      bool async, 
      Mesh mesh = null, 
      UVGenerationMode uvGeneration = UVGenerationMode.None
    )
    {
      m_isMeshLocked = true;
    }

    public virtual void UnlockMesh()
    {
      m_isMeshLocked = false;
      m_needsCompute = true;
      RestoreBoneTransforms();

      MarkNeedsCompute();
    }

    #endregion // end: Mesh Utilities

    //-------------------------------------------------------------------------

    #region Optimization

    public bool UseCutoffVolume = false;
    public Transform CutoffVolumeCenter;
    public Vector3 CutoffVolumeSize = Vector3.one;

    #endregion // end: Optimization

    //-------------------------------------------------------------------------

    #region Callbacks

    protected virtual void OnSharedMaterialChanged(UnityEngine.Object material) { }

    private void Start()
    {
      if ((MeshGenerationLockOnStart || (MeshGenerationLockOnStartByEditor && MeshGenerationRenderableMeshMode == RenderableMeshMode.Procedural)) 
          && Application.isPlaying 
          && !m_isMeshLocked)
      {
        if (MeshGenerationCreateCollider)
          AddCollider(gameObject, false, null, MeshGenerationCreateRigidBody);

        LockMesh(MeshGenerationAutoRigging, false);
      }
    }

    protected virtual void OnEnable()
    {
      s_renderers.Add(this);

      m_needRescanBrushes = true;
      m_needsCompute = true;

      m_aabbTree = new AabbTree<MudBrushBase>(AabbTreeFatBoundsRadius);

      MarkNeedsCompute();

      MudSharedMaterialBase.OnSharedMaterialChanged += OnSharedMaterialChanged;

      m_previousTrackedVersion = m_currentTrackedVersion;
      m_currentTrackedVersion = MudBun.Version;
      if (m_firstTrackedVersion.Equals(""))
        m_firstTrackedVersion = m_currentTrackedVersion;
      if (m_previousTrackedVersion.Equals(""))
        m_previousTrackedVersion = m_currentTrackedVersion;
    }

    protected virtual void OnDisable()
    {
      s_renderers.Remove(this);

      TryDisposeResources();

      m_aabbTree.Dispose();
      m_aabbTree = null;

      MudSharedMaterialBase.OnSharedMaterialChanged -= OnSharedMaterialChanged;
    }

    protected virtual void OnValidate()
    {
      SanitizeParameters();
      m_numVoxelsHighWaterMark = 0;
      m_numChunksHighWaterMark = 0;

      if (!m_isMeshLocked)
        m_needsCompute = true;
    }

    internal void OnBrushDisabled(MudBrushBase brush)
    {
      RemoveBrush(brush);
    }

    public void MarkNeedsCompute() { m_needsCompute = true; }
    private bool m_needsCompute = false;
    private bool UpdateNeedsCompute()
    {
      if (MeshLocked)
      {
        bool needsCompute = false;
        if (MeshGenerationRenderableMeshMode == RenderableMeshMode.Procedural)
            needsCompute = m_needsCompute;

        m_needsCompute = false;
        return needsCompute;
      }

      if (m_needsCompute)
        return true;

      Profiler.BeginSample("UpdateNeedsCompute");

      // check dirty brushes
      if (!m_needsCompute)
      {
        foreach (var b in m_aBrush)
        {
          if (!b.m_dirty && !b.transform.hasChanged)
            continue;

          m_needsCompute = true;
          break;
        }
      }

      // check animation
      if (!m_needsCompute)
      {
        var animation = GetComponent<Animation>();
        m_needsCompute = (animation != null && animation.isPlaying);
      }

      // check animator
      if (!m_needsCompute)
      {
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
          var animState = animator.GetCurrentAnimatorStateInfo(0);
          m_needsCompute = (animState.length > 0.0f && animState.speed > 0.0f && animState.speedMultiplier > 0.0f);
        }
      }

      Profiler.EndSample();

      return m_needsCompute;
    }

    protected virtual bool IsEditorBusy() { return false; }

    private int m_numVoxelsHighWaterMark = 0;
    private int m_numChunksHighWaterMark = 0;
    private void TryAutoAdjustBudgets()
    {

      if (!Application.isEditor)
        return;

      if (!ShowGpuMemoryUsage)
      {
        m_autoAdjustBudgetsToHighWaterMarks = false;
        return;
      }

      if (!AutoAdjustBudgetsToHighWaterMarks)
      {
        m_autoAdjustBudgetsToHighWaterMarks = false;
        return;
      }

      Profiler.BeginSample("TryAutoAdjustBudgets");

      if (AutoAdjustBudgetsToHighWaterMarks && !m_autoAdjustBudgetsToHighWaterMarks)
      {
        m_numVoxelsHighWaterMark = 0;
        m_numChunksHighWaterMark = 0;
      }
      m_autoAdjustBudgetsToHighWaterMarks = AutoAdjustBudgetsToHighWaterMarks;

      m_numVoxelsHighWaterMark = Mathf.Max(NumVoxelsUsed, m_numVoxelsHighWaterMark);
      m_numChunksHighWaterMark = Mathf.Max(NumChunksUsed, m_numChunksHighWaterMark);

      int effectiveNumVoxelsHighWaterMark = m_numVoxelsHighWaterMark;
      if (ShouldDoAutoSmoothing && EnableSmoothCorner)
      {
        effectiveNumVoxelsHighWaterMark = Mathf.Max(effectiveNumVoxelsHighWaterMark, NumVerticesGenerated / VoxelToVertexFactor);
      }

      MaxVoxelsK = Mathf.Max(1, (int) Mathf.Ceil(effectiveNumVoxelsHighWaterMark * (1.0f + AutoAdjustBudgetsToHighWaterMarksMarginPercent / 100.0f) / 1024.0f));
      MaxChunks = Mathf.Max(16, (int) Mathf.Ceil(m_numChunksHighWaterMark * (1.0f + AutoAdjustBudgetsToHighWaterMarksMarginPercent / 100.0f)));

      Profiler.EndSample();
    }

    protected void LateUpdate()
    {
      if (m_isMeshLocked)
      {
        switch (MeshGenerationRenderableMeshMode)
        {
          case RenderableMeshMode.Procedural:
            // keep on rendering in procedural mode
            break;

          default:
            return;
        }
      }

      TryCompute();
      Render();
    }

    public void ForceCompute()
    {
      TryCompute(true);
    }

    public virtual void ReloadShaders()
    {
      DisposeGlobalResources();
      DisposeLocalResources();
      MarkNeedsCompute();
    }

    private void TryCompute(bool forceCompute = false)
    {
      if (IsEditorBusy())
        return;

      TryAutoAdjustBudgets();

      SanitizeParameters();
      if (!ValidateResources())
        return;

      if (!s_globalResourcesValid || !m_localResourcesValid)
        return;

      if (forceCompute || UpdateNeedsCompute())
      {
        Compute();
        m_needsCompute = false;
      }

      if (!MeshLocked)
      {
        Profiler.BeginSample("Clear Transform Change Flags");

        foreach (var b in m_aBrush)
        {
          b.m_dirty = false;
          b.transform.hasChanged = false;
        }

        Profiler.EndSample();
      }
    }

    private void SanitizeParameters()
    {
      Validate.Range(-1, VoxelNodeDepth, ref DrawVoxelNodesDepth);
    }

    #if MUDBUN_FREE
    private static readonly int s_watermarkWidth = 300;
    private static readonly int s_watermariHeight = 100;
    private static readonly string s_watermarkStr = "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";
    private static Texture2D s_watermark;
    private void OnGUI()
    {
      switch (Event.current.type)
      {
        case EventType.Repaint:
          //if (MudBun.IsFreeVersion)
          {
            if (s_watermark == null)
            {
              s_watermark = new Texture2D(s_watermarkWidth, s_watermariHeight);
              s_watermark.LoadImage(Convert.FromBase64String(s_watermarkStr));
            }

            // just let one renderer draw
            var itRenderer = s_renderers.GetEnumerator();
            if (!itRenderer.MoveNext() || itRenderer.Current != this)
              return;

            Graphics.DrawTexture(new Rect(0.0f, Screen.height - s_watermark.height, s_watermark.width, s_watermark.height), s_watermark);
          }
          break;
      }
    }
    #endif

    #endregion // end: Callbacks

    //-------------------------------------------------------------------------

    #region Resources

    private bool ValidateResources()
    {
      Profiler.BeginSample("ValidateResources");
      
      if (m_needRescanBrushes)
      {
        RescanBrushersImmediate();
        m_needRescanBrushes = false;
      }

      if (!ValidateGlobalResources())
        return false;

      if (!ValidateLocalResources())
        return false;

      /*
      foreach (var b in m_aBrush)
        b.ValidateMaterial();
      */

      Profiler.EndSample();

      return true;
    }

    private void TryDisposeResources()
    {
      if (s_renderers.Count == 0)
        DisposeGlobalResources();

      DisposeLocalResources();
    }

    private static bool s_shaderConstantIdPopulated = false;
    public static void ValidateShaderConstantId()
    {
      if (s_shaderConstantIdPopulated)
        return;

      Const.TriTable = Shader.PropertyToID("triTable");
      Const.VertTable = Shader.PropertyToID("vertTable");
      Const.TriTable2d = Shader.PropertyToID("triTable2d");

      Const.Brushes = Shader.PropertyToID("aBrush");
      Const.BrushMaterials = Shader.PropertyToID("aBrushMaterial");
      Const.NumBrushes = Shader.PropertyToID("numBrushes");

      Const.SurfaceShift = Shader.PropertyToID("surfaceShift");

      Const.MeshingMode = Shader.PropertyToID("meshingMode");
      Const.RenderMode = Shader.PropertyToID("renderMode");
      Const.MeshingMode = Shader.PropertyToID("meshingMode");

      Const.NormalDifferentiationStep = Shader.PropertyToID("normalDifferentiationStep");
      Const.NormalQuantization = Shader.PropertyToID("normalQuantization");
      Const.Normal2dFadeDist = Shader.PropertyToID("normal2dFadeDist");
      Const.Normal2dStrength = Shader.PropertyToID("normal2dStrength");
      Const.EnableAutoSmooth = Shader.PropertyToID("enableAutoSmooth");
      Const.AutoSmoothMaxAngle = Shader.PropertyToID("autoSmoothMaxAngle");
      Const.AutoSmoothVertDataTable = Shader.PropertyToID("autoSmoothVertDataTable");
      Const.AutoSmoothVertDataPoolSize = Shader.PropertyToID("autoSmoothVertDataPoolSize");
      Const.EnableSmoothCorner = Shader.PropertyToID("enableSmoothCorner");
      Const.SmoothCornerSubdivision = Shader.PropertyToID("smoothCornerSubdivision");
      Const.SmoothCornerNormalBlur = Shader.PropertyToID("smoothCornerNormalBlur");
      Const.SmoothCornerFade = Shader.PropertyToID("smoothCornerFade");

      Const.SplatSize = Shader.PropertyToID("splatSize");
      Const.SplatSizeJitter = Shader.PropertyToID("splatSizeJitter");
      Const.SplatNormalShift = Shader.PropertyToID("splatNormalShift");
      Const.SplatNormalShiftJitter = Shader.PropertyToID("splatNormalShiftJitter");
      Const.SplatPositionJitter = Shader.PropertyToID("splatPositionJitter");
      Const.SplatColorJitter = Shader.PropertyToID("splatColorJitter");
      Const.SplatRotationJitter = Shader.PropertyToID("splatRotationJitter");
      Const.SplatOrientationJitter = Shader.PropertyToID("splatOrientationJitter");
      Const.SplatJitterNoisiness = Shader.PropertyToID("splatJitterNoisiness");
      Const.SplatCameraFacing = Shader.PropertyToID("splatCameraFacing");
      Const.SplatScreenSpaceFlattening = Shader.PropertyToID("splatScreenSpaceFlattening");

      Const.SurfaceNetsDualQuadsBlend = Shader.PropertyToID("surfaceNetsDualQuadsBlend");
      Const.SurfaceNetsBinarySearchIterations = Shader.PropertyToID("surfaceNetsBinarySearchIterations");
      Const.SurfaceNetsGradientDescentIterations = Shader.PropertyToID("surfaceNetsGradientDescentIterations");
      Const.SurfaceNetsGradientDescentFactor = Shader.PropertyToID("surfaceNetsGradientDescentFactor");

      Const.DualContouringDualQuadsBlend = Shader.PropertyToID("dualContouringDualQuadsBlend");
      Const.DualContouringRelaxation = Shader.PropertyToID("dualContouringRelaxation");
      Const.DualContouringBinarySearchIterations = Shader.PropertyToID("dualContouringBinarySearchIterations");
      Const.DualContouringGradientDescentIterations = Shader.PropertyToID("dualContouringGradientDescentIterations");
      Const.DualContouringGradientDescentFactor = Shader.PropertyToID("dualContouringGradientDescentFactor");

      Const.AabbTree = Shader.PropertyToID("aabbTree");
      Const.AabbRoot = Shader.PropertyToID("aabbRoot");

      Const.Enable2dMode = Shader.PropertyToID("enable2dMode");
      Const.ForceAllBrushes = Shader.PropertyToID("forceAllBrushes");
      Const.NumAllocations = Shader.PropertyToID("aNumAllocation");
      Const.NodeHashTable = Shader.PropertyToID("nodeHashTable");
      Const.NodeHashTableSize = Shader.PropertyToID("nodeHashTableSize");
      Const.NodePool = Shader.PropertyToID("nodePool");
      Const.NodePoolSize = Shader.PropertyToID("nodePoolSize");
      Const.NumNodesAllocated = Shader.PropertyToID("aNumNodesAllocated");
      Const.UseVoxelCache = Shader.PropertyToID("useVoxelCache");
      Const.VoxelCacheIdTable = Shader.PropertyToID("voxelCacheIdTable");
      Const.VoxelCache = Shader.PropertyToID("voxelCache");
      Const.VoxelCacheSize = Shader.PropertyToID("voxelCacheSize");
      Const.BrushMaskPool = Shader.PropertyToID("brushMaskPool");
      Const.BrushMaskPoolSize = Shader.PropertyToID("brushMaskPoolSize");
      Const.IndirectDispatchArgs = Shader.PropertyToID("indirectDispatchArgs");
      Const.CurrentNodeDepth = Shader.PropertyToID("currentNodeDepth");
      Const.CurrentNodeBranchingFactor = Shader.PropertyToID("currentNodeBranchingFactor");
      Const.CurrentNodeSize = Shader.PropertyToID("currentNodeSize");
      Const.VoxelSize = Shader.PropertyToID("voxelSize");
      Const.MaxNodeDepth = Shader.PropertyToID("maxNodeDepth");
      Const.ChunkVoxelDensity = Shader.PropertyToID("chunkVoxelDensity");
      Const.GenPoints = Shader.PropertyToID("aGenPoint");
      Const.MaxGenPoints = Shader.PropertyToID("maxGenPoints");
      Const.IndirectDrawArgs = Shader.PropertyToID("indirectDrawArgs");

      Const.MasterColor = Shader.PropertyToID("_Color");
      Const.MasterEmission = Shader.PropertyToID("_Emission");
      Const.MasterMetallic = Shader.PropertyToID("_Metallic");
      Const.MasterSmoothness = Shader.PropertyToID("_Smoothness");

      Const.LocalToWorld = Shader.PropertyToID("localToWorld");
      Const.LocalToWorldIt = Shader.PropertyToID("localToWorldIt");
      Const.LocalToWorldScale = Shader.PropertyToID("localToWorldScale");
      Const.WorldToLocal = Shader.PropertyToID("worldToLocal");

      Const.NoiseCache = Shader.PropertyToID("noiseCache");
      Const.NoiseCacheDimension = Shader.PropertyToID("noiseCacheDimension");
      Const.NoiseCacheDensity = Shader.PropertyToID("noiseCacheDensity");
      Const.NoiseCachePeriod = Shader.PropertyToID("noiseCachePeriod");

      s_shaderConstantIdPopulated = true;
    }

    private void RegisterCommonMeshingConstants(ComputeShader shader)
    {
      ComputeManager.RegisterConstantId(shader, Const.RenderMode);
      ComputeManager.RegisterConstantId(shader, Const.MeshingMode);

      ComputeManager.RegisterConstantId(shader, Const.NormalDifferentiationStep);
      ComputeManager.RegisterConstantId(shader, Const.NormalQuantization);
      ComputeManager.RegisterConstantId(shader, Const.Normal2dFadeDist);
      ComputeManager.RegisterConstantId(shader, Const.Normal2dStrength);
      ComputeManager.RegisterConstantId(shader, Const.EnableAutoSmooth);
      ComputeManager.RegisterConstantId(shader, Const.AutoSmoothMaxAngle);
      ComputeManager.RegisterConstantId(shader, Const.AutoSmoothVertDataTable);
      ComputeManager.RegisterConstantId(shader, Const.AutoSmoothVertDataPoolSize);
      ComputeManager.RegisterConstantId(shader, Const.EnableSmoothCorner);
      ComputeManager.RegisterConstantId(shader, Const.SmoothCornerSubdivision);
      ComputeManager.RegisterConstantId(shader, Const.SmoothCornerNormalBlur);
      ComputeManager.RegisterConstantId(shader, Const.SmoothCornerFade);

      ComputeManager.RegisterConstantId(shader, Const.NodeHashTableSize);
      ComputeManager.RegisterConstantId(shader, Const.NodePoolSize);
      ComputeManager.RegisterConstantId(shader, Const.UseVoxelCache);
      ComputeManager.RegisterConstantId(shader, Const.VoxelCacheSize);

      ComputeManager.RegisterConstantId(shader, Const.BrushMaskPoolSize);
      ComputeManager.RegisterConstantId(shader, Const.Brushes);
      ComputeManager.RegisterConstantId(shader, Const.NumBrushes);
      ComputeManager.RegisterConstantId(shader, Const.BrushMaterials);

      ComputeManager.RegisterConstantId(shader, Const.SurfaceShift);

      ComputeManager.RegisterConstantId(shader, Const.AabbTree);
      ComputeManager.RegisterConstantId(shader, Const.AabbRoot);

      ComputeManager.RegisterConstantId(shader, Const.Enable2dMode);
      ComputeManager.RegisterConstantId(shader, Const.ForceAllBrushes);
      ComputeManager.RegisterConstantId(shader, Const.NodeHashTable);
      ComputeManager.RegisterConstantId(shader, Const.NodePool);
      ComputeManager.RegisterConstantId(shader, Const.NumNodesAllocated);
      ComputeManager.RegisterConstantId(shader, Const.NumAllocations);
      ComputeManager.RegisterConstantId(shader, Const.VoxelCacheIdTable);
      ComputeManager.RegisterConstantId(shader, Const.VoxelCache);
      ComputeManager.RegisterConstantId(shader, Const.BrushMaskPool);

      ComputeManager.RegisterConstantId(shader, Const.IndirectDispatchArgs);
      ComputeManager.RegisterConstantId(shader, Const.GenPoints);
      ComputeManager.RegisterConstantId(shader, Const.IndirectDrawArgs);
      ComputeManager.RegisterConstantId(shader, Const.VoxelSize);
      ComputeManager.RegisterConstantId(shader, Const.MaxNodeDepth);
      ComputeManager.RegisterConstantId(shader, Const.ChunkVoxelDensity);

      ComputeManager.RegisterConstantId(shader, Const.NoiseCache);
      ComputeManager.RegisterConstantId(shader, Const.NoiseCacheDimension);
      ComputeManager.RegisterConstantId(shader, Const.NoiseCacheDensity);

      ComputeManager.RegisterConstantId(shader, Const.CurrentNodeDepth);
      ComputeManager.RegisterConstantId(shader, Const.CurrentNodeBranchingFactor);
      ComputeManager.RegisterConstantId(shader, Const.CurrentNodeSize);
    }

    private bool ValidateGlobalResources()
    {
      Profiler.BeginSample("ValidateGlobalResources");

      if (s_computeVoxelGen == null 
          || s_computeMarchingCubes == null 
          || s_computeDualMeshing == null 
          || s_computeNoiseCache == null 
          || s_computeMeshLock == null)
      {
        ComputeManager.Reset();

        ValidateShaderConstantId();

        // voxel gen
        {
          s_computeVoxelGen = ResourcesUtil.VoxelGen;
          if (s_computeVoxelGen == null)
            return false;

          ComputeManager.RegisterShader(s_computeVoxelGen);

          Const.Kernel.ClearVoxelHashTable = 
            ComputeManager.RegisterKernel(s_computeVoxelGen, "clear_voxel_hash_table");
          Const.Kernel.ClearAutoSmoothVertDataTable = 
            ComputeManager.RegisterKernel(s_computeVoxelGen, "clear_auto_smooth_vert_data_table");
          Const.Kernel.ClearVoxelCache = 
            ComputeManager.RegisterKernel(s_computeVoxelGen, "clear_voxel_cache");
          Const.Kernel.RegisterTopNodes = 
            ComputeManager.RegisterKernel(s_computeVoxelGen, "register_top_nodes");
          Const.Kernel.UpdateBranchingIndirectDispatchArgs = 
            ComputeManager.RegisterKernel(s_computeVoxelGen, "update_branching_indirect_dispatch_args");
          Const.Kernel.AllocateChildNodes = 
            ComputeManager.RegisterKernel(s_computeVoxelGen, "allocate_child_nodes");
          Const.Kernel.UpdateVoxelIndirectDispatchArgs = 
            ComputeManager.RegisterKernel(s_computeVoxelGen, "update_voxel_indirect_dispatch_args");

          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NumBrushes);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NodeHashTableSize);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NodePoolSize);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.BrushMaskPoolSize);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.Brushes);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.SurfaceShift);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.AabbTree);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.AabbRoot);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.Enable2dMode);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.ForceAllBrushes);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NodeHashTable);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NodePool);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NumNodesAllocated);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NumAllocations);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.BrushMaskPool);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.IndirectDispatchArgs);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.GenPoints);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.IndirectDrawArgs);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.VoxelSize);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.MaxNodeDepth);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.ChunkVoxelDensity);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NoiseCache);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NoiseCacheDimension);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NoiseCacheDensity);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.CurrentNodeDepth);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.CurrentNodeBranchingFactor);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.CurrentNodeSize);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.NormalDifferentiationStep);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.EnableAutoSmooth);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.AutoSmoothMaxAngle);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.AutoSmoothVertDataTable);
          ComputeManager.RegisterConstantId(s_computeVoxelGen, Const.AutoSmoothVertDataPoolSize);
        } // end: voxel gen

        // marching cubes
        {
          s_computeMarchingCubes = ResourcesUtil.MarchingCubes;
          if (s_computeMarchingCubes == null)
            return false;

          ComputeManager.RegisterShader(s_computeMarchingCubes);

          Const.Kernel.GenerateFlatMarchingCubesMesh = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "generate_flat_marching_cubes_mesh");
          Const.Kernel.GenerateSmoothMarchingCubesMesh = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "generate_smooth_marching_cubes_mesh");
          Const.Kernel.GenerateMarchingCubesSplats = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "generate_marching_splats");
          Const.Kernel.GenerateFlatMarchingCubesMesh2d = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "generate_flat_marching_cubes_mesh_2d");
          Const.Kernel.GenerateSmoothMarchingCubesMesh2d = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "generate_smooth_marching_cubes_mesh_2d");
          Const.Kernel.GenerateMarchingCubesSplats2d = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "generate_marching_splats_2d");
          Const.Kernel.UpdateMarchingCubesAutoSmoothIndirectDispatchArgs = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "update_marching_cubes_auto_smooth_indirect_dispatch_args");
          Const.Kernel.MarchingCubesUpdateAutoSmooth = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "marching_cubes_update_auto_smooth");
          Const.Kernel.MarchingCubesComputeAutoSmooth = 
            ComputeManager.RegisterKernel(s_computeMarchingCubes, "marching_cubes_compute_auto_smooth");

          RegisterCommonMeshingConstants(s_computeMarchingCubes);
          ComputeManager.RegisterConstantId(s_computeMarchingCubes, Const.TriTable);
          ComputeManager.RegisterConstantId(s_computeMarchingCubes, Const.VertTable);
          ComputeManager.RegisterConstantId(s_computeMarchingCubes, Const.TriTable2d);
        }

        // dual meshing
        {
          s_computeDualMeshing = ResourcesUtil.DualMeshing;
          if (s_computeDualMeshing == null)
            return false;

          ComputeManager.RegisterShader(s_computeDualMeshing);

          Const.Kernel.GenerateDualQuads = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "generate_dual_quads");
          Const.Kernel.GenerateDualQuads2d = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "generate_dual_quads_2d");
          Const.Kernel.UpdateDualMeshingIndirectDispatchArgs = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "update_dual_meshing_indirect_dispatch_args");
          Const.Kernel.DualMeshingFlatMeshNormal = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_flat_mesh_normal");
          Const.Kernel.DualMeshingSmoothMeshNormal = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_smooth_mesh_normal");
          Const.Kernel.DualMeshingFlatMeshNormal2d = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_flat_mesh_normal_2d");
          Const.Kernel.DualMeshingSmoothMeshNormal2d = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_smooth_mesh_normal_2d");
          Const.Kernel.DualMeshingUpdateAutoSmooth = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_update_auto_smooth");
          Const.Kernel.DualMeshingComputeAutoSmooth = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_compute_auto_smooth");
          Const.Kernel.DualMeshingUpdateSmoothCornerIndirectDispatchArgs = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_udpate_auto_smooth_SmoothCorner_indirect_dispatch_args");
          Const.Kernel.DualMeshingSmoothCorner = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "dual_meshing_auto_smooth_smooth_corner");
          Const.Kernel.UpdateDualMeshingSplatsIndirectArgs = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "update_dual_meshing_splats_indirect_args");
          Const.Kernel.ConvertDualMeshingSplats = 
            ComputeManager.RegisterKernel(s_computeDualMeshing, "convert_dual_meshing_splats");

          RegisterCommonMeshingConstants(s_computeDualMeshing);
        }

        // surface nets
        {
          s_computeSurfaceNets = ResourcesUtil.SurfaceNets;
          if (s_computeSurfaceNets == null)
            return false;

          ComputeManager.RegisterShader(s_computeSurfaceNets);

          Const.Kernel.SurfaceNetsMovePoint = 
            ComputeManager.RegisterKernel(s_computeSurfaceNets, "surface_nets_move_point");

          Const.Kernel.SurfaceNetsMovePoint2d = 
            ComputeManager.RegisterKernel(s_computeSurfaceNets, "surface_nets_move_point_2d");

          RegisterCommonMeshingConstants(s_computeSurfaceNets);
          ComputeManager.RegisterConstantId(s_computeSurfaceNets, Const.SurfaceNetsDualQuadsBlend);
          ComputeManager.RegisterConstantId(s_computeSurfaceNets, Const.SurfaceNetsBinarySearchIterations);
          ComputeManager.RegisterConstantId(s_computeSurfaceNets, Const.SurfaceNetsGradientDescentIterations);
          ComputeManager.RegisterConstantId(s_computeSurfaceNets, Const.SurfaceNetsGradientDescentFactor);
        }

        // dual contouring
        {
          s_computeDualContouring = ResourcesUtil.DualContouring;
          if (s_computeDualContouring == null)
            return false;

          ComputeManager.RegisterShader(s_computeDualContouring);

          Const.Kernel.DualContouringMovePoint = 
            ComputeManager.RegisterKernel(s_computeDualContouring, "dual_contouring_move_point");
          Const.Kernel.DualContouringMovePoint2d = 
            ComputeManager.RegisterKernel(s_computeDualContouring, "dual_contouring_move_point_2d");

          RegisterCommonMeshingConstants(s_computeDualContouring);
          ComputeManager.RegisterConstantId(s_computeDualContouring, Const.DualContouringDualQuadsBlend);
          ComputeManager.RegisterConstantId(s_computeDualContouring, Const.DualContouringRelaxation);
          ComputeManager.RegisterConstantId(s_computeDualContouring, Const.DualContouringBinarySearchIterations);
          ComputeManager.RegisterConstantId(s_computeDualContouring, Const.DualContouringGradientDescentIterations);
          ComputeManager.RegisterConstantId(s_computeDualContouring, Const.DualContouringGradientDescentFactor);
        }

        // noise cache
        {
          s_computeNoiseCache = ResourcesUtil.NoiseCache;
          if (s_computeNoiseCache == null)
            return false;

          ComputeManager.RegisterShader(s_computeNoiseCache);

          Const.Kernel.GenerateNoiseCache = 
            ComputeManager.RegisterKernel(s_computeNoiseCache, "generate_noise_cache");

          ComputeManager.RegisterConstantId(s_computeNoiseCache, Const.NoiseCache);
          ComputeManager.RegisterConstantId(s_computeNoiseCache, Const.NoiseCacheDimension);
          ComputeManager.RegisterConstantId(s_computeNoiseCache, Const.NoiseCacheDensity);
          ComputeManager.RegisterConstantId(s_computeNoiseCache, Const.NoiseCachePeriod);
        } // end: noise cache

        // mesh lock
        {
          s_computeMeshLock = ResourcesUtil.MeshLock;
          if (s_computeMeshLock == null)
            return false;
            
          ComputeManager.RegisterShader(s_computeMeshLock);

          Const.Kernel.RigBones = 
            ComputeManager.RegisterKernel(s_computeMeshLock, "rig_bones");

          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NumBrushes);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.BrushMaskPoolSize);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.Brushes);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.BrushMaterials);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.AabbTree);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.AabbRoot);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NodeHashTable);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NodePool);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NumNodesAllocated);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NumAllocations);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.VoxelCacheIdTable);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.VoxelCache);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.BrushMaskPool);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.IndirectDispatchArgs);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.GenPoints);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.MaxGenPoints);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.IndirectDrawArgs);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.VoxelSize);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.MaxNodeDepth);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.ChunkVoxelDensity);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NoiseCache);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NoiseCacheDimension);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.NoiseCacheDensity);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.CurrentNodeDepth);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.CurrentNodeBranchingFactor);
          ComputeManager.RegisterConstantId(s_computeMeshLock, Const.CurrentNodeSize);
        } // end: mesh lock

        ComputeManager.ActivateAllShaders();

        // generate noise cache
        {
          if (s_noiseCache != null)
          {
            s_noiseCache.Release();
            s_noiseCache = null;
          }
          s_noiseCache = new RenderTexture(NoiseCacheDimensionInts[0], NoiseCacheDimensionInts[1], 0, RenderTextureFormat.RFloat);
          s_noiseCache.dimension = TextureDimension.Tex3D;
          s_noiseCache.volumeDepth = NoiseCacheDimensionInts[2];
          s_noiseCache.enableRandomWrite = true;
          s_noiseCache.Create();
          ComputeManager.SetTexture(Const.NoiseCache, s_noiseCache);
          ComputeManager.SetInts(Const.NoiseCacheDimension, NoiseCacheDimensionInts);
          ComputeManager.SetFloat(Const.NoiseCacheDensity, NoiseCacheDensity);
          ComputeManager.SetFloats(Const.NoiseCachePeriod, NoiseCachePeriod);
          ComputeManager.Dispatch
          (
            s_computeNoiseCache, 
            Const.Kernel.GenerateNoiseCache, 
            (s_noiseCache.width  + ThreadGroupExtent - 1) / ThreadGroupExtent, 
            (s_noiseCache.height + ThreadGroupExtent - 1) / ThreadGroupExtent, 
            (s_noiseCache.volumeDepth + ThreadGroupExtent - 1) / ThreadGroupExtent
          );
          ComputeManager.Deactivate(s_computeNoiseCache);
        } // end: generate noise cache
      }

      if (s_triTableBuffer == null)
      {
        s_triTableBuffer = new ComputeBuffer(16 * 256, sizeof(int));
        s_triTableBuffer.SetData(MarchingCubes.TriTable);
        ComputeManager.SetBuffer(Const.TriTable, s_triTableBuffer);
        m_needsCompute = true;
      }

      if (s_vertTableBuffer == null)
      {
        s_vertTableBuffer = new ComputeBuffer(2 * 12, sizeof(int));
        s_vertTableBuffer.SetData(MarchingCubes.VertTable);
        ComputeManager.SetBuffer(Const.VertTable, s_vertTableBuffer);
        m_needsCompute = true;
      }

      if (s_triTable2dBuffer == null)
      {
        s_triTable2dBuffer = new ComputeBuffer(12 * 16, sizeof(int));
        s_triTable2dBuffer.SetData(MarchingCubes.TriTable2d);
        ComputeManager.SetBuffer(Const.TriTable2d, s_triTable2dBuffer);
        m_needsCompute = true;
      }

      if (AllowSharedRWBuffers)
      {
        if (s_brushesBuffer == null)
        {
          s_brushesBuffer = new ComputeBuffer(MaxBrushes, SdfBrush.Stride);
          m_needsCompute = true;
        }

        if (s_brushMaterialBuffer == null)
        {
          s_brushMaterialBuffer = new ComputeBuffer(MaxBrushes, SdfBrushMaterial.Stride);
          m_needsCompute = true;
        }
      }

      if (s_dummyBuffer == null)
      {
        s_dummyBuffer = new ComputeBuffer(4, sizeof(int));
      }

      Profiler.EndSample();

      s_globalResourcesValid = true;

      return true;
    }

    private static RenderPipelineEnum DetermineRenderPipeline()
    {
      if (s_renderPipeline >= 0)
        return s_renderPipeline;

      if (s_renderPipeline < 0)
      {
        string rpAsset = "";
        if (GraphicsSettings.renderPipelineAsset != null)
          rpAsset = GraphicsSettings.renderPipelineAsset.GetType().Name;

        if (rpAsset.Equals("HDRenderPipelineAsset"))
          s_renderPipeline = RenderPipelineEnum.HDRP;
        else if (rpAsset.Equals("UniversalRenderPipelineAsset"))
          s_renderPipeline = RenderPipelineEnum.URP;
        else
          s_renderPipeline = RenderPipelineEnum.BuiltIn;
      }

      return s_renderPipeline;
    }

    protected virtual bool ValidateLocalResources()
    {
      Profiler.BeginSample("ValidateLocalResources");

      if (AllowSharedRWBuffers)
      {
        m_brushesBuffer = s_brushesBuffer;
        m_brushMaterialBuffer = s_brushMaterialBuffer;
        m_aabbTreeBuffer = s_aabbTreeBuffer;
      }
      else
      {
        if (m_brushesBuffer == null)
        {
          m_brushesBuffer = new ComputeBuffer(MaxBrushes, SdfBrush.Stride);
          m_needsCompute = true;
        }

        if (m_brushMaterialBuffer == null)
        {
          m_brushMaterialBuffer = new ComputeBuffer(MaxBrushes, SdfBrushMaterial.Stride);
          m_needsCompute = true;
        }
      }

      if (m_nodeHashTableBuffer == null 
          || m_nodeHashTableBuffer.count != NodeHashTableSize)
      {
        if (m_nodeHashTableBuffer != null)
          m_nodeHashTableBuffer.Release();

        m_nodeHashTableBuffer = new ComputeBuffer(NodeHashTableSize, VoxelHashEntry.Stride);
        m_needsCompute = true;
      }

      if (m_nodePoolBuffer == null 
          || m_nodePoolBuffer.count != MaxVoxels)
      {
        if (m_nodePoolBuffer != null)
          m_nodePoolBuffer.Release();

        m_nodePoolBuffer = new ComputeBuffer(MaxVoxels, VoxelNode.Stride);
        m_needsCompute = true;
      }

      int voxelCacheSize = MaxVoxels * 2;
      if (UseVoxelCache)
      {
        if (m_voxelCacheIdTableBuffer == null 
            || m_voxelCacheIdTableBuffer.count != voxelCacheSize)
        {
          if (m_voxelCacheIdTableBuffer != null)
          {
            m_voxelCacheIdTableBuffer.Release();
            m_voxelCacheBuffer.Release();
          }

          m_voxelCacheIdTableBuffer = new ComputeBuffer(voxelCacheSize, sizeof(int));
          m_voxelCacheBuffer = new ComputeBuffer(voxelCacheSize, 4 * sizeof(float));
          m_needsCompute = true;
        }
      }
      else if (m_voxelCacheBuffer != null)
      {
        m_voxelCacheIdTableBuffer.Release();
        m_voxelCacheBuffer.Release();

        m_voxelCacheIdTableBuffer = null;
        m_voxelCacheBuffer = null;
      }

      if (m_brushMaskPoolBuffer == null 
          || m_brushMaskPoolBuffer.count != MaxBrushMasks * MaxBrushMaskInts)
      {
        if (m_brushMaskPoolBuffer != null)
          m_brushMaskPoolBuffer.Release();

        m_brushMaskPoolBuffer = new ComputeBuffer(MaxBrushMasks * MaxBrushMaskInts, sizeof(int));
        m_needsCompute = true;
      }

      int maxGenPoints = MaxGenPoints;
      if (m_genPointsBufferDefault == null 
          || m_genPointsBufferDefault.count != maxGenPoints)
      {
        if (m_genPointsBufferDefault != null)
        {
          m_numNodesAllocatedBuffer.Release();
          m_numAllocationsBuffer.Release();
          m_indirectDispatchArgsBuffer.Release();
          m_genPointsBufferDefault.Release();
          m_indirectDrawArgsBufferDefault.Release();
        }

        m_numNodesAllocatedBuffer = new ComputeBuffer(VoxelNodeDepth + 2, sizeof(int));
        m_numAllocationsBuffer = new ComputeBuffer(4, sizeof(int));
        m_indirectDispatchArgsBuffer = new ComputeBuffer(3, sizeof(int), ComputeBufferType.IndirectArguments);
        m_genPointsBufferDefault = new ComputeBuffer(maxGenPoints, GenPoint.Stride);
        m_indirectDrawArgsBufferDefault = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        m_needsCompute = true;
      }

      if (ShouldDoAutoSmoothing)
      {
        if (m_autoSmoothVertDataTableBuffer != null 
            && m_autoSmoothVertDataTableBuffer.count != AutoSmoothVertDataTableSize)
        {
          m_autoSmoothVertDataTableBuffer.Release();
          m_autoSmoothVertDataTableBuffer = null;
        }

        if (m_autoSmoothVertDataTableBuffer == null)
        {
          m_autoSmoothVertDataTableBuffer = new ComputeBuffer(AutoSmoothVertDataTableSize, AutoSmoothVertData.Stride);
        }
      }
      else
      {
        if (m_autoSmoothVertDataTableBuffer != null)
        {
          m_autoSmoothVertDataTableBuffer.Release();
          m_autoSmoothVertDataTableBuffer = null;
        }
      }

      DetermineRenderPipeline();

      if (m_usedSharedMaterial != SharedMaterial)
      {
        m_usedSharedMaterial = SharedMaterial;
        m_needsCompute = true;
      }

      Profiler.EndSample();

      m_localResourcesValid = true;

      return true;
    }

    protected void DisposeGlobalResources()
    {
      s_globalResourcesValid = false;

      s_computeVoxelGen = null;

      if (s_triTableBuffer != null)
      {
        s_triTableBuffer.Release();
        s_triTableBuffer = null;
      }

      if (s_vertTableBuffer != null)
      {
        s_vertTableBuffer.Release();
        s_vertTableBuffer = null;
      }

      if (s_triTable2dBuffer != null)
      {
        s_triTable2dBuffer.Release();
        s_triTable2dBuffer = null;
      }

      if (s_brushesBuffer != null)
      {
        s_brushesBuffer.Release();
        s_brushesBuffer = null;
      }

      if (s_brushMaterialBuffer != null)
      {
        s_brushMaterialBuffer.Release();
        s_brushMaterialBuffer = null;
      }

      if (s_aabbTreeBuffer != null)
      {
        s_aabbTreeBuffer.Release();
        s_aabbTreeBuffer = null;
      }

      if (s_dummyBuffer != null)
      {
        s_dummyBuffer.Release();
        s_dummyBuffer = null;
      }

      if (s_indirectDrawArgsInitData.Length > 0)
        s_indirectDrawArgsInitData.Dispose();

      if (s_unitIndirectDispatchArgsInitData.Length > 0)
        s_unitIndirectDispatchArgsInitData.Dispose();
    }

    protected void DisposeLocalResources()
    {
      m_localResourcesValid = false;

      if (!AllowSharedRWBuffers)
      {
        if (m_brushesBuffer != null)
        {
          m_brushesBuffer.Release();
          m_brushesBuffer = null;
        }

        if (m_brushMaterialBuffer != null)
        {
          m_brushMaterialBuffer.Release();
          m_brushMaterialBuffer = null;
        }

        if (m_aabbTreeBuffer != null)
        {
          m_aabbTreeBuffer.Release();
          m_aabbTreeBuffer = null;
        }
      }

      if (m_nodeHashTableBuffer != null)
      {
        m_nodeHashTableBuffer.Release();
        m_nodeHashTableBuffer = null;
      }

      if (m_nodePoolBuffer != null)
      {
        m_nodePoolBuffer.Release();
        m_nodePoolBuffer = null;
      }

      if (m_voxelCacheIdTableBuffer != null)
      {
        m_voxelCacheIdTableBuffer.Release();
        m_voxelCacheBuffer.Release();

        m_voxelCacheIdTableBuffer = null;
        m_voxelCacheBuffer = null;
      }

      if (m_brushMaskPoolBuffer != null)
      {
        m_brushMaskPoolBuffer.Release();
        m_brushMaskPoolBuffer = null;
      }

      if (m_genPointsBufferDefault != null)
      {
        m_numNodesAllocatedBuffer.Release();
        m_numAllocationsBuffer.Release();
        m_indirectDispatchArgsBuffer.Release();
        m_genPointsBufferDefault.Release();
        m_indirectDrawArgsBufferDefault.Release();

        m_numNodesAllocatedBuffer = null;
        m_numAllocationsBuffer = null;
        m_indirectDispatchArgsBuffer = null;
        m_genPointsBufferDefault = null;
        m_indirectDrawArgsBufferDefault = null;
      }

      if (m_autoSmoothVertDataTableBuffer != null)
      {
        m_autoSmoothVertDataTableBuffer.Release();
        m_autoSmoothVertDataTableBuffer = null;
      }

      m_materialCloned = null;
      m_materialUsed = null;

      if (m_pendingMeshTable != null)
      {
        foreach (var pair in m_pendingMeshTable)
          pair.Value.Dispose();
        m_pendingMeshTable.Clear();
      }
    }

    #endregion // end: Resources

    //-------------------------------------------------------------------------

    #region Brushes

    public void AddBrush(MudBrushBase brush)
    {
      Assert.True(m_aBrush.Count < MaxBrushes, $"Maximum of {MaxBrushes} brushes per renderer reached.");

      if (brush.m_renderer != null)
        brush.m_renderer.RemoveBrush(brush);


      brush.m_renderer = this;

      ValidateAabbTree();
      brush.UpdateProxies(m_aabbTree, brush.Bounds);

      m_aBrush.Add(brush);
      m_aBrushToProcess.Add(brush);

      m_needsCompute = true;
    }

    public void RemoveBrush(MudBrushBase brush)
    {
      Assert.True(brush.m_renderer != null, "Brush was never added.");
      Assert.True(brush.m_renderer == this, "Brush was not added to this renderer.");

      ValidateAabbTree();
      brush.DestroyProxies(m_aabbTree);

      brush.m_renderer = null;

      m_aBrush.Remove(brush);
      m_aBrushToProcess.Remove(brush);
      if (brush.IsBrushGroup)
        m_aBrushToProcess.Remove(brush);

      m_needsCompute = true;
    }

    public void DestoryAllBrushes()
    {
      var aBrushCopy = Brushes.ToArray();
      foreach (var b in aBrushCopy)
      {
        var m = b.GetComponent<MudMaterialBase>();
        Destroy(b);
        Destroy(m);
      }
    }

    public void DestoryAllBrushesImmediate()
    {
      var aBrushCopy = Brushes.ToArray();
      foreach (var b in aBrushCopy)
      {
        var m = b.GetComponent<MudMaterialBase>();
        DestroyImmediate(b);
        DestroyImmediate(m);
      }
    }

    public void NotifyHierarchyChange()
    {
      RescanBrushes();
    }

    private void ClearBrushes()
    {
      ValidateAabbTree();
      foreach (var brush in m_aBrush)
      {
        brush.m_renderer = null;
        brush.DestroyProxies(m_aabbTree);
      }

      m_aBrush.Clear();
      m_aBrushToProcess.Clear();

      m_needsCompute = true;
    }

    public void RescanBrushes()
    {
      m_needRescanBrushes = true;
      m_needsCompute = true;
    }

    private int m_brushGroupDepth = 0;
    public void RescanBrushersImmediate()
    {
      ClearBrushes();
      ScanHierarchyRecursive(transform);
    }

    private void ScanHierarchyRecursive(Transform parent)
    {
      if (parent == null)
        return;

      var parentBrush = parent.GetComponent<MudBrushBase>();
      bool parentIsGroup = (parentBrush != null && parentBrush.IsBrushGroup && parentBrush.isActiveAndEnabled);
      if (parentIsGroup)
      {
        ++m_brushGroupDepth;
        if (m_brushGroupDepth >= MaxBrushGroupDepth)
          Assert.Warn($"MudBun: Exceeded maximum group depth of {MaxBrushGroupDepth}!");
      }

      for (int iChild = 0; iChild < parent.childCount; ++iChild)
      {
        var child = parent.GetChild(iChild);

        // renderer blocks recursion
        if (child.GetComponent<MudRendererBase>() != null)
          continue;

        var brush = child.GetComponent<MudBrushBase>();
        if (brush != null && brush.isActiveAndEnabled)
          AddBrush(brush);

        ScanHierarchyRecursive(child);
      }

      if (parentIsGroup)
      {
        m_aBrushToProcess.Add(parentBrush);
        --m_brushGroupDepth;
      }
    }

    private void SetUpResources()
    {
      Profiler.BeginSample("SetUpResources");

      WriteResources();
      BindComputeResources();

      Profiler.EndSample();
    }

    private void UpdateActivePreCompute()
    {
      if (m_doRigging)
        ComputeManager.Activate(s_computeMeshLock);
      else
        ComputeManager.Deactivate(s_computeMeshLock);

      switch (MeshingMode)
      {
        case MeshingModeEnum.MarchingCubes:
          ComputeManager.Activate(s_computeMarchingCubes);
          ComputeManager.Deactivate(s_computeDualMeshing);
          ComputeManager.Deactivate(s_computeSurfaceNets);
          ComputeManager.Deactivate(s_computeDualContouring);
          break;

        case MeshingModeEnum.DualQuads:
          ComputeManager.Activate(s_computeDualMeshing);
          ComputeManager.Deactivate(s_computeMarchingCubes);
          ComputeManager.Deactivate(s_computeSurfaceNets);
          ComputeManager.Deactivate(s_computeDualContouring);
          break;

        case MeshingModeEnum.SurfaceNets:
          ComputeManager.Activate(s_computeDualMeshing);
          ComputeManager.Activate(s_computeSurfaceNets);
          ComputeManager.Deactivate(s_computeMarchingCubes);
          ComputeManager.Deactivate(s_computeDualContouring);
          break;

        case MeshingModeEnum.DualContouring:
          ComputeManager.Activate(s_computeDualMeshing);
          ComputeManager.Activate(s_computeDualContouring);
          ComputeManager.Deactivate(s_computeMarchingCubes);
          ComputeManager.Deactivate(s_computeSurfaceNets);
          break;
      }
    }

    private void UpdateActivePostCompute()
    {
      if (m_doRigging)
        ComputeManager.Deactivate(s_computeMeshLock);
    }

    private bool UpdateAabbTreeBuffer(ref ComputeBuffer aabbTreeBuffer)
    {
      if (m_aabbTreeBuffer != null
          && m_aabbTreeBuffer.count >= m_aabbTree.Capacity)
        return false;

      if (aabbTreeBuffer != null)
        aabbTreeBuffer.Release();

      aabbTreeBuffer = new ComputeBuffer(m_aabbTree.Capacity, AabbTree<MudBrushBase>.NodePod.Stride);

      return true;
    }

    private int m_numSdfBrushes;
    private int m_numSdfMaterials;
    protected List<Transform> m_aBone;
    private Stack<Aabb> m_brushGroupBoundsStack = new Stack<Aabb>();
    private void WriteResources()
    {
      Profiler.BeginSample("WriteResources");


      Profiler.BeginSample("Brushes");

      // brushes
      m_numSdfBrushes = 0;
      m_numSdfMaterials = 0;
      s_sdfBrushMaterialIndexMap.Clear();
      m_aBone = (m_doRigging ? new List<Transform>() : null);
      for (int iBrush = 0, numBrushes = m_aBrushToProcess.Count; iBrush < numBrushes; ++iBrush)
      {
        var brush = m_aBrushToProcess[iBrush];

        Profiler.BeginSample("Each Brush");

        brush.m_iSdfBrush = m_numSdfBrushes;

        Profiler.BeginSample("Fill Material");
        int materialIndex = -1;
        if (brush.UsesMaterial 
            && !s_sdfBrushMaterialIndexMap.TryGetValue(brush.MaterialHash, out materialIndex))
        {
          materialIndex = m_numSdfMaterials;
          s_sdfBrushMaterialIndexMap.Add(brush.MaterialHash, materialIndex);
          brush.FillBrushMaterialData(ref s_aSdfBrushMaterial[m_numSdfMaterials]);
          ++m_numSdfMaterials;
        }
        Assert.True(!brush.UsesMaterial || materialIndex >= 0);
        Profiler.EndSample();

        Profiler.BeginSample("Fill Compute Data");
        int numNewBrushes = 
          (!brush.IsBrushGroup || !brush.m_preChildrenFlag)
            ? brush.FillComputeData(s_aSdfBrush, m_numSdfBrushes, (brush.CountAsBone ? m_aBone : null)) 
            : brush.FillComputeDataPostChildren(s_aSdfBrush, m_numSdfBrushes);
        Profiler.EndSample();

        Profiler.BeginSample("Fill Brush Data");
        float hash = Mathf.Abs(Codec.Hash(brush.GetHashCode()) % 0xFFFF) / ((float) 0xFFFF);
        for (int iNewBrush = 0; iNewBrush < numNewBrushes; ++iNewBrush)
        {
          int iSdfBrush = m_numSdfBrushes + iNewBrush;
          s_aSdfBrush[iSdfBrush].Index = iSdfBrush;

          if (!brush.IsBrushGroup || !brush.m_preChildrenFlag)
            brush.FillBrushData(ref s_aSdfBrush[iSdfBrush], iSdfBrush);
          else
            brush.FillBrushDataPostChildren(ref s_aSdfBrush[iSdfBrush], iSdfBrush);

          s_aSdfBrush[iSdfBrush].MaterialIndex = materialIndex;
          s_aSdfBrush[iSdfBrush].Hash = hash;
        }
        Profiler.EndSample();

        m_numSdfBrushes += numNewBrushes;

        if (brush.IsBrushGroup)
          brush.m_preChildrenFlag = !brush.m_preChildrenFlag;

        Profiler.EndSample();
      }
      m_brushesBuffer.SetData(s_aSdfBrush, 0, 0, m_numSdfBrushes);
      m_brushMaterialBuffer.SetData(s_aSdfBrushMaterial, 0, 0, m_numSdfMaterials);
      ComputeManager.SetInt(Const.NumBrushes, m_numSdfBrushes);
      ComputeManager.SetFloat(Const.SurfaceShift, SurfaceShift);
      ComputeManager.SetInt(Const.RenderMode, (int) RenderMode);
      ComputeManager.SetInt(Const.MeshingMode, (int) MeshingMode);
      ComputeManager.SetFloat(Const.NormalDifferentiationStep, Mathf.Max(0.01f * VoxelSize, SmoothNormalBlurRelative * VoxelSize + SmoothNormalBlurAbsolute));
      ComputeManager.SetFloat(Const.NormalQuantization, NormalQuantization);
      ComputeManager.SetFloat(Const.Normal2dFadeDist, Normal2dFade);
      ComputeManager.SetFloat(Const.Normal2dStrength, Normal2dStrength);
      bool shouldDoAutoSmoothing = ShouldDoAutoSmoothing;
      ComputeManager.SetBool(Const.EnableAutoSmooth, shouldDoAutoSmoothing);
      ComputeManager.SetFloat(Const.AutoSmoothMaxAngle, AutoSmoothingMaxAngle * MathUtil.Deg2Rad);
      ComputeManager.SetBuffer(Const.AutoSmoothVertDataTable, (shouldDoAutoSmoothing && m_autoSmoothVertDataTableBuffer != null) ? m_autoSmoothVertDataTableBuffer : s_dummyBuffer);
      ComputeManager.SetInt(Const.AutoSmoothVertDataPoolSize, (shouldDoAutoSmoothing && m_autoSmoothVertDataTableBuffer != null) ? m_autoSmoothVertDataTableBuffer.count : 0);
      ComputeManager.SetBool(Const.EnableSmoothCorner, EnableSmoothCorner);
      ComputeManager.SetInt(Const.SmoothCornerSubdivision, SmoothCornerSubdivision);
      ComputeManager.SetFloat(Const.SmoothCornerNormalBlur, 0.05f * VoxelSize + SmoothCornerNormalBlur);
      ComputeManager.SetFloat(Const.SmoothCornerFade, SmoothCornerFade);

      Profiler.EndSample();


      Profiler.BeginSample("BVH");

      // BVH
      Aabb successorModifierBounds = Aabb.Empty;
      Aabb accumulatedBounds = Aabb.Empty;
      Aabb groupBounds = Aabb.Empty;
      foreach (var brush in m_aBrushToProcess)
      {
        Aabb opBounds = brush.Bounds;

        if (brush.IsBrushGroup)
        {
          if (!brush.m_preChildrenFlag)
          {
            // begin group
            m_brushGroupBoundsStack.Push(groupBounds);
            groupBounds = Aabb.Empty;
          }
          else
          {
            // end group
            opBounds = groupBounds;
            opBounds.Expand(m_aabbTree.FatBoundsRadius);
            groupBounds = m_brushGroupBoundsStack.Pop();
          }
          brush.m_preChildrenFlag = !brush.m_preChildrenFlag;
        }

        if (opBounds.IsEmpty)
        {
          if (brush.IsBrushGroup)
            brush.UpdateProxies(m_aabbTree, opBounds);

          continue;
        }

        if (brush.IsSuccessorModifier)
          successorModifierBounds.Include(opBounds);

        opBounds.Include(successorModifierBounds);

        accumulatedBounds.Include(opBounds);
        groupBounds.Include(opBounds);

        if (brush.IsPredecessorModifier)
          opBounds = accumulatedBounds;

        opBounds.Expand(brush.BoundsPadding);
        accumulatedBounds.Include(opBounds);
        groupBounds.Include(opBounds);

        brush.UpdateProxies(m_aabbTree, opBounds);
      }
      bool aabbTreeNeedsCompute = false;
      if (AllowSharedRWBuffers)
      { 
        aabbTreeNeedsCompute = UpdateAabbTreeBuffer(ref s_aabbTreeBuffer);
        m_aabbTreeBuffer = s_aabbTreeBuffer;
      }
      else
      {
        aabbTreeNeedsCompute = UpdateAabbTreeBuffer(ref m_aabbTreeBuffer);
      }
      if (aabbTreeNeedsCompute)
        m_needsCompute = true;
      m_aabbTree.Fill(m_aabbTreeBuffer);

      Profiler.EndSample();


      Profiler.BeginSample("Voxel Tree");

      ComputeManager.SetInt(Const.NodeHashTableSize, m_nodeHashTableBuffer.count);

      // general allocation counters
      m_numAllocationsBuffer.SetData(IndirectDrawArgsInitData);

      // node pool
      ComputeManager.SetInt(Const.NodePoolSize, m_nodePoolBuffer.count);
      if (m_numAllocationsBufferInitData == null || m_numAllocationsBufferInitData.Length != m_numNodesAllocatedBuffer.count)
        m_numAllocationsBufferInitData = new int[m_numNodesAllocatedBuffer.count];
      for (int depth = 0; depth < m_numAllocationsBufferInitData.Length; ++depth)
        m_numAllocationsBufferInitData[depth] = 0;
      m_numNodesAllocatedBuffer.SetData(m_numAllocationsBufferInitData);

      // voxel cache
      ComputeManager.SetBool(Const.UseVoxelCache, UseVoxelCache);
      ComputeManager.SetInt(Const.VoxelCacheSize, (m_voxelCacheBuffer != null ? m_voxelCacheBuffer.count : 0));

      // brush bit masks
      ComputeManager.SetInt(Const.BrushMaskPoolSize, MaxBrushMasks);

      // dispatch args
      m_indirectDispatchArgsBuffer.SetData(UnitIndirectDispatchArgsInitData);

      Profiler.EndSample();


      Profiler.BeginSample("Indirect Draw Args");

      // indirect draw args
      m_indirectDrawArgsBufferDefault.SetData(IndirectDrawArgsInitData);
      if (m_indirectDrawArgsBufferOverride != null)
        m_indirectDrawArgsBufferOverride.SetData(IndirectDrawArgsInitData);

      Profiler.EndSample();


      Profiler.EndSample();
    }

    private void BindComputeResources()
    {
      Profiler.BeginSample("BindComputeResources");

      m_genPointsBufferUsedForCompute = m_genPointsBufferOverride != null ? m_genPointsBufferOverride : m_genPointsBufferDefault;
      m_indirectDrawArgsBufferUsedForCompute = m_indirectDrawArgsBufferOverride != null ? m_indirectDrawArgsBufferOverride : m_indirectDrawArgsBufferDefault;

      ComputeManager.SetBuffer(Const.Brushes, m_brushesBuffer);
      ComputeManager.SetBuffer(Const.BrushMaterials, m_brushMaterialBuffer);
      ComputeManager.SetBuffer(Const.AabbTree, m_aabbTreeBuffer);

      ComputeManager.SetBuffer(Const.NodeHashTable, m_nodeHashTableBuffer);
      ComputeManager.SetBuffer(Const.NodePool, m_nodePoolBuffer);
      ComputeManager.SetBuffer(Const.NumNodesAllocated, m_numNodesAllocatedBuffer);
      ComputeManager.SetBuffer(Const.NumAllocations, m_numAllocationsBuffer);
      ComputeManager.SetBuffer(Const.VoxelCacheIdTable, UseVoxelCache ? m_voxelCacheIdTableBuffer : m_numAllocationsBuffer);
      ComputeManager.SetBuffer(Const.VoxelCache, UseVoxelCache ? m_voxelCacheBuffer : m_numAllocationsBuffer);
      ComputeManager.SetBuffer(Const.BrushMaskPool, m_brushMaskPoolBuffer);
      ComputeManager.SetBuffer(Const.IndirectDispatchArgs, m_indirectDispatchArgsBuffer);
      ComputeManager.SetBuffer(Const.GenPoints, m_genPointsBufferUsedForCompute);
      ComputeManager.SetBuffer(Const.IndirectDrawArgs, m_indirectDrawArgsBufferUsedForCompute);

      ComputeManager.SetBool(Const.Enable2dMode, Enable2dMode);
      ComputeManager.SetBool(Const.ForceAllBrushes, ShouldForceAllBrushes());
      ComputeManager.SetFloat(Const.VoxelSize, NodeSizes[VoxelNodeDepth]);
      ComputeManager.SetInt(Const.MaxNodeDepth, VoxelNodeDepth);
      ComputeManager.SetInt(Const.ChunkVoxelDensity, ChunkVoxelDensity);
      ComputeManager.SetInt(Const.MaxGenPoints, MaxGenPoints);
      ComputeManager.SetInt(Const.AabbRoot, m_aabbTree.Root);

      ComputeManager.SetFloats(Const.NoiseCacheDimension, NoiseCacheDimensionFloats);
      ComputeManager.SetFloat(Const.NoiseCacheDensity, NoiseCacheDensity);

      switch (MeshingMode)
      {
        case MeshingModeEnum.SurfaceNets:
          ComputeManager.SetFloat(Const.SurfaceNetsDualQuadsBlend, SurfaceNetsDualQuadsBlend);
          /*
          ComputeManager.SetInt(Const.SurfaceNetsBinarySearchIterations, SurfaceNetsBinarySearchIterations);
          ComputeManager.SetInt(Const.SurfaceNetsGradientDescentIterations, SurfaceNetsGradientDescentIterations);
          ComputeManager.SetFloat(Const.SurfaceNetsGradientDescentFactor, SurfaceNetsGradientDescentFactor);
          */
          /*
          if (SurfaceNetsHighAccuracyMode)
          {
            ComputeManager.SetInt(Const.SurfaceNetsBinarySearchIterations, 6);
            ComputeManager.SetInt(Const.SurfaceNetsGradientDescentIterations, 1);
            ComputeManager.SetFloat(Const.SurfaceNetsGradientDescentFactor, 1.0f);
          }
          else
          {
            ComputeManager.SetInt(Const.SurfaceNetsBinarySearchIterations, 0);
            ComputeManager.SetInt(Const.SurfaceNetsGradientDescentIterations, 0);
            ComputeManager.SetFloat(Const.SurfaceNetsGradientDescentFactor, 0.0f);
          }
          */
          break;

        case MeshingModeEnum.DualContouring:
          ComputeManager.SetFloat(Const.DualContouringDualQuadsBlend, DualContouringDualQuadsBlend);
          ComputeManager.SetFloat(Const.DualContouringRelaxation, 0.15f + 0.85f * DualContouringRelaxation);
          /*
          ComputeManager.SetInt(Const.DualContouringBinarySearchIterations, DualContouringBinarySearchIterations);
          ComputeManager.SetInt(Const.DualContouringGradientDescentIterations, DualContouringGradientDescentIterations);
          ComputeManager.SetFloat(Const.DualContouringGradientDescentFactor, DualContouringGradientDescentFactor);
          */
          if (DualContouringHighAccuracyMode)
          {
            ComputeManager.SetInt(Const.DualContouringBinarySearchIterations, 8);
            ComputeManager.SetInt(Const.DualContouringGradientDescentIterations, 1);
            ComputeManager.SetFloat(Const.DualContouringGradientDescentFactor, 1.0f);
          }
          else
          {
            ComputeManager.SetInt(Const.DualContouringBinarySearchIterations, 0);
            ComputeManager.SetInt(Const.DualContouringGradientDescentIterations, 0);
            ComputeManager.SetFloat(Const.DualContouringGradientDescentFactor, 0.0f);
          }
          break;
      }

      Profiler.EndSample();
    }

    private void BindRenderResources()
    {
      Profiler.BeginSample("BindRenderResources");
  
      if (m_materialProps == null)
      {
        m_materialProps = new MaterialPropertyBlock();
        m_needsCompute = true;
      }

      m_materialProps.SetBuffer(Const.GenPoints, m_genPointsBufferDefault);

      m_materialProps.SetColor(Const.MasterColor, MasterColor);
      m_materialProps.SetColor(Const.MasterEmission, MasterEmission);
      m_materialProps.SetFloat(Const.MasterMetallic, MasterMetallic);
      m_materialProps.SetFloat(Const.MasterSmoothness, MasterSmoothness);

      m_materialProps.SetInt(Const.Enable2dMode, Enable2dMode ? 1 : 0);
      m_materialProps.SetFloat(Const.VoxelSize, NodeSizes[VoxelNodeDepth]);
      m_materialProps.SetFloat(Const.SplatSize, SplatSize * (1.5f / VoxelDensity));
      m_materialProps.SetFloat(Const.SplatSizeJitter, SplatSizeJitter);
      m_materialProps.SetFloat(Const.SplatNormalShift, SplatNormalShift);
      m_materialProps.SetFloat(Const.SplatNormalShiftJitter, SplatNormalShiftJitter);
      m_materialProps.SetFloat(Const.SplatColorJitter, SplatColorJitter);
      m_materialProps.SetFloat(Const.SplatPositionJitter, SplatPositionJitter);
      m_materialProps.SetFloat(Const.SplatRotationJitter, SplatRotationJitter);
      m_materialProps.SetFloat(Const.SplatOrientationJitter, SplatOrientationJitter);
      m_materialProps.SetFloat(Const.SplatJitterNoisiness, SplatJitterNoisiness);
      m_materialProps.SetFloat(Const.SplatCameraFacing, SplatCameraFacing);
      m_materialProps.SetFloat(Const.SplatScreenSpaceFlattening, SplatScreenSpaceFlattening);
      m_materialProps.SetMatrix(Const.LocalToWorld, transform.localToWorldMatrix);
      m_materialProps.SetMatrix(Const.LocalToWorldIt, transform.localToWorldMatrix.inverse.transpose);
      m_materialProps.SetVector(Const.LocalToWorldScale, transform.localScale);
      m_materialProps.SetMatrix(Const.WorldToLocal, transform.worldToLocalMatrix);

      m_materialProps.SetInt(Const.RenderMode, (int) RenderMode);
      m_materialProps.SetInt(Const.MeshingMode, (int) MeshingMode);
      m_materialProps.SetFloat(Const.NormalQuantization, NormalQuantization);
      m_materialProps.SetFloat(Const.Normal2dFadeDist, Normal2dFade);
      m_materialProps.SetFloat(Const.Normal2dStrength, Normal2dStrength);

      m_materialProps.SetFloat(Const.AutoSmoothMaxAngle, EnableAutoSmoothing ? AutoSmoothingMaxAngle * MathUtil.Deg2Rad : -1.0f);

      Profiler.EndSample();
    }

    #endregion // end: Brushes

    //-------------------------------------------------------------------------

    #region Core

    private void SetComputeNodeDepth(int depth)
    {
      ComputeManager.SetInt(Const.CurrentNodeDepth, depth);
      ComputeManager.SetInt(Const.CurrentNodeBranchingFactor, (depth < VoxelNodeDepth ? VoxelTreeBranchingFactors[depth] : 0));
      ComputeManager.SetFloat(Const.CurrentNodeSize, NodeSizes[depth]);
    }

    private void Compute()
    {
      Profiler.BeginSample("Compute");

      UpdateActivePreCompute();

      SetUpResources();

      Profiler.BeginSample("Core Dispatch");

      ComputeManager.Dispatch
      (
        s_computeVoxelGen, 
        Const.Kernel.ClearVoxelHashTable, 
        Mathf.Max(1, (m_nodeHashTableBuffer.count + ClearThreadGroupSize - 1) / ClearThreadGroupSize), 1, 1
      );

      if (ShouldDoAutoSmoothing)
      {
        ComputeManager.Dispatch
        (
          s_computeVoxelGen, 
          Const.Kernel.ClearAutoSmoothVertDataTable, 
          Mathf.Max(1, (m_autoSmoothVertDataTableBuffer.count + ClearThreadGroupSize - 1) / ClearThreadGroupSize), 1, 1
        );
      }

      if (UseVoxelCache)
      {
        ComputeManager.Dispatch
        (
          s_computeVoxelGen, 
          Const.Kernel.ClearVoxelCache, 
          Mathf.Max(1, (m_voxelCacheIdTableBuffer.count + ClearThreadGroupSize - 1) / ClearThreadGroupSize), 1, 1
        );
      }

      SetComputeNodeDepth(0);
      ComputeManager.Dispatch
      (
        s_computeVoxelGen, 
        Const.Kernel.RegisterTopNodes, 
        Mathf.Max(1, (m_numSdfBrushes + ThreadGroupSize - 1) / ThreadGroupSize), 1, 1
      );

      for (int depth = 0; depth < VoxelNodeDepth; ++depth)
      {
        SetComputeNodeDepth(depth);
        ComputeManager.Dispatch(s_computeVoxelGen, Const.Kernel.UpdateBranchingIndirectDispatchArgs, 1, 1, 1);
        ComputeManager.DispatchIndirect(s_computeVoxelGen, Const.Kernel.AllocateChildNodes, m_indirectDispatchArgsBuffer);
      }

      SetComputeNodeDepth(VoxelNodeDepth);
      ComputeManager.Dispatch(s_computeVoxelGen, Const.Kernel.UpdateVoxelIndirectDispatchArgs, 1, 1, 1);

      switch (MeshingMode)
      {
        case MeshingModeEnum.MarchingCubes:
          switch (RenderMode)
          {
            case RenderModeEnum.FlatMesh:
              if (Enable2dMode)
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.GenerateFlatMarchingCubesMesh2d, m_indirectDispatchArgsBuffer);
              else
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.GenerateFlatMarchingCubesMesh, m_indirectDispatchArgsBuffer);
              if (ShouldDoAutoSmoothing)
              {
                ComputeManager.Dispatch(s_computeMarchingCubes, Const.Kernel.UpdateMarchingCubesAutoSmoothIndirectDispatchArgs, 1, 1, 1);
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.MarchingCubesUpdateAutoSmooth, m_indirectDispatchArgsBuffer);
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.MarchingCubesComputeAutoSmooth, m_indirectDispatchArgsBuffer);
              }
              break;

            case RenderModeEnum.SmoothMesh:
              if (ShouldDoAutoSmoothing)
              {
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.GenerateFlatMarchingCubesMesh, m_indirectDispatchArgsBuffer);
                ComputeManager.Dispatch(s_computeMarchingCubes, Const.Kernel.UpdateMarchingCubesAutoSmoothIndirectDispatchArgs, 1, 1, 1);
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.MarchingCubesUpdateAutoSmooth, m_indirectDispatchArgsBuffer);
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.MarchingCubesComputeAutoSmooth, m_indirectDispatchArgsBuffer);
              }
              else
              {
                if (Enable2dMode)
                  ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.GenerateSmoothMarchingCubesMesh2d, m_indirectDispatchArgsBuffer);
                else
                  ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.GenerateSmoothMarchingCubesMesh, m_indirectDispatchArgsBuffer);
              }
              break;

            case RenderModeEnum.CircleSplats:
            case RenderModeEnum.QuadSplats:
              if (Enable2dMode)
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.GenerateMarchingCubesSplats2d, m_indirectDispatchArgsBuffer);
              else
                ComputeManager.DispatchIndirect(s_computeMarchingCubes, Const.Kernel.GenerateMarchingCubesSplats, m_indirectDispatchArgsBuffer);
              break;
          }
          break;

        case MeshingModeEnum.DualQuads:
          if (Enable2dMode)
          {
            ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.GenerateDualQuads2d, m_indirectDispatchArgsBuffer);
          }
          else
          {
            ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.GenerateDualQuads, m_indirectDispatchArgsBuffer);
            switch (RenderMode)
            {
              case RenderModeEnum.FlatMesh:
                // do nothing
                break;

              case RenderModeEnum.SmoothMesh:
                ComputeManager.Dispatch(s_computeDualMeshing, Const.Kernel.UpdateDualMeshingIndirectDispatchArgs, 1, 1, 1);
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingSmoothMeshNormal, m_indirectDispatchArgsBuffer);
                break;
            }
          }
          break;

        case MeshingModeEnum.SurfaceNets:
          if (Enable2dMode)
            ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.GenerateDualQuads2d, m_indirectDispatchArgsBuffer);
          else
            ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.GenerateDualQuads, m_indirectDispatchArgsBuffer);
          ComputeManager.Dispatch(s_computeDualMeshing, Const.Kernel.UpdateDualMeshingIndirectDispatchArgs, 1, 1, 1);
          if (Enable2dMode)
            ComputeManager.DispatchIndirect(s_computeSurfaceNets, Const.Kernel.SurfaceNetsMovePoint2d, m_indirectDispatchArgsBuffer);
          else
            ComputeManager.DispatchIndirect(s_computeSurfaceNets, Const.Kernel.SurfaceNetsMovePoint, m_indirectDispatchArgsBuffer);
          switch (RenderMode)
          {
            case RenderModeEnum.FlatMesh:
              if (ShouldDoAutoSmoothing)
              {
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingUpdateAutoSmooth, m_indirectDispatchArgsBuffer);
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingComputeAutoSmooth, m_indirectDispatchArgsBuffer);
              }
              else
              {
                if (Enable2dMode)
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingFlatMeshNormal2d, m_indirectDispatchArgsBuffer);
                else
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingFlatMeshNormal, m_indirectDispatchArgsBuffer);
              }
              break;

            case RenderModeEnum.SmoothMesh:
              if (ShouldDoAutoSmoothing)
              {
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingUpdateAutoSmooth, m_indirectDispatchArgsBuffer);
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingComputeAutoSmooth, m_indirectDispatchArgsBuffer);
              }
              else
              {
                if (Enable2dMode)
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingSmoothMeshNormal2d, m_indirectDispatchArgsBuffer);
                else
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingSmoothMeshNormal, m_indirectDispatchArgsBuffer);
              }
              break;

            case RenderModeEnum.CircleSplats:
            case RenderModeEnum.QuadSplats:
              ComputeManager.Dispatch(s_computeDualMeshing, Const.Kernel.UpdateDualMeshingSplatsIndirectArgs, 1, 1, 1);
              ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.ConvertDualMeshingSplats, m_indirectDispatchArgsBuffer);
              break;
          }
          break;

        case MeshingModeEnum.DualContouring:
          if (Enable2dMode)
            ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.GenerateDualQuads2d, m_indirectDispatchArgsBuffer);
          else
            ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.GenerateDualQuads, m_indirectDispatchArgsBuffer);
          ComputeManager.Dispatch(s_computeDualMeshing, Const.Kernel.UpdateDualMeshingIndirectDispatchArgs, 1, 1, 1);
          if (Enable2dMode)
            ComputeManager.DispatchIndirect(s_computeDualContouring, Const.Kernel.DualContouringMovePoint2d, m_indirectDispatchArgsBuffer);
          else
            ComputeManager.DispatchIndirect(s_computeDualContouring, Const.Kernel.DualContouringMovePoint, m_indirectDispatchArgsBuffer);
          switch (RenderMode)
          {
            case RenderModeEnum.FlatMesh:
              if (ShouldDoAutoSmoothing)
              {
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingUpdateAutoSmooth, m_indirectDispatchArgsBuffer);
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingComputeAutoSmooth, m_indirectDispatchArgsBuffer);
                if (EnableSmoothCorner)
                {
                  ComputeManager.Dispatch(s_computeDualMeshing, Const.Kernel.DualMeshingUpdateSmoothCornerIndirectDispatchArgs, 1, 1, 1);
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingSmoothCorner, m_indirectDispatchArgsBuffer);
                }
              }
              else
              {
                if (Enable2dMode)
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingFlatMeshNormal2d, m_indirectDispatchArgsBuffer);
                else
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingFlatMeshNormal, m_indirectDispatchArgsBuffer);
              }
              break;

            case RenderModeEnum.SmoothMesh:
              if (ShouldDoAutoSmoothing)
              {
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingUpdateAutoSmooth, m_indirectDispatchArgsBuffer);
                ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingComputeAutoSmooth, m_indirectDispatchArgsBuffer);
                if (EnableSmoothCorner)
                {
                  ComputeManager.Dispatch(s_computeDualMeshing, Const.Kernel.DualMeshingUpdateSmoothCornerIndirectDispatchArgs, 1, 1, 1);
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingSmoothCorner, m_indirectDispatchArgsBuffer);
                }
              }
              else
              {
                if (Enable2dMode)
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingSmoothMeshNormal2d, m_indirectDispatchArgsBuffer);
                else
                  ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.DualMeshingSmoothMeshNormal, m_indirectDispatchArgsBuffer);
              }
              break;

            case RenderModeEnum.CircleSplats:
            case RenderModeEnum.QuadSplats:
              ComputeManager.Dispatch(s_computeDualMeshing, Const.Kernel.UpdateDualMeshingSplatsIndirectArgs, 1, 1, 1);
              ComputeManager.DispatchIndirect(s_computeDualMeshing, Const.Kernel.ConvertDualMeshingSplats, m_indirectDispatchArgsBuffer);
              break;
          }
          break;
      }

      Profiler.EndSample();

      if (MudBun.IsFreeVersion)
      {
        if (LockMeshIntermediateState != LockMeshIntermediateStateEnum.Idle 
            && m_indirectDrawArgsBufferUsedForCompute != null)
        {
          int[] aIndirectDrawArgs = new int[4];
          m_indirectDrawArgsBufferUsedForCompute.GetData(aIndirectDrawArgs);
          aIndirectDrawArgs[0] = Mathf.Min(aIndirectDrawArgs[0], 3 * MaxMeshGenerationTrianglesFreeVersion);
          m_indirectDrawArgsBufferUsedForCompute.SetData(aIndirectDrawArgs);
        }
      }

      if (m_doRigging)
      {
        Profiler.BeginSample("Rigging Dispatch");

        ComputeManager.SetBuffer(Const.IndirectDrawArgs, m_indirectDrawArgsBufferUsedForCompute);
        ComputeManager.SetBuffer(Const.GenPoints, m_genPointsBufferUsedForCompute);
        ComputeManager.SetBuffer(Const.BrushMaterials, m_brushMaterialBuffer);
        ComputeManager.SetBuffer(Const.Brushes, m_brushesBuffer);
        ComputeManager.SetTexture(Const.NoiseCache, s_noiseCache);
        ComputeManager.SetInt(Const.NumBrushes, m_numSdfBrushes);

        int[] aIndirectDrawArgs = new int[4];
        m_indirectDrawArgsBufferUsedForCompute.GetData(aIndirectDrawArgs);
        ComputeManager.Dispatch
        (
          s_computeMeshLock, 
          Const.Kernel.RigBones, 
          (aIndirectDrawArgs[0] + ThreadGroupSize - 1) / ThreadGroupSize, 1, 1
        );

        Profiler.EndSample();
      }

      // only use buffer overrides once
      if (m_genPointsBufferOverride != null)
        m_genPointsBufferOverride = null;
      if (m_indirectDrawArgsBufferOverride != null)
        m_indirectDrawArgsBufferOverride = null;

      UpdateActivePostCompute();

      Profiler.EndSample();
    }

    private void Render()
    {
      if (IsEditorBusy())
        return;

      if (m_indirectDrawArgsBufferDefault == null)
        return;

      if (!s_globalResourcesValid || !m_localResourcesValid)
        return;

      Profiler.BeginSample("Render");

      Aabb renderBounds = RenderBounds;

      Material material = null;
      switch (RenderModeCategory)
      {
        case RenderModeCatergoryEnum.Mesh:
          material = RenderMaterialMesh;
          break;

        case RenderModeCatergoryEnum.Splats:
          material = RenderMaterialSplats;
          break;
      }

      {
        Profiler.BeginSample("Copy Render Material Properties");

        if (m_materialCloned != material)
        {
          m_materialUsed = new Material(material);
          m_materialCloned = material;

          if (!Application.isEditor)
          {
            m_materialUsed.CopyPropertiesFromMaterial(m_materialCloned);
            m_materialUsed.enableInstancing = true;
          }
        }
        if (Application.isEditor)
        {
          // need to do this constantly in editor because the user can change the referenced material
          // TODO: timeslice this when in play mode
          m_materialUsed.CopyPropertiesFromMaterial(m_materialCloned);
          m_materialUsed.enableInstancing = true;
        }

        Profiler.EndSample();
      }

      m_materialUsed.EnableKeyword("MUDBUN_PROCEDURAL");
      switch (RenderMode)
      {
        case RenderModeEnum.QuadSplats:
          m_materialUsed.EnableKeyword("MUDBUN_QUAD_SPLATS");
          break;
        default:
          m_materialUsed.DisableKeyword("MUDBUN_QUAD_SPLATS");
          break;
      }

      BindRenderResources();

      Graphics.DrawProceduralIndirect
      (
        m_materialUsed, 
        new Bounds(renderBounds.Center, renderBounds.Extents), 
        MeshTopology.Triangles, 
        m_indirectDrawArgsBufferDefault, 
        0, 
        null, 
        m_materialProps, 
        CastShadows, 
        ReceiveShadows, 
        0
      );

      Profiler.EndSample();
    }

    #endregion // end: Core
  }
}
