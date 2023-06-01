using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuscandoLaFormaBarataDeV2Magnitude : MonoBehaviour {

    public Vector2 v;
    public float vDiff;
    public bool autoMult;
    public float multmult;
    [Range (0,1.5f)]public float finalMult; // 0.70710675 <--- si son iguales x e y , este mult nos da la magnitud exacta
    [Header("---")]
    public float real;
    public float cheap;
    [Header("---")]
    public float diff;


    
    void OnValidate(){
//        print("Diff= " + (Mathf.Abs(v.x) - Mathf.Abs(v.y)));
        Vector2 absV = new Vector2 (Mathf.Abs(v.x), Mathf.Abs(v.y));
        float big = Mathf.Max(absV.x, absV.y);
        vDiff = Mathf.Abs(absV.x - absV.y);
        vDiff = vDiff / big;
        if (autoMult)
            finalMult = 0.70710675f - (vDiff * multmult);
        real = v.magnitude;
        cheap = (absV.x + absV.y) * finalMult;
        diff = real - cheap;
    }

}
