using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Steamworks;
using System.IO;


[InitializeOnLoad]
public static class EditorExtras
{
	static EditorExtras (){
		EditorApplication.update += UpdateStatic;
		EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
        PrefabUtility.prefabInstanceUpdated += PrefabUpdateDelegate; 
        EditorApplication.quitting += OnEditorQuits;
        /* FACEPUNCH
        try
        {
            SteamClient.Init(GameData.instance.GetSteamAppId(), true); 
        }
        catch
        {
            Debug.LogError("INTENTO INIT STEAM CLIENT PERO ALGO HA IDO MAL, ESTA ABIERTO STEAM?");
        }
        */
    }

    private static int selectionCountOld = -1;
    private static uint updateCount = 0;
    static List<Object> back3dModels;

    static void UpdateStatic()
    {
        bool inPrefabMode = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
        if (!Layers.initialised)
            Layers.Initialise();
        if (!Masks.initialised)
            Masks.Initialise();
        if (ShaderProp._AlphaTex == 0) // a veces en editor se quedan todos a cero, solo hace falta comprobar uno
            ShaderProp.Initialise();
        if (WorldMap.width == 0 || WorldMap.height == 0)
        {
            if (!WorldMap.instance)
                WorldMap.instance = MonoBehaviour.FindObjectOfType<WorldMap>();
            if (WorldMap.instance)
                WorldMap.instance.InitialiseInEditor();
        }

        if (!inPrefabMode)
        {
            if (WorldMap.instance)
            {
                WorldMap.instance.ZeroAllTerrainTransforms();
                if (WorldMap.indestructible && WorldMap.indestructible.HaveLostMySpriteRendererOrSprite()) // hemos salvado y pòr lo tanto se han destruido los sprites y texturas?
                    WorldMap.instance.InitialiseAllTerrains(true);
            }

            if (back3dModels == null)
                back3dModels = new List<Object>();
            back3dModels.Clear();
            if (Selection.objects.Length > 0 && Selection.objects.Length != selectionCountOld)
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    Background3DModel parent = Selection.gameObjects[i].GetComponentInParent<Background3DModel>();
                    if (parent)
                    {
                        back3dModels.Add(parent.gameObject);
                    }
                }

                /*if (Selection.gameObjects.Length == 1)
                {
                    if (Selection.gameObjects[0].GetComponent<Background3DModelParent>())// a pesar de que es no pickable y ground puede seleccionarse por rectangulo ya que tiene hijos que son pickables y es el padre del prefab
                        Selection.activeObject = null;
                }
                */

                if (back3dModels.Count > 0)
                {
                    Selection.objects = back3dModels.ToArray();
                }

                if (Selection.gameObjects.Length == 1)
                {
                    var go = Selection.gameObjects[0];
                    if (go == LevelObjectDataBase.LevelObjectObjectList.lastInstance || go.transform.root.gameObject == LevelObjectDataBase.LevelObjectObjectList.lastInstance) // seleccionado !
                    {
                        LevelObjectDataBase.LevelObjectObjectList.ConsolidateLastInstance();
                    }
                }
            }
        }

        if (updateCount > 0)
        {
            if (BuildPlayerWindow.levelsFullPath_child == null)
                BuildPlayerWindow.levelsFullPath_child = KuchoHelper.FixDirectorySeparators(Directory.GetCurrentDirectory() + BuildPlayerWindow.levelsPath);
            if (!Directory.Exists(BuildPlayerWindow.levelsFullPath_child))
            {
                Debug.Log("INITIALISING USER LEVEL FILES BY COPYING LEVEL TEMPLATES");
                var data = GameData.GetKuchoBuildData();
                string templatesPath = KuchoHelper.FixDirectorySeparators(data.projectPath + BuildPlayerWindow.levelTemplatesPath);
                string levelsPath = KuchoHelper.FixDirectorySeparators(data.projectPath + BuildPlayerWindow.levelsPath);
                string levelTemplates3DModelsPath = KuchoHelper.FixDirectorySeparators(data.projectPath + BuildPlayerWindow.levelTemplates_3DModelsPath);
                string levels3DModelsPath = KuchoHelper.FixDirectorySeparators(data.projectPath + BuildPlayerWindow.levels_3DModelsPath);
                if (!Directory.Exists(levelsPath))
                {
                    Directory.CreateDirectory(levelsPath);
                }

                if (Directory.Exists(levelsPath))
                {
                    for (int i = 1; i < 6; i++)
                    {
                        {
                            string srcPath = KuchoHelper.FixDirectorySeparators(templatesPath + "/Level USER " + i + ".unity");
                            string destPath = KuchoHelper.FixDirectorySeparators(levelsPath + "/Level USER " + i + ".unity");
                            if (File.Exists(srcPath))
                            {
                                if (File.Exists(destPath))
                                {
                                    File.Delete(destPath);
                                }

                                File.Copy(srcPath, destPath);
                                Debug.Log("COPIED LEVEL TEMPLATE " + i + " TO -LEVELS FOLDER");
                            }
                            else
                            {
                                Debug.LogError("I COULD NOT FIND FILE " + srcPath + " DID YOU DELETE THIS LEVEL TEMPLATE?");
                            }

                            srcPath = KuchoHelper.FixDirectorySeparators(levelTemplates3DModelsPath + "/USER " + i + " - BACKGROUND 3D MODELS.prefab");
                            destPath = KuchoHelper.FixDirectorySeparators(levels3DModelsPath + "/USER " + i + " - BACKGROUND 3D MODELS.prefab");
                            if (File.Exists(srcPath))
                            {
                                if (File.Exists(destPath))
                                {
                                    File.Delete(destPath);
                                }

                                File.Copy(srcPath, destPath);
                                //Debug.Log("COPIED 3DMODELS FOR LEVEL TEMPLATE " + i + " TO ITS CORRESPONDING FOLDER");
                            }
                            else
                            {

                            }
                        }
                    }
                    AssetDatabase.Refresh();
                }
                /* FACEPUNCH
                if (!SteamClient.IsValid)
                {
        
                }
                else
                {
                    Steamworks.SteamClient.RunCallbacks();
                }
                */
            }
        }

        updateCount++;
    }

    static void OnPlayModeStateChanged(){
		if (!EditorApplication.isUpdating && !EditorApplication.isPaused && !EditorApplication.isCompiling && !EditorApplication.isPlayingOrWillChangePlaymode)
		{
			Debug.Log("REFRESCANDO ASSET DATABASE");
			AssetDatabase.Refresh();
		}
	}
    public static bool IsPrefabInstance(GameObject gameObject)
    {
       return PrefabUtility.GetPrefabParent(gameObject) != null && PrefabUtility.GetPrefabObject(gameObject.transform) != null;
    }
    public static bool isPrefabOriginal(GameObject gameObject)
    {
        return PrefabUtility.GetPrefabParent(gameObject) == null && PrefabUtility.GetPrefabObject(gameObject.transform) != null;
    }
    public static bool isDisconnectedPrefabInstance(GameObject gameObject)
    {
        return PrefabUtility.GetPrefabParent(gameObject) != null && PrefabUtility.GetPrefabObject(gameObject.transform) == null;
    }
    static void PrefabUpdateDelegate(GameObject go) {
        Debug.Log(go.name +  " PREFAB UPDATEADO"); 
    }
    public static void PlayClip(AudioClip clip, float startTime, bool loop)
    {
        if (!clip)
            return;

        int startSample = (int)(startTime * clip.frequency);

        Assembly assembly = typeof(AudioImporter).Assembly;
        if (assembly != null)
        {
            System.Type audioUtilType = assembly.GetType("UnityEditor.AudioUtil");
            if (audioUtilType != null)
            {
                System.Type[] typeParams = { typeof(AudioClip), typeof(int), typeof(bool) };
                object[] objParams = { clip, startSample, loop };

                MethodInfo method = audioUtilType.GetMethod("PlayClip", typeParams);
                if (method != null)
                {
                    method.Invoke(null, BindingFlags.Static | BindingFlags.Public, null, objParams, null);
                }
            }
        }
    }
    public static void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    public static void OnEditorQuits()
    {
        //SteamClient.Shutdown();
    }
}