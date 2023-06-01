using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
public class MyEditorShortcuts : Editor
{
    [MenuItem("GameObject/ActiveToggle _a")]
    static void ToggleActivationSelection()
    {
        if (Application.isPlaying) // raton
            return;

        foreach(GameObject go in Selection.gameObjects)
            go.SetActive(!go.activeSelf);
        
        if (!EditorApplication.isPlaying)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    [MenuItem("GameObject/ZeroLocalPos #S")]
    static void ZeroLocalPosition()
    {
        if (Application.isPlaying)
            return;
        var activeGO = Selection.activeGameObject;
        if (activeGO && !activeGO.name.StartsWith("GroundEdit2"))
        {
            activeGO.transform.localPosition = Vector3.zero;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
    [MenuItem("GameObject/ZeroLocalRotation #D")]
    static void ZeroLocalRotation()
    {
        if (Application.isPlaying)
            return;
        var activeGO = Selection.activeGameObject;
        if (activeGO && !activeGO.name.StartsWith("GroundEdit2"))
        {
            activeGO.transform.localRotation = Quaternion.identity;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
    [MenuItem("GameObject/RandomRotation _D")]
    static void RandomRotationActivationSelection()
    {
        if (Application.isPlaying)
            return;
        if (!Application.isPlaying)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                if (!go.name.StartsWith("GroundEdit2"))
                {
                    var sWizSpr = go.GetComponentInChildren<SWizSprite>();
                    var sRend = go.GetComponentInChildren<SpriteRenderer>();
                    var mRend = go.GetComponentInChildren<MeshRenderer>();
                    if (!sWizSpr)
                    {
                        if (!sRend)
                        {
                            if (mRend) // solo roto los objetos 3D
                            {
                                Vector3 rotation;
                                rotation.x = Random.Range(0, 360);
                                rotation.y = Random.Range(0, 360);
                                rotation.z = Random.Range(0, 360);
                                mRend.gameObject.transform.eulerAngles = rotation;
                                Debug.Log("RANDOM ROTATION TO " + mRend.name + " APPLIED");
                            }
                            else
                            {
                                Debug.Log("RANDOM ROTATION ABORTED BECAUSE THE GAMEOBJECT DOES NOT HAVE A MESH RENDERER");
                            }
                        }
                        else
                        {
                            Debug.Log("RANDOM ROTATION ABORTED BECAUSE THE GAMEOBJECT HAS A SPRITE RENDERER " + sRend.name);
                        }
                    }
                    else
                    {
                        Debug.Log("RANDOM ROTATION ABORTED BECAUSE THE GAMEOBJECT HAS A SPRITE WIZARD SPRITE " + sWizSpr.name);
                    }
                }
            }
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}