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
  public class ComputeManager
  {
    private static HashSet<ComputeShader> s_shaderSet = new HashSet<ComputeShader>();
    private static HashSet<ComputeShader> s_activeShaderSet = new HashSet<ComputeShader>();
    private static Dictionary<ComputeShader, List<int>> s_shaderKernelMap = new Dictionary<ComputeShader, List<int>>();
    private static Dictionary<int, HashSet<ComputeShader>> s_constantShaderMap = new Dictionary<int, HashSet<ComputeShader>>();

    public static void Reset()
    {
      s_shaderSet.Clear();
      s_activeShaderSet.Clear();
      s_shaderKernelMap.Clear();
      s_constantShaderMap.Clear();
    }

    public static void RegisterShader(ComputeShader shader)
    {
      s_shaderSet.Add(shader);
      s_shaderKernelMap.Add(shader, new List<int>());
    }

    public static int RegisterKernel(ComputeShader shader, string kernelName)
    {
      if (!s_shaderKernelMap.TryGetValue(shader, out var aKernel))
      {
        aKernel = new List<int>();
        s_shaderKernelMap.Add(shader, aKernel);
      }
      int kernel = shader.FindKernel(kernelName);
      Assert.True(kernel >= 0, $"Kernel {kernelName} not found in: {shader}");
      int iKernel = aKernel.IndexOf(kernel);
      Assert.True(iKernel < 0);
      aKernel.Add(kernel);

      return kernel;
    }

    public static void RegisterConstantId(ComputeShader shader, int id)
    {
      if (!s_constantShaderMap.TryGetValue(id, out var shaderSet))
      {
        shaderSet = new HashSet<ComputeShader>();
        s_constantShaderMap.Add(id, shaderSet);
      }
      shaderSet.Add(shader);
    }

    public static void ActivateAllShaders()
    {
      foreach (var shader in s_shaderSet)
        s_activeShaderSet.Add(shader);
    }

    public static void DeactivateAllShaders()
    {
      foreach (var shader in s_shaderSet)
        s_activeShaderSet.Remove(shader);
    }

    public static void Activate(ComputeShader shader)
    {
      s_activeShaderSet.Add(shader);
    }

    public static void Deactivate(ComputeShader shader)
    {
      s_activeShaderSet.Remove(shader);
    }

    public static void Dispatch(ComputeShader shader, int kernel, int x, int y, int z)
    {
      Assert.True(s_activeShaderSet.Contains(shader), $"Compute shader {shader} not active.");
      shader.Dispatch(kernel, x, y, z);
    }

    public static void DispatchIndirect(ComputeShader shader, int kernel, ComputeBuffer indirectArgs)
    {
      Assert.True(s_activeShaderSet.Contains(shader), $"Compute shader {shader} not active.");
      shader.DispatchIndirect(kernel, indirectArgs);
    }

    public static void SetBool(int id, bool value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
         shader.SetBool(id, value);
    }

    public static void SetInt(int id, int value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          shader.SetInt(id, value);
    }

    public static void SetInts(int id, int[] value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          shader.SetInts(id, value);
    }

    public static void SetFloat(int id, float value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          shader.SetFloat(id, value);
    }

    public static void SetFloats(int id, float[] value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          shader.SetFloats(id, value);
    }

    public static void SetVector(int id, Vector4 value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          shader.SetVector(id, value);
    }

    public static void SetMatrix(int id, Matrix4x4 value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          shader.SetMatrix(id, value);
    }

    public static void SetBuffer(int id, ComputeBuffer value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          foreach (var kernel in s_shaderKernelMap[shader])
            shader.SetBuffer(kernel, id, value);
    }

    public static void SetTexture(int id, Texture value)
    {
      foreach (var shader in s_activeShaderSet)
        if (s_constantShaderMap[id].Contains(shader))
          foreach (var kernel in s_shaderKernelMap[shader])
            shader.SetTexture(kernel, id, value);
    }
  }
}

