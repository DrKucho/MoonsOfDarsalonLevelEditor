using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetGameViewSize : MonoBehaviour {

    public Vector2Int[] validSizes;

    #if UNITY_EDITOR

    void OnValidate(){
        if (Application.isEditor && !BuildPipeline.isBuildingPlayer)
        {

            if (!Application.isPlaying && enabled)
            {
                DoIt();
            }
            else if (Application.isPlaying && isActiveAndEnabled) // isactiveandenabled no funciona al cargar nivel desde editor, da false 
            {
                DoIt();
            }
        }
    }

    void DoIt()
    {
        Vector2Int cs = GameViewUtils.GetCurrentSize();
        foreach (Vector2Int s in validSizes)
        {
            if (s.x == cs.x && s.y == cs.y)// current size es un valor valido
            {
                return; // no hagas nada
            }
        }
        //la ventana gameview no tiene un valor valido fijo el primero de la tabla
        GameViewUtils.SetSize(validSizes[0], true);
    }
#endif
}
