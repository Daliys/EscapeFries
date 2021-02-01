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
  //[CreateAssetMenu(fileName = "MudBun Texture Packer", menuName = "MudBun/Texture Packer", order = 151)]
  public class MudTexturePacker : ScriptableObject
  {
    public enum TextureType
    {
      White, 
      Gray, 
      Black, 
      FlatNormal, 
      Texutre, 
    }

    public TextureType ColorType;
    public Texture2D Color;

    public TextureType EmissionType;
    public Texture2D Emission;

    public TextureType NormalType;
    public Texture2D Normal;

    public TextureType MetallicType;
    public Texture2D Metallic;

    public TextureType SmoothnessType;
    public Texture2D Smoothness;

    private Color [] GetPixels(TextureType type, int numPixels)
    {
      switch (type)
      {
        case TextureType.White:
          Color white = new Color(255, 255, 255, 255);
          Color[] aWhite = new Color[numPixels];
          for (int i = 0; i < numPixels; ++i)
          {
            aWhite[i] = white;
          }
          return aWhite;

        case TextureType.Gray:
          Color gray = new Color(128, 128, 128, 255);
          Color[] aGray = new Color[numPixels];
          for (int i = 0; i < numPixels; ++i)
          {
            aGray[i] = gray;
          }
          return aGray;

        case TextureType.Black:
          Color black = new Color(0, 0, 0, 255);
          Color[] aBlack = new Color[numPixels];
          for (int i = 0; i < numPixels; ++i)
          {
            aBlack[i] = black;
          }
          return aBlack;

        case TextureType.FlatNormal:
          Color flatNormal = new Color(0, 0, 0, 255);
          Color[] aFlatNormal = new Color[numPixels];
          for (int i = 0; i < numPixels; ++i)
          {
            aFlatNormal[i] = flatNormal;
          }
          return aFlatNormal;
      }

      Assert.Warn("Invalid texture type: " + type);
      return null;
    }

    public void Pack(string path)
    {
      int width = -1;
      int height = -1;

      if (Color != null)
      {
        width = Color.width;
        height = Color.height;
      }

      if (Emission != null)
      {
        if ((width >= 0 && width != Emission.width) 
            || (height >= 0 && height != Emission.height))
        {
          Debug.LogError($"MudBun Texture Packer: Emission texture doesn't match other textures dimsions (need {width} x {height} but got {Emission.width} x {Emission.height}).");
          return;
        }

        width = Emission.width;
        height = Emission.height;
      }

      if (Normal != null)
      {
        if ((width >= 0 && width != Normal.width) 
            || (height >= 0 && height != Normal.height))
        {
          Debug.LogError($"MudBun Texture Packer: ENormal texture doesn't match other textures dimsions (need {width} x {height} but got {Normal.width} x {Normal.height}).");
          return;
        }

        width = Normal.width;
        height = Normal.height;
      }

      if (Metallic != null)
      {
        if ((width >= 0 && width != Metallic.width) 
            || (height >= 0 && height != Metallic.height))
        {
          Debug.LogError($"MudBun Texture Packer: EMetallic texture doesn't match other textures dimsions (need {width} x {height} but got {Metallic.width} x {Metallic.height}).");
          return;
        }

        width = Metallic.width;
        height = Metallic.height;
      }

      if (Smoothness != null)
      {
        if ((width >= 0 && width != Smoothness.width)
            || (height >= 0 && height != Smoothness.height))
        {
          Debug.LogError($"MudBun Texture Packer: ESmoothness texture doesn't match other textures dimsions (need {width} x {height} but got {Smoothness.width} x {Smoothness.height}).");
          return;
        }

        width = Smoothness.width;
        height = Smoothness.height;
      }

      if (width <= 0 || height <= 0)
      {
        Debug.LogError("MudBun Texture Packer: EAt least one non-empty texture needs to be specified.");
        return;
      }

      int numPixels = width * height;

      Color [] aColor = (Color != null) ? Color.GetPixels() : GetPixels(ColorType, numPixels);
      Color [] aEmission = (Emission != null) ? Emission.GetPixels() : GetPixels(EmissionType, numPixels);
      Color [] aNormal = (Normal != null) ? Normal.GetPixels() : GetPixels(NormalType, numPixels);
      Color [] aMetallic = (Metallic != null) ? Metallic.GetPixels() : GetPixels(MetallicType, numPixels);
      Color [] aSmoothness = (Smoothness != null) ? Metallic.GetPixels() : GetPixels(SmoothnessType, numPixels);
      for (int x = 0; x < width; ++x)
      {
        for (int y = 0; y < height; ++y)
        {
          // TODO: don't need this right now
        }
      }
    }
  }
}

