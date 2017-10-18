using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotTest : MonoBehaviour {

    m3piComm ctrl;

    public string name;

    public string cmd;

	// Use this for initialization
	void Start () {
        ctrl = new m3piComm();
        ctrl.setName(name);
        ctrl.open();
        Time.fixedDeltaTime = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnDestroy()
    {
        ctrl.close();
    }

    private void FixedUpdate()
    {
        ctrl.m_command = cmd;
        ctrl.run2();

    }
}
