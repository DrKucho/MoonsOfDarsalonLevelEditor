using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class MaterialRestorer2 : MonoBehaviour
{
    public Renderer[] rends;
    public SWizSpriteMaterialAssigner[] matAssigners;
    public Terrain2D[] terrains;

    
    void CreateInstancesOfMaterials()
    {
        rends = FindObjectsOfType<Renderer>();
        foreach (Renderer r in rends)
        {
            r.sharedMaterial = GetMaterialCloneIfItsNotACloneAlreadyOrGetDefaultIfNull(r.sharedMaterial);
        }
        matAssigners = FindObjectsOfType<SWizSpriteMaterialAssigner>();
        foreach (SWizSpriteMaterialAssigner ma in matAssigners)
        {
            ma.material = GetMaterialCloneIfItsNotACloneAlreadyOrGetDefaultIfNull(ma.material);
            ma.GetMainTex();
            ma.matToAssign = ma.material;
        }
        /*
        terrains = FindObjectsOfType<Terrain2D>();
        foreach (Terrain2D t in terrains)
        {
            if (t.tileMaterial == null)
                t.RestoreTileMaterialFromTerrainDataBase();
            t.tileMaterial = GetMaterialCloneIfItsNotACloneAlreadyOrGetDefaultIfNull(t.tileMaterial);
        }
        */
    }
    Material GetMaterialOrGetDefaultMaterial(Material mat)
    {
        if (mat == null)
            mat = MaterialDataBase.instance.defaultSpritesMat;
        return mat;
    }
    Material GetMaterialCloneIfItsNotACloneAlreadyOrGetDefaultIfNull(Material mat)
    {
        if (mat == null)
            mat = MaterialDataBase.instance.defaultSpritesMat;
        else
        {
            if (mat.name.Contains("Clone") || mat.name.Contains("(Instance)"))
                return mat; // no copies !
        }
        return Instantiate(mat);
    }
}
