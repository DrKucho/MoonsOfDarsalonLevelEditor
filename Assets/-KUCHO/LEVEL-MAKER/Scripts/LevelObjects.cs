using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelObjects : MonoBehaviour
{
#if UNITY_EDITOR
	public static LevelObjects instance;
	public static List<Transform> levelObjectListParentsThatNeedToBeZeroed;
	void Update(){
		transform.position = Constants.zero3;
		if (levelObjectListParentsThatNeedToBeZeroed != null)
		{
			int end = levelObjectListParentsThatNeedToBeZeroed.Count;
			for (int i = 0; i < end; i++)
			{
				var t = levelObjectListParentsThatNeedToBeZeroed[i];
				if (t != null)
					t.position = Constants.zero3;
				else
				{
					levelObjectListParentsThatNeedToBeZeroed.RemoveAt(i);
					i -= 1;
					end -= 1;
				}
			}
		}
	}

	public static void AddToTransformParentList(Transform t)
	{
		if (levelObjectListParentsThatNeedToBeZeroed == null)
			levelObjectListParentsThatNeedToBeZeroed = new List<Transform>();
		if (!levelObjectListParentsThatNeedToBeZeroed.Contains(t))
			levelObjectListParentsThatNeedToBeZeroed.Add(t);
	}
#endif
}
