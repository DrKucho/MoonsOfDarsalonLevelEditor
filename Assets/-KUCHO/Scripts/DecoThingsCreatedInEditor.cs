using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpritePlane))]
public class DecoThingsCreatedInEditor : MonoBehaviour {
	[Range(0,2)] public RangeFloat scaleRandomizer;

	public OutOfScreen outOfScreen;

    [HideInInspector] public GameObject[] original; // deprecated
    public List<Item> originals;
//	public List<GameObject> things = new List<GameObject>();
    public List<Item> items = new List<Item>();
    public SpritePlane spritePlane;
	public Vector2 detectSize = new Vector2(20,20);
	int originalIndex;
	Vector2 detectSizeHalf;
	Vector2 _pos;

   #if UNITY_EDITOR
	void OnValidate(){
		detectSizeHalf = detectSize/2;
        if (isActiveAndEnabled && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (items.Count == 0)
                 RecomposeListWithChildren();
            // copia la antigua tabla de gos original a originals (tabla de decothings)
            if (originals == null || originals.Count == 0)
            {
                if (originals == null)
                    originals = new List<Item>();
                foreach (GameObject or in original)
                {
                    Item dt = or.GetComponent<Item>();
                    originals.Add(dt);
                }
            }
        }
        if (!spritePlane)
            spritePlane = GetComponent<SpritePlane>();
        if (!spritePlane)
            spritePlane = gameObject.AddComponent<SpritePlane>();
        spritePlane.enabled = false; // es solo para copiar a elementos
        spritePlane.updateOnEnableOnValidate = false;
	}
	#endif
	
    public void InitialiseInEditor(){
	    /*
        originals.Clear();
        foreach (Transform child in transform)
        {
            if (child != transform) // no es yo mismo
            {
                Item it = child.GetComponentInChildren<Item>();
                if (!it)
                    it = child.gameObject.AddComponent<Item>();
                if (!child.name.Contains("(Clone)"))
                    originals.Add(it);
            }
        }
        */
        RecomposeListWithChildren();
    }


	
	public void RandomizePlanesOnElements(){
		var originalGrow = originals[0].GetComponent<GrowAnimator>();
        foreach (Item dt in items)
		{
            GameObject go = dt.gameObject;
			var grow = go.GetComponent<GrowAnimator>(); // TODO deberia randomizar planos si darkness es scalebased? deberia hacerlo al reves en este caso? osea si randomizar, pero cambiar la escala tambien?
			if (grow)
			{
				grow.zToDarknessBackground = originalGrow.zToDarknessBackground;
				grow.zToDarknessCoverground = originalGrow.zToDarknessCoverground;
                grow.scaleToDarknessFactor = originalGrow.scaleToDarknessFactor;
                grow.minScale = originalGrow.minScale;
                grow.maxScale = originalGrow.maxScale;
                grow.darknessMode = originalGrow.darknessMode;
				var plane = go.GetComponent<SpritePlane>();
				if (!plane)
					plane = go.AddComponent<SpritePlane>();
                plane.type = spritePlane.type;
                switch ( grow.darknessMode)
                {
                    case (GrowAnimator.DarknessMode.zBackgroundAndCovergroundBased):
                        plane.randomType = new SpritePlane.Type[2];
                        plane.randomType[0] = SpritePlane.Type.FarBackground;
                        plane.randomType[1] = SpritePlane.Type.Coverground2;
                        plane.RandomizeMyPlane();
                        plane.updateOnEnableOnValidate = false;
                        grow.relativeSpritePlanesZHasBeenSet = false; 
                        grow.SetDarkness(true);
                        break;
                    case (GrowAnimator.DarknessMode.zBackgroundBased):
                        plane.randomType = null;
                        plane.type = SpritePlane.Type.FarBackground;
                        plane.RandomizeMyPlane();
                        plane.updateOnEnableOnValidate = false;
                        grow.relativeSpritePlanesZHasBeenSet = false; 
                        grow.SetDarkness(true);
                        break;
                    case (GrowAnimator.DarknessMode.ScaleBased):
                        plane.randomType = null;
                        plane.type = SpritePlane.Type.FarBackground;
                        plane.RandomizeMyPlane(1 - grow.minScale, 1 - grow.maxScale);
                        plane.updateOnEnableOnValidate = true; // para que si me apetece pueda ajustar cosas
                        // deberia saltar spriteplane.OnSpritePlaneChanged
                        break;
                }

			}
		}
	}
	
