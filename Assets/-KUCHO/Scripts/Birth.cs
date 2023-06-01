using UnityEngine;
using System.Collections;

// una class para hacer nacer personajes con una animación especial, solo sirve para personajes que tengan CC
// la podemos poner en enemigos que disparen enemigos y generadores de enemigos

public class Birth : MonoBehaviour{
	public string anim;
	public Hook hook; // para actiHook el modo HANG en el momento de nacer referenciar un traqnsform en el editor, de esta forma no hay animacion de nacimiento , ocurre instantaneamente
	public float zLockTime = 0f;
	public Jump jump;
	public VelocityBirth velocity;
    [ReadOnly2Attribute] public CC cC;
    [ReadOnly2Attribute] public SWizSpriteAnimationClip animClip;

    public void InitialiseIneditor()
    {
        cC = GetComponentInParent<CC>();
    }
    [System.Serializable]
	public class Jump{
		public bool activated = false;
		public bool lookToTarget = true;
		public MinMax heigth = new MinMax(2f, 18f);
		public MinMax fwdSpeed = new MinMax (0f, 3f);
		[HideInInspector] public float finalHeigth; // el resultado de calcular el random range de jump heigth min y max
		[HideInInspector] public float finalFwdSpeed;  // el resultado de calcular el random range de jump fwd speed min y max
		public float ZOffsetOnceIsGoingDown = -0.5f;

	}
	[System.Serializable]
	public class VelocityBirth{
		public bool activated= false;  // si True se usa la variable birthVelocity para determinar nacimiento , si false, nacera cuando la animacion de nacimiento se complete
		public Vector2 velocityToBeBorn; // la velocidad a la que se activara isBorn en CC y habra nacido oficialmente
		public MinMax startVelocityX; // velocidad inicial para darle impulso, son valores con maximo y minimo para poder tener velocidades aleatorias
		public MinMax startVelocityY;
		[HideInInspector] public Vector2 finalStartVelocity; // aqui se vuelca la velocidad aleatoria final
		public float gravityScale; // duranete el nacimiento, hasta el momento en que nace oficialmente se usa esta gravity scale en el rigidbody¡

	}

}