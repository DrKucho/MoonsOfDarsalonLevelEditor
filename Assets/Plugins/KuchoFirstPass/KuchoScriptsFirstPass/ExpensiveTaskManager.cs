using UnityEngine;
using System.Collections.Generic;


public enum ExpTaskType { RadialSearch, ThinAreaCheck, RebuildColliders1, RebuildColliders2,TextureApply, DestroyPixels, Undefined, } //, FixtureInitialise, FixtureDelete }

[System.Serializable]
public class ExpensiveTask
{
    public ExpTaskType task;
    public ThinAreaCheck thinAreaCheck;
    public RadialSearch radialSearch;

}

[System.Serializable]
public class ThinAreaCheck
{
    public int xMin;
    public int xMax;
    public int yMin;
    public int yMax;
    public float thinCheckOffset;
    public float thinCheckDistance;
    public float thinCheckInc;
    public float thinCheckStampOffset;
    public MoveAndExplodeDelegate moveAndExplode;
    
}

public struct CleanSpot
{
    public Vector2 point;
    public float radius;
    
}

public struct CleanSpotLoopArray
{
    public CleanSpot[] spots;
    private int ind;
    int arrayLength; // para usar temporalmente en los calculos, podria causar problemas si se accede desde varios threads ... ? 
    public bool ready; // se activa cuando se rellena la tabla completa
    
}
[System.Serializable]
public class ExpensiveTaskReport
{
    public int frame;
    public double ms;
    public bool rebuildCollidersBreak = false;

    public List<ExpTaskType> type;

}

[System.Serializable]
public class SimpleDelegateWrap{
    public Delegates.Simple task;
    public bool full;

}
[System.Serializable]
public class SimpleDelegateFIFO{
    public SimpleDelegateWrap[] elements;
public int fill = 0;
public int get = 0;
public int count = 0;
    [HideInInspector] SimpleDelegateWrap e; // variable temporal de trabajo

}
public class ExpensiveTaskManager : MonoBehaviour
{
    
}
public delegate void MoveAndExplodeDelegate(Vector2 point);

