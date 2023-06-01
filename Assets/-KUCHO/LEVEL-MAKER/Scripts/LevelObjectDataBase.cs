using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New LevelObjects Data", menuName = "New LevelObjects Data", order = 51)]
public class LevelObjectDataBase : ScriptableObject
{
    [System.Serializable]
    public class LevelObjectObjectList
    {

        public string name = "No Named Level Object List";
        public List<GameObject> gos;
        [SerializeField] [HideInInspector] int i = -1;
        [HideInInspector] static private LevelObjectObjectList lastList;
        [HideInInspector] static public GameObject lastInstance;
        [HideInInspector] static private int lastInc;

        LevelObjectObjectList()
        {
            gos = new List<GameObject>();
            i = -1;
        }

        #if UNITY_EDITOR
        public void BringNext()
        {
            IncAndBring(1);

        }
        public void BringPrevious()
        {
            IncAndBring(-1);
        }

        void IncAndBring(int inc)
        {
            KuchoHelper.IncAndWrapInsideArrayLength(ref i, inc, gos.Count);
            lastInc = inc;
            InstantiateLevelObject();
        }

        void InstantiateLevelObject()
        {
            if (gos != null && i <= gos.Count)
            {
                if (lastInstance)
                    DestroyImmediate(lastInstance);
                var cameras = SceneView.GetAllSceneCameras();

                Vector3 pos = EditorHelper.GetMousePositionOnSceneView();
                pos.z = 1;
                LevelObject npo = gos[i].GetComponent<LevelObject>();
                if (npo)
                {
                    lastInstance = Instantiate(gos[i], pos, npo.rotation);
                    if (LevelObjects.instance == null)
                    {
                        var los = FindObjectsOfType<LevelObjects>();
                        if (los == null || los.Length == 0)
                        {
                            var parentGO = new GameObject();
                            los = new LevelObjects[1];
                            los[0] = parentGO.AddComponent<LevelObjects>();
                            los[0].name = "LevelObjects";
                        }
                        else if (los.Length > 1)
                        {
                            Debug.LogError("THERE ARE " + los.Length + " SCRIPTS OF TYPE 'LevelObjects'... THERE CAN BE ONLY ONE ...Heeeeere we aaaare!. Boooorn to be kings, we're the princes of the uuuuuuniverse...");
                        }

                        if (los.Length == 1)
                        {
                            LevelObjects.instance = los[0];
                        }
                    }

                    Transform parent = null;
                    var firstChildrens = LevelObjects.instance.GetComponentsInChildren<Transform>(false);
                    foreach (Transform child in firstChildrens)
                    {
                        if (child.name == name)
                        {
                            parent = child;
                        }
                    }

                    if (parent == null)
                    {
                        parent = new GameObject(name).transform;
                        parent.name = name;
                        parent.position = Constants.zero2;
                        parent.parent = LevelObjects.instance.transform;
                    }
                    
                    LevelObjects.AddToTransformParentList(parent);

                    lastInstance.transform.parent = parent.transform;

                        //MakePickableOnlyThePickableInLastInstance();
                    CenterLastInstance();
                    lastList = this;
                }
                else
                    Debug.LogError(gos[i] + " NO TIENE SCRIPT LEVEL OBJECT");
            }
        }
        static void CenterGameObject(GameObject go)
        {
            var rends = go.GetComponentsInChildren<Renderer>();
            Vector2 min;
            Vector2 max;
            min.x = float.MaxValue;
            min.y = float.MaxValue;
            max.x = float.MinValue;
            max.y = float.MinValue;
            bool valuesAreValid = false;
            foreach (Renderer rend in rends)
            {
                LineRenderer line = rend as LineRenderer;
                if (!line)
                {
                    var b = rend.bounds;
                    if (b.size.x > 0.01f && b.size.y > 0.01f) // mas pequeños pueden ser PArticleSystemRenderers con size 0,0,0 y min/Max 0,0,0
                    {
                        if (b.min.x < min.x)
                            min.x = b.min.x;
                        if (b.min.y < min.y)
                            min.y = b.min.y;
                        if (b.max.x > max.x)
                            max.x = b.max.x;
                        if (b.max.y > max.y)
                            max.y = b.max.y;
                        valuesAreValid = true;
                    }
                }
            }
            Vector2 c = (max - min) * 0.5f;
            c += min;
            Vector2 pos = lastInstance.transform.position;
            Vector2 diff;
            Vector2 newPos;
            if (valuesAreValid)
            {
                diff = pos - c;
                newPos = pos + diff;
            }
            else
            {
                newPos = pos;
            }
            go.transform.position = new Vector3(newPos.x, newPos.y, go.transform.position.z);
        }

