using UnityEngine;
using System.Collections;

public class Offset {

	static Offset m_inst = null;

	Vector3 offsetPos;

	public static Offset getInst(){
		if(m_inst == null)
			m_inst = new Offset();
		return m_inst;
	}

	public Offset(){
		offsetPos = new Vector3 (1.4f, -0.12f, 0);
	}

	public Vector3 getOffset(){

		return offsetPos;
	}

}
