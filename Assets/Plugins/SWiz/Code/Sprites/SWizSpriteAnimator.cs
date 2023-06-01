using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SWizSpriteAnimator : MonoBehaviour
{
	[SerializeField] SWizSpriteAnimation library;
	[SerializeField] int defaultClipId = 0;

    public bool debug = false; // KUCHO HACK
    public string stopOnPlayThisClip;// KUCHO HACK
    public string stopOnStopThisClip;// KUCHO HACK

    public delegate void OnPlay(); // KUCHO HACK
    public event OnPlay onPlay; // KUCHO HACK

    public bool playAutomatically = false;
	
	static State globalState = 0;

	public static bool g_Paused
	{
		get { return (globalState & State.Paused) != 0; }
		set { globalState = value?State.Paused:(State)0; }
	}

	public bool Paused
	{
		get { return (state & State.Paused) != 0; }
		set 
		{ 
			if (value) state |= State.Paused;
			else state &= ~State.Paused;
		}
	}

	public SWizSpriteAnimation Library {
		get { return library; }
		set { library = value; }
	}

	public int DefaultClipId {
		get { return defaultClipId; }
		set { defaultClipId = value; }
	}

	public SWizSpriteAnimationClip DefaultClip {
		get { return GetClipById(defaultClipId); }
	}

    public bool reverse = false; // KUCHO HACK

    SWizSpriteAnimationClip currentClip = null;

    float clipTime = 0.0f;
    float clipFps = -1.0f;
    int previousFrame = -1;
    public System.Action<SWizSpriteAnimator, SWizSpriteAnimationClip> AnimationCompleted;
    public System.Action<SWizSpriteAnimator, SWizSpriteAnimationClip, int> AnimationEventTriggered;

	enum State 
	{
		Init = 0,
		Playing = 1,
		Paused = 2,
	}
	State state = State.Init; // init state. Do not use elsewhere
	
	protected SWizBaseSprite _sprite = null;

	virtual public SWizBaseSprite Sprite {
		get {
			if (_sprite == null) {
				_sprite = GetComponent<SWizBaseSprite>();
				if (_sprite == null) {
					Debug.LogError(this + "S-WIZ SPRITE NOT FOUND");
				}
			}
			return _sprite;
		}
	}
	public SWizSpriteAnimationClip GetClipById(int id) {
		if (library == null) {
			return null;
		}
		else {
			return library.GetClipById(id);
		}
	}
	public SWizSpriteAnimationClip CurrentClip
	{
		get
		{
			if (currentClip == null)
			{
				currentClip = DefaultClip;
			}
				return currentClip;
		}
	}

	public static float DefaultFps { get { return 0; } }

	public int GetClipIdByName(string name)
	{
		return library ? library.GetClipIdByName(name) : -1;
	}
	
	// KUCHO HACK
	public List<int> GetClipIDsByNamePrefix(string pref)
	{
		return library ? library.GetClipIDsByNamePrefix(pref) : null;
	}

	public SWizSpriteAnimationClip GetClipByName(string name)
	{
		return library ? library.GetClipByName(name) : null;
	}

	public virtual void SetSprite(SWizSpriteCollectionData spriteCollection, int spriteId) {
		Sprite.SetSprite(spriteCollection, spriteId);
	}

}
