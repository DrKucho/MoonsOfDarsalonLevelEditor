using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;


public class Door : MonoBehaviour
{

	public bool debug = false;

	public enum DoorWorkingMode
	{
		Vision,
		Timer
	};

	public DoorWorkingMode workingMode = DoorWorkingMode.Vision;

	bool IsVisionMode()
	{
		return workingMode == DoorWorkingMode.Vision;
	}

	bool IsTimerMode()
	{
		return workingMode == DoorWorkingMode.Timer;
	}

	public Action onEnable = Action.Nothing;
	public Action onLevelWasLoaded = Action.Nothing;
	public Vision vision;
	public Action onSomethingDetected = Action.Open;
	public Action onNothingDetected = Action.Close;
	public Vector2 openCloseTimer;

	public enum DoorMode
	{
		MoveTransform,
		SwizAnimation,
		RigidBodyMovePos,
		RigidBodySpeed
	}

	public DoorMode mode = DoorMode.MoveTransform;
	 public Rigidbody2D rb;
	 public Transform transformToMove;
	 public TowerBuilder tower;
	 public DragMeHandle handle;
	 public Vector3 endPosition;
	 public float speed = 1f;
	 public SWizSpriteAnimator anim;
	public DoorMovementExtension[] extensions;
	public float openDelay = 0;
	public float closeDelay = 0;
	Vector3 pos;
	Vector3 startPos; // aqui en Start() se copia la posicion que tiene el GO
	Vector3 from;
	Vector3 to;
	Vector2 toMinusFrom;
	Vector2 toMinusFromNormalized;
	float journeyLength;
	float startTime;
	float distCovered;
	float fracJourney;

	public enum Status
	{
		Opening,
		Closing,
		Opened,
		Closed,
		Init
	};

	public TileDoorLightObstacle tileDoorLightObstacle;

	public enum Action
	{
		Open,
		Close,
		Nothing
	};

	public bool loop = false;
	public Collider2D activeOnStartPos;
	public GameObject[] switchOnOpenClose;
	public float audioGain = 0.5f;
	public int doorSoundSetIndex = 0;
	public AudioClip goAudio;
	public AudioClip backAudio;
	public AudioClip endPosAudio;
	public AudioClip startPosAudio;

	[SerializeField] AudioManager aM;
	VisibleObjectList visionList;

	SWizSpriteAnimationClip openClip;
	SWizSpriteAnimationClip closeClip;
	int stoppedAtFrame = 0;
	int startFrame = 0;

	bool IsMoveTransform()
	{
		return mode == DoorMode.MoveTransform;
	}

	bool IsSwizAnimator()
	{
		return mode == DoorMode.SwizAnimation;
	}

	bool IsRigidBodySpeedOrMovePos()
	{
		return mode == DoorMode.RigidBodySpeed | mode == DoorMode.RigidBodyMovePos;
	}

	bool IsNotAnimation()
	{
		return mode != DoorMode.SwizAnimation;
	}

	[ReadOnly2Attribute] public Door.Status status = Door.Status.Closed;
	[HideInInspector] public float lastStatusChangeTime;

	public bool closeAllowed;

	void OnValidate()
	{
		if (isActiveAndEnabled && tower)
		{
			endPosition = tower.bottom.localPosition;
			transformToMove = tower.top;
			startPos = tower.top.localPosition;
		}

	}

	public void InitialiseInEditor()
	{
		aM = GetComponent<AudioManager>();
		GameObject goToGrabShit = null;
		switch (mode)
		{
			case (DoorMode.SwizAnimation):
				goToGrabShit = anim.gameObject;
				openClip = anim.GetClipByName("Open");
				closeClip = anim.GetClipByName("Close");
				break;
			case (DoorMode.MoveTransform):
				pos = transformToMove.localPosition;
				startPos = transformToMove.localPosition;
				goToGrabShit = transformToMove.gameObject;
				break;
		}

		if (!aM)
			aM = goToGrabShit.GetComponent<AudioManager>();
		if (!aM)
			aM = goToGrabShit.GetComponentInChildren<AudioManager>();
		if (!aM)
			aM = GetComponentInParent<AudioManager>();
		if (!tileDoorLightObstacle)
			tileDoorLightObstacle = GetComponentInChildren<TileDoorLightObstacle>();
		handle = GetComponentInChildren<DragMeHandle>();
		extensions = GetComponentsInChildren<DoorMovementExtension>();
		if (workingMode == DoorWorkingMode.Vision)
		{
			if (!vision)
				vision = GetComponent<Vision>();
			if (!vision)
				vision = GetComponentInChildren<Vision>();
			if (!vision && transform.parent)
				vision = transform.parent.gameObject.GetComponentInChildren<Vision>();

			if (vision)
			{
				visionList = vision.GetVisibleObjectListByType(VisibleObjectType.Switch);
				if (visionList)
				{
					visionList.onDetected += OnSomethingDetected;
					visionList.onEverythingIsGone += OnNothingDetected;
				}

				if (vision.mainGO == null)
					vision.mainGO = this.gameObject;
			}
		}
	}



	private Action pendingAction;
	private float pendingActionTime;
	private static int activeDoorsCount;

	public void OnSomethingDetected(VisibleObjectList list, Collider2D col)
	{

	}

	public void OnNothingDetected(VisibleObjectList list, Collider2D col)
	{

	}


	bool openingCoroutineIsAlive; // se ve que aun no conocia el handle.isRunning



	Vector3 snappedPos;

}
