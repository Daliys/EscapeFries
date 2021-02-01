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

using UnityEngine;
using UnityEngine.Rendering;

namespace MudBun
{
  [ExecuteInEditMode]
  public class MudLockedMeshRenderer : MonoBehaviour, ISerializationCallbackReceiver
  {
    private class ComputeBufferCache
    {
      public int RefCount;

      public int [] IndirectDrawArgsData;
      public GenPoint [] GenPointsData;

      public ComputeBuffer IndirectDrawArgsBuffer;
      public ComputeBuffer GenPointsBuffer;

      public void Release()
      {
        IndirectDrawArgsData = null;
        GenPointsData = null;
        
        IndirectDrawArgsBuffer.Release();
        GenPointsBuffer.Release();
        IndirectDrawArgsBuffer = null;
        GenPointsBuffer = null;
      }
    }

    private static Dictionary<int, ComputeBufferCache> s_bufferCachePool = new Dictionary<int, ComputeBufferCache>();

    [SerializeField] [HideInInspector] private int m_numVerts = 0;
    [SerializeField] [HideInInspector] private int [] m_indirectDrawArgsData;
    [SerializeField] [HideInInspector] private GenPoint [] m_aGenPointData;
    [SerializeField] [HideInInspector] private Aabb m_renderBoundsCs;
    public Material RenderMaterial;
    private Material m_materialCloned;
    private Material m_materialUsed;

    public Color MasterColor;
    public Color MasterEmission;
    public float MasterMetallic;
    public float MasterSmoothness;

    [SerializeField] [HideInInspector] private MudRendererBase.RenderModeEnum m_renderMode = MudRendererBase.RenderModeEnum.SmoothMesh;
    [SerializeField] [HideInInspector] private float m_voxelDensity = 1.0f;
    [ConditionalField("m_renderMode", 
      MudRendererBase.RenderModeEnum.CircleSplats, 
      MudRendererBase.RenderModeEnum.QuadSplats, 
      Min = 0.0f, Max = 5.0f)]
    public float SplatSize = 1.0f;
    [ConditionalField("m_renderMode", 
      MudRendererBase.RenderModeEnum.CircleSplats, 
      MudRendererBase.RenderModeEnum.QuadSplats, 
      Min = 0.0f, Max = 360.0f)]
    public float SplatRotation = 0.0f;
    [ConditionalField("m_renderMode", 
      MudRendererBase.RenderModeEnum.CircleSplats, 
      MudRendererBase.RenderModeEnum.QuadSplats, 
      Min = 0.1f, Max = 10.0f)]
    public float SplatRotationNoisiness = 1.0f;
    [ConditionalField("m_renderMode", 
      MudRendererBase.RenderModeEnum.CircleSplats, 
      MudRendererBase.RenderModeEnum.QuadSplats, 
      Min = 0.0f, Max = 1.0f)]
    public float SplatCameraFacing = 1.0f;

    public ShadowCastingMode CastShadows = ShadowCastingMode.On;
    public bool ReceiveShadows = true;

    private ComputeBuffer m_indirectDrawArgsBuffer;
    private ComputeBuffer m_genPointsBuffer;
    private MaterialPropertyBlock m_renderMaterialProps;

    [SerializeField] [HideInInspector] private int m_hash;

    public void Config
    (
      ComputeBuffer drawArgsBuffer, 
      ComputeBuffer getPointsBuffer, 
      MudRendererBase renderer
    )
    {
      m_indirectDrawArgsData = new int[4];
      drawArgsBuffer.GetData(m_indirectDrawArgsData);

      m_numVerts = m_indirectDrawArgsData[0];
      m_aGenPointData = new GenPoint[m_numVerts];
      getPointsBuffer.GetData(m_aGenPointData);

      RenderMaterial = renderer.RenderMaterial;
      MasterColor = renderer.MasterColor;
      MasterEmission = renderer.MasterEmission;
      MasterMetallic = renderer.MasterMetallic;
      MasterSmoothness = renderer.MasterSmoothness;
      m_renderBoundsCs = renderer.RenderBoundsCs;
      m_renderMode = renderer.RenderMode;
      m_voxelDensity = renderer.VoxelDensity;
      SplatSize = renderer.SplatSize;
      SplatRotation = renderer.SplatRotationJitter;
      SplatCameraFacing = renderer.SplatCameraFacing;

      CastShadows = renderer.CastShadows;
      ReceiveShadows = renderer.ReceiveShadows;

      m_hash = Codec.Hash(GetHashCode());
      m_hash = Codec.HashConcat(m_hash, DateTime.Now.Ticks);

      Validate();
    }

    public void OnBeforeSerialize()
    {
      ComputeBufferCache bufferCache;
      if (s_bufferCachePool.TryGetValue(m_hash, out bufferCache))
      {
        m_indirectDrawArgsData = bufferCache.IndirectDrawArgsData;
        m_aGenPointData = bufferCache.GenPointsData;
      }
    }
    public void OnAfterDeserialize() { }

    private void OnEnable()
    {
      
    }

