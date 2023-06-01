using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TS_DynamicSprite))]
public abstract class TS_SpriteCollider : MonoBehaviour
{
	public bool IsTrigger;

	public bool rebuildAllCollidersAllowed = true;
	
	public PhysicsMaterial2D Material;


	[SerializeField]
	protected LPBody lpBody; 
	[SerializeField]
	protected ExpensiveTaskManager globalExplosionManager;

	public GameObject parentOfAllColliders;
	
	protected TS_DynamicSprite DynamicSprite;
	
	protected Texture2D alphaTex;
	
	private bool dirty;

	private bool _awakeCalled;

	public bool rebuildCollidersAtStart = false;

}