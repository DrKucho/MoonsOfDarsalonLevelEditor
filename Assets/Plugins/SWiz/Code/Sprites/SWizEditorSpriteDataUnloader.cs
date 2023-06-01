// #define ENABLE_UNLOAD_MANAGER
// Disabled, was an old workaround on memory hungry platforms

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // KUCHO HACK

// This is deliberately an ExecuteInEditMode object as opposed to InitializeOnLoad static
// to get it to unload stuff when scripts are reloaded, and reload at the correct point.
[ExecuteInEditMode]
public class SWizEditorSpriteDataUnloader : MonoBehaviour
{

    public static void Register(SWizSpriteCollectionData data)
    {
#if ENABLE_UNLOAD_MANAGER && UNITY_EDITOR
		GetInst().DestroyDisconnectedResources();
#endif
    }

    public static void Unregister(SWizSpriteCollectionData data)
    {
#if ENABLE_UNLOAD_MANAGER && UNITY_EDITOR
		GetInst();
#endif
    }

#if ENABLE_UNLOAD_MANAGER && UNITY_EDITOR

	static SWizEditorSpriteDataUnloader _inst = null;	 
	static SWizEditorSpriteDataUnloader GetInst() {
		if (_inst == null) {
			SWizEditorSpriteDataUnloader[] allInsts = Resources.FindObjectsOfTypeAll(typeof(SWizEditorSpriteDataUnloader)) as SWizEditorSpriteDataUnloader[];
			_inst = (allInsts.Length > 0) ? allInsts[0] : null;
			if (_inst == null) {
				GameObject go = new GameObject("@SWizEditorSpriteDataUnloader");
				go.hideFlags = HideFlags.HideAndDontSave;
				_inst = go.AddComponent<SWizEditorSpriteDataUnloader>();
			}
		}
		return _inst;
	}



	void OnEnable() {
		UnityEditor.EditorApplication.update += EditorUpdate;
	}

	void OnDisable() {
		UnityEngine.Object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)) as UnityEngine.Object[];
		DestroyInternalObjects( allObjects );		
		UnityEditor.EditorApplication.update -= EditorUpdate;
	}

	void DestroyInternalObjects(UnityEngine.Object[] allObjects) {
		// int numDestroyed = 0;
		foreach (UnityEngine.Object obj in allObjects) { 
			if (obj.name.IndexOf(SWizSpriteCollectionData.internalResourcePrefix) == 0)  {
				Object.DestroyImmediate(obj); 
				// numDestroyed++;
			}
		}
		// Debug.Log("Destroyed " + numDestroyed + " internal assets");
	}
 
	public void DestroyDisconnectedResources() {
		List<UnityEngine.Object> allObjects = new List<UnityEngine.Object>( Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)) as UnityEngine.Object[] );
		SWizSpriteCollectionData[] objects = Resources.FindObjectsOfTypeAll(typeof(SWizSpriteCollectionData)) as SWizSpriteCollectionData[];
		foreach (SWizSpriteCollectionData data in objects) {
			if (data.needMaterialInstance) {
				if (data.textureInsts != null) {
					foreach (Texture2D tex in data.textureInsts) {
						if (allObjects.Contains(tex)) {
							allObjects.Remove(tex);
						}
					}
				}
				if (data.materialInsts != null) {
					foreach (Material mtl in data.materialInsts) {
						if (allObjects.Contains(mtl)) {
							allObjects.Remove(mtl);
						}
					}
				}
			}
		}
		DestroyInternalObjects( allObjects.ToArray() );
	}

//	public string oldScene = "";
//	void EditorUpdate() {
//		bool needDestroy = false;
//#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
//		if (oldScene != UnityEditor.EditorApplication.currentScene) {
//			oldScene = UnityEditor.EditorApplication.currentScene;
//			needDestroy = true;
//		}
//#else
//		if (oldScene != UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path) {
//			oldScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
//			needDestroy = true;
//		}
//#endif

//		if (needDestroy) {
//			DestroyDisconnectedResources();
//		}
//	}
//#endif

    // KUCHO HACK todo esta funcion sustituye a la anterior
    public Scene oldScene; // KUCHO HACK mejor usar una variable tipo Scene que una string, la comparacion es mas rapida
    void EditorUpdate()
    {
        bool needDestroy = false;
#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        if (oldScene != UnityEditor.EditorApplication.currentScene) {
        oldScene = UnityEditor.EditorApplication.currentScene;
        needDestroy = true;
        }
#else
        Scene currentScene = SceneManager.GetActiveScene();
        if (oldScene != currentScene)
        {
            oldScene = currentScene;
            needDestroy = true; // esto solo esta por compatibilidad con versiones anteriores de unity que no voy a volver a usar pero por si acaso lo dejo asi
        }
#endif

        if (needDestroy)
        {
            DestroyDisconnectedResources();
        }
    }
#endif
}
