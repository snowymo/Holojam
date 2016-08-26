using UnityEngine;
using System.Collections;
using System.Threading;

public class testThread : MonoBehaviour {

	MyThread mt;
	Thread newThrd;

	// Use this for initialization
	void Start () {
		Debug.Log("start main"+Time.time);

		//Thread newThrd = new Thread(new ThreadStart(mt.run));
		 mt = new MyThread("CHILE ");
		 
		//Thread.Sleep(8000);
		Debug.Log("ssssss");
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log(Time.time);
		this.transform.Rotate(Time.time,Time.time,Time.time);



		if (Input.GetKeyDown ("r")) {
			newThrd = new Thread(mt.run);
			newThrd.Start();
		}

		//Debug.Log("updata------"+MyThread.mt.count);

	}
}
