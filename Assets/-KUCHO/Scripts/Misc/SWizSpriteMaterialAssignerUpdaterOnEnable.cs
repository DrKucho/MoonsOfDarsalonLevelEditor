// para no alñadir un OnEnable a cada SWizSpriteMaterialAssigner con el consiguiente overhead de unity cada vez que se activara uno de ellos y tuviera que llamar a su onenable 

using UnityEngine;
using System.Collections;


public class SWizSpriteMaterialAssignerUpdaterOnEnable : MonoBehaviour {

    public SWizSpriteMaterialAssigner myMatAssigner;

    private void OnValidate()
    {
        if (!myMatAssigner)
            myMatAssigner = GetComponent<SWizSpriteMaterialAssigner>();

    }
    private void OnEnable()
    {
        myMatAssigner.AssignMaterial(null);
    }
}
