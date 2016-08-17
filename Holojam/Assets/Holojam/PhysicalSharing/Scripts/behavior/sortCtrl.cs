//Created by Zhenyi He on 16.08.16

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class sortCtrl : MonoBehaviour {

	public int[] m_order;

	public int cubeCount;

	int curIdx;

	// input number of objects
	// output an array of the order
	void generateOrder(int count){
		for (int i = count; i > 0; i--) {
			int order = Random.Range (0,i);
			int temp = m_order [i - 1];
			m_order [i - 1] = m_order [order];
			m_order [order] = temp;
		}

	}

	// Use this for initialization
	void Start () {
		// initialize
		cubeCount = 5;
		m_order = new int[cubeCount];
		for (int i = 0; i < cubeCount; i++)
			m_order [i] = i;
		curIdx = 0;

		// generate random order
		generateOrder (cubeCount);
	}
	
	// Update is called once per frame
	void Update () {
//		if(curIdx
	}
}
