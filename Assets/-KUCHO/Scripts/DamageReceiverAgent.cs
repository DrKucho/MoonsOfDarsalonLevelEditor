using UnityEngine;
using System.Collections;

/// <summary>
/// Reporta las colisiones a un damage receiver de otro gameObject, util cuando tienes un enemigo con varios rigidbody ya que en este caso no se reportan las colisiones mas arriba de este rigidbody.
/// </summary>
public class DamageReceiverAgent : MonoBehaviour {
	public DamageReceiver main;
	
}
