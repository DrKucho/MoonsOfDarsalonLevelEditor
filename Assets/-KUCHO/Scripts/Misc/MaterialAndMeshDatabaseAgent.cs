using System;
using System.Collections;
using System.Collections.Generic;
//using AdvancedInspector;
using Light2D;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MaterialAndMeshDatabaseAgent : MonoBehaviour
{
    [System.Serializable]
    public class RendererRecoveryInfo
    {
        public int indexInDatabase;
        public string matName;
        public Renderer script;
        public RendererRecoveryInfo(Renderer rend)
        {
            script = rend;
            Material m = script.sharedMaterial;
            matName = m.name;
            indexInDatabase = MaterialDataBase.instance.GetIndexInMiscMaterialDatabase(m);
            if (false && indexInDatabase == -1)
                Debug.LogError(" MATERIAL " + m + " DE GAMEOBJECT " + script.gameObject + " NO SE ENCUENTRA EN LA BASE DE DATOS, Y NO PODRA SER RECUPERADO AL CARGAR NIVEL");
        }

        public void Restore()
        {
            Material mat = null;
            mat = MaterialDataBase.instance.GetMaterialFromMiscListByIndexAndName(indexInDatabase, matName);
            script.sharedMaterial = mat;
            MyLogs.PrintMaterialDataToResolutions("RESTAURADO RENDERER " + script + " INDEX:" + indexInDatabase , mat);
        }
    }
    [System.Serializable]
    public class ParticleSystemRendererRecoveryInfo
    {
        public int indexInDatabase;
        public string matName;
        public ParticleSystem script;

        public override string ToString()
        {
            return matName;
        }
        public ParticleSystemRendererRecoveryInfo(ParticleSystem ps)
        {
            script = ps;
            var rend = script.GetComponent<ParticleSystemRenderer>();
            Material m = rend.sharedMaterial;
            matName = m.name;
            indexInDatabase = MaterialDataBase.instance.GetIndexInMiscMaterialDatabase(m);
            if (false && indexInDatabase == -1)
                Debug.LogError(" MATERIAL " + m + " DE GAMEOBJECT " + script.gameObject + " NO SE ENCUENTRA EN LA BASE DE DATOS, Y NO PODRA SER RECUPERADO AL CARGAR NIVEL");
        }

        public void Restore()
        {
            Material mat = null;
            mat = MaterialDataBase.instance.GetMaterialFromMiscListByIndexAndName(indexInDatabase, matName);
            var rend = script.GetComponent<ParticleSystemRenderer>();
            rend.sharedMaterial = mat;
            MyLogs.PrintMaterialDataToResolutions("RESTAURADO PARTICLE SYSTEM RENDERER " + script + " INDEX:" + indexInDatabase , mat);
        }
    }
    [System.Serializable]
    public class MatAssignerRecoveryInfo
    {
        public int indexInDatabase;
        public string matName;
        public SWizSpriteMaterialAssigner script;
        public MatAssignerRecoveryInfo(SWizSpriteMaterialAssigner matAssigner)
        {
            script = matAssigner;
            Material m = matAssigner.material;
            matName = m.name;

            indexInDatabase = MaterialDataBase.instance.GetIndexInMiscMaterialDatabase(m);
            if (false && indexInDatabase == -1)
                Debug.LogError(" MATERIAL " + m + " DE GAMEOBJECT " + script.gameObject + " NO SE ENCUENTRA EN LA BASE DE DATOS, Y NO PODRA SER RECUPERADO AL CARGAR NIVEL");
        }

        public void Restore()
        {
            Material mat = null;
            mat = MaterialDataBase.instance.GetMaterialFromMiscListByIndexAndName(indexInDatabase, matName);
            script.material = mat;
            script.matToAssign = mat;
            if(script.asignMaterialAtStart)
                script.AssignMaterial(null);
            MyLogs.PrintMaterialDataToResolutions("RESTAURADO MAT ASSIGNER " + script + " INDEX:" + indexInDatabase , mat);
        }
    }
    [System.Serializable]
    public class LightObstacleRecoveryInfo
    {
        public int indexInDatabase;
        public string matName;
        public LightObstacleGenerator script;
        public LightObstacleRecoveryInfo(LightObstacleGenerator lightObstacleGenerator)
        {
            script = lightObstacleGenerator;
            Material m = script.Material;
            matName = m.name;

            indexInDatabase = MaterialDataBase.instance.GetIndexInMiscMaterialDatabase(m);
            if (false && indexInDatabase == -1)
                Debug.LogError(" MATERIAL " + m + " DE GAMEOBJECT " + script.gameObject + " NO SE ENCUENTRA EN LA BASE DE DATOS, Y NO PODRA SER RECUPERADO AL CARGAR NIVEL");
        }

        public void Restore()
        {
            Material mat = null;
            mat = MaterialDataBase.instance.GetMaterialFromMiscListByIndexAndName(indexInDatabase, matName);
            script.Material = mat;
            MyLogs.PrintMaterialDataToResolutions("RESTAURADO LIGHT OBSTACLE " + script + " INDEX:" + indexInDatabase , mat);
        }
    }
    [System.Serializable]
    public class CustomSpriteRecoveryInfo
    {
        public int indexInDatabase;
        public string matName;
        public CustomSprite script;
        public CustomSpriteRecoveryInfo(CustomSprite customSprite)
        {
            script = customSprite;
            Material m = script.Material;
            matName = m.name;

            indexInDatabase = MaterialDataBase.instance.GetIndexInMiscMaterialDatabase(m);
            if (false && indexInDatabase == -1)
                Debug.LogError(" MATERIAL " + m + " DE GAMEOBJECT " + script.gameObject + " NO SE ENCUENTRA EN LA BASE DE DATOS, Y NO PODRA SER RECUPERADO AL CARGAR NIVEL");
        }

        public void Restore()
        {
            Material mat = null;
            mat = MaterialDataBase.instance.GetMaterialFromMiscListByIndexAndName(indexInDatabase, matName);
            script.Material = mat;
            script.ReCreateMaterial();
            MyLogs.PrintMaterialDataToResolutions("RESTAURADO CUSTOM SPRITE " + script + " INDEX:" + indexInDatabase , mat);
        }
    }
    [System.Serializable]
    public class ExplosionInvokerRecoveryInfo
    {
        public string[] matName;
        public string[] lightMatName;
        public ExplosionInvoker script;
        public ExplosionInvokerRecoveryInfo(ExplosionInvoker exploInvoker)
        {
            script = exploInvoker;
            matName = new string[script.death.part.Length];
            lightMatName = new string[script.death.part.Length];
            for (int i = 0; i < script.death.part.Length; i++) // solo death the es la unica que uso
            {
                if (script.death.part[i].material)
                    matName[i] = script.death.part[i].material.name;
                else
                    matName[i] = "";
                
                if(script.death.part[i].lightSpriteMaterial)
                    lightMatName[i] = script.death.part[i].lightSpriteMaterial.name;
                else
                    lightMatName[i] = "";
            }
            //en este decido ignorar el tema del indexIndatabase porque se me complica al ser varios materiales por script , tiro siempre del nombre y a correr
        }

        public void Restore()
        {
            Material mat = null;
            for (int i = 0; i < script.death.part.Length; i++) // solo death the es la unica que uso
            {
                mat = MaterialDataBase.instance.GetMiscMaterialByName(matName[i]);
                script.death.part[i].material = mat;
                MyLogs.PrintMaterialDataToResolutions("RESTAURADO MATERIAL DE EXPLO INVOKER" + script + " ", mat);
                mat = MaterialDataBase.instance.GetMiscMaterialByName(lightMatName[i]);
                script.death.part[i].lightSpriteMaterial = mat;
                MyLogs.PrintMaterialDataToResolutions("RESTAURADO LIGHT SPRITE MATERIAL DE EXPLO INVOKER" + script + " ", mat);
            }
        }
    }
    [System.Serializable]
    public class MeshRecoveryInfo
    {
        public int indexInDatabase;
        public string meshName;
        public MeshFilter script;
        public MeshRecoveryInfo(MeshFilter meshFilter)
        {
            script = meshFilter;
            Mesh m = script.sharedMesh;
            meshName = m.name;

            indexInDatabase = MeshDataBase.instance.GetIndexInMeshDatabase(m);
            if (false && indexInDatabase == -1)
                Debug.LogError(" MESH " + m + " DE GAMEOBJECT " + script.gameObject + " NO SE ENCUENTRA EN LA BASE DE DATOS, Y NO PODRA SER RECUPERADO AL CARGAR NIVEL");
        }

        public void Restore()
        {
            Mesh mesh = null;
            mesh = MeshDataBase.instance.GetMeshByIndexAndName(indexInDatabase, meshName);
            script.sharedMesh = mesh;
            if (mesh)
                MyLogs.Resolutions("RESTAURADO MESH FILTER:" + script + " INDEX:" + indexInDatabase + " MESH NAME:" + mesh.name + " CRC:" + MeshDataBase.instance.GetMeshKuchoCRC(mesh));
            else
                MyLogs.Resolutions("NO RESTAURO MESH FILTER:" + script + " INDEX:" + indexInDatabase + " PORQUE MESH ES NULL");

        }
    }

    public bool FindThings;
    public string meshFind;
    public string matFind;
    
    public int materialCount = 0;
    public int meshCount = 0;
    
    public List<RendererRecoveryInfo> rendererRecoveryInfos;
    public List<ParticleSystemRendererRecoveryInfo> particleSystemRendererRecoveryInfos;
    public List<MatAssignerRecoveryInfo> matAssignerRecoveryInfos;
    public List<LightObstacleRecoveryInfo> lightObstaclesRecoveryInfos;
    public List<CustomSpriteRecoveryInfo> customSpritesRecoveryInfos;
    public List<ExplosionInvokerRecoveryInfo> exploInvokersRecoveryInfos;
    public List<MeshRecoveryInfo> meshRecoveryInfos;
    
    private void OnValidate()
    {
        if (FindThings)
        {
            FindThings = false;
            if (meshFind != "")
            {
                Debug.Log("BUSCANDO '" + meshFind + " EN TODOS LOS MESHES DE LA LISTA");

                foreach (MeshRecoveryInfo r in meshRecoveryInfos)
                {
                    if (r.script.name.Contains(meshFind))
                        Debug.Log("ENCONTRADO EN " + r.script.name);
                }
            }
            if (matFind != "")
            {
                Debug.Log("BUSCANDO '" + matFind + " EN TODOS LOS MATERIALS DE LA LISTA");

                foreach (MatAssignerRecoveryInfo r in matAssignerRecoveryInfos)
                {
                    if (r.script.name.Contains(matFind))
                        Debug.Log("ENCONTRADO EN " + r.script.name);
                }
                foreach (RendererRecoveryInfo r in rendererRecoveryInfos)
                {
                    if (r.script.name.Contains(matFind))
                        Debug.Log("ENCONTRADO EN " + r.script.name);
                }
                foreach (LightObstacleRecoveryInfo r in lightObstaclesRecoveryInfos)
                {
                    if (r.script.name.Contains(matFind))
                        Debug.Log("ENCONTRADO EN " + r.script.name);
                }
                foreach (CustomSpriteRecoveryInfo r in customSpritesRecoveryInfos)
                {
                    if (r.script.name.Contains(matFind))
                        Debug.Log("ENCONTRADO EN " + r.script.name);
                }
                foreach (ExplosionInvokerRecoveryInfo r in exploInvokersRecoveryInfos)
                {
                    if (r.script.name.Contains(matFind))
                        Debug.Log("ENCONTRADO EN " + r.script.name);
                }
            }
        }
    }
    void Start()
    {
        RestoreMaterialsAndMeshesInThisLevel();
    }
    
#if UNITY_EDITOR
    public void InitialiseInEditor()
    {
        FindMaterialsAndMeshesToRecoverInThisLevel();
    }

    //[Inspect]
    void FindMaterialsAndMeshesToRecoverInThisLevel()
    {
        ShaderProp.Initialise();
        WorldMap.instance.LookForDestructibles();
        var db = MaterialDataBase.instance;
        if (!KuchoHelper.IsUserLevel())
        {
            MyLogs.Resolutions("ESTE NIVEL NO ES UN USER LEVEL, NO HAY MATERIALES QUE RESTAURAR");
            return;
        }

        int i = 0; // para localizar entradas en inspector si es necesario
        materialCount = 0;
        customSpritesRecoveryInfos = new List<CustomSpriteRecoveryInfo>();
        var all = FindObjectsOfType<CustomSprite>();
        foreach (CustomSprite c in all)
        {
            if (db.ShouldBeStoredAndRestored(c.Material, c.gameObject))
            {
                customSpritesRecoveryInfos.Add(new CustomSpriteRecoveryInfo(c));
                materialCount++;
            }
        }

        rendererRecoveryInfos = new List<RendererRecoveryInfo>();
        var rends = FindObjectsOfType<Renderer>();
        i = 0;
        foreach (Renderer r in rends)
        {
            if (r.sharedMaterial && !db.IsTerrainShader(r.sharedMaterial.shader))
            {
                if (db.ShouldBeStoredAndRestored(r.sharedMaterial, r.gameObject))
                {
                    rendererRecoveryInfos.Add(new RendererRecoveryInfo(r));
                    materialCount++;
                    i++;
                }
            }
        }
        
        particleSystemRendererRecoveryInfos = new List<ParticleSystemRendererRecoveryInfo>();
        var allp = FindObjectsOfType<ParticleSystem>(true);
        foreach (ParticleSystem ps in allp)
        {
            var r = ps.GetComponent<Renderer>();
            if (r.sharedMaterial && !db.IsTerrainShader(r.sharedMaterial.shader))
            {
                if (db.ShouldBeStoredAndRestored(r.sharedMaterial, r.gameObject))
                {
                    particleSystemRendererRecoveryInfos.Add(new ParticleSystemRendererRecoveryInfo(ps));
                    materialCount++;
                }
            }
        }

        matAssignerRecoveryInfos = new List<MatAssignerRecoveryInfo>();
        var allm = FindObjectsOfType<SWizSpriteMaterialAssigner>();
        foreach (SWizSpriteMaterialAssigner ma in allm)
        {
            if (db.ShouldBeStoredAndRestored(ma.material, ma.gameObject))
            {
                matAssignerRecoveryInfos.Add(new MatAssignerRecoveryInfo(ma));
                materialCount++;
            }
        }

        lightObstaclesRecoveryInfos = new List<LightObstacleRecoveryInfo>();
        var allg = FindObjectsOfType<LightObstacleGenerator>();
        foreach (LightObstacleGenerator l in allg)
        {
            if (db.ShouldBeStoredAndRestored(l.Material, l.gameObject))
            {
                lightObstaclesRecoveryInfos.Add(new LightObstacleRecoveryInfo(l));
                materialCount++;
            }
        }
        
        exploInvokersRecoveryInfos = new List<ExplosionInvokerRecoveryInfo>();
        var alle = FindObjectsOfType<ExplosionInvoker>();
        foreach (ExplosionInvoker l in alle)
        { // warning aqui no estoy comprobando si el material deberia ser stored and restored
            exploInvokersRecoveryInfos.Add(new ExplosionInvokerRecoveryInfo(l));
            materialCount++;
        }
        
        Debug.Log("FOUND " + materialCount + " MATERIALS TO BE RECOVERED");

        meshCount = 0;
        meshRecoveryInfos = new List<MeshRecoveryInfo>();
        var allme = FindObjectsOfType<MeshFilter>();
        foreach (MeshFilter mf in allme)
        {
            if (MeshDataBase.instance.ShouldBeStoredAndRestored(mf))
            {
                meshRecoveryInfos.Add(new MeshRecoveryInfo(mf));
                meshCount++;
            }
        }
        Debug.Log("FOUND " + meshCount + " MESHES TO BE RECOVERED");
    }
    
#endif
    //[Inspect]
    public void RestoreMaterialsAndMeshesInThisLevel()
    {
        MyLogs.ResolutionsAndUnityConsole(" !!!!!!!!!!!!!!!!!!!! " + SceneManager.GetActiveScene().name.ToUpper() + " EMPEZANDO RESTAURACION DE MATERIALES");
        if (!KuchoHelper.IsUserLevel())
        {
            if (!Application.isPlaying)
            {
                MyLogs.ResolutionsAndUnityConsole("ESTE NIVEL NO ES UN USER LEVEL, NO HAY MATERIALES QUE RESTAURAR");
            }
            return;
        }

        int i = 0;
        foreach (RendererRecoveryInfo ri in rendererRecoveryInfos)
        {
            ri.Restore();
            i++;// para localizar el elemento en inspector si es necesario
        }
        foreach (ParticleSystemRendererRecoveryInfo ri in particleSystemRendererRecoveryInfos)
        {
            ri.Restore();
        }
        foreach (MatAssignerRecoveryInfo ri in matAssignerRecoveryInfos)
        {
            ri.Restore();
        }
        foreach (LightObstacleRecoveryInfo ri in lightObstaclesRecoveryInfos)
        {
            ri.Restore();
        }
        foreach (CustomSpriteRecoveryInfo ri in customSpritesRecoveryInfos)
        {
            ri.Restore();
        }
        foreach (ExplosionInvokerRecoveryInfo ri in exploInvokersRecoveryInfos)
        {
            ri.Restore();
        }
        
        foreach (MeshRecoveryInfo ri in meshRecoveryInfos)
        {
            ri.Restore();
        }
    }
}
