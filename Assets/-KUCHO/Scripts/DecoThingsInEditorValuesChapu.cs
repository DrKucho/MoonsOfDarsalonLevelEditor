// script chapuza para no tener que rehacer los arboles dentro de los Gameobjects DecothingsInEditor ya que tienen una referencia aprefab , aunque el vinculo con ellos esta roto algo pasa que me cuando modifico sus valores y guardo escena, al cargar de nuevo vuelven a apaarecer los valres antiguao
// creo que sealed solucionaria creando Gos de nuevo sin relacion con el prefab , pero eso me lleva a tener que plantar los arboles de nuevo, para futuros niveles ok , pero para los que estan hago esta chapu para reescribir valores

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoThingsInEditorValuesChapu : MonoBehaviour {

    public DecoThingsCreatedInEditor[] decoThingsScripts;
    public void InitialiseInEditor(){
        decoThingsScripts = GetComponentsInChildren<DecoThingsCreatedInEditor>();
    }
    void Awake(){
        foreach (DecoThingsCreatedInEditor o in decoThingsScripts)
        {
            if (o.name.Contains("PinkTree"))
            {
                o.outOfScreen.enabled = true;
                o.outOfScreen.checkAllAtOnce = true;
                o.outOfScreen.margin = new Vector2(100, 100);
                o.outOfScreen.offset = new Vector2(0, -40);
            }
            else if (o.name.Contains("RootTree"))
            {
                o.outOfScreen.enabled = true;
                o.outOfScreen.checkAllAtOnce = true;
                o.outOfScreen.margin = new Vector2(40, 40);
                o.outOfScreen.offset = new Vector2(0, -10);
            }
            o.InitialiseInEditor();
        }
    }
}
