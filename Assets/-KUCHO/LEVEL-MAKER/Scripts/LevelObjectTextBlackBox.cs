using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LevelObjectTextBlackBox : MonoBehaviour
{
    public SpriteRenderer rend;
    public TextMesh tm;

    public void InitialiseInEditor()
    {
        rend = GetComponent<SpriteRenderer>();
        if (!rend)
            rend = gameObject.AddComponent<SpriteRenderer>();
        tm = GetComponentInParent<TextMesh>();
    }

    public void Update()
    {
#if UNITY_EDITOR
        rend.sprite = MaterialDataBase.instance.singlePixelCenter;
        Color c = Color.black;
        c.a = 0.5f;
        rend.color = c;
        rend.gameObject.name = "BlackBox";
        rend.gameObject.layer = Layers.defaultLayer;
        if(!rend.gameObject.CompareTag("Pickable"))
            rend.gameObject.tag = "Pickable";

        if (tm)
        {
            var s = tm.GetComponent<Renderer>().bounds.size;
            s.x *= 1.05f;
            s.y *= 1.038f;
            var ls = tm.transform.lossyScale;
            s.x /= ls.x;
            s.y /= ls.y;
            transform.localScale = s;
            if (Selection.activeObject == gameObject)
            {
                Selection.activeObject = tm.gameObject;
            }
        }

        transform.localPosition = new Vector3(0,0,1);

#endif
    }
}

