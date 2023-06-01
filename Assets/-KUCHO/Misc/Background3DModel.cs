using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;

#endif
/*
[System.Serializable]
public class Background3DModelList{
    public List<Background3DModel> models;
    public bool changeAllowed = true;

    public static implicit operator bool(Background3DModelList me) // para poder hacer if(class) en lugar de if(class != null) NULLABLE nullable
    {
        return me != null;
    }
    public Background3DModelList(){
        models = new List<Background3DModel>();
    }
    public void Add(Background3DModel model){
        if (changeAllowed)
        {
            if (!models.Find(x => x == model))
            {
                WorldMap.background._3dModelsToSpriteList.Add(model);
            }
        }
    }
    public void Remove(Background3DModel model){
        if (changeAllowed)
        {
            models.Remove(model);
        }
    }
    public void SwitchAll(bool flag){
        changeAllowed = false; // para que no se modifique la lista al activar o desactivar los Gos! (ellos tienen un script que modifica la lista
        if (models != null) // puede venir null
        {
            foreach (Background3DModel m in models)
            {
                if(m) // podria estar vacio si he olvidado refrescar en editor 
                    m.gameObject.SetActive(flag);
            }
        }
        changeAllowed = true;
    }
    public void FindAll() {
        if (models == null)
            models = new List<Background3DModel>();
        else
        {
            foreach (Background3DModel o in models)
            {
                if (o)
                    o.gameObject.SetActive(true);// enciendelos antes de cargarte la lista que si no los pierdes !
            }
            models.Clear();
        }
        Background3DModel[] modelsInScene = GameObject.FindObjectsOfType<Background3DModel>();

        foreach (Background3DModel m in modelsInScene)
            models.Add(m);
        
    }
}
*/
[ExecuteInEditMode][SelectionBase]
public class Background3DModel : MonoBehaviour {    

    // las oculto por si acaso tengo qeu recuperar algo pero en principio ya no sirven
    [HideInInspector] public float randomScaleRange = 0.2f;
    [HideInInspector] public bool syncScale = true;
    [HideInInspector] public bool _fixedScale = false;
    [HideInInspector] public float fixedScale = 5;
    
    public Vector3 scaleMults = Vector3.one;
    public bool scaleXAllwasPostitive;
    public bool scaleYAllwasPostitive;
    public bool scaleZAllwasPostitive;
    public Vector3 rotLimits = new Vector3(30,0,360);
    public Transform offsetChild;
    public Vector3 childRotLimits = new Vector3(0,360,0);
    //public List<Vector3> favScales;
    //public List<Quaternion> favRotations;
    [Range (300, 550)]public float z = 400;
    public LayerType layer = LayerType.Default;

#if UNITY_EDITOR
    void Awake()
    {
        Init();
        MyUpdate();
    }

    public void InitialiseInEditor()
    {
        Init();
        //transform.localScale = scaleMults * fixedScale;
    }

