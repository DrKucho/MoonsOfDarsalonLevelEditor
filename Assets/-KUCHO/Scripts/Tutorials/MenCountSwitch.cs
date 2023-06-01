using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public struct ManInfo { 
    public Collider2D trigCol;
    public CC cC;
    
    public ManInfo (Collider2D _trigCol, CC _cC)
    {
        trigCol = _trigCol;
        cC = _cC;
    }
}
public class MenCountSwitch : MonoBehaviour
{

    public int minMenCount = 1;
    public Collider2D myCol;
    public SWizTextMesh display; // NO HAGAS QUE AUTOPILLE EL PRIMER SWizTextMesh DE LOS HIJOS, Me jode el tuto de voz del nivel 2 y seguramente otras cosas feas
    public bool getCCs;
    public Light2D.LightSprite displayLight; 
    public Color notEnough;
    public Color enough;
    public EnableOrDisable onEnter;
    public EnableOrDisable onExit = EnableOrDisable.Disable_Close;
    public bool disableOnOpen;
    public int deadMenThreshold;
    public EnableOrDisable onDeadMenThreshold;
    public string[] tags;
    public float switchDelay = 0.1f; // si desde mi evento ontrigger exit desactivo otros colliders o gameobjects con colliders, sus eventos ontriggerexit no saltan, creo que por que el motor de fisicas no se entera al hacerlo desde un evento ontrigger exit
    public GameObject[] swtichGO;
    public Collider2D[] swtichCol;
    public Door[] switchMove;
    public MenCountSwitch[] switchMenCount;
    [ReadOnly2Attribute] public List<ManInfo> men = new List<ManInfo>();
    bool GotDisplay() { return display; }

    int deadCount = 0;

}
