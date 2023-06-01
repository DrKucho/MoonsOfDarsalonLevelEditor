using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloorButton : MonoBehaviour {

    public Collider2D myCol;
    public Collider2D switchCol;
    public Rigidbody2D myRb;
    public Rigidbody2D swRb;
    public EnableOrDisable doorActionOnPress;
    public Door[] switchDoors;
    public EnableOrDisable gosActionOnPress;
    public GameObject[] switchGos;

    public AudioManager aM;
    bool WeGotAudioManager(){return aM;}
    public AudioClipArray audioOn;
    public AudioClipArray audioOff;    


}