    private void OnDisable()
    {
      m_indirectDrawArgsBuffer = null;
      m_genPointsBuffer = null;

      ComputeBufferCache bufferCache = null;
      if (s_bufferCachePool.TryGetValue(m_hash, out bufferCache))
      {
        --bufferCache.RefCount;
        if (bufferCache.RefCount == 0)
        {
          bufferCache.Release();
          s_bufferCachePool.Remove(m_hash);
        }
      }
    }

    private void Validate()
    {
      if (m_indirectDrawArgsBuffer == null 
          || m_genPointsBuffer == null)
      {
        ComputeBufferCache bufferCache = null;
        if (s_bufferCachePool.TryGetValue(m_hash, out bufferCache))
        {
          m_indirectDrawArgsBuffer = bufferCache.IndirectDrawArgsBuffer;
          m_genPointsBuffer = bufferCache.GenPointsBuffer;

          m_indirectDrawArgsData = null;
          m_aGenPointData = null;

          ++bufferCache.RefCount;
        }
        else if (m_indirectDrawArgsData != null 
                 && m_aGenPointData != null)
        {
          m_indirectDrawArgsBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
          m_indirectDrawArgsBuffer.SetData(m_indirectDrawArgsData);

          m_genPointsBuffer = new ComputeBuffer(m_numVerts, GenPoint.Stride);
          m_genPointsBuffer.SetData(m_aGenPointData);

          s_bufferCachePool.Add
          (
            m_hash, 
            new ComputeBufferCache()
            {
              RefCount = 1, 
              IndirectDrawArgsData = m_indirectDrawArgsData, 
              GenPointsData = m_aGenPointData, 
              IndirectDrawArgsBuffer = m_indirectDrawArgsBuffer, 
              GenPointsBuffer = m_genPointsBuffer
            }
          );

          m_indirectDrawArgsData = null;
          m_aGenPointData = null;
        }
        else
        {
          Assert.Warn("Null constant buffers and no data to initialize from.");
        }
      }

      if (m_renderMaterialProps == null)
      {
        m_renderMaterialProps = new MaterialPropertyBlock();
      }
    }

    private void LateUpdate()
    {
      Validate();
      Render();
    }

    private void BindRenderResources()
    {
      MudRendererBase.ValidateShaderConstantId();

      m_renderMaterialProps.SetBuffer(MudRendererBase.Const.GenPoints, m_genPointsBuffer);

      m_renderMaterialProps.SetColor(MudRendererBase.Const.MasterColor, MasterColor);
      m_renderMaterialProps.SetColor(MudRendererBase.Const.MasterEmission, MasterEmission);
      m_renderMaterialProps.SetFloat(MudRendererBase.Const.MasterMetallic, MasterMetallic);
      m_renderMaterialProps.SetFloat(MudRendererBase.Const.MasterSmoothness, MasterSmoothness);

      m_renderMaterialProps.SetFloat(MudRendererBase.Const.SplatSize, SplatSize * (1.5f / m_voxelDensity));
      m_renderMaterialProps.SetFloat(MudRendererBase.Const.SplatRotationJitter, SplatRotation * MathUtil.Deg2Rad);
      m_renderMaterialProps.SetFloat(MudRendererBase.Const.SplatJitterNoisiness, SplatRotationNoisiness);
      m_renderMaterialProps.SetFloat(MudRendererBase.Const.SplatCameraFacing, SplatCameraFacing);
      m_renderMaterialProps.SetMatrix(MudRendererBase.Const.LocalToWorld, transform.localToWorldMatrix);
      m_renderMaterialProps.SetMatrix(MudRendererBase.Const.LocalToWorldIt, transform.localToWorldMatrix.inverse.transpose);
      m_renderMaterialProps.SetVector(MudRendererBase.Const.LocalToWorldScale, transform.localScale);
    }

    private void Render()
    {
      if (m_indirectDrawArgsBuffer == null 
          || m_genPointsBuffer == null 
          || RenderMaterial == null 
          || m_renderBoundsCs.IsEmpty 
          || RenderMaterial == null)
        return;

      BindRenderResources();

      var renderBounds = m_renderBoundsCs;
      renderBounds.Transform(transform);

      if (m_materialCloned != RenderMaterial)
      {
        m_materialUsed = new Material(RenderMaterial);
        m_materialCloned = RenderMaterial;
      }
      m_materialUsed.CopyPropertiesFromMaterial(m_materialCloned);

      if (ReceiveShadows)
        RenderMaterial.EnableKeyword("MUDBUN_RECEIVE_SHADOWS");
      else
        RenderMaterial.DisableKeyword("MUDBUN_RECEIVE_SHADOWS");

      Graphics.DrawProceduralIndirect
      (
        RenderMaterial, 
        new Bounds(renderBounds.Center, renderBounds.Size), 
        MeshTopology.Triangles, 
        m_indirectDrawArgsBuffer, 
        0, 
        null, 
        m_renderMaterialProps, 
        CastShadows, 
        ReceiveShadows, 
        0
      );
    }
  }
}
