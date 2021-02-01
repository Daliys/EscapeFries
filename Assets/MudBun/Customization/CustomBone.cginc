/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#ifndef MUDBUN_CUSTOM_BONE
#define MUDBUN_CUSTOM_BONE

// apply bone weights for this brush
// keep track of the 4 closest brushes (sorted closest first)
// typically it should just call blend_bone_weight, 
// unless there's more than one bone for the brush (like a curve with multiple control points)
void apply_custom_brush_bone_weights
(
  float3 p,               // sample position in world space
  float3 pRel,            // sample position in brush space
  SdfBrush brush,         // brush data (see BrushDefs.cginc for data layout)
  float brushRes,         // signed distance from the brush's surface
  inout float4 boneRes,   // signed distances from the 4 closest brushes' surface
  inout int4 boneIndex,   // indices of the 4 closest brushes
  inout float4 boneWeight // bone weights of the 4 closest brushes
)
{
  switch (brush.type)
  {
    default:
    {
      blend_bone_weights(brushRes, brush.boneIndex, boneRes, boneIndex, boneWeight);
      break;
    }
  }
}

#endif

