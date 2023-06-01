// Sistema para que funcionen los materiales/Shaders compilados para windows en un osx y vice versa
// este objeto es inclusido en resources como parte del core del juego que tiene los shaders compilados para cada sistema correctamente
// se actualiza solo al buildear con los shaders incluidos en el nivel, gracias a que he incluido llamada desde levelDifficulty.OnValidate
// en modo Play los scripts pertinentes llaman a GetShaderFromList para pillar la version del shader compilada para su sistema
// no parece funcionar , me estba haciendo todos los shaders rosa, probablemente esta pillando la info del resorces folder compilado para la ultima compilacion y esta era windows
// lo cual se solucionaria con un if para que solo lo hiciera en build (e incluso solo en WIN, ya puestos...)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ShaderData", menuName = "Shader Data Base", order = 51)]
public class ShaderDataBase : ScriptableObject
{
    [System.Serializable]
    public class LevelShaderList
    {
        public string name;
        public List<Shader> shaders;

        public LevelShaderList(string levelName)
        {
            name = levelName;
            shaders = new List<Shader>();
        }

        public override string ToString()
        {
            if (name == null || shaders == null)
                return "";
            return name + " - " + shaders.Count + " Shaders";
        }
    }

    private static ShaderDataBase _instance;
    public static ShaderDataBase instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameData.instance.shaderDataBase;
                if (!_instance)
                {
                    Debug.Log("NO ENCUENTRO SHADER DATA BASE EN RESOURCES");
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    public string testFind;
    public List<LevelShaderList> levelShaders;
    public LevelShaderList allShaders;
    public Shader standard3D;
    public Shader standard2D;

    //[AdvancedInspector.Inspect]
    public void ReconstructAllShaderListFromLevelLists()
    {
        foreach (LevelShaderList list in levelShaders)
        {
            foreach (Shader sh in list.shaders)
            {
                if (!allShaders.shaders.Contains(sh))
                    allShaders.shaders.Add(sh);
            }
        }
    }

    public static void SerializeCurrentLevelShaders()
    {
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var levelShaderList = instance.GetShaderLGetList(sceneName);
        var allMonos = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour mono in allMonos)
        {
            if (mono != null)
            {
                var type = mono.GetType();
                if (type != null)
                {
                    UnityEngine.Object obj = mono;
                    FieldInfo[] fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        if (field.FieldType == typeof(Material))
                        {
                            try
                            {
                                Material m = field.GetValue(obj) as Material;
                                if (m)
                                    AddShadderIfNew(m.shader, levelShaderList); 
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e, obj);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("UN MONOBEHABIOUR EN GO " + mono.name + " ES NUL??");
            }
        }
        var allRends = FindObjectsOfType<Renderer>();

        foreach (Renderer rend in allRends)
        {
            foreach (Material m in rend.sharedMaterials)
            {
                if (m)
                    AddShadderIfNew(m.shader, levelShaderList);
            }
        }
    }

    LevelShaderList GetShaderLGetList(string levelName)
    {
        foreach (LevelShaderList lm in levelShaders)
        {
            if (lm.name == levelName)
                return lm;
        }
        // no existe
        var a = new LevelShaderList(levelName);
        levelShaders.Add(a);
        return a;
    }

    //[AdvancedInspector.Inspect]
    public void CleanAll()
    {
        foreach (LevelShaderList lm in levelShaders)
        {
           lm.shaders.Clear(); 
        }
    }
    public static void AddShadderIfNew(Shader s, LevelShaderList lmList)
    {
        if (s == null)
            return;
        
        if (!instance.allShaders.shaders.Contains(s))
        {
            instance.allShaders.shaders.Add(s);
        }
        
        if (lmList == null)
            return;
        if (lmList.shaders == null)
            return;

        if (!lmList.shaders.Contains(s))
        {
            lmList.shaders.Add(s);
        }
    }

    public Shader GetShaderByName(string name)
    {
        foreach(Shader sh in allShaders.shaders)
            if (sh.name == name)
                return sh;
        return null;
    }

    public Shader GetShaderFromList(Shader sha)
    {
        var levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        var list = GetShaderLGetList(levelName);
        if (sha == null)
            return null;
        
        foreach (Shader s in list.shaders)
        {
            if (s.name == sha.name)
            {
                return s;
            }
        }
        return sha;// si no lo encuentra retorna el original 
    }
    

    public void RestoreShaders()
    {
        Debug.Log("-BUSCANDO SHADERS QUE RESTAURAR EN LEVEL " + SceneManager.GetActiveScene().name);

        string log = "";
        var rends = FindObjectsOfType<MeshRenderer>();
        foreach (MeshRenderer r in rends)
        {
            if (r.sharedMaterial)
            {

                if (r.sharedMaterial.shader.name.Contains("Error"))
                {
                    if (r.bounds.size.z < 1) // sprite Swiz
                    {
                        log += "-RESTAURANDO SHADER DE " + r.gameObject.name + " POR STANDARD 2D SHADER";
                        r.sharedMaterial.shader = standard2D;
                    }
                    else
                    {
                        log += "-RESTAURANDO SHADER DE " + r.gameObject.name + " POR STANDARD 3D SHADER";
                        r.sharedMaterial.shader = standard3D;
                    }
                }
                else
                {
                    log += r.gameObject.name + " TIENE MATERIAL:" + r.sharedMaterial.name + " Y SHADER:" + r.sharedMaterial.shader.name;
                }
            }
            else
            {
                log += "-INTENTO RESTAURAR SHADER DE " + r.gameObject.name + " PERO NO TIENE SHARED MATERIAL";
            }

            log += "\n";
        }
        log += "\n";

        var sprRends = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer r in sprRends)
        {
            
            if (r.sharedMaterial)
            {
                if (r.sharedMaterial.shader.name.Contains("Error"))
                {
                    log +="-RESTAURANDO SHADER DE " + r.gameObject.name + " POR STANDARD 2D SHADER";
                    r.sharedMaterial.shader = standard2D;
                }
                else
                {
                    log += r.gameObject.name + " TIENE MATERIAL:" + r.sharedMaterial.name + " Y SHADER:" + r.sharedMaterial.shader.name;
                }
            }
            else
            {
                log +="-INTENTO RESTAURAR SHADER DE " + r.gameObject.name + " PERO NO TIENE SHARED MATERIAL";
            }
            log += "\n";
        }
        Debug.Log(log);
    }

}