	public void SetDarkenssOnElements(){
        foreach (Item dt in items)
		{
            var grow = dt.gameObject.GetComponent<GrowAnimator>();
			if (grow)
				grow.InitAndSetDarkness();
		}
	}
	
	public void RemoveSpritePlanes(){
        foreach (Item dt in items)
		{
            var plane = dt.gameObject.GetComponent<SpritePlane>();
			if (plane)
				DestroyImmediate(plane);
		}
	}
    
    public void RecomposeListWithChildren(){
        items.Clear();
        var children = GetComponentsInChildren<Item>();
        if (children.Length > 0)
        {
            foreach (Item child in children)
            {
                bool isOriginal = false;
                foreach (Item orig in originals)
                {
                    if (child == orig)// || child.gameObject == gameObject)
                    {
                        isOriginal = true;
                        break;
                    }
                    if (isOriginal == false)
                    {
                        items.Add(child);
                        child.myDecoThingsInEditorManager = this;
                        if (child.gameObject.layer == Layers.defaultLayer)
	                        child.gameObject.layer = Layers.ground;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning(this + " NO HE ENCONTRADO NINGUN SCRIPT ITEM EN LOS HIJOS");
        }
    }
	
	public void ClearList(){
        Item[] its = items.ToArray(); // necesario para que no me de error advanced inspector
        items.Clear();
        for (int i = 0; i < its.Length; i++)
        {
            GameObject go = its[i].gameObject;
            DestroyImmediate(go);
        }
        var children = GetComponentsInChildren<Item>();
        foreach(Item child in children)
		{
			bool isOriginal = false;
            foreach(Item orig in originals)
			{
                if (child == orig)//  || child.gameObject == gameObject)
				{
					isOriginal = true;
					break;
				}
				if (isOriginal == false)
				{
					DestroyImmediate(child.gameObject);
				}
			}
		}
	}

    
    public void RandomizeScaleOnElements(){
	    foreach (Item dt in items)
	    {
		    var spr = dt.gameObject.GetComponent<SWizSprite>();
		    if (spr)
		    {
			    float s = Random.Range(scaleRandomizer.min, scaleRandomizer.max);
			    s = Mathf.Clamp01(s);
			    spr.scale = new Vector3(s,s,1);
		    }
	    }
    }
	public GameObject GetGameObject(Vector2 pos){
		_pos = pos;
//        GameObject go = things.Find(Intersect);
        Item dt = items.Find(Intersect);
//      GameObject go = things.Find(x => KuchoHelper.Intersect( (Vector2) x.transform.position,  new Rect(pos.x/2, pos.y/2, detectSize.x, detectSize.y)));
        if (dt)
            return dt.gameObject;
        return null;
	}

    public bool Intersect(Item item)
    {
	    if (!item)
		    return false;
        GameObject go = item.gameObject;
		if (go == null)
		{
			print (this + " ERROR GO NULL?");
			return false;
		}
		Vector2 goPos = (Vector2) go.transform.position;
		Rect rect = new Rect(_pos.x - detectSizeHalf.x, _pos.y - detectSizeHalf.y, detectSize.x, detectSize.y);
		return KuchoHelper.Intersect( goPos, rect);
	}
    public Item CycleOriginal(){
		originalIndex ++;
		if (originalIndex >= originals.Count) originalIndex = 0;
		return originals[originalIndex];
	}
    public Item RandomOriginal(){
        originalIndex = Random.Range(0,originals.Count -1);
		return originals[originalIndex];
	}
}
