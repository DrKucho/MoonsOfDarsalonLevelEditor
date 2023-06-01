#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Reflection;
using Light2D;
using Object = UnityEngine.Object;


[ExecuteInEditMode]

public class KuchoToolsMenu : MonoBehaviour {
    
    [MenuItem("Tools/Initialise Kucho Scripts In Editor _F1")]
	public static void InitialiseAllKuchoScripts(MenuCommand command){

        if (!Application.isPlaying)
        {
            Layers.Initialise();
            Masks.Initialise();
            ShaderProp.Initialise();

            var selectionBackup = Selection.activeObject;
            if (command.userData == 1) // 1 == allScene
                Selection.activeObject = null;
            //if (Application.isPlaying)
            //return;

            //esto carga el prefab pero al ejecutar los initialize DA PROBLEMAS segun parece no se pueden hacer GetComponent en un prefab que esta en la database, si que se puede si esta siendo editado en prefab edit, pero no se como acceder a el por script
            //GameObject game = AssetDatabase.LoadAssetAtPath("Assets/-KUCHO/Mis Prefabs/Game-OK.prefab", typeof(GameObject)) as GameObject;
            var selection = UnityEditor.Selection.gameObjects;

            if (selection == null || selection.Length == 0)
            {
                Debug.Log("INITIALISING THE WHOLE SCENE");
                var scene = EditorSceneManager.GetActiveScene();
                selection = scene.GetRootGameObjects();
            }

            foreach (GameObject go in selection)
            {
                if (go.name != "Prefab Indexer")
                {
                    var monos0 = go.transform.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour mono in monos0)
                    {
                        InitialiseMonoBehaviour(mono, go);
                    }
                    var childs = go.GetComponentsInChildRecursive<Transform>();
                    foreach (Transform child in childs)
                    {
                        var monos = child.GetComponents<MonoBehaviour>();
                        foreach (MonoBehaviour mono in monos)
                        {
                            InitialiseMonoBehaviour(mono, go);
                        }
                    }
                }
            }

            //foreach (GameObject go in selection)
            //{
            //    var allAudioManagers = go.GetComponentsInChildren<AudioManager>();
            //    foreach(AudioManager am in allAudioManagers)
            //    {
            //    if (am.useLoopedNoise && am.noise.inputMode == AudioManager.SpeedType.None)
            //        {
            //            Debug.Log("GO:" + am.gameObject.name + " TIENE None");
            //        }
            //    }
            //}
            EditorUtility.SetDirty(selection[0]);
            EditorSceneManager.MarkSceneDirty(selection[0].scene);
            Selection.activeObject = selectionBackup;

        }
    }

    static void InitialiseMonoBehaviour(MonoBehaviour mono, GameObject go)
    {
        if (mono != null)
        {
            if (mono.isActiveAndEnabled) // si estan apagados los getcomponentinparent o children no encuentran nada y se me vacian cosas que antes podian estar llenas
            {
                var init = mono.GetType().GetMethod("InitialiseInEditor");
                if (init != null)
                {
                    //Debug.Log("INICIALIZANDO:" + mono.name + "(" + mono.GetType() + ")");
                    var myAction = (System.Action) System.Delegate.CreateDelegate(typeof(System.Action), mono, init);
                    myAction();
                    //Debug.Log("ENVIANDO MSG A " + mono.GetType() + " " + mono.name);
                    //mono.SendMessage("InitialiseInEditor");
                }
                else
                {
                    //Debug.Log("NO TIENE INIT " + mono.GetType() + " " + mono.name);
                }
            }
        }
        else
        {
            //Debug.Log("UN MONOBEHABIOUR EN GO " + go.name + " ES NUL");
        }
    }

    [MenuItem("Tools/Rebuild Colliders _F2")]
    public static void InitAndRebuild()
    {
        if (!Application.isPlaying)
        {
            WorldMap map = FindObjectOfType<WorldMap>();
            if (map)
                map.RebuildAllCollidersOfAllMaps();
        }
    }
    [MenuItem("Tools/Swap Background 3D models and Sprite _F3")]
    public static void SwapBackground3dObjectsByTexture()
    {
        if (!Application.isPlaying)
        {
            WorldMap map = FindObjectOfType<WorldMap>();
            var b = map._background;
            if (map && map._background)
            {
                if (b && b.backgroundType == BackgroundType._3DModels)
                {
                    if (!b.spriteRenderer.enabled)
                    {
                        b.FillSpriteTextureWith3DObjects();
                    }
                    else
                    {
                        b.DisableSpriteAndEnable3DModels();
                    }
                }
            }
        }
    }
    
    
    [MenuItem("Tools/BringGroundEditor _F4")]
    public static void BringGroundEditor()
    {
        bool found = false;
        var rootsInScene = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in rootsInScene)
        {
            if (go.name.StartsWith("GroundEdit") || go.name.StartsWith(GameData.instance.groundEdit.name))
            {
                var tc = go.GetComponent<ExplosionStampExtras>();
                if (tc)
                {
                    Debug.Log(" GROUND EDIT ENCONTRADO EN ESCENA DESTRUYENDO");
                    DestroyImmediate(go);
                    found = true;
                }
            }
        }
        if (!found)
        {
            GameObject go = Instantiate(GameData.instance.groundEdit, Constants.zero3, Constants.zeroQ) as GameObject;
            // Vector3 pos = WorldMap.size * 0.5f;
            var cameras = SceneView.GetAllSceneCameras();
            Vector3 pos = EditorHelper.GetMousePositionOnSceneView();
            pos.z = -1;
            go.transform.localPosition = pos;
            Selection.activeGameObject = go;
        }
    }
    [MenuItem("Tools/Bring Tree Manager _F5")]
    public static void BringTreeManager()
    {
        bool found = false;
        var rootsInScene = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in rootsInScene)
        {
            if (go.name.StartsWith("TreeManager"))
            {
                found = true;
            }
        }
        if (!found)
        {
            string path = GameData.MOTHER_AND_CHILD_PROJECTS_SHARED_FOLDER + "/TreeManager/TreeManager.prefab";
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (!go)
                Debug.LogError("NO PUEDO INSTANCIAR TREE MANAGER, NO ENCONTRADO EN:" + path);
            else
                Instantiate(go, Constants.zero3, Constants.zeroQ);
        }
    }


    #region LEVEL OBJECTS
    [MenuItem("MOD Objects/IncObjectVariation _HOME")] 
    public static void IncObjectVariation()
    {
        if (Selection.gameObjects.Length > 0)
        {
            var back3D = Selection.gameObjects[0].GetComponent<Background3DModel>();
            if (back3D)
            {
                back3D.ObjectVariation();
            }
            else
            {
                var npo = Selection.gameObjects[0].GetComponent<LevelObject>();
                if (npo)
                {
                    npo.DoObjectVariation();
                }
            }
        }
    }
    [MenuItem("MOD Objects/DecObjectVariation #HOME")] 
    public static void DecObjectVariation() 
    {
        if (Selection.gameObjects.Length > 0)
        {
            var back3D = Selection.gameObjects[0].GetComponent<Background3DModel>();
            if (back3D)
            {
                back3D.ObjectVariation();
            }
        }
    }
    [MenuItem("MOD Objects/BringNextMisc _F10")] 
    public static void BringNextMisc()
    {
        GameData.instance.levelObjects.misc.BringNext();
    }
    [MenuItem("MOD Objects/BringPreviousMisc #F10")]
    public static void BringPreviousMisc()
    {
        GameData.instance.levelObjects.misc.BringPrevious();
    }
    [MenuItem("MOD Objects/BringNextDarsanautsVehicles _F7")] 
    public static void BringNextDarsanautsVehicles()
    {
        GameData.instance.levelObjects.darsanautsVehiclesAndKeyBuildings.BringNext();
    }
    [MenuItem("MOD Objects/BringPreviousDarsanautsVehicles #F7")]
    public static void BringPreviousDarsanautsVehicles()
    {
        GameData.instance.levelObjects.darsanautsVehiclesAndKeyBuildings.BringPrevious();
    }
    [MenuItem("MOD Objects/BringNextGoodGuysAndPickups _F6")] 
    public static void BringNextGoodGuysAndPickups()
    {
        GameData.instance.levelObjects.darsanautsAndPickups.BringNext();
    }
    [MenuItem("MOD Objects/BringPreviousGoodGuysAndPickups #F6")]
    public static void BringPreviousGoodGuysAndPickups()
    {
        GameData.instance.levelObjects.darsanautsAndPickups.BringPrevious();
    }
    [MenuItem("MOD Objects/BringEnemy _F9")] 
    public static void BringNextEnemy()
    {
        GameData.instance.levelObjects.enemies.BringNext();
    }
    [MenuItem("MOD Objects/BringPreviousEnemy #F9")]
    public static void BringPreviousEnemy()
    {
        GameData.instance.levelObjects.enemies.BringPrevious();
    }
    [MenuItem("MOD Objects/BringNextPlatform _F8")]
    public static void BringNextPlatform()
    {
        GameData.instance.levelObjects.platforms.BringNext();
    }
    [MenuItem("MOD Objects/BringPreviousPlatform #F8")]
    public static void BringPreviousPlatform()
    {
        GameData.instance.levelObjects.platforms.BringPrevious();
    }
    [MenuItem("MOD Objects/BringNextDeco _F11")]
    public static void BringNextDeco()
    {
        GameData.instance.levelObjects.decoration.BringNext();
    }
    [MenuItem("MOD Objects/BringPreviousDeco #F11")]
    public static void BringPreviousDeco()
    {
        GameData.instance.levelObjects.decoration.BringPrevious();
    }    [MenuItem("MOD Objects/BringNextBackgroundRock _F12")]
    public static void BringNextBackgroundRock()
    {
        var b = FindObjectOfType<Background3DModelParent>();
        if (!b)
            SwapBackground3dObjectsByTexture();
        if (b)
        {
            if (b.list == null)
                b.OnValidate();
            b.list.BringNext();
        }
    }
    [MenuItem("MOD Objects/BringPreviousBackgroundRock #F12")]
    public static void BringPreviousBackgroundRock()
    {
        var b = FindObjectOfType<Background3DModelParent>();
        if (!b)
            SwapBackground3dObjectsByTexture();
        if (b)
        {
            if (b.list == null)
                b.OnValidate();
            b.list.BringPrevious();
        }
    }
    
    /*[MenuItem("MOD Objects/ConsolidateNotPoolableObject _HOME")]
    public static void ConsolidateNotPoolableObject()
    {
        NotPooledObjectsDataBase.NotPooledObjectsList.ConsolidateLastInstance();
    }
    */ 
    #endregion


    [MenuItem("Tools/FixKuchoTile")]
    public static void FixKuchoTile()
    {
        var selection = UnityEditor.Selection.gameObjects;
        if (selection.Length == 0)
            return;

        var guids = AssetDatabase.FindAssets("SinglePixelBottomLeft");
        
        Sprite spr = null;
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        
        //Sprite spr = Resources.Load("p.psd") as Sprite;
        if (!spr)
        {
            Debug.LogError("Sprite no encontrado");
            return;
        }

        var all = selection[0].GetComponentsInChildren<Light2D.LightObstacleSprite>();
        foreach (Light2D.LightObstacleSprite lm in all)
        {
            var rend = lm.GetComponent<MeshRenderer>();
            var filt = lm.GetComponent<MeshFilter>();
            if (rend)
                DestroyImmediate(rend);
            if (filt)
                DestroyImmediate(filt);

            var spRend = lm.GetComponent<SpriteRenderer>();
            if (!spRend)
                spRend = lm.gameObject.AddComponent<SpriteRenderer>();
            spRend.sprite = spr;
            spRend.color = Color.black;

            var kobs = lm.GetComponent<KuchoLightObstacleSprite>();
            if (!kobs)
                kobs = lm.gameObject.AddComponent<KuchoLightObstacleSprite>();
            
            kobs.InitialiseInEditor();
            kobs._sprRenderer.color = lm.Color;
            EditorUtility.SetDirty(selection[0]);
            EditorSceneManager.MarkSceneDirty(selection[0].scene);
            DestroyImmediate(lm);
        }
    }

    [MenuItem("Tools/Log Asset Path")]
    public static void LogAssetPath()
    {
        var s = Selection.activeGameObject;
        Debug.Log(AssetDatabase.GetAssetPath(s));
    }
    
}
#endif
