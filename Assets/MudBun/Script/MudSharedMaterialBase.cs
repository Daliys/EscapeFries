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
  public class MudSharedMaterialBase : ScriptableObject
  {
    public delegate void SharedMaterialChanged(Object material);
    public static event SharedMaterialChanged OnSharedMaterialChanged;

    public Color Color = Color.white;
    public Color Emission = Color.black;
    [Range(0.0f, 1.0f)] public float Metallic = 0.0f;
    [Range(0.0f, 1.0f)] public float Smoothness = 0.5f;
    [Range(0, 3)] public int TextureIndex = 0;
    [Range(0.0f, 5.0f)] public float SplatSize = 1.0f;
    [Range(0.0f, 1.0f)] public float BlendTightness = 0.0f;

    [SerializeField] [HideInInspector] private int m_materialHash = -1;
    public int MaterialHash => m_materialHash;

    private void OnEnable()
    {
      MarkDirty();
    }

    private void OnValidate()
    {
      Validate.Saturate(ref Metallic);
      Validate.Saturate(ref Smoothness);
      Validate.Saturate(ref BlendTightness);

      MarkDirty();
    }

    private void MarkDirty()
    {
      m_materialHash = Codec.Hash(this);

      OnSharedMaterialChanged?.Invoke(this);
    }
  }
}