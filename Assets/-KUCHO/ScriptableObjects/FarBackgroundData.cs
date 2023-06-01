using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FarBackground Data", menuName = "FarBackground Data", order = 51)]

public class FarBackgroundData : ScriptableObject
{
    public List<GameObject> parallax;
    public string[] parallaxNames;
    public GameObject sky;
    public SkyManager.SkyTexttureWarap[] skyTexturesWrap;
    
    private static FarBackgroundData _instance;
    public static FarBackgroundData instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameData.instance.farBackgroundData;
                if (!_instance)
                {
                    Debug.Log("NO ENCUENTRO FAR BACKGROUND DATA EN GAME DATA");
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public void InitialiseInEditor()
    {
        if (GUILayout.Button("DO PARALLAX NAMES"))
        {
            parallaxNames = new string[parallax.Count];
            for (int i = 0; i < parallax.Count; i++)
            {
                string n = parallax[i].name;
                string N = n.ToUpper();
                if (N.StartsWith("PARALLAX "))
                    n = n.Substring(9, n.Length - 9);
                if (N.StartsWith("PARALLAX"))
                    n = n.Substring(8, n.Length - 8);
                parallaxNames[i] = n;
            }
        }
    }
}
