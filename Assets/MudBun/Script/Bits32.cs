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
using System.Runtime.InteropServices;

using UnityEngine;

namespace MudBun
{
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  [Serializable]
  public struct Bits32
  {
    [SerializeField] private uint m_bits;
    public uint RawValue { get => m_bits; set => m_bits = value; }

    public Bits32(uint bits = 0) { m_bits = bits; }

    public void SetAllBits() { m_bits = ~0u; }
    public void ClearAllBits() { m_bits = 0; }

    public void AssignBit(int index, bool value)
    {
      if (value)
        m_bits |= (1u << index);
      else
        m_bits &= ~(1u << index);
    }

    public void AssignBit(Enum index, bool value)
    {
      AssignBit(Convert.ToInt32(index), value);
    }

    public void SetBit(int index)
    {
      AssignBit(index, true);
    }

    public void SetBit(Enum index)
    {
      AssignBit(index, true);
    }

    public void ClearBit(int index)
    {
      AssignBit(index, false);
    }

    public void ClearBit(Enum index)
    {
      AssignBit(index, false);
    }

    public bool IsBitSet(int index)
    {
      return (m_bits & (1u << index)) != 0u;
    }

    public bool IsBitSet(Enum index)
    {
      return IsBitSet(Convert.ToInt32(index));
    }
  }
}
