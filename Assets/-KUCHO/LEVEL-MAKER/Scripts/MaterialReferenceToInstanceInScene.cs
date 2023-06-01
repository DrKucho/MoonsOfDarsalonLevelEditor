using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialReferenceToInstanceInScene : MonoBehaviour
{
    public Renderer rend;
    //public Material instanceMat;
    public string shaderName;
    public string materialName;
    public void OnEnable()
    {
        if (rend.sharedMaterial == null)
            MyLogs.Resolutions("MATERIAL DE " + rend.name + " ES NULL");
        else
        {
            MyLogs.Resolutions("MATERIAL DE " + name + ":" + rend.sharedMaterial.name);
            if (rend.sharedMaterial.shader == null)
                MyLogs.Resolutions("SHADER DE " + rend.name + " ES NULL");
            else
                MyLogs.Resolutions("SHADER DE " + name + ":" + rend.sharedMaterial.shader.name);
        }

        if (true || Application.isPlaying && !Application.isEditor)
        {
            if (true || rend.sharedMaterial == null || rend.sharedMaterial.name.Contains("Error") || rend.sharedMaterial.shader == null || rend.sharedMaterial.shader.name.Contains("Error"))
            {

                if (MaterialDataBase.instance)
                {
                    var restoredMaterial = MaterialDataBase.instance.GetMaterialFromDatabase(materialName);
                    string restoredMaterialName;

                    if (restoredMaterial)
                    {
                        restoredMaterialName = restoredMaterial.name;
                        rend.sharedMaterial = restoredMaterial;
                    }
                    else
                        restoredMaterialName = "NULL";

                    MyLogs.Resolutions("RESTORED MATERIAL " + restoredMaterialName);
                }
                else
                {
                    MyLogs.Resolutions("NO HAY MATERIAL DATABASE INSTANCE");

                }

                /*
                var restoredShader = ShaderDataBase.instance.GetShaderByName(shaderName);
                string restoredShaderName;
                if (restoredShader)
                    restoredShaderName = restoredShader.name;
                else
                    restoredShaderName = "NULL";
                Debug.Log("RESTORED SHADER " + restoredShaderName);
                rend.sharedMaterial.shader = restoredShader;
                */
            }
        }
    }

    public void InitialiseInEditor()
    {
        rend = GetComponent<Renderer>();
        if (rend)
        {
            //Material mat = rend.sharedMaterial;
            //instanceMat = Instantiate(mat);
            //rend.sharedMaterial = instanceMat;
            shaderName = rend.sharedMaterial.shader.name;
            materialName = rend.sharedMaterial.name;
        }
    }
}