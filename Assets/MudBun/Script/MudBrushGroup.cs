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
  public class MudBrushGroup : MudSolid
  {
    public enum TypeEnum
    {
      BeginGroup = -2, 
      EndGroup = -3, 
    }

    public override bool IsBrushGroup => true;

    public MudBrushGroup()
    {
      m_countAsBone = false;
      m_canCountAsBone = false;
    }

    private int m_iBegin;
    private int m_iEnd;
    internal int m_iProxyBegin = AabbTree<MudBrushBase>.Null;
    internal int m_iProxyEnd = AabbTree<MudBrushBase>.Null;

    public override void UpdateProxies(AabbTree<MudBrushBase> tree, Aabb opBounds)
    {
      if (m_iProxyBegin == AabbTree<MudBrushBase>.Null)
        m_iProxyBegin = tree.CreateProxy(opBounds, this);

      if (m_iProxyEnd == AabbTree<MudBrushBase>.Null)
        m_iProxyEnd = tree.CreateProxy(opBounds, this);

      tree.UpdateProxy(m_iProxyBegin, opBounds, m_iBegin);
      tree.UpdateProxy(m_iProxyEnd, opBounds, m_iEnd);
    }

    public override void DestroyProxies(AabbTree<MudBrushBase> tree)
    {
      tree.DestroyProxy(m_iProxyBegin);
      tree.DestroyProxy(m_iProxyEnd);

      m_iProxyBegin = AabbTree<MudBrushBase>.Null;
      m_iProxyEnd = AabbTree<MudBrushBase>.Null;
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) TypeEnum.BeginGroup;
      aBrush[iStart] = brush;
      m_iBegin = iStart;
      return 1;
    }

    public override void FillBrushData(ref SdfBrush brush, int iBrush)
    {
      base.FillBrushData(ref brush, iBrush);

      brush.Proxy = m_iProxyBegin;
    }

    public override int FillComputeDataPostChildren(SdfBrush [] aBrush, int iStart)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) TypeEnum.EndGroup;
      aBrush[iStart] = brush;
      m_iEnd = iStart;
      return 1;
    }

    public override void FillBrushDataPostChildren(ref SdfBrush brush, int iBrush)
    {
      brush.Proxy = m_iProxyEnd;
      brush.Flags.AssignBit(SdfBrush.FlagBit.Hidden, Hidden);

      brush.Operator = (int) Operator;
      brush.Blend = Blend;

      brush.Flags.AssignBit(SdfBrush.FlagBit.ContributeMaterial, m_material.ContributeMaterial);
      brush.Flags.AssignBit(SdfBrush.FlagBit.CountAsBone, CountAsBone);
    }
  }
}

