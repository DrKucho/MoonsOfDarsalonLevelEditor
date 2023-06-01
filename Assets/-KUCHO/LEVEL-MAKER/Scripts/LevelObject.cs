using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class LevelObjectVariation
{
    public Sprite previewSprite;
    public int previewSwizSpriteID;
    public Vector3 previewRot;
    public Vector3 previewScale;
    public Vector3 rot;
    public Vector3 scale;

    public LevelObjectVariation()
    {
        scale = new Vector3(1,1,1);
        previewScale = scale;
    }
}

[ExecuteInEditMode][SelectionBase]
public class LevelObject : MonoBehaviour
{
    public bool makePickableWhatShouldBePickable = false;
    public bool pixelSnap = false;
    public bool canRotate;
    public Quaternion rotation;
    public LevelObjectVariation[] variations;
    public int rotIndex = -1;
    public Transform flippingTransform;
    public SpriteRenderer previewSpriteRenderer;
    public SWizSprite previewSwizSprite;
    public bool useOwnMaterial = false;
    public Material spriteMaterial;
    public TextMesh[] tms;
#if UNITY_EDITOR
    public PivotMode pivotMode = PivotMode.Pivot;
    void OnEnable()
    {
        if (makePickableWhatShouldBePickable)
            MakePickableWhatShouldBePickable();
        if (Application.isPlaying)
            enabled = false;
    }

    private void OnValidate()
    {
        if (tms != null && tms.Length > 0)
        {
            if (canRotate)
                tms[0].color = Color.yellow;
            else
                tms[0].color = Color.white;
        }

        if (useOwnMaterial)
        {
            useOwnMaterial = false;
            spriteMaterial = previewSpriteRenderer.material;
            previewSpriteRenderer.material = spriteMaterial;
        }
    }

    public void InitialiseInEditor()
    {
        rotation = transform.rotation;
        previewSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        previewSwizSprite = GetComponentInChildren<SWizSprite>();
        var allTms = GetComponentsInChildren<TextMesh>();
        List<TextMesh> l = new List<TextMesh>();
        foreach(TextMesh tm in allTms)
            if(!tm.GetComponent<PatrolPoint>())// excluyo los que tienen patrol point
                l.Add(tm);
        tms = l.ToArray();
        
        UpdateTextMeshes();
        var waves = GetComponentsInChildren<WaveRotateLocalEulers>();
        foreach (WaveRotateLocalEulers w in waves)
            w.referenceParent = transform;
        MakePickableWhatShouldBePickable();
    }
    
    public void InstantiateMaterial()
    {
        
    }

    void UpdateTextMeshes()
    {
        foreach (TextMesh tm in tms)
        {
            if (tm)
            {
                tm.gameObject.layer = Layers.defaultLayer;
                var pos = tm.transform.position;
                pos.z = -25F; // para que siempre sea visible
                tm.transform.position = pos;
                var blackBox = tm.GetComponentInChildren<LevelObjectTextBlackBox>();
                if (!blackBox)
                {
                    var o = Instantiate(new GameObject(), tm.transform);
                    blackBox = o.AddComponent<LevelObjectTextBlackBox>();
                    blackBox.InitialiseInEditor();
                }
                blackBox.Update();
            }
        }
    }

    void Update()
    {
        if (pixelSnap)
        {
            if (transform.parent)
                transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), transform.localPosition.z);
            else
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), transform.position.z);
        }

        if (!canRotate)
        {
            transform.localRotation = rotation;
        }

        if (Constants.appIsLevelEditor)
        {
            foreach (var tm in tms)
            {
                //impido que se pueda seleccionar el texto si es que el texto esta en un hijo en lugar del mismo go que este script
                if (tm.gameObject != gameObject && Selection.activeObject == tm.gameObject)
                    Selection.activeObject = gameObject;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Selection.activeObject == gameObject)
            Tools.pivotMode = pivotMode;
    }
    public void DoObjectVariation()
    {
        if (variations.Length > 0)
        {
            KuchoHelper.IncAndWrapInsideArrayLength(ref rotIndex, 1, variations.Length);
            SetCurrentVariation();
        }
    }

    public LevelObjectVariation GetCurrentVariation() // al compilar da error si sticker llama aqui , que no tiene este metodo dice , pfff, por eso sticker hace esto por su cuenta
    {
        if (variations.Length > 0)
        {
            return variations[rotIndex];
        }
        else
        {
            return null;
        }
    }

    public void SetCurrentVariation()
    {
        if (variations.Length > 0)
        {
            Transform transtoVariate;
            if (flippingTransform)
                transtoVariate = flippingTransform;
            else
                transtoVariate = transform;
                
            var variation = variations[rotIndex];
            if (previewSpriteRenderer && variation.previewSprite != null)
            {
                previewSpriteRenderer.sprite = variation.previewSprite;
            }
            else if (previewSwizSprite && variation.previewSwizSpriteID > 0)
            {
                previewSwizSprite.SetSprite(variation.previewSwizSpriteID);
            }
            else
            {
                if (true)
                {
                    transtoVariate.rotation = Quaternion.identity;
                    transtoVariate.rotation = Quaternion.Euler(variation.rot);
                    transtoVariate.localScale = variation.previewScale;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(variation.rot);
                    transform.localScale = variation.previewScale;
                }
            }

            transtoVariate.rotation = Quaternion.Euler(variation.previewRot);
            foreach (var t in tms)
            {
                if (t.transform != transtoVariate)
                {
                    var s = t.transform.localScale;
                    if (Mathf.Sign(s.x) != Mathf.Sign(transform.localScale.x))
                    {
                        s.x *= -1;
                        t.transform.localScale = s;
                    }
                }
            }
        }
    }

    public void MakePickableWhatShouldBePickable()
    {
        if (makePickableWhatShouldBePickable)
        {
            var all = GetComponentsInChildren<Transform>(true);
            int count = 0;
            foreach (Transform t in all)
            {
                if (t.CompareTag("Pickable"))
                {
                    SceneVisibilityManager.instance.EnablePicking(t.gameObject, false);
                    count++;
                }
                else
                    SceneVisibilityManager.instance.DisablePicking(t.gameObject, false);
            }

            if (count == 0)
            {
                var biggest = KuchoHelper.FindBiggest3DRenderer(gameObject.transform, float.MinValue);
                if (biggest)
                    SceneVisibilityManager.instance.EnablePicking(biggest.gameObject, false);
            }

            var handles = GetComponentsInChildren<DragMeHandle>();

            foreach (DragMeHandle d in handles)
                SceneVisibilityManager.instance.EnablePicking(d.gameObject, false);

            SceneVisibilityManager.instance.EnablePicking(gameObject, false); // el raiz siempre pickable
        }
    }
#endif
}
