// teletransporta objetos que tengan rigidbody, puede hacer varios a la vez, contempla caso especial el de los CC al que llama a cC.Freezed

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

using UnityEngine.Serialization;

[ExecuteInEditMode]
public class Teleport : MonoBehaviour {
	public bool initialised = false;
	public enum TeleportOperatingMode{SendAndReceive, ReceiveOnly}
	public TeleportOperatingMode mode = TeleportOperatingMode.SendAndReceive;
    [Range(0,2)] public float delay;
    [Range(0,1)] public float reenableDelay = 0.3f;
    [Range(0,1)] public float spitOutDelay = 0.3f;
	public Teleport theOtherSide;
    public ExplosionManager warmUpFX;
    public ExplosionManager endFX;
	CyclingLight[] cyclingLight;
	public Vector2 camSpeedDuringTeleport = new Vector2(0.1f, 0.1f);
	public Collider2D myCollider;
	Vector2 camWalkerSpeedBackUp;
	Vector2 camFlyerSpeedBackUp;
	float gravityScaleBackup;

	float attractionSpeed = 1;
    bool readyToReceive;

	[SerializeField] List<TeleportedItem> tpItems = new List<TeleportedItem>();
	[SerializeField] List<Rigidbody2D> uniqueBodies = new List<Rigidbody2D>();

	[System.Serializable]
	class TeleportedItem{
		public CC cC;
		public Collider2D col;
		public Rigidbody2D rb;
		public float gravityScale;
		public Vector2 velocity;
	}
#if UNITY_EDITOR
	void Update()
	{
		if (theOtherSide == null)
		{ 
			var all = FindObjectsOfType<Teleport>();
			if (all.Length > 1) // si hay solo uno me he encontrado a mi mismo
			{
				float winnerDist = float.MaxValue;
				Teleport winner = null;
				for (int i = 0; i < all.Length; i++)
				{
					if (this != all[i])
					{
						if (all[i].theOtherSide == this)
						{
							winner = all[i];
							break;
						}
						else
						{
							Vector2 dist;
							dist.x = all[i].transform.position.x - transform.position.x;
							dist.y = all[i].transform.position.y - transform.position.y;
							float distMag = dist.sqrMagnitude;
							if (distMag < winnerDist)
							{
								winner = all[i];
								winnerDist = distMag;
							}
						}
					}
				}

				theOtherSide = winner;
			}
		}
	}
#endif
	void OnValidate(){
        if (isActiveAndEnabled)
    		Initialise();
	}
	
	void Initialise () {
		cyclingLight = GetComponentsInChildren<CyclingLight>(true);
		warmUpFX.gameObject.SetActive(false);
		myCollider = GetComponentInChildren<Collider2D>();
		tpItems.Clear();
		initialised = true;
	}
}