        static void CenterLastInstance()
        {
            CenterGameObject(lastInstance);
        }

        public static void ConsolidateLastInstance()
        {
            if (lastInstance && lastList != null)
            {
                Undo.RegisterCreatedObjectUndo(lastInstance, "Consolidated not poolable object" + lastInstance.name);
                KuchoHelper.IncAndWrapInsideArrayLength(ref lastList.i, -lastInc, lastList.gos.Count);

                lastInstance = null;
            }
        }
        #endif
        /*
public static void MakePickableOnlyThePickableInLastInstance()
{
    var all = lastInstance.GetComponentsInChildren<Transform>(true);
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
        var biggest = KuchoHelper.FindBiggest3DRenderer(lastInstance.transform, float.MinValue);
        if (biggest)
            SceneVisibilityManager.instance.EnablePicking(biggest.gameObject, false);
    }
    
    var handles = lastInstance.GetComponentsInChildren<DragMeHandle>();

    foreach(DragMeHandle d in handles)
        SceneVisibilityManager.instance.EnablePicking(d.gameObject, false);
    
    SceneVisibilityManager.instance.EnablePicking(lastInstance, false); // el raiz siempre pickable
}
*/
    }
    public string find;

    public LevelObjectObjectList darsanautsAndPickups;
    public LevelObjectObjectList darsanautsVehiclesAndKeyBuildings;
    public LevelObjectObjectList enemies;
    public LevelObjectObjectList platforms;
    public LevelObjectObjectList decoration;
    public LevelObjectObjectList misc;
    public LevelObjectObjectList[] backgroundrocks;

    //[AdvancedInspector.Inspect]
    public void Find()
    {
        for (int i = 0; i < enemies.gos.Count; i++)
        {
            if (enemies.gos[i].name.Contains(find))
            {
                Debug.Log("ENCONTRADO EN ENEMIES POS:" + i);
            }
        }
        for (int i = 0; i < decoration.gos.Count; i++)
        {
            if (decoration.gos[i].name.Contains(find))
            {
                Debug.Log("ENCONTRADO EN DECORATION POS:" + i);
            }
        }
        for (int i = 0; i < platforms.gos.Count; i++)
        {
            if (platforms.gos[i].name.Contains(find))
            {
                Debug.Log("ENCONTRADO EN PLATFORMS POS:" + i);
            }
        }
        for (int i = 0; i < darsanautsAndPickups.gos.Count; i++)
        {
            if (darsanautsAndPickups.gos[i].name.Contains(find))
            {
                Debug.Log("ENCONTRADO EN GOOD GUYS & PICKUPS POS:" + i);
            }
        }
        for (int i = 0; i < misc.gos.Count; i++)
        {
            if (misc.gos[i].name.Contains(find))
            {
                Debug.Log("ENCONTRADO EN MISC POS:" + i);
            }
        }
        for (int i = 0; i < darsanautsVehiclesAndKeyBuildings.gos.Count; i++)
        {
            if (darsanautsVehiclesAndKeyBuildings.gos[i].name.Contains(find))
            {
                Debug.Log("ENCONTRADO EN VEHICLESAND.. POS:" + i);
            }
        }
    }
}