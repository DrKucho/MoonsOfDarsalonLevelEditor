using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Please don't add this component manually
[AddComponentMenu("")]
public class SWizUpdateManager : MonoBehaviour {
	static SWizUpdateManager inst;
	static SWizUpdateManager Instance {
		get {
			if (inst == null) {
				inst = GameObject.FindObjectOfType(typeof(SWizUpdateManager)) as SWizUpdateManager;
				if (inst == null) {
					GameObject go = new GameObject("@SWizUpdateManager");
					go.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
					inst = go.AddComponent<SWizUpdateManager>();
					DontDestroyOnLoad(go);
				}
			}
			return inst;
		}
	}

	// Add textmeshes to the list here
	// Take care to not add twice
	// Never queue in edit mode
	public static void QueueCommit( SWizTextMesh textMesh ) {
#if UNITY_EDITOR
		if (!Application.isPlaying) {
		}
		else 
#endif
		{
			Instance.QueueCommitInternal( textMesh );
		}
	}

	// This can be called more than once, and we do - to try and catch all possibilities
	public static void FlushQueues() {
#if UNITY_EDITOR
		if (Application.isPlaying) {
			Instance.FlushQueuesInternal();
		}
#else
		Instance.FlushQueuesInternal();
#endif
	}

	void OnEnable() {
		// for when the assembly is reloaded, coroutine is killed then
		StartCoroutine(coSuperLateUpdate());
	}

	// One in late update
	void LateUpdate() {
		FlushQueuesInternal();
	}

	IEnumerator coSuperLateUpdate() {
		FlushQueuesInternal();
		yield break;
	}

	void QueueCommitInternal( SWizTextMesh textMesh ) {
		textMeshes.Add( textMesh );
	}

	void FlushQueuesInternal() {
		int count = textMeshes.Count;
		for (int i = 0; i < count; ++i) {
			SWizTextMesh tm = textMeshes[i];
			if (tm != null) {
			}
		}
		textMeshes.Clear();
	}

	// Preallocate these lists to avoid allocation later
	[SerializeField] List<SWizTextMesh> textMeshes = new List<SWizTextMesh>(64);
}
