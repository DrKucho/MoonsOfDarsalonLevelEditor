using UnityEngine;
using System.Collections;

public enum RaycasterUse{ GroundDetection, WallDetection, RoofDetection, RampDetection};

public class Raycaster : MonoBehaviour
{

    public bool debug = false;
    public RaycasterUse usedFor;
    public bool push;
    bool Push() { return push; }
    public LayerType pushLayer;
    int pushLayerInt;
    [ReadOnly2Attribute] public bool shouldPush;
    public float shouldPushOffset;
	public MaskType layerMask;
    public int _layerMask;
    public Transform fakeStart; // el rayo empieza realmente en localPosition 0,0 pero si llega menos que fakeStart dara valor negativo
    public Transform rayEnd;
	public float rayLength;
    public Collider2D aimToThisCollider;
	public Vector2 rayDir; // la original no se modifica
    [ReadOnly2Attribute] public Vector2 lastRayDir; // la ultima usada, se ha podido cambiar a mano, calcular a partir de la original etc... 
	public float distanceToStop;
	//int updateEach = 2; // se updateará cada X frames o mejor dicho cada X veces que se intente disparar el rayo
	//int countDown = 2;
	
	public RaycastHit2D hit;
	public Collider2D found;
	public float distance;
	public Vector2 point;
	public bool tooClose = false;
    public int lastShotFrame;
	
	
	public void Awake()
	{	
		Initialize();
	}

    
	public void Initialize(){
        fakeStart = transform.Find("FakeStart");
        rayEnd = transform.Find("End");
        if (!fakeStart || !rayEnd)
        {
            Debug.Break(); Debug.Log(this + "!!!!!!!!!!!!!!!!!!!!!!!!!");
            print(this + " ERROR , NO ENCUENTRO UNO DE LOS DOS GOS FAKE START O RAY END");
        }
        fakeStart.gameObject.SetActive(true);
		rayEnd.gameObject.SetActive(true);
		rayLength = rayEnd.localPosition.magnitude;
		distanceToStop = fakeStart.localPosition.magnitude;
		_layerMask = Masks.GetLayerMaskFromEnum(layerMask);
//		rayDir = (rayEnd.position - transform.position).normalized; // bug de unity, me dice que ambas position son iguales a pesar de que la localPos de rayEnd no es cero.
		rayDir = rayEnd.localPosition.normalized;
		fakeStart.gameObject.SetActive(false);
		rayEnd.gameObject.SetActive(false);
		lastRayDir = rayDir;
        pushLayerInt = Layers.GetLayerFromEnum(pushLayer);

	}
    public void ShotRelative(float newScaleX) // para invertir el rayo 
    {
        TransformHelper.SetLocalScaleX(transform, newScaleX);
        lastRayDir = (rayEnd.position - transform.position).normalized;
        Shot();
    }
    public void ShotRelative(){
		lastRayDir = (rayEnd.position - transform.position).normalized;
		Shot();
	}
	public void ShotRelative(Vector2 dirScale){
		lastRayDir = (rayEnd.position - transform.position).normalized;
		ShotMult(dirScale);
	}
	public void Shot(Vector2 dir){
		lastRayDir = dir;
		Shot();
	}
	public void ShotMult(Vector2 dirMultiplier){
		lastRayDir.x = rayDir.x * dirMultiplier.x;
		lastRayDir.y = rayDir.y * dirMultiplier.y;
		Shot();
	}
    
	public void Shot(){
        if (aimToThisCollider)
        {
            Vector2 destination;
            destination.x = aimToThisCollider.bounds.center.x;
            destination.y = aimToThisCollider.bounds.center.y;
            lastRayDir.x = destination.x - transform.position.x;
            lastRayDir.y = destination.y - transform.position.y;
            lastRayDir = lastRayDir.normalized;
        }

        hit = Physics2D.Raycast(transform.position, lastRayDir, rayLength, _layerMask);
        found = hit.collider;
        if (hit.transform)
		{
			distance = hit.fraction * rayLength - distanceToStop;
			point = hit.point;
            if (distance < 0)
            {
                tooClose = true;
            }
            else
            {
                tooClose = false;
            }
            if (push && found.gameObject.layer == pushLayerInt && !found.gameObject.CompareTag("PickUp"))
            {
                var pushDistance = hit.fraction * rayLength - distanceToStop + shouldPushOffset;
                if (pushDistance < 0)
                {
                    shouldPush = true;
                }
                else
                {
                    shouldPush = false; 
                }
            }
		}
		else
		{
			point = Constants.zero2;
			distance = rayLength - distanceToStop;
			tooClose = false;
            shouldPush = false;
        }
        lastShotFrame = KuchoTime.frameCount;
	}
	//function ShouldIShot(){	// decide si debe lanzar el rayo o no basandose en la frecuencia a la que este raycast puede actualizarse ( updateEach define esta frecuencia)
	//	
	//}
}
