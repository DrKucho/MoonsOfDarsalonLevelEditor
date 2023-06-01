using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class SingleUnityLayer
{
	[SerializeField]
	private int m_index = 0;
	public int index
	{
		get { return m_index; }
	}
	public void Set(int _index)
	{
		if (_index > 0 && _index < 32)
		{
			m_index = _index;
		}
	}
	public int Mask
	{
		get { return 1 << m_index; }
	}
}

[System.Serializable]
public class GradualZChange{
	public bool enabled;
	public float speed;
	public float maxChange;
	[NonSerialized] [ReadOnly2Attribute] public bool pending;

	public void CopyFrom(GradualZChange o){
		enabled = o.enabled;
		speed = o.speed;
		maxChange = o.maxChange;
	}
}

[System.Serializable]
public class LayerDelayedChange{
    public bool enabled;
    public bool setStart;
     public SingleUnityLayer start;
    public SingleUnityLayer end;
    public float delay = 1f;
    bool SetStart(){return setStart;}
    [NonSerialized] [ReadOnly2Attribute] public bool pending;

    public void CopyFrom(LayerDelayedChange o){
        enabled = o.enabled;
        setStart = o.setStart;
        start = o.start;
        end = o.end;
        delay = o.delay;
    }
}
[System.Serializable]
public class Z_DelayedCahnge{
    public bool enabled;
    public bool setStart;
     public float start;
    public float end;
    public float delay = 1f;
    bool SetStart(){return setStart;}
    [NonSerialized] [ReadOnly2Attribute] public bool pending;


    public void CopyFrom(Z_DelayedCahnge o){
        enabled = o.enabled;
        setStart = o.setStart;
        start = o.start;
        end = o.end;
        delay = o.delay;
    }
}
public class OnEnableDelayedChanges : MonoBehaviour
{
	public GradualZChange gradualZChange;
	public LayerDelayedChange layerChange;
    public Z_DelayedCahnge z_Change;

	void OnValidate(){
		if (isActiveAndEnabled)
            gameObject.layer = layerChange.start.index;
	}



	public void CopyFrom(OnEnableDelayedChanges o, bool forceCopyAll)
	{
		if (forceCopyAll)
		{
			gradualZChange.CopyFrom(o.gradualZChange);
			layerChange.CopyFrom(o.layerChange);
			z_Change.CopyFrom(o.z_Change);
		}
		else
		{
			if (o.gradualZChange.enabled)
				gradualZChange.CopyFrom(o.gradualZChange);
			if (o.layerChange.enabled)
				layerChange.CopyFrom(o.layerChange);
			if (o.z_Change.enabled)
				z_Change.CopyFrom(o.z_Change);			
		}
	}
}
