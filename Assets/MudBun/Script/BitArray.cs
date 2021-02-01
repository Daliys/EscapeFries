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
  public class BitArray
  {
    private static readonly int BitsPerInt = 8 * sizeof(int);
    private int [] m_aInt;

    public static int ComputeNumInts(int numBits)
    {
      return (numBits + BitsPerInt - 1) / BitsPerInt;
    }

    public BitArray(int capacity = 512)
    {
      m_aInt = new int[ComputeNumInts(capacity)];
      ClearAll();
    }

    public void SetAll()
    {
      for (int i = 0; i < m_aInt.Length; ++i)
        m_aInt[i] = ~0;
    }

    public void ClearAll()
    {
      for (int i = 0; i < m_aInt.Length; ++i)
        m_aInt[i] = 0;
    }

    public void Set(int bit)
    {
      int iInt = bit / BitsPerInt;
      int offset = bit % BitsPerInt;
      m_aInt[iInt] |= (1 << offset);
    }

    public void Clear(int bit)
    {
      int iInt = bit / BitsPerInt;
      int offset = bit % BitsPerInt;
      m_aInt[iInt] &= ~(1 << offset);
    }

    public bool Get(int bit)
    {
      int iInt = bit / BitsPerInt;
      int offset = bit % BitsPerInt;
      return (m_aInt[iInt] & (1 << offset)) != 0;
    }

    public int Capacity
    {
      get => m_aInt.Length * BitsPerInt;
      set
      {
        var aOldInt = m_aInt;
        var aNewInt = new int[ComputeNumInts(value)];
        int n = Mathf.Min(aOldInt.Length, aNewInt.Length);

        for (int i = 0; i < n; ++i)
          aNewInt[i] = aOldInt[i];

        for (int i = n; n < aNewInt.Length; ++i)
          aNewInt[i] = 0;
      }
    }

    public override string ToString()
    {
      var str = "";
      for (int i = m_aInt.Length - 1; i >= 0; --i)
      {
        for (int j = BitsPerInt - 1; j >= 0; --j)
          str += (m_aInt[i] & (1 << j)) != 0 ? "1" : "0";

        if (i > 0)
          str += ", ";
      }

      return str;
    }
  }
}

