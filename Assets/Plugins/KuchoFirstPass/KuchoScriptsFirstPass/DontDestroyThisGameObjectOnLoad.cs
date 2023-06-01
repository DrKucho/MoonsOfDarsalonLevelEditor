using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyThisGameObjectOnLoad : MonoBehaviour {

    void Awake(){
        DontDestroyOnLoad(gameObject);
        gameObject.tag = "DontDestroyOnNewLevel";
    }
}
