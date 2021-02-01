/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEditor;
using UnityEngine;

namespace MudBun
{
  [CustomEditor(typeof(MudBrush), true)]
  [CanEditMultipleObjects]
  public class MudBrushEditor : MudEditorBase
  {
    public override void OnInspectorGUI()
    {
      Header("Quick Selection");

      var brush = (MudBrush) target;
      var renderer = brush.Renderer;

      if (GUILayout.Button("Select Renderer"))
      {
        if (renderer != null)
        {
          Selection.activeGameObject = renderer.gameObject;
          return;
        }
      }

      {
        var t = brush.transform.parent;
        while (t != null)
        {
          var group = t.GetComponent<MudBrushGroup>();
          if (group != null)
          {
            if (GUILayout.Button("Select Brush Group"))
            {
              Selection.activeGameObject = group.gameObject;
              return;
            }
          }

          t = t.parent;
        }
      }

      Space();

      Header("Brush Parameters");

      base.OnInspectorGUI();
    }

    public Aabb GetChildBounds(Transform t)
    {
      Aabb bounds = Aabb.Empty;
      GetChildBounds(t, ref bounds);
      return bounds;
    }

    private void GetChildBounds(Transform t, ref Aabb bounds)
    {
      if (t == null)
        return;

      var renderer = t.GetComponent<MudRenderer>();
      if (renderer != null)
        return;

      var brush = t.GetComponent<MudBrush>();
      if (brush != null)
        bounds.Include(brush.Bounds);

      for (int i = 0; i < t.childCount; ++i)
        GetChildBounds(t.GetChild(i), ref bounds);
    }

    public bool HasFrameBounds()
    {
      var brush = (MudBrush) target;

      if (brush is MudBrushGroup)
      {
        Aabb bounds = GetChildBounds(brush.transform);
        if (bounds.IsEmpty)
          return false;
      }
        
      return true;
    }

    public Bounds OnGetFrameBounds()
    {
      var brush = (MudBrush) target;
      var renderer = brush.Renderer;
      if (renderer == null)
        return new Bounds(brush.transform.position, Vector3.zero);

      var bounds = brush.Bounds;

      if (brush is MudBrushGroup)
      {
        bounds = GetChildBounds(brush.transform);
        if (bounds.IsEmpty)
          return new Bounds(brush.transform.position, Vector3.zero);
      }

      bounds.Expand(renderer.SurfaceShift);
      bounds.Transform(renderer.transform);

      return new Bounds(bounds.Center, bounds.Size);
    }
  }
}

