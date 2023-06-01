using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]

public class SWizSpriteAttachPoint : MonoBehaviour {
	[System.Serializable] // TODA ESTA CLASS ES KUCHO HACK
	public class AttachPointExtras // se auto genera por cada attach point
	{
		public Transform trans; // el transform con el que esta asociado , esto permite eliminar la array attachPoint de SWiz directamente, pero la dejo por no marear
		public Renderer myRender;
		public bool update; // pueden apagarse externamente para evitar que se updateen (weapon3d on hurt)
		public bool updateEvenOnRenderOff = true; // pueden apagarse externamente para evitar que se updateen (weapon3d on hurt)
		public bool leaveZAlone;
		public float zOffset = 0.1f;
		//public bool relativeMode;// si esta activado, la posicion a aplicar sera relativa al transform attach point PREVIO en la array
		public Renderer relativeRenderer;// el renderer del attach point para comprobar si esta encendido y si no lo está, ignorar el modo relativo

		public override string ToString()
		{
			if (trans)
				return trans.name.ToUpper();
			else
				return "";
		}

		public AttachPointExtras(Transform trans, Renderer relativeRenderer)
		{
			this.trans = trans;
			this.relativeRenderer = relativeRenderer;
		}
	}
	[HideInInspector] public SWizBaseSprite sprite;// KUCHO HACK la hice publica y oculta
	[HideInInspector] public Renderer rend;// KUCHO HACK
    public bool debug = false; // KUCHO HACK
    public string debugName = ""; //KUCHO HACK
    public Transform mainSpriteForRelativeAttachPointsWhenTheirRelativeRendererIsOff;
    [HideInInspector] public bool[] relativeMode_; // DEPRECATED BORRAME

    public List<Transform> attachPoints = new List<Transform>();
    public AttachPointExtras[] kuchoExtras; // KUCHO HACK

	static bool[] attachPointUpdated = new bool[32];


	public bool deactivateUnusedAttachPoints = false;

    public bool ignoreScale = false; // KUCHO HACK paa que no cambie la escala de los attachpoints
    public bool flipScaleOnRotation = false; // KUCHO HACK
    public float pixelShiftX_OnFlip = 1; // KUCHO HACK
    public float flipScaleOnRotationEqualsTo = -180; // KUCHO HACK

    Dictionary<Transform, string> cachedInstanceNames = new Dictionary<Transform, string>();

}
