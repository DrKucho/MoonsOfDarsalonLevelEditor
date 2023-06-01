using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Configuration;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ParallaxFather : MonoBehaviour
{

    public Vector2 firstElementSpeed = new Vector2(0.3f, 0.1f);
    public Vector2 speedProportion = new Vector2(2,2);
    public Vector2 editorSpeedInc = new Vector2(0.02f,0.02f);
    public CopyPosition copyPositionScript;
    public ParallaxMeCentered[] elements;
    public bool useCopySunColor = true;
    public CopySunColor[] copySunColor; // otros scriupt lo usan, gifmaker
    public bool useDayNightProcessor = false;
    public DayNightCycleColorProcessor[] dayNightProcessor; // otros scriupt lo usan, gifmaker
    public bool useColorManager;
    public ParallaxColorManager colorManager;
    static public ParallaxFather instance;
    public static Vector2 globalSpeedMult = new Vector2(1,1);

    #if UNITY_EDITOR
    
    public void IncreaseYSpeedOnChildren()
    {
        elements[elements.Length-1].speed.y += editorSpeedInc.y; // el ultimo es el mas cercano
        SetProportionalSpeedOnEnements();
    }
    
    public void ReduceYSpeedOnChildren()
    {
        elements[elements.Length-1].speed.y -= editorSpeedInc.y;
        SetProportionalSpeedOnEnements();
    }

    public void SetProportionalSpeedOnEnements()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        int lastIndex = elements.Length - 1;
        float closerY = elements[lastIndex].speed.y;
        for (int i = 0; i < lastIndex; i++)
        {
            float inverseIndex = lastIndex - i;
            elements[i].speed.y = closerY / (speedProportion.y * inverseIndex);
        }
    }

    public void InitialiseInEditor(){
        elements = GetComponentsInChildren<ParallaxMeCentered>();
        copySunColor = GetComponentsInChildren<CopySunColor>();
        dayNightProcessor = GetComponentsInChildren<DayNightCycleColorProcessor>();
        colorManager = GetComponentInChildren<ParallaxColorManager>();
    }
    #endif

    int initFrame = -1;
    public Vector3 initPosition;

}
