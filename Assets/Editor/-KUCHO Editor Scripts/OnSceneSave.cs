using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
//public class OnSceneSave : SaveAssetsProcessor {
public class OnSceneSave : UnityEditor.AssetModificationProcessor {

    static int lastExecutionFrame = -1;
    static string[] OnWillSaveAssets (string[] paths)
    {

        if (Terrain2D.saving3DModelsAsset)
            return null;
        if (Time.frameCount == lastExecutionFrame)// unity llama dos veces aqui no se por que 
            return null;
        lastExecutionFrame = Time.frameCount;
        var currentScene = SceneManager.GetActiveScene();
        string stack = StackTraceUtility.ExtractStackTrace();
        /*bool allGood = true;
        if (BuildPipeline.isBuildingPlayer)
            allGood = false;
        if (allGood)
        {
            if (stack.Contains("FillSpriteTextureWith3DObjects"))
                allGood = false;
            else if (stack.Contains("BuildPlayerWindow.cs"))
                allGood = false;
            else if (stack.Contains("AddressableAssetSettings.cs"))
                allGood = false;
        }
        */
      
        if (stack.Contains("ShortcutManagement"))
        {
            Debug.Log("ON WILL SAVE ASSETS"); // ON SAVE ASSETS 

            var stickerModels = GameObject.FindObjectOfType<StickerModels>();
            if (stickerModels)
                GameObject.DestroyImmediate(stickerModels.gameObject);

            var rootsInScene = currentScene.GetRootGameObjects();

            #region ELIMINA GROUND EDIT

            foreach (GameObject go in rootsInScene)
            {
                if (go.name == "GroundEdit" || go.name == GameData.instance.groundEdit.name)
                {
                    var tc = go.GetComponent<ExplosionStampExtras>();
                    if (tc)
                    {
                        Debug.Log(" GROUND EDIT ENCONTRADO EN ESCENA DESTRUYENDO");
                        Object.DestroyImmediate(go);
                        break;
                    }
                }
            }

            #endregion

            #region COMPRUEBA QUE WORLDMAP ESTA BIEN

            var worldMap = WorldMap.instance;
            if (!worldMap)
            {
                foreach (GameObject go in rootsInScene)
                {
                    worldMap = go.GetComponent<WorldMap>();
                    if (worldMap != null)
                        break;
                }
            }

            if (worldMap && worldMap.topLimit == null) // no esta inicializado, le faltan cosas
            {
                worldMap.InitialiseInEditor();
            }

            #endregion

            bool prefabSelected = false;
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                GameObject selectedGameObject = Selection.gameObjects[0];

                bool selectedGameObjectIsInScene = false;

                foreach (GameObject go in rootsInScene)
                {

                    var allChildren = go.GetComponentsInChildren<Transform>();
                    foreach (Transform child in allChildren)
                    {
                        if (child.gameObject == selectedGameObject)
                        {
                            selectedGameObjectIsInScene = true;
                            break;
                        }
                    }

                    //if (go == selectedGameObject)
                    //    selectedGameObjectIsInScene = true;
                    //else
                    //{
                    //    foreach (Transform child in go.transform)
                    //    {
                    //        if (child.gameObject == selectedGameObject)
                    //            selectedGameObjectIsInScene = true;
                    //    }
                    //    if (selectedGameObjectIsInScene)
                    //        break;
                    //}
                }

                if (!selectedGameObjectIsInScene)
                {
                    Debug.Log("GO SELECCIONADO NO ESTA EN LA ESCENA, EDITANDO EN PREFAB MODE?" + prefabSelected); // ON SAVE ASSETS
                    return null;
                }
                else
                {
                    bool isPartOfPrefabRoot = PrefabUtility.IsAnyPrefabInstanceRoot(selectedGameObject); //true
                    bool isPartOfPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(selectedGameObject); //true

                    if (isPartOfPrefabInstance)
                    {
                        Debug.Log("GO SELECCIONADO EN LA ESCENA Y ES PREFAB");
                        prefabSelected = true;
                        return null;
                    }
                    else
                    {
                        Debug.Log("GO SELECCIONADO EN LA ESCANA Y ES NORMAL");
                        prefabSelected = false;
                    }
                }

                // asi lo comprobaba antes 
                //if (selectedGameObject)
                //{
                //    if (EditorExtras.IsPrefabInstance(selectedGameObject) || EditorExtras.isDisconnectedPrefabInstance(selectedGameObject) || EditorExtras.isPrefabOriginal(selectedGameObject))
                //    {
                //        Debug.Log("PREFAB SELECCIONADO, SEGURAMENTE ESTOY APLICANDO CAMBIOS EN EL , NO SALVANDO SCENE ASI QUE NO DESTRUYO NI CREO MAPAS"); // ON SAVE ASSETS 
                //        prefabGameSelected = true;
                //    }
                //}
            }
            
            // INICIALIZO TODOS LOS SCRIPTS - NECESARIO POR SI ALGUIEN EN LEVEL MAKER MODIFICA EL COLLIDER VISION DE UN GO Y NO LE DA A F1 POR EJEMPLO
            KuchoToolsMenu.InitialiseAllKuchoScripts(new MenuCommand(null, 1)); // 1 == allScene

            if (!prefabSelected && WorldMap.instance && !WorldMap.instance.useDesignedMap)
            {
                // OJO, SI UNITY PIENSA QUE NO HAS MODIFICADO LA ESCENA (comprimir alphadata no cuenta) NO GRABA A DISCO UNA MIERDA ! por eso: PERO NO LO HAGO AQUI POR QUE GRABA SIEMPRE INCLUSO CUANDO NO SE MODIFICA, PONLO ALLI DONDE HAGAS CAMBIOS Y UNITY NO SE ENTERA
//            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

                foreach (Terrain2D terr in WorldMap.terrains2D)
                {
                    if (!terr)
                        WorldMap.instance.PublicOnValidate();

                    if (terr)
                    {
                        if (terr.tileMap)
                        {
                            if (terr.d2dSprite)
                                Debug.LogError(" TERRENO " + terr.groundType + " TIENE TILE Y D2DSPRITE?");

                            DestroySpriteAndTexture(terr);
                        }

                        if (terr.tile && terr.tileMap)
                        {
                            Debug.LogError(" TERRENO " + terr.groundType + " TIENE TILE Y TILEMAP");
                        }
                        else
                        {
                            if (terr.backgroundType == BackgroundType._3DModels)
                            {
                                if (terr.background3DModelsParent != null) // hay models 3d activos (rocas)
                                {
                                    terr.FillSpriteTextureWith3DObjects();
                                }
                                terr.CompressGrayscaleKuchoFormatTexture();
                                
                                DestroySpriteAndTexture(terr);
                            }

                            if (terr.tile)
                            {
                                terr.d2dSprite.CompressAlphaDataShort(); // antes de eliminar el alpha data OJO!
                                DestroySpriteAndTexture(terr);
                            }

                            if (terr.d2dSprite)
                            {
                                Object.DestroyImmediate(terr.d2dSprite.alphaTex); // cargate la textura alpha
                                terr.d2dSprite.alphaData = null;
                                DestroySpriteAndTexture(terr);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("TERRENO NULO, NO PUEDE SER COMPRIMIDO NI SALVADO!");
                    }
                }
            }

            //LevelInfoSerializer.SaveCurrentLevelInfoIntoTextFile();
            //GameData.SerializeCurrentLevel();
            
            DecoThingsCreatedInEditor[] allDecoThingsEditor = MonoBehaviour.FindObjectsOfType<DecoThingsCreatedInEditor>();
            foreach (DecoThingsCreatedInEditor thing in allDecoThingsEditor)
            {
                var allSpritePlanes = thing.GetComponentsInChildren<SpritePlane>();
                foreach (SpritePlane sp in allSpritePlanes)
                {
                    if (sp.gameObject != thing.gameObject)
                    {
                        Object.DestroyImmediate(sp);
                    }
                }
            }
            var materialAndMeshAgent = GameObject.FindObjectOfType<MaterialAndMeshDatabaseAgent>();
            if (materialAndMeshAgent)
                materialAndMeshAgent.InitialiseInEditor();// esto lanza el buscarod de materiales y meshes y se guardan en su base de datos local para luego ser recuperados en run time
            //ShaderDataBase.SerializeCurrentLevelShaders(); // que sentido tiene esto? en el proyecto hijo no siene sentido y en el madre, estoy funcionando con la MATERIAL_dataBase, no necesito guardar los shaders
        }


        return paths;
	}
    static void DestroySpriteAndTexture(Terrain2D terr){
        if (terr.texture)
        {
            Object.DestroyImmediate(terr.texture); // elimina la textura
        }
        if (terr.spriteRenderer && terr.spriteRenderer.sprite)
        {
            Object.DestroyImmediate(terr.spriteRenderer.sprite); // elimina el sprite
        }
        terr.undoAlphaData = null;
    }

}