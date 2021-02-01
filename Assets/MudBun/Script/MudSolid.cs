/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Rendering;

namespace MudBun
{
  [ExecuteInEditMode]
  [RequireComponent(typeof(MudMaterial))]
  public class MudSolid : MudBrush
  {
    [SerializeField] private SdfBrush.OperatorEnum m_operator = SdfBrush.OperatorEnum.Union;
    public SdfBrush.OperatorEnum Operator { get => m_operator; set { m_operator = value; MarkDirty(); } }

    [SerializeField] private float m_blend;
    public float Blend { get => m_blend; set { m_blend = value; MarkDirty(); } }

    // TODO: not ready for auto-rigging yet
    /*
    [SerializeField] private bool m_mirrorX = false;
    public bool MirrorX { get => m_mirrorX; set { m_mirrorX = value; MarkDirty(); } }
    */

    [Tooltip("If checked, this brush will be counted as bone during auto rigging.")]
    [SerializeField] [ConditionalField("m_canCountAsBone", true)] public bool m_countAsBone = true;
    [SerializeField] [HideInInspector] protected bool m_canCountAsBone = true;
    public override bool CountAsBone => m_countAsBone || (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal); // Metal is weird
    //[ConditionalField("m_mirrorX", true, Label = "  Create Mirrored Bone")] public bool CreateMirroredBone = true;

    public override float BoundsPadding => m_blend;
    public override bool IsPredecessorModifier => (m_operator == SdfBrush.OperatorEnum.Intersect);

    public override bool UsesMaterial => true;
    public override int MaterialHash => GetComponent<MudMaterial>().MaterialHash;

    internal MudMaterial m_material;

    public override void SanitizeParameters()
    {
      base.SanitizeParameters();

      Validate.NonNegative(ref m_blend);
    }

    public override void FillBrushData(ref SdfBrush brush, int iBrush)
    {
      base.FillBrushData(ref brush, iBrush);
      
      brush.Operator = (int) m_operator;
      brush.Blend = Blend;

      if (!m_material)
        m_material = GetComponent<MudMaterial>();
      Assert.True(m_material != null, "Mussing brush material. A solid brush must have a MudMaterial component.");

      brush.Flags.AssignBit(SdfBrush.FlagBit.ContributeMaterial, m_material.ContributeMaterial);
      //brush.Flags.AssignBit(SdfBrush.FlagBit.MirrorX, MirrorX);
      brush.Flags.AssignBit(SdfBrush.FlagBit.CountAsBone, CountAsBone);
      //brush.Flags.AssignBit(SdfBrush.FlagBit.CreateMirroredBone, CreateMirroredBone);
    }

    public override void FillBrushMaterialData(ref SdfBrushMaterial mat)
    {
      base.FillBrushMaterialData(ref mat);

      if (!m_material)
        m_material = GetComponent<MudMaterial>();
      Assert.True(m_material != null, "Mussing brush material. A solid brush must have a MudMaterial component.");

      mat.Color = m_material.Color;
      mat.Emission = m_material.Emission;
      mat.MetallicSmoothnessSizeTightness.Set(m_material.Metallic, m_material.Smoothness, m_material.SplatSize, m_material.BlendTightness);
      mat.IntWeight.Set
      (
        (m_material.TextureIndex == 0 ? 1.0f : 0.0f), 
        (m_material.TextureIndex == 1 ? 1.0f : 0.0f), 
        (m_material.TextureIndex == 2 ? 1.0f : 0.0f), 
        (m_material.TextureIndex == 3 ? 1.0f : 0.0f)
      );
    }

    public override void ValidateMaterial()
    {
      var material = GetComponent<MudMaterial>();
      if (material != null)
        return;

      material = gameObject.AddComponent<MudMaterial>();
    }

    public override void DrawGizmosRs()
    {
      base.DrawGizmosRs();

      #if UNITY_EDITOR
      bool selected = Selection.Contains(gameObject);
      #else
      bool selected = false;
      #endif

      bool shouldDrawOutlines = selected || (Renderer != null && Renderer.Enable2dMode);
      if (!shouldDrawOutlines)
      {
        switch (m_operator)
        {
          case SdfBrush.OperatorEnum.Union:
          {
            if (Renderer == null)
              break;

            if (Renderer.RenderModeCategory != MudRendererBase.RenderModeCatergoryEnum.Splats)
              break;

            var material = GetComponent<MudMaterial>();
            if (material == null)
              break;
            
            shouldDrawOutlines = 
              Renderer.SplatSize * material.SplatSize < 0.1f 
              || material.Color.a < 0.25f;
            break;
          }

          case SdfBrush.OperatorEnum.Subtract:
          case SdfBrush.OperatorEnum.Intersect:
          case SdfBrush.OperatorEnum.Dye:
          case SdfBrush.OperatorEnum.NoOp:
          {
            shouldDrawOutlines = true;
            break;
          }
        }
      }

      if (shouldDrawOutlines)
      {
        Color prevColor = Gizmos.color;
        if (selected)
          Gizmos.color = GizmosUtil.OutlineSelected;

        DrawOutlineGizmosRs();

        Gizmos.color = prevColor;
      }
    }
  }
}

