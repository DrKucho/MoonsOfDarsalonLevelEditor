using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PickUpText : MonoBehaviour
{

    public bool debug = false;
    public Vector3 positionOffset;
    [ReadOnly2Attribute] public bool cycleColor = true;
    [Range(0, 1)] public float alpha = 1;
    public int slideUpTo = 30;
    [ReadOnly2Attribute] public float timeTextFloating = 1f;
    //  List<Color> colors = new List<Color> {Color.white, Color.cyan, Color.blue, Color.magenta, Color.red, Color.yellow, Color.green}; // ya no la uso?
    float[] c = { 0f, 0f, 0f }; //color del texto que sube?
    private float cinc = 0.5f;
    private int index = 0;
    public SWizTextMesh textMesh;
    public Item item;
    public Vector2 speed = new Vector2(0, 10);
    //float inc; // el incremente de coordenada y para que suba poco a poco en pixeles
    //float cont = 0f; // contador que ira incrementandose con inc hasta llegar a slideUpTo
    float timeToGetBackToStore;
    Transform myTransform;
    public float xSixePerChar = 4;
    [ReadOnly2Attribute] public BoxCollider2D myCol;
    [ReadOnly2Attribute] public Rigidbody2D rb;

    public void InitialiseInEditor()
    {
        textMesh = GetComponentInChildren<SWizTextMesh>();
        item = GetComponentInParent<Item>();
        myCol = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
   
}
