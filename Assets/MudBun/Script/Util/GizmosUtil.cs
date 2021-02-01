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
  public class GizmosUtil
  {
    public static readonly Color OutlineDefault = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public static readonly Color OutlineSelected = new Color(1.0f, 0.7f, 0.1f, 0.5f);
    public static readonly Color Transparent = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    public static void DrawLine(Vector3 a, Vector3 b)
    {
      Gizmos.DrawLine(a, b);
    }

    public static void DrawCircle(float radius, Vector3 center, Quaternion rotation)
    {
      int numSegments = 32;
      float t = 0.0f;
      float dt = MathUtil.TwoPi / numSegments;
      Vector3 prev = center + rotation * new Vector3(radius, 0.0f, 0.0f);
      for (int i = 0; i < numSegments; ++i)
      {
        t += dt;
        Vector3 curr = center + rotation * new Vector3(radius * Mathf.Cos(t), 0.0f, radius * Mathf.Sin(t));
        DrawLine(prev, curr);
        prev = curr;
      }
    }

    public static void DrawWireBox(Vector3 center, Vector3 size, Quaternion rotation)
    {
      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(center, rotation, size);
      Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
      Gizmos.matrix = prevMatrix;
    }

    public static void DrawInvisibleBox(Vector3 center, Vector3 size, Quaternion rotation)
    {
      Color prevColor = Gizmos.color;
      Gizmos.color = Transparent;
      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(center, rotation, size);
      Gizmos.DrawCube(Vector3.zero, Vector3.one);
      Gizmos.matrix = prevMatrix;
      Gizmos.color = prevColor;
    }

    public static void DrawWireSphere(Vector3 center, float radius, Vector3 scale, Quaternion rotation)
    {
      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(center, rotation, scale);
      Gizmos.DrawWireSphere(Vector3.zero, radius);
      Gizmos.matrix = prevMatrix;
    }

    public static void DrawInvisibleSphere(Vector3 center, float radius, Vector3 scale, Quaternion rotation)
    {
      Color prevColor = Gizmos.color;
      Gizmos.color = Transparent;
      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(center, rotation, scale);
      Gizmos.DrawSphere(Vector3.zero, radius);
      Gizmos.matrix = prevMatrix;
      Gizmos.color = prevColor;
    }

    public static void DrawWireCylinder(Vector3 center, float radius, float topRadiusOffset, float height, Quaternion rotation)
    {
      float topRadius = Mathf.Max(0.0f, radius + topRadiusOffset);
      Vector3 hh = new Vector3(0.0f, 0.5f * height, 0.0f);

      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
      DrawCircle(radius, -hh, Quaternion.identity);
      DrawCircle(topRadius, hh, Quaternion.identity);
      DrawLine(new Vector3(-radius, 0.0f, 0.0f) - hh, new Vector3(-topRadius, 0.0f, 0.0f) + hh);
      DrawLine(new Vector3(radius, 0.0f, 0.0f) - hh, new Vector3(topRadius, 0.0f, 0.0f) + hh);
      DrawLine(new Vector3(0.0f, 0.0f, -radius) - hh, new Vector3(0.0f, 0.0f, -topRadius) + hh);
      DrawLine(new Vector3(0.0f, 0.0f, radius) - hh, new Vector3(0.0f, 0.0f, topRadius) + hh);
      Gizmos.matrix = prevMatrix;
    }

    public static void DrawInvisibleCylinder(Vector3 center, float radius, float height, Quaternion rotation)
    {
      Color prevColor = Gizmos.color;
      Gizmos.color = Transparent;
      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(center, rotation, new Vector3(radius, height, radius));
      Gizmos.DrawMesh(CylinderMesh);
      Gizmos.matrix = prevMatrix;
      Gizmos.color = prevColor;
    }

    public static void DrawWireCone(Vector3 baseCenter, float radius, float height, Quaternion rotation)
    {
      Vector3 h = new Vector3(0.0f, height, 0.0f);

      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(baseCenter, rotation, Vector3.one);
      DrawCircle(radius, Vector3.zero, Quaternion.identity);
      DrawLine(h, new Vector3(-radius, 0.0f, 0.0f));
      DrawLine(h, new Vector3(radius, 0.0f, 0.0f));
      DrawLine(h, new Vector3(0.0f, 0.0f, -radius));
      DrawLine(h, new Vector3(0.0f, 0.0f, radius));
      Gizmos.matrix = prevMatrix;
    }

    public static void DrawInvisibleCone(Vector3 baseCenter, float radius, float height, Quaternion rotation)
    {
      Color prevColor = Gizmos.color;
      Gizmos.color = Transparent;
      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(baseCenter, rotation, new Vector3(radius, height, radius));
      Gizmos.DrawMesh(ConeMesh);
      Gizmos.matrix = prevMatrix;
      Gizmos.color = prevColor;
    }

    public static void DrawWireSolidAngle(Vector3 center, float radius, float angle, Quaternion rotation)
    {
      int numSegments = 32;
      float t = 0.0f;
      float dt = angle / numSegments;
      float s = Mathf.Sin(angle);
      float c = Mathf.Cos(angle);
      Vector3 h = new Vector3(0.0f, radius * c, 0.0f);

      Matrix4x4 prevMatrix = Gizmos.matrix;
      Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
      DrawCircle(radius * s, h, Quaternion.identity);
      float prevS = 0.0f;
      float prevC = 1.0f;
      for (int i = 0; i < numSegments; ++i)
      {
        t += dt;
        float currS = Mathf.Sin(t);
        float currC = Mathf.Cos(t);
        DrawLine(new Vector3(-radius * prevS, radius * prevC, 0.0f), new Vector3(-radius * currS, radius * currC, 0.0f));
        DrawLine(new Vector3(radius * prevS, radius * prevC, 0.0f), new Vector3(radius * currS, radius * currC, 0.0f));
        DrawLine(new Vector3(0.0f, radius * prevC, -radius * prevS), new Vector3(0.0f, radius * currC, -radius * currS));
        DrawLine(new Vector3(0.0f, radius * prevC, radius * prevS), new Vector3(0.0f, radius * currC, radius * currS));
        prevS = currS;
        prevC = currC;

      }
      if (angle < MathUtil.Pi - MathUtil.Epsilon)
      {
        DrawLine(Vector3.zero, new Vector3(-radius * prevS, radius * prevC, 0.0f));
        DrawLine(Vector3.zero, new Vector3(radius * prevS, radius * prevC, 0.0f));
        DrawLine(Vector3.zero, new Vector3(0.0f, radius * prevC, -radius * prevS));
        DrawLine(Vector3.zero, new Vector3(0.0f, radius * prevC, radius * prevS));
      }
      if (angle > MathUtil.HalfPi)
      {
        DrawCircle(radius, Vector3.zero, Quaternion.identity);
      }
      Gizmos.matrix = prevMatrix;
    }

    public static void DrawBezierQuad(Vector3 a, Vector3 b, Vector3 controlPoint)
    {
      int numSegments = 32;
      float t = 0;
      float dt = 1.0f / numSegments;
      Vector3 prev = a;
      for (int i = 0; i < numSegments; ++i)
      {
        t += dt;
        Vector3 curr = VectorUtil.BezierQuad(a, b, controlPoint, t);
        DrawLine(prev, curr);
        prev = curr;
      }
    }

    public static void DrawWireCatmullRom(Vector3 [] aPoint, float [] aRadius, Vector3 headControlPoint, Vector3 tailControlPoint)
    {
      for (int i = 0; i < aPoint.Length; ++i)
      {
        DrawWireSphere(aPoint[i], aRadius[i], Vector3.one, Quaternion.identity);
      }

      if (VectorUtil.IsValid(headControlPoint))
      {
        DrawWireSphere(headControlPoint, aRadius[0], Vector3.one, Quaternion.identity);
      }

      if (VectorUtil.IsValid(tailControlPoint))
      {
        DrawWireSphere(tailControlPoint, aRadius[aPoint.Length - 1], Vector3.one, Quaternion.identity);
      }

      if (aPoint.Length == 1)
        return;

      var head = aPoint[0];
      var postHead = aPoint[1];
      Vector3 preHeadPos = 
        VectorUtil.IsValid(headControlPoint) 
          ? headControlPoint 
          : 2.0f * head - postHead;

      var tail = aPoint[aPoint.Length - 1];
      var preTail = aPoint[aPoint.Length - 2];
      Vector3 postTailPos = 
        VectorUtil.IsValid(tailControlPoint) 
          ? tailControlPoint 
          : 2.0f * tail - preTail;

      int numSegments = 16;
      float t = 0.0f;
      float dt = 1.0f / numSegments;
      float prevT = 0.0f;
      for (int i = 0; i < numSegments; ++i)
      {
        t += dt;
        float currT = t;
        DrawLine
        (
          VectorUtil.CatmullRom(preHeadPos, aPoint[0], aPoint[1], aPoint.Length > 2 ? aPoint[2] : postTailPos, prevT), 
          VectorUtil.CatmullRom(preHeadPos, aPoint[0], aPoint[1], aPoint.Length > 2 ? aPoint[2] : postTailPos, currT)
        );
        if (aPoint.Length > 2)
        {
          DrawLine
          (
            VectorUtil.CatmullRom(aPoint.Length > 2 ? aPoint[aPoint.Length - 3] : preHeadPos, aPoint[aPoint.Length - 2], aPoint[aPoint.Length - 1], postTailPos, prevT), 
            VectorUtil.CatmullRom(aPoint.Length > 2 ? aPoint[aPoint.Length - 3] : preHeadPos, aPoint[aPoint.Length - 2], aPoint[aPoint.Length - 1], postTailPos, currT)
          );
        }
        for (int j = 0; j < aPoint.Length - 3; ++j)
        {
          DrawLine
          (
            VectorUtil.CatmullRom(aPoint[j], aPoint[j + 1], aPoint[j + 2], aPoint[j + 3], prevT),
            VectorUtil.CatmullRom(aPoint[j], aPoint[j + 1], aPoint[j + 2], aPoint[j + 3], currT)
          );
        }
        prevT = currT;
      }
    }

    public static void DrawInvisibleCatmullRom(Vector3 [] aPoint, float [] aRadius, Vector3 headControlPoint, Vector3 tailControlPoint)
    {
      for (int i = 0; i < aPoint.Length; ++i)
      {
        DrawInvisibleSphere(aPoint[i], aRadius[i], Vector3.one, Quaternion.identity);
      }

      if (aPoint.Length == 1)
        return;

      var head = aPoint[0];
      var postHead = aPoint[1];
      Vector3 preHeadPos = 
        VectorUtil.IsValid(headControlPoint) 
          ? headControlPoint 
          : 2.0f * head - postHead;

      var tail = aPoint[aPoint.Length - 1];
      var preTail = aPoint[aPoint.Length - 2];
      Vector3 postTailPos = 
        VectorUtil.IsValid(tailControlPoint) 
          ? tailControlPoint 
          : 2.0f * tail - preTail;

      int numSegments = 4;
      float t = 0.0f;
      float dt = 1.0f / numSegments;
      float prevT = 0.0f;
      for (int i = 0; i < numSegments; ++i)
      {
        t += dt;
        float currT = t;
        DrawInvisibleSphere
        (
          VectorUtil.CatmullRom(preHeadPos, aPoint[0], aPoint[1], aPoint.Length > 2 ? aPoint[2] : postTailPos, prevT), 
          Mathf.Lerp(aRadius[0], aRadius[1], prevT), 
          Vector3.one, 
          Quaternion.identity
        );
        if (aPoint.Length > 2)
        {
          DrawInvisibleSphere
          (
            VectorUtil.CatmullRom(aPoint.Length > 2 ? aPoint[aPoint.Length - 3] : preHeadPos, aPoint[aPoint.Length - 2], aPoint[aPoint.Length - 1], postTailPos, prevT), 
            Mathf.Lerp(aRadius[aPoint.Length - 2], aRadius[aPoint.Length - 1], prevT), 
            Vector3.one, 
            Quaternion.identity
          );
        }
        prevT = currT;
      }
    }

    public static void DrawWireTorus(Vector3 center, float radius, float width, float depth, Quaternion rotation)
    {
      width = Mathf.Abs(width);
      depth = Mathf.Abs(depth);

      DrawCircle(radius, center + 0.5f * (width - radius) * (rotation * Vector3.right), rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f));
      DrawCircle(radius, center - 0.5f * (width - radius) * (rotation * Vector3.right), rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f));
      DrawCircle(radius, center + 0.5f * (depth - radius) * (rotation * Vector3.forward), rotation * Quaternion.Euler(0.0f, 0.0f, 90.0f));
      DrawCircle(radius, center - 0.5f * (depth - radius) * (rotation * Vector3.forward), rotation * Quaternion.Euler(0.0f, 0.0f, 90.0f));

      int numSegments = 16;
      float dt = MathUtil.Pi / numSegments;
      float dimDiff = Mathf.Abs(width - depth);
      float r = 0.5f * (Mathf.Min(width, depth) - radius);
      Vector3 axisA = 
        width > depth 
          ? rotation * Vector3.right 
          : rotation * Vector3.forward;
      Vector3 axisB = 
        width > depth 
          ? rotation * Vector3.forward 
          : rotation * Vector3.right;
      float a = (width > depth ? width : depth) - 2.0f * r - 0.5f * radius;

      {
        float t = 0.0f;
        Vector3 c = center + 0.5f * a * axisA;
        Vector3 prev = c + r * axisB;
        for (int i = 0; i < numSegments; ++i)
        {
          t += dt;
          Vector3 curr = c + (r * (Mathf.Sin(t) * axisA + Mathf.Cos(t) * axisB));
          DrawLine(prev, curr);
          prev = curr;
        }
      }

      {
        float t = Mathf.PI;
        Vector3 c = center - 0.5f * a * axisA;
        Vector3 prev = c - r * axisB;
        for (int i = 0; i < numSegments; ++i)
        {
          t += dt;
          Vector3 curr = c + (r * (Mathf.Sin(t) * axisA + Mathf.Cos(t) * axisB));
          DrawLine(prev, curr);
          prev = curr;
        }
      }

      DrawLine(center + 0.5f * a * axisA + r * axisB, center - 0.5f * a * axisA + r * axisB);
      DrawLine(center + 0.5f * a * axisA - r * axisB, center - 0.5f * a * axisA - r * axisB);
    }

    public static void DrawInvisibleTorus(Vector3 center, float radius, float width, float depth, Quaternion rotation)
    {
      width = Mathf.Abs(width);
      depth = Mathf.Abs(depth);

      Color prevColor = Gizmos.color;
      Gizmos.color = Transparent;

      /*
      DrawInvisibleSphere(center + 0.5f * (width - radius) * (rotation * Vector3.right), radius, Vector3.one, Quaternion.identity);
      DrawInvisibleSphere(center - 0.5f * (width - radius) * (rotation * Vector3.right), radius, Vector3.one, Quaternion.identity);
      DrawInvisibleSphere(center + 0.5f * (depth - radius) * (rotation * Vector3.forward), radius, Vector3.one, Quaternion.identity);
      DrawInvisibleSphere(center - 0.5f * (depth - radius) * (rotation * Vector3.forward), radius, Vector3.one, Quaternion.identity);
      */

      int numSegments = 6;
      float dt = MathUtil.Pi / numSegments;
      float dimDiff = Mathf.Abs(width - depth);
      float r = 0.5f * (Mathf.Min(width, depth) - radius);
      Vector3 axisA = 
        width > depth 
          ? rotation * Vector3.right 
          : rotation * Vector3.forward;
      Vector3 axisB = 
        width > depth 
          ? rotation * Vector3.forward 
          : rotation * Vector3.right;
      float a = (width > depth ? width : depth) - 2.0f * r - 0.5f * radius;

      {
        float t = 0.0f;
        Vector3 c = center + 0.5f * a * axisA;
        for (int i = 0; i < numSegments; ++i)
        {
          t += dt;
          Vector3 curr = c + (r * (Mathf.Sin(t) * axisA + Mathf.Cos(t) * axisB));
          DrawInvisibleSphere(curr, radius, Vector3.one, Quaternion.identity);
        }
      }

      {
        float t = Mathf.PI;
        Vector3 c = center - 0.5f * a * axisA;
        for (int i = 0; i < numSegments; ++i)
        {
          t += dt;
          Vector3 curr = c + (r * (Mathf.Sin(t) * axisA + Mathf.Cos(t) * axisB));
          DrawInvisibleSphere(curr, radius, Vector3.one, Quaternion.identity);
        }
      }

      {
        numSegments = 4;
        float t = 0.0f;
        dt = 1.0f / numSegments;
        for (int i = 0; i <= numSegments; ++i)
        {
          DrawInvisibleSphere(Vector3.Lerp(center + 0.5f * a * axisA + (r * axisB), center - 0.5f * a * axisA + (r * axisB), t), radius, Vector3.one, Quaternion.identity);
          DrawInvisibleSphere(Vector3.Lerp(center + 0.5f * a * axisA - (r * axisB), center - 0.5f * a * axisA - (r * axisB), t), radius, Vector3.one, Quaternion.identity);
          t += dt;
        }
      }

      Gizmos.color = prevColor;
    }

    private static Mesh s_cylinderMesh;
    private static Mesh CylinderMesh
    {
      get
      {
        if (s_cylinderMesh != null)
          return s_cylinderMesh;

        int numSegments = 16;
        s_cylinderMesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 6 + 2];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numSegments * 12];

        Vector3 bottom = new Vector3(0.0f, -0.5f, 0.0f);
        Vector3 top = new Vector3(0.0f, 0.5f, 0.0f);

        int iBottomCapStart = 0;
        int iTopCapStart = numSegments;
        int iSideStart = numSegments * 2;
        int iBottom = numSegments * 6;
        int iTop = numSegments * 6 + 1;

        aVert[iBottom] = bottom;
        aVert[iTop] = top;

        aNormal[iBottom] = new Vector3(0.0f, -1.0f, 0.0f);
        aNormal[iTop] = new Vector3(0.0f, 1.0f, 0.0f);

        int iIndex = 0;
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          // caps
          Vector3 offset = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aVert[iBottomCapStart + i] = bottom + offset;
          aVert[iTopCapStart + i] = top + offset;

          aNormal[iBottomCapStart + i] = new Vector3(0.0f, -1.0f, 0.0f);
          aNormal[iTopCapStart + i] = new Vector3(0.0f, 1.0f, 0.0f);

          aIndex[iIndex++] = iBottom;
          aIndex[iIndex++] = iBottomCapStart + i;
          aIndex[iIndex++] = iBottomCapStart + ((i + 1) % numSegments);

          aIndex[iIndex++] = iTop;
          aIndex[iIndex++] = iTopCapStart + ((i + 1) % numSegments);
          aIndex[iIndex++] = iTopCapStart + i;

          angle += angleIncrement;

          // sides
          Vector3 offsetNext = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          aVert[iSideStart + i * 4] = bottom + offset;
          aVert[iSideStart + i * 4 + 1] = top + offset;
          aVert[iSideStart + i * 4 + 2] = bottom + offsetNext;
          aVert[iSideStart + i * 4 + 3] = top + offsetNext;

          Vector3 sideNormal = Vector3.Cross(top - bottom, offsetNext - offset).normalized;
          aNormal[iSideStart + i * 4] = sideNormal;
          aNormal[iSideStart + i * 4 + 1] = sideNormal;
          aNormal[iSideStart + i * 4 + 2] = sideNormal;
          aNormal[iSideStart + i * 4 + 3] = sideNormal;

          aIndex[iIndex++] = iSideStart + i * 4;
          aIndex[iIndex++] = iSideStart + i * 4 + 3;
          aIndex[iIndex++] = iSideStart + i * 4 + 2;

          aIndex[iIndex++] = iSideStart + i * 4;
          aIndex[iIndex++] = iSideStart + i * 4 + 1;
          aIndex[iIndex++] = iSideStart + i * 4 + 3;
        }

        s_cylinderMesh.vertices = aVert;
        s_cylinderMesh.normals = aNormal;
        s_cylinderMesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        return s_cylinderMesh;
      }
    }

    private static Mesh s_coneMesh;
    private static Mesh ConeMesh
    {
      get
      {
        if (s_coneMesh != null)
          return s_coneMesh;

        int numSegments = 16;
        s_coneMesh = new Mesh();

        Vector3[] aVert = new Vector3[numSegments * 3 + numSegments];
        Vector3[] aNormal = new Vector3[aVert.Length];
        int[] aIndex = new int[numSegments * 3 + (numSegments - 2) * 3];

        Vector3 top = new Vector3(0.0f, 1.0f, 0.0f);

        Vector3[] aBaseVert = new Vector3[numSegments];
        float angleIncrement = 2.0f * Mathf.PI / numSegments;
        float angle = 0.0f;
        for (int i = 0; i < numSegments; ++i)
        {
          aBaseVert[i] = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.forward;
          angle += angleIncrement;
        }

        int iVert = 0;
        int iIndex = 0;
        int iNormal = 0;
        for (int i = 0; i < numSegments; ++i)
        {
          int iSideTriStart = iVert;

          aVert[iVert++] = top;
          aVert[iVert++] = aBaseVert[i];
          aVert[iVert++] = aBaseVert[(i + 1) % numSegments];

          Vector3 sideTriNormal = Vector3.Cross(aVert[iSideTriStart + 2] - aVert[iSideTriStart], aVert[iSideTriStart + 1] - aVert[iSideTriStart]).normalized;
          aNormal[iNormal++] = sideTriNormal;
          aNormal[iNormal++] = sideTriNormal;
          aNormal[iNormal++] = sideTriNormal;

          aIndex[iIndex++] = iSideTriStart;
          aIndex[iIndex++] = iSideTriStart + 2;
          aIndex[iIndex++] = iSideTriStart + 1;
        }

        int iBaseStart = iVert;
        for (int i = 0; i < numSegments; ++i)
        {
          aVert[iVert++] = aBaseVert[i];

          aNormal[iNormal++] = new Vector3(0.0f, -1.0f, 0.0f);

          if (i >= 2)
          {
            aIndex[iIndex++] = iBaseStart;
            aIndex[iIndex++] = iBaseStart + i - 1;
            aIndex[iIndex++] = iBaseStart + i;
          }
        }

        s_coneMesh.vertices = aVert;
        s_coneMesh.normals = aNormal;
        s_coneMesh.SetIndices(aIndex, MeshTopology.Triangles, 0);

        return s_coneMesh;
      }
    }
  }
}

