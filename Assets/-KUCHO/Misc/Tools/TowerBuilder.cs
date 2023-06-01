using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class TowerBuilder : MonoBehaviour {
    public Transform top;
    public Transform platformSprites;
    public GameObject midPart;
    public int midPartHeight = 8;
    public float midPartZOffset = 0.001f;
    public Transform bottom;

    [ReadOnly2Attribute] public int startPos;
    [ReadOnly2Attribute] public int   endPos;

    public List<GameObject> list;

#if UNITY_EDITOR
    void Update(){
        Transform t = transform;

        //        Vector2Int snappedBottomPos = new Vector2Int(Mathf.RoundToInt(bottom.localPosition.x), Mathf.RoundToInt(bottom.localPosition.y));
        //        bottom.localPosition = new Vector3(snappedBottomPos.x, snappedBottomPos.y, bottom.localPosition.z);
        endPos = (int)bottom.position.y; // importante fijar la posicion del final antes  por que al mover el padre (startPos) se va a mover
        Vector3 bottomPos = bottom.position;// importante fijar la posicion del final antes  por que al mover el padre (startPos) se va a mover

        Vector2Int snappedTopPos = new Vector2Int(Mathf.RoundToInt(top.localPosition.x), Mathf.RoundToInt(top.localPosition.y));
        top.localPosition = new Vector3(snappedTopPos.x, snappedTopPos.y, top.localPosition.z);
        startPos = (int)top.position.y;
     
        bottom.position = bottomPos; // si el padre mueve el bottom, aqui lo recupero

        int diffY = startPos - endPos;
        float partCount = diffY / midPartHeight;
        if (partCount != list.Count)
        {
            if (list.Count > 0)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    DestroyImmediate(list[i]);
                }
            }
            list.Clear();
            list.Add(midPart);
            list[0].transform.localPosition = new Vector3(0, 0, midPartZOffset);;
            for (int i = 1; i < partCount; i++)
            {
                GameObject newPart = Instantiate(midPart, t);
                newPart.transform.localPosition = new Vector3(0, -midPartHeight * i, midPartZOffset);
                list.Add(newPart);
            }
        }
    }
#endif
}
