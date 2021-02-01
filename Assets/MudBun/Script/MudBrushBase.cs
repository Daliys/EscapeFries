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
  [ExecuteInEditMode]
  public abstract class MudBrushBase : MonoBehaviour
  {
    internal MudRendererBase m_renderer;
    internal int m_iSdfBrush;
    internal bool m_dirty = true;
    [HideInInspector] public bool Hidden = false;

    public MudRendererBase Renderer => m_renderer;

    public void MarkDirty() { m_dirty = true; }

    public virtual Aabb Bounds => Aabb.Empty;
    public virtual float BoundsPadding => 0.0f;
    public virtual bool IsSuccessorModifier => false;
    public virtual bool IsPredecessorModifier => false;

    internal bool m_preChildrenFlag = false;
    public virtual bool IsBrushGroup => false;

    public virtual bool UsesMaterial => false;
    public virtual int MaterialHash => 0;

    internal int m_iProxy = AabbTree<MudBrushBase>.Null;
    public virtual void UpdateProxies(AabbTree<MudBrushBase> tree, Aabb opBounds)
    {
      if (m_iProxy == AabbTree<MudBrushBase>.Null)
        m_iProxy = tree.CreateProxy(opBounds, this);

      tree.UpdateProxy(m_iProxy, opBounds, m_iSdfBrush);
    }

    public virtual void DestroyProxies(AabbTree<MudBrushBase> tree)
    {
      tree.DestroyProxy(m_iProxy);
      m_iProxy = AabbTree<MudBrushBase>.Null;
    }

    public Aabb BoundaryShapeBounds(SdfBrush.BoundaryShapeEnum boundaryShape, float radius)
    {
      Vector3 size = transform.localScale;
      Aabb bounds = Aabb.Empty;
      switch (boundaryShape)
      {
        case SdfBrush.BoundaryShapeEnum.Box:
          {
            Vector3 r = 0.5f * size;
            bounds = new Aabb(-r, r);
            bounds.Rotate(RotationRs(transform.rotation));
            break;
          }

        case SdfBrush.BoundaryShapeEnum.Sphere:
          {
            Vector3 r = radius * size;
            bounds = new Aabb(-r, r);
            bounds.Rotate(RotationRs(transform.rotation));
            break;
          }

        case SdfBrush.BoundaryShapeEnum.Cylinder:
          {
            Vector3 r = new Vector3(radius, 0.5f * size.y, radius);
            bounds = new Aabb(-r, r);
            bounds.Rotate(RotationRs(transform.rotation));
            break;
          }

        case SdfBrush.BoundaryShapeEnum.Torus:
          {
            Vector3 r = new Vector3(0.5f * size.x, 0.0f, 0.5f * size.z);
            bounds = new Aabb(-r, r);
            bounds.Rotate(RotationRs(transform.rotation));
            Vector3 round = radius * Vector3.one;
            bounds.Min -= round;
            bounds.Max += round;
            break;
          }

        case SdfBrush.BoundaryShapeEnum.SolidAngle:
          {
            Vector3 r = radius * VectorUtil.Abs(transform.localScale);
            bounds = new Aabb(-r, r);
            break;
          }
      }

      return bounds;
    }

    protected virtual void ScanRenderer() { }

    public virtual void OnEnable()
    {
      m_renderer = null;
      m_iProxy = AabbTree<MudBrushBase>.Null;
      m_iSdfBrush = -1;
      MarkDirty();

      ScanRenderer();
    }

    public virtual void OnDisable()
    {
      if (m_renderer != null)
        m_renderer.OnBrushDisabled(this);
    }

    private void OnValidate()
    {
      SanitizeParameters();
      MarkDirty();
    }

    public virtual void SanitizeParameters() { }

    // Ws: world space
    // Rs: renderer space

    protected Vector3 PointRs(Vector3 posWs)
    {
      return 
        m_renderer != null 
          ? m_renderer.transform.InverseTransformPoint(posWs) 
          : posWs;
    }

    protected Vector3 VectorRs(Vector3 vecWs)
    {
      return 
        m_renderer != null 
          ? Quaternion.Inverse(m_renderer.transform.rotation) * vecWs 
          : vecWs;
    }

    protected Quaternion RotationRs(Quaternion rotWs)
    {
      return
        m_renderer != null
          ? Quaternion.Inverse(m_renderer.transform.rotation) * rotWs
          : rotWs;
    }

    public virtual bool CountAsBone => false;

    public virtual int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone) { return 0; }
    public virtual void FillBrushData(ref SdfBrush brush, int iBrush)
    {
      brush.Proxy = m_iProxy;

      brush.Position = PointRs(transform.position);
      brush.Rotation = RotationRs(transform.rotation);
      brush.Size = transform.localScale;

      brush.Flags.AssignBit(SdfBrush.FlagBit.Hidden, Hidden);
    }

    public virtual int FillComputeDataPostChildren(SdfBrush[] aBrush, int iStart) { return 0; }
    public virtual void FillBrushDataPostChildren(ref SdfBrush brush, int iBrush) { }

    public virtual void FillBrushMaterialData(ref SdfBrushMaterial mat) { }

    public virtual void ValidateMaterial() { }

    protected virtual void OnDrawGizmos()
    {
      if (Renderer == null)
        return;

      if (!Renderer.EnableClickSelection)
        return;

      Gizmos.matrix = Renderer.transform.localToWorldMatrix;
      DrawSelectionGizmosRs();
      Gizmos.matrix = Matrix4x4.identity;
    }

    public virtual void DrawGizmosRs() { }
    public virtual void DrawSelectionGizmosRs() { }
    public virtual void DrawOutlineGizmosRs() { }
  }
}


