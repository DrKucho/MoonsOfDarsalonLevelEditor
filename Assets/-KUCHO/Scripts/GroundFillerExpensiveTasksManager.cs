using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

[System.Serializable]
public class GroundFillerTaskFIFO
{
    public GroundFiller[] pendingFillers;
    public int fill = 0;
    public int get = 0;
    public int count = 0;

    public class GroundFillerExpensiveTasksManager : MonoBehaviour
    {

        public int howManyPerFrame = 2;
        public GroundFillerTaskFIFO pendingFillersFIFO;
        ExpensiveTaskManager expensiveTaskManager;
        [HideInInspector] public static GroundFillerExpensiveTasksManager instance;
        [HideInInspector] public static GroundFillerTaskFIFO staticPendingFillersFIFO;

    }
}