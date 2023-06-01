using UnityEngine;
using System.Collections;

public class CustomFixedUpdateJ : MonoBehaviour {

	//
	//private float m_FixedDeltaTime;
	//private var m_ReferenceTime = 0f;
	//private var m_FixedTime = 0f;
	//private var m_MaxAllowedTimestep = 0.3f;
	//private System.Action m_FixedUpdate;
	//private var m_Timeout = new System.Diagnostics.Stopwatch();
	//
	//function CustomFixedUpdate(aFixedDeltaTime:float, aFixecUpdateCallback:System.Action){
	//    m_FixedDeltaTime = aFixedDeltaTime;
	//    m_FixedUpdate = aFixecUpdateCallback;
	//}
	//
	//function Update(aDeltaTime:float){
	//    m_Timeout.Reset();
	//    m_Timeout.Start();
	//
	//    m_ReferenceTime += aDeltaTime;
	//    while (m_FixedTime < m_ReferenceTime)
	//    {
	//        m_FixedTime += m_FixedDeltaTime;
	//        if (m_FixedUpdate != null)
	//            m_FixedUpdate();
	//        if ((m_Timeout.ElapsedMilliseconds / 1000.0f) > m_MaxAllowedTimestep)
	//            return false;
	//    }
	//    return true;
	//}
	//
	//function FixedDeltaTime(value:float) {
	//    get { return m_FixedDeltaTime; },
	//    set { m_FixedDeltaTime = value; }
	//}
	//function MaxAllowedTimestep() {
	//    get { return m_MaxAllowedTimestep; }
	//    set { m_MaxAllowedTimestep = value; }
	//}
	//function ReferenceTime() {
	//    get { return m_ReferenceTime; }
	//}
	//function FixedTime() {
	//    get { return m_FixedTime; }
	//}
}
