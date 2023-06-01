using UnityEngine;
using System.Collections;

public class RemoteController : MonoBehaviour {

	public bool debug = false;
	public float textDelay = 0.05f;
	public string[] GO_Names;
	public GameObject[] GO = new GameObject[2];
	public RemoteReceiver[] ReceiverSC = new RemoteReceiver[2];
	private GameObject[] GOTextBallon;
	private int i = 0;
	public TextAsset textAsset;
	public string commands;
	public string command;
	public string c;
	public int GOi;
	private GameObject ballon;
	public string[] textToSayAfterHit = new string[0];
	public string[] textToSayAfterHitIfEnemyIsAlive = new string[0];
	public int farCloseThreshold;
	public string[] textToSayAfterHitIfEnemyIsAliveAndIsFarAway = new string[0];
	public string[] textToSayAfterHitIfEnemyIsAliveAndIsClose = new string[0];
	public string[] textToSayAfterHitIfEnemyIsDead = new string[0];
	public string[] textToSayAfterYouHitYourself = new string[0];
	
	public string[] textToSayWhenDead = new string[0];
	public string[] textToSayWhenFire = new string[0];
	public string[] textToSayWhenPunch = new string[0];
	
}
