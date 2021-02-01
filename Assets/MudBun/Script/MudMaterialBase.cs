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
  [ExecuteInEditMode]
  public abstract class MudMaterialBase : MonoBehaviour
  {
    [SerializeField] private MudSharedMaterialBase m_sharedMaterial;
    public MudSharedMaterialBase SharedMaterial { get => m_sharedMaterial; set { m_sharedMaterial = value; MarkDirty(); } }

    [SerializeField] [ConditionalField("m_sharedMaterial", "NULL")] private Color m_color = Color.white;
    [SerializeField] [ConditionalField("m_sharedMaterial", "NULL")] private Color m_emission = Color.black;
    [SerializeField] [ConditionalField("m_sharedMaterial", "NULL", Min = 0.0f, Max = 1.0f)] private float m_metallic = 0.0f;
    [SerializeField] [ConditionalField("m_sharedMaterial", "NULL", Min = 0.0f, Max = 1.0f)] private float m_smoothness = 0.5f;
    [SerializeField] [ConditionalField("m_sharedMaterial", "NULL", Min = 0,    Max = 3   )] private int m_textureIndex = 0;
    [SerializeField] [ConditionalField("m_sharedMaterial", "NULL", Min = 0.0f, Max = 5.0f)] private float m_splatSize = 1.0f;
    [SerializeField] [ConditionalField("m_sharedMaterial", "NULL", Min = 0.0f, Max = 1.0f)] private float m_blendTightness = 0.0f;
    [SerializeField] private bool m_contributeMaterial = true;
    public Color Color { get => m_sharedMaterial != null ? m_sharedMaterial.Color : m_color; set { m_color = value; MarkDirty(); } }
    public Color Emission { get => m_sharedMaterial != null ? m_sharedMaterial.Emission : m_emission; set { m_emission = value; MarkDirty(); } }
    public float Metallic { get => m_sharedMaterial != null ? m_sharedMaterial.Metallic : m_metallic; set { m_metallic = value; MarkDirty(); } }
    public float Smoothness { get => m_sharedMaterial != null ? m_sharedMaterial.Smoothness : m_smoothness; set { m_smoothness = value; MarkDirty(); } }
    public int TextureIndex { get => m_sharedMaterial != null ? m_sharedMaterial.TextureIndex : m_textureIndex; set { m_textureIndex = value; MarkDirty(); } }
    public float SplatSize { get => m_sharedMaterial != null ? m_sharedMaterial.SplatSize : m_splatSize; set { m_splatSize = value; MarkDirty(); } }
    public float BlendTightness { get => m_sharedMaterial != null ? m_sharedMaterial.BlendTightness : m_blendTightness; set { m_blendTightness = value; MarkDirty(); } }
    public bool ContributeMaterial { get => m_contributeMaterial; set { m_contributeMaterial = value; MarkDirty(); } }

    private int m_materialHash = -1;
    public int MaterialHash => (m_sharedMaterial != null ? m_sharedMaterial.MaterialHash : m_materialHash);

    private void OnEnable()
    {
      MarkDirty();
    }

    private void OnValidate()
    {
      Validate.Saturate(ref m_metallic);
      Validate.Saturate(ref m_smoothness);
      Validate.Range(0, 3, ref m_textureIndex);
      Validate.Saturate(ref m_blendTightness);

      MarkDirty();
    }

    public void MarkDirty()
    {
      m_materialHash = Codec.Hash(this);
      GetComponent<MudBrushBase>()?.MarkDirty();
    }
  }
}

