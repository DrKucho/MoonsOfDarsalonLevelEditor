using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
[CreateAssetMenu(fileName = "MeshData", menuName = "Mesh DataBase", order = 51)]

public class MeshDataBase : ScriptableObject
{
 
    public Mesh[] meshes;

    private static MeshDataBase _instance;
    public static MeshDataBase instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameData.instance.meshDataBase;
                if (!_instance)
                {
                    Debug.Log("NO ENCUENTRO MESH DATA");
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

  

    #region MESH STORER / RESTORER
#if UNITY_EDITOR // esto elimina los clones
    
    public bool ShouldBeStoredAndRestored(MeshFilter mf)
    {
        if (mf.Equals(null))
            return false;
        if (mf.sharedMesh == null)
            return false;
        if (mf.sharedMesh.vertexCount <= 0)
            return false;
        if (MaterialDataBase.instance.IsOneOfTheThreeTerrainGameObjects(mf.gameObject))
            return false;
        if (mf.gameObject.GetComponent<SWizSprite>()) // los Swiz sprites tienen un mesh autogenerado segun el frame que sean, no esta guardado a disco, sino en la escena, asi que la referencia no se pierde
            return false;
        if (mf.name == Light2D.CustomSprite.GeneratedMaterialName)
            return false;
        string path = AssetDatabase.GetAssetPath(mf);
        if (path == "" && false)
        {
            Debug.LogError("MESH FILTER:" + mf.name + " IN GAMEOBJECT:" + mf.gameObject +" IS AN INSTANCE; IT CAN'T BE COPIED/RESCUED");
            return false;
        }
        return true;
    }
    public int AddMeshFilterToDatabaseIfNew(Mesh mesh)
    {
        if (GetIndexInMeshDatabase(mesh) == -1)
        {
            if (mesh.name == " Instance" || mesh.name == "")
                Debug.LogError("NO HE PODIDO AÑADIR MESH " + mesh);
            int nullIndex = MeshDatabaseHasNullAt();
            if (nullIndex >= 0)
            {
                meshes[nullIndex] = mesh;
                Debug.Log("ADDED MESH:" + mesh.name + " ON POS:" + nullIndex);
                return nullIndex;
            }
            else
            {
                List<Mesh> list = meshes.ToList();
                list.Add(mesh);
                Debug.Log("ADDED MAT:" + mesh.name + " ON POS:" + (meshes.Length - 1) +  "(END)");
                meshes = list.ToArray();
                return meshes.Length - 1;
            }
        }
        return -1;
    }

    int MeshDatabaseHasNullAt()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i] == null)
                return i;
        }
        return -1;
    }
#endif
    public int GetIndexInMeshDatabase(Mesh m) // devuelve el indice si lo encuentra
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            Mesh m2 = meshes[i];
            if (m2 != null) // podria haberse nuleado alguno de mi database
            {
                if (m != null)
                {
                    if (CompareMeshes(m, m2))
                        return i;
                }
            }
        }
        return -1;
    }
    
    public Mesh GetMeshByIndexAndName(int index, string matName)
    {
        if (index > 0 && index < meshes.Length && meshes[index].name == matName)
            return meshes[index];
        return GetMeshByName(matName);
    }

    public Mesh GetMeshByName(string meshName)
    {
        foreach(Mesh mesh in meshes)
            if (mesh.name == meshName)
                return mesh;
        return null;
    }

    public bool CompareMeshes(Mesh m1, Mesh m2)
    {
        if (m1.name != m2.name)
            return false;
        if (m1 == null && m2 != null)
            return false;
        if (m1 != null && m2 == null)
            return false;
        if (m1.vertices.Length != m2.vertices.Length)
            return false;
        if (!CompareVector3Arrays(m1.vertices, m2.vertices))
            return false;
        if (!CompareIntArrays(m1.triangles, m2.triangles))
            return false;
        if (!CompareVector2Arrays(m1.uv, m2.uv))
            return false;
        if (!CompareVector2Arrays(m1.uv2, m2.uv2))
            return false;
        if (!CompareVector2Arrays(m1.uv3, m2.uv3))
            return false;
        if (!CompareVector2Arrays(m1.uv4, m2.uv4))
            return false;
        if (!CompareVector3Arrays(m1.normals, m2.normals))
            return false;
        if (!CompareVector4Arrays(m1.tangents, m2.tangents))
            return false;
        return true;
        
    }
    bool CompareIntArrays(int[] fa1, int[] fa2)
    {
        if (fa1.Length != fa2.Length)
            return false;
        for (int i = 0; i < fa1.Length; i++)
        {
            if (fa1[i] != fa2[i])
                return false;
        }
        return true;
    }
    bool CompareVector2Arrays(Vector2[] va1, Vector2[] va2)
    {
        if (va1.Length != va2.Length)
            return false;
        for (int i = 0; i < va1.Length; i++)
        {
            if (va1[i] != va2[i])
                return false;
        }
        return true;
    }
    bool CompareVector3Arrays(Vector3[] va1, Vector3[] va2)
    {
        if (va1.Length != va2.Length)
            return false;
        for (int i = 0; i < va1.Length; i++)
        {
            if (va1[i] != va2[i])
                return false;
        }
        return true;
    }
    bool CompareVector4Arrays(Vector4[] va1, Vector4[] va2)
    {
        if (va1.Length != va2.Length)
            return false;
        for (int i = 0; i < va1.Length; i++)
        {
            if (va1[i] != va2[i])
                return false;
        }
        return true;
    }

    public float GetMeshKuchoCRC(Mesh m)
    {
        float r = 0;
        foreach (Vector3 v in m.vertices)
        {
            r += v.x;
            r += v.y;
            r += v.z;
        }
        foreach (Vector2 v in m.uv)
        {
            r += v.x;
            r += v.y;
        }
        return r;
    }

    #endregion
}