    public void Init()
    {
        if (isActiveAndEnabled)
        {
            if (GetComponentInParent<Background3DModelParent>() == null)
            {
                var parent = FindObjectOfType<Background3DModelParent>();
                if (parent)
                {
                    transform.parent = parent.transform;
                }
                else
                    Debug.Log(this + " NO ENCUENTRO BACKGROUDN 3D MODELS PARENT PARA HACERME HIJO");
            }
        }
        gameObject.layer = Layers.GetLayerFromEnum(layer);
        var allMc = GetComponentsInChildren<MeshCollider>();
        foreach(MeshCollider m in allMc)
            DestroyImmediate(m);
        var all = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in all)
        {
            if (t.name == "GameObject")
            {
                if (t.GetComponentsInChildren<Transform>().Length == 1) // 1 = no tiene hijos
                {
                    DestroyImmediate(t.gameObject);
                }
                else
                {
                    offsetChild = t;
                    offsetChild.name = "Offset";
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        MyUpdate();
    }

    Vector3 s;
	void MyUpdate() {
        /*
        if (_fixedScale)
            transform.localScale = scaleMults * fixedScale;
        
        if (syncScale)
        */
        {
            s = transform.localScale;
            s.x = Mathf.Abs(s.x);
            s.y = Mathf.Abs(s.y);
            s.z = Mathf.Abs(s.z);

            if (s.x != s.y && s.x != s.z)
            {
                s = new Vector3(s.x * scaleMults.x, s.x * scaleMults.y, s.x * scaleMults.z);
                transform.localScale = s;
            }
            else if (s.y != s.x && s.y != s.z)
            {
                s = new Vector3(s.y * scaleMults.x, s.y * scaleMults.y, s.y * scaleMults.z);
                transform.localScale = s;
            }
            else if (s.z != s.x && s.z != s.y)
            {
                s = new Vector3(s.z * scaleMults.x, s.z * scaleMults.y, s.z * scaleMults.z);
                transform.localScale = s;
            }
        }
        TransformHelper.SetPosZ(transform, z);
        if (Selection.activeObject == gameObject)
        {
            Tools.pivotMode = PivotMode.Center;
            Tools.pivotRotation = PivotRotation.Global;
        }
    }

    public enum LastRandomRotation { X,Y,Z,Y_AndSignX,All }

    public static LastRandomRotation lastRandomRotation;

    
    void RandomRotX()
    {
        TransformHelper.SetEulerAngleX(transform, Random.Range(-rotLimits.x, rotLimits.x));
        if (offsetChild)
            TransformHelper.SetEulerAngleX(offsetChild, Random.Range(-childRotLimits.x, childRotLimits.x));
        lastRandomRotation = LastRandomRotation.X;
    }
    
    void RandomRotY()
    {
        TransformHelper.SetEulerAngleY(transform, Random.Range(-rotLimits.y, rotLimits.y));
        if (offsetChild)
            TransformHelper.SetEulerAngleY(offsetChild, Random.Range(-childRotLimits.y, childRotLimits.y));

        lastRandomRotation = LastRandomRotation.Y;
    }
    
    void RandomRotZ()
    {
        TransformHelper.SetEulerAngleZ(transform, Random.Range(-rotLimits.z, rotLimits.z));
        if (offsetChild)
            TransformHelper.SetEulerAngleZ(offsetChild, Random.Range(-childRotLimits.z, childRotLimits.z));

        lastRandomRotation = LastRandomRotation.Z;
    }
    
    void RandomRotY_AndScaleSignX()
    {
        RandomRotY();
        RandomScaleSignX();
        lastRandomRotation = LastRandomRotation.Y_AndSignX;
    }
    void RandomScaleSignX()
    {
        if (!syncScale)
        {
            float s = 1;
            if (!scaleXAllwasPostitive)
            {
                s = Random.Range(-100, 100);
                s = Mathf.Sign(s);
            }
            if (_fixedScale)
            {
                scaleMults.x *= s; 
            }
            else
            {
                TransformHelper.SetLocalScaleX(transform, transform.localScale.x * s);
            }
        }
    }
    void RandomScaleSignY()
    {
        if (!syncScale)
        {
            float s = 1;
            if (!scaleYAllwasPostitive)
            {
                s = Random.Range(-100, 100);
                s = Mathf.Sign(s);
            }
            if (_fixedScale)
            {
                scaleMults.y *= s; 
            }
            else
            {
                TransformHelper.SetLocalScaleY(transform, transform.localScale.y * s);
            }
        }
    }
    void RandomScaleSignZ()
    {
        if (!syncScale)
        {
            float s = 1;
            if (!scaleZAllwasPostitive)
            {
                s = Random.Range(-100, 100);
                s = Mathf.Sign(s);
            }
            if (_fixedScale)
            {
                scaleMults.z *= s; 
            }
            else
            {
                TransformHelper.SetLocalScaleZ(transform, transform.localScale.z * s);
            }
        }
    }

    
    void _(){

    }
    
    void FlipX()
    {
        TransformHelper.SetEulerAngleX(transform, transform.eulerAngles.x -180);
    }
    
    void FlipY()
    {
        TransformHelper.SetEulerAngleY(transform, transform.eulerAngles.y - 180);
    }
    
    void FlipZ()
    {
        TransformHelper.SetEulerAngleZ(transform, transform.eulerAngles.z - 180);
    }
    
    void __()
    {

    }
    
    void RandomRotationAndScaleSigns()
    {
        float x = Random.Range(-rotLimits.x, rotLimits.x);
        float y = Random.Range(-rotLimits.y, rotLimits.y);
        float z = Random.Range(-rotLimits.z, rotLimits.z);
        transform.localRotation = Quaternion.Euler(x, y, z);
        if (!syncScale)
        {
            Vector3 s;
            if (scaleXAllwasPostitive)
                s.x = 1;
            else
            {
                s.x = Random.Range(-100, 100);
                s.x = Mathf.Sign(s.x);
            }
            if (scaleYAllwasPostitive)
                s.y = 1;
            else
            {
                s.y = Random.Range(-100, 100);
                s.y = Mathf.Sign(s.y);
            }
            if (scaleZAllwasPostitive)
                s.z = 1;
            else
            {
                s.z = Random.Range(-100, 100);
                s.z = Mathf.Sign(s.z);
            }

            if (_fixedScale)
            {
                scaleMults.x *= s.x;
                scaleMults.y *= s.y;
                scaleMults.z *= s.z;            }
            else
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x *= s.x;
                currentScale.y *= s.y;
                currentScale.z *= s.z;
                transform.localScale = currentScale;
            }
        }

        if (offsetChild)
        {
            x = Random.Range(-childRotLimits.x, childRotLimits.x);
            y = Random.Range(-childRotLimits.y, childRotLimits.y);
            z = Random.Range(-childRotLimits.z, childRotLimits.z);
            offsetChild.localRotation = Quaternion.Euler(x, y, z);
        }
        lastRandomRotation = LastRandomRotation.All;
    }

    
    void ZeroRotationsAndscaleMults()
    {
        transform.localEulerAngles = Vector3.zero;
        if (offsetChild)
            offsetChild.localEulerAngles = Vector3.zero;
        if (_fixedScale)
        {
            scaleMults = Vector3.one;
        }
    }
    void RandomScale()
    {
        float oldScale = (transform.localScale.x + transform.localScale.y + transform.localScale.z) /3;
        float randomFactor = Random.Range(-randomScaleRange, randomScaleRange);
        float newScale = oldScale + oldScale * randomFactor;
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void ObjectVariation()
    {
        switch (lastRandomRotation)
        {
            case(LastRandomRotation.X):
                RandomRotX();
                break;
            case(LastRandomRotation.Y):
                RandomRotX();
                break;
            case(LastRandomRotation.Z):
                RandomRotX();
                break;
            case(LastRandomRotation.Y_AndSignX):
                RandomRotY_AndScaleSignX();
                break;
            case(LastRandomRotation.All):
                RandomRotationAndScaleSigns();
                break;
        }
    }
    /*
    
    void AddRotationToFavorites()
    {
        favRotations.Add(transform.rotation);
    }
    
    void AddScaleToFavorites()
    {
        favScales.Add(transform.localScale);
    }
    private int rotIndex;
    public void O(int inc)
    {
        if (favRotations.Count > 0)
        {
            KuchoHelper.IncAndWrapInsideArrayLength(ref rotIndex, inc, favRotations.Count);
            transform.rotation = favRotations[rotIndex];
        }
    }
    */
#endif
}
