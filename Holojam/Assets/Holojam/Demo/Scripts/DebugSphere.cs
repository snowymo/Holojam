﻿//DebugSphere.cs
//Created by Aaron C Gaudette on 02.07.16

using UnityEngine;

public class DebugSphere : MonoBehaviour{
	public float offset = 0;
	public Holojam.Actor target;
	
	void Update(){
		transform.position=target.transform.position+target.transform.forward*offset;
	}
}