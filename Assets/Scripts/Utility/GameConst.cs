using UnityEngine;

public class GameConst
{
    public const string GameName = "战棋：帕弥什";

    public const float Map_GridWith = 2.56f;
    public const float Map_GridOffsetY = 1.92f;
    public const float Map_HexRadius = 1.478f;

    public const int Infinity = 999999;
    
    public static WaitForSeconds WaitForDotOneSecond = new WaitForSeconds(0.1f);
    public static WaitForSeconds WaitForOneSecond = new WaitForSeconds(1f);
    
    //每一层的间隔
    public const int OrderGapPerRow = 10;


    public const string STR_Unused = "UNUSED";

    public const string STR_GRID = "Grid";
    //SortingLayer
    public static readonly int SortingLayer_Battle_Map = SortingLayer.NameToID("Battle_Map");
    public static readonly int SortingLayer_View_Background = SortingLayer.NameToID("View_Background");
    public static readonly int SortingLayer_View_Base = SortingLayer.NameToID("View_Base");
    public static readonly int SortingLayer_View_HUD = SortingLayer.NameToID("View_HUD");
    public static readonly int SortingLayer_View_Popup = SortingLayer.NameToID("View_Popup");
    public static readonly int SortingLayer_View_Top = SortingLayer.NameToID("View_Top");
    public static readonly int SortingLayer_View_Debug = SortingLayer.NameToID("View_Debug");
}
