/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

namespace MudBun
{
  public static class MudBun
  {
    public static readonly int MajorVersion = 0;
    public static readonly int MinorVersion = 9;
    public static readonly int Revision = 17;
    #if MUDBUN_FREE
    public static readonly string Suffix = "f";
    public static readonly bool IsFreeVersion = true;
    #else
    public static readonly string Suffix = "b";
    public static readonly bool IsFreeVersion = false;
    #endif
    public static string Version =>
        MajorVersion + "." 
      + MinorVersion + "." 
      + Revision  
      + Suffix 
      + (IsFreeVersion ? " (Trial)" : "");
  }
}
