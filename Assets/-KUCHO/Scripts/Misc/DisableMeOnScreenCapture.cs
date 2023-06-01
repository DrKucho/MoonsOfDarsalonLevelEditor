using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMeOnScreenCapture : MonoBehaviour
{

    public static List<DisableMeOnScreenCapture> instances;

    private bool gameobjectActiveSelf;
    // Update is called once per frame
    void Awake()
    {
        if (instances == null)
            instances = new List<DisableMeOnScreenCapture>();
        instances.Add(this);
    }

    private void OnEnable()
    {
        gameobjectActiveSelf = gameObject.activeSelf;
    }
    private void OnDisable()
    {
        gameobjectActiveSelf = gameObject.activeSelf;
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    public static void DisableAllInstances()
    {
        if (instances != null)
        {
            foreach (DisableMeOnScreenCapture d in instances)
            {
                if (d.gameobjectActiveSelf)
                    d.gameObject.SetActive(false);
            }
        }
    }
    public static void RestoreAllInstances()
    {
        if (instances != null)
        {
            foreach (DisableMeOnScreenCapture d in instances)
            {
                if (d.gameobjectActiveSelf)
                    d.gameObject.SetActive(true);
            }
        }
    }
}

