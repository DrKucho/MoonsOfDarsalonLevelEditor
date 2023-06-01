using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleTest : MonoBehaviour {

	public int data = -2;

	public float signedByte = 255;

	int t;
	int elapsed;

	
	void DoIt() {
//		Conversions();
//		GetComponent();
		var i = Random.Range(0,2);
		print(i);
	}
	void GetComponent(){
		t = System.Environment.TickCount;
		for (int i = 1; i < data; i++)
		{
			CC col = GetComponent<CC>();
		}
		elapsed = System.Environment.TickCount - t;
		print("ELAPSED TICKS GetComponent CC = " + elapsed);

		t = System.Environment.TickCount;
		for (int i = 1; i < data; i++)
		{
			AI ai = GetComponent<AI>();
		}
		elapsed = System.Environment.TickCount - t;
		print("ELAPSED TICKS GetComponent AI = " + elapsed);

		t = System.Environment.TickCount;
		for (int i = 1; i < data; i++)
		{
			Component[] comps = GetComponents<Component>();
		}
		elapsed = System.Environment.TickCount - t;
		print("ELAPSED TICKS GetComponents = " + elapsed);

		t = System.Environment.TickCount;
		for (int i = 1; i < data; i++)
		{
			string tag = gameObject.tag;
		}
		elapsed = System.Environment.TickCount - t;
		print("ELAPSED TICKS GetGameObject.Tag = " + elapsed);

		t = System.Environment.TickCount;
		for (int i = 1; i < data; i++)
		{
			gameObject.CompareTag("Civilian");
		}
		elapsed = System.Environment.TickCount - t;
		print("ELAPSED TICKS Compare.Tag = " + elapsed);
	}

	void Conversions(){
		// diferencias entre int y uint?
		print ("int -2 = " + System.Convert.ToString(data, 2));
		uint result = (uint)data;
		print ("uint 2 = " + System.Convert.ToString(result, 2));

		sbyte sByte = (sbyte)signedByte;
		// tratando de convertir el signo de valores negativos
		print ("conversion1 = " + (int)sByte);

		int t;
		int elapsed;

		//		t = System.Environment.TickCount;
		//		for (int i = 0; i < data; i++)
		//		{
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//		}
		//		elapsed = System.Environment.TickCount - t;
		//		print("ELAPSED TICKS (int & int) = " + elapsed);
		//
		//		uint dataUint = (uint)data;
		//		t = System.Environment.TickCount;
		//		for (uint i = 0; i < dataUint; i++)
		//		{
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//
		//		}
		//		elapsed = System.Environment.TickCount - t;
		//		print("ELAPSED TICKS (uint & uint) = " + elapsed);

		t = System.Environment.TickCount;
		for (uint i = 0; i < data; i++)
		{
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		}
		elapsed = System.Environment.TickCount - t;
		print("ELAPSED TICKS (uint & int ++) = " + elapsed);

		t = System.Environment.TickCount;
		for (uint i = 0; i < data; i++)
		{
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
			i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; i+=1; i-=1; 
		}
		elapsed = System.Environment.TickCount - t;
		print("ELAPSED TICKS (uint & int +=1) = " + elapsed);

		//		t = System.Environment.TickCount;
		//		for (ushort i = 0; i < data; i++)
		//		{
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//		}
		//		elapsed = System.Environment.TickCount - t;
		//		print("ELAPSED TICKS (ushort & int)= " + elapsed); 
		//
		//		ushort data16 = (ushort)data;
		//		t = System.Environment.TickCount;
		//		for (ushort i = 0; i < data16; i++) 
		//		{
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//			i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; i++; i--; 
		//		}
		//		elapsed = System.Environment.TickCount - t;
		//		print("ELAPSED TICKS (ushort & ushort)= " + elapsed);
	}
}
