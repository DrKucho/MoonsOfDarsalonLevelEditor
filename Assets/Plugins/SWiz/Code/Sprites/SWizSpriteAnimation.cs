using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FrameEventTransport { NONE, PAUSE, STOP, FPS } // KUCHO HACK

[System.Serializable]
public class SWizSpriteAnimationFrame
{
	/// <summary>
	/// The sprite collection.
	/// </summary>
	public SWizSpriteCollectionData spriteCollection;
	/// <summary>
	/// The sprite identifier.
	/// </summary>
	public int spriteId;
	
	/// <summary>
	/// When true will trigger an animation event when this frame is displayed
	/// </summary>
	public bool triggerEvent = false;
	/// <summary>
	/// Custom event data (string)
	/// </summary>
	public string eventInfo = "";
	/// <summary>
	/// Custom event data (int)
	/// </summary>
	public int eventInt = 0;
	/// <summary>
	/// Custom event data (float)
	/// </summary>
	public float eventFloat = 0.0f;
    #region OJO! PARA AÑADIR CAMPOS AQUI HAY QUE HACERLO TAMBIEN EN CopyTriggerFrom y ClearTrigger SI NO NO SE CONSOLIDARAN EN LA ANIMACION AL DARLE A COMMIT
    #endregion
    public Vector2 eventVector2; // KUCHO HACK
    public FrameEventTransport eventTransport; // KUCHO HACK

    public void CopyFrom(SWizSpriteAnimationFrame source)
	{
		CopyFrom(source, true);
	}

    public void CopyTriggerFrom(SWizSpriteAnimationFrame source)
    {
        triggerEvent = source.triggerEvent;
        eventInfo = source.eventInfo;
        eventInt = source.eventInt;
        eventFloat = source.eventFloat;
        eventVector2 = source.eventVector2;  // KUCHO HACK
        eventTransport = source.eventTransport;// KUCHO HACK
    }

    public void ClearTrigger()
    {
        triggerEvent = false;
        eventInt = 0;
        eventFloat = 0;
        eventInfo = "";
        eventVector2 = Vector2.zero;// KUCHO HACK
        eventTransport = FrameEventTransport.NONE;// KUCHO HACK
    }

    public void CopyFrom(SWizSpriteAnimationFrame source, bool full)
	{
		spriteCollection = source.spriteCollection;
		spriteId = source.spriteId;
		
		if (full) CopyTriggerFrom(source);
	}
}

[System.Serializable]

public class SWizSpriteAnimationClip
{
	public string name = "Default";
	public SWizSpriteAnimationFrame[] frames = new SWizSpriteAnimationFrame[0];
	public float fps = 30.0f;
	public int loopStart = 0;
	public enum WrapMode
	{
		Loop,
		LoopSection,
		Once,
		PingPong,
		RandomFrame,
		RandomLoop,
		Single
	};
	public WrapMode wrapMode = WrapMode.Loop;

    public int clipID = -1; 
    public bool isAttackClip = false;
    public bool isAimingClip = false;
    public bool isGesticulatingClip = false; 
    public string clipInfo = ""; 
    public float clipFloat = 0;



	public bool Empty
	{
		get { return name.Length == 0 || frames == null || frames.Length == 0; }
	}
	


}


public class SWizSpriteAnimation : MonoBehaviour 
{
	public SWizSpriteAnimationClip[] clips;
	public SWizSpriteAnimationClip GetClipByName(string name)
	{
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i].name == name) return clips[i];
		return null;
	}
	public SWizSpriteAnimationClip GetClipById(int id) {
		if (id < 0 || id >= clips.Length || clips[id].Empty) {
			return null;
		}
		else {
			return clips[id];
		}
	}

	public int GetClipIdByName(string name) {
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i].name == name) return i;
		return -1;
	}
	//KUCHO HACK (funcion totalmente nueva)
	public List<int> GetClipIDsByNamePrefix(string pref)
	{
		List<int> result = new List<int>();
		if (pref == "")
			return result;
		for (int i = 0; i < clips.Length; ++i)
		{
			if (clips[i].name.StartsWith(pref))
			{
				result.Add(i);
			}
		}
		return result;
	}
	
}


