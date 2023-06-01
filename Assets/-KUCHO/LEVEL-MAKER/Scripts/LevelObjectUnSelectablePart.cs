using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LevelObjectUnSelectablePart : MonoBehaviour
{
    #if UNITY_EDITOR
    private LevelObjectSelectablePart selectable;
    void Update()
    {
        if (Constants.appIsLevelEditor)
        {
            if (!selectable)
                selectable = GetComponentInParent<LevelObjectSelectablePart>();
            else
            {
                if (Selection.activeObject == gameObject)
                    Selection.activeObject = selectable;
            }
        }
    }
#endif
}
