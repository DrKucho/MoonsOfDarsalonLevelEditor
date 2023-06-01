using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour {

	public enum SequentialOrder{Wrap, PingPong, PingPongOnce}
	public enum SequentialDirection{Normal, Inverse}

	public SequentialOrder order = SequentialOrder.Wrap;
	public int arrayInc = 1;
	public List<Transform> targets;
	public float arrivedThreshold = 2;
	int targetIndex = 0;
	public Vector2 toVel;
	public Vector2 maxVel;
	public Vector2 maxForce;
	public float gain = 5f;

	[Header ("INFO")]
	public Transform target;
	public Vector2 distToTarget;
	public Vector2 force;
	public Vector2 velocity;

	private Rigidbody2D rb;

	public enum RotationType
	{ Dont, SetAngularVelocity, TorqueStable }

	[Header("ROTATE")] public RotationType rotType = RotationType.SetAngularVelocity;
	public float rotationSpeed = 6;
	public float maxAngle = 2;
	[ReadOnly2Attribute] public float _rotationSpeed;

	public float rotationTorque = 10;

	bool IsSetAngularVelocity() { return rotType == RotationType.SetAngularVelocity; }
	bool IsTorqueStable() { return rotType == RotationType.TorqueStable; }

#if  UNITY_EDITOR
	public void InitialiseInEditor(){
		if (isActiveAndEnabled)
		{
				GrabAndSortTargets();
		}
	}
#endif
	void Start(){
		rb = GetComponentInChildren<Rigidbody2D>();
		_rotationSpeed = rotationSpeed;
	}

	void OnEnable()
	{
		targetIndex = GetIndexOfCloserTarget();
		target = targets[targetIndex];
		if (order == SequentialOrder.PingPongOnce)
		{
			targetIndex = 0;
			arrayInc = -1;
		}
	}

#if  UNITY_EDITOR
	void GrabAndSortTargets(){
		if (!transform.parent)
		{
			Debug.LogError(this + " NECESITO TENER PADRE DEL QUE BUSCAR LOS TARGETS");
			return;
		}

		Transform parentToLookForTargetChildrens = transform.parent;
		
		if (transform.parent.GetComponent<DragMeHandle>())
		{
			parentToLookForTargetChildrens = transform.parent.parent;
		}

		if (!parentToLookForTargetChildrens)
			parentToLookForTargetChildrens = transform.root;

		var children = parentToLookForTargetChildrens.GetComponentsInChildren<Transform>();

		List<Transform> tars = new List<Transform>();
		foreach (Transform child in children)
		{
			if (child.name.StartsWith("Target"))
			{
				child.tag = "Pickable";
				tars.Add(child);
			}
		}

		if (order != SequentialOrder.Wrap)
		{
			// encuentra el inicial
			Vector3 delta;
			float dist;
			float minDist = float.MaxValue;
			Vector3 minOrigin = Constants.zero3;
			int winner = -1;
			int tarsCount = tars.Count; //Optimizacion: leer .Count es el doble de lento

			for (int i = 0; i < tarsCount; i++)
			{
				delta = tars[i].position - minOrigin;
				dist = delta.sqrMagnitude;
				dist = Mathf.Abs(dist);
				if (dist < minDist)
				{
					minDist = dist;
					winner = i;
				}
			}

			targets.Clear();
			targets.Add(tars[winner]);
			// añade los demas por cercania 
			for (int i = 0; i < tarsCount; i++)
			{
				minDist = float.MaxValue;
				winner = -1;
				for (int j = 0; j < tarsCount; j++)
				{
					if (!targets.Contains(tars[j]))
					{
						delta = targets[targets.Count - 1].position - tars[j].position;
						dist = delta.sqrMagnitude;
						dist = Mathf.Abs(dist);
						if (dist < minDist && dist != 0) // hemos encontrado un punto mas cercano que el que teniamos
						{
							minDist = dist;
							winner = j;
						}
					}
				}

				if (winner >= 0)
					targets.Add(tars[winner]);
			}
		}
		else
		{
			targets = tars;
		}
	}
#endif
    void FixedUpdate(){
        distToTarget = (Vector2)target.position - rb.position;

        Vector2 tgtVel;
        tgtVel.x = Mathf.Clamp(toVel.x * distToTarget.x, -maxVel.x, maxVel.x);
        tgtVel.y = Mathf.Clamp(toVel.y * distToTarget.y, -maxVel.y, maxVel.y);
        // calculate the velocity error
        Vector2 error = tgtVel - rb.velocity;

        force.x = Mathf.Clamp(gain * error.x * rb.mass, -maxForce.x, maxForce.x);
        force.y = Mathf.Clamp(gain * error.y * rb.mass, -maxForce.y, maxForce.y);
        //print(KuchoTime.fixedUpdateCallCount + " FIX PFRM POS:" + (rb.position.x + TestValues.instance.testFloat2).ToString("0.00") + " VEL:" + rb.velocity.x.ToString("0.00"));
        rb.AddForce(force);
    }

    public bool running = true;
	void Update(){
		//print("----" + KuchoTime.frameCount + " UPD PFRM POS:" + (rb.position.x + TestValues.instance.testFloat2).ToString("0.00") + " VEL:" + rb.velocity.x.ToString("0.00"));

		// hemos llegado a un target, selecciona el siguiente
		if (Mathf.Abs(distToTarget.x) < arrivedThreshold && Mathf.Abs(distToTarget.y) < arrivedThreshold)
		{	
			if (order == SequentialOrder.Wrap)
			{
				KuchoHelper.IncAndWrapInsideArrayLength(ref targetIndex, 1, targets.Count);
			}
			else
			{
				int previousTargetIndex = targetIndex;

				if (running)
				{
					targetIndex += arrayInc;
					bool clamped = KuchoHelper.ClampInsideArrayLength(ref targetIndex, targets.Count);
					if (clamped)
					{
						arrayInc *= -1;
						targetIndex += arrayInc;
					}

					if (order == SequentialOrder.PingPongOnce && targets.Count > 1)
					{
						if (targetIndex == 0 && previousTargetIndex > 0) // hemos completado un trayecto?
						{
							running = false;
						}
					}
				}

			}
			target = targets[targetIndex];
		}
		velocity = rb.velocity;
		
		if (rotType != RotationType.Dont)
		{
			var rotZ = rb.rotation;// rot.z;
			rotZ = KuchoHelper.GetUsefullRotation(rotZ);
			if (rotZ > 180)
				rotZ = rotZ - 360;

			if (rotType == RotationType.SetAngularVelocity) // asi era antes
			{
				if (rotZ > maxAngle)
					_rotationSpeed = -rotationSpeed;
				else if (rotZ < -maxAngle)
					_rotationSpeed = rotationSpeed;
				rb.angularVelocity = _rotationSpeed;
			}
			else // TorqueStable	
			{
				float torque =  - rotZ * rotationTorque * rb.mass;
				rb.AddTorque(torque);
			}
		}
	}

	int GetIndexOfCloserTarget()
	{
		Vector2 delta;
		float dist;
		float minDist = float.MaxValue;
		int winner = -1;
		
		for (int i = 0; i < targets.Count; i++)
		{
			Vector2 pos = targets[i].position;
			delta.x = transform.position.x - pos.x;
			delta.y = transform.position.y - pos.y;
			dist = delta.sqrMagnitude;
			dist = Mathf.Abs(dist);
			if (dist < minDist)
			{
				winner = i;
				minDist = dist;
			}
		}

		return winner;
	}
}
