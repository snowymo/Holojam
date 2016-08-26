using UnityEngine;
using System.Collections;

public class testStartCo : MonoBehaviour {

	void Start() {
		print("Starting " + Time.time);
		StartCoroutine(WaitAndPrint(2.0F));
		print("Before WaitAndPrint Finishes " + Time.time);
	}
	IEnumerator WaitAndPrint(float waitTime) {
		yield return new WaitForSeconds(2);
		//for(int i = 0;i < 100; i++)
			print("WaitAndPrint " + Time.time);
	}

	IEnumerator WaitAndPrint2(float waitTime) {
		yield return StartCoroutine(WaitAndPrint(2.0F));
		//for(int i = 0;i < 100; i++)
		print("WaitAndPrint 2 " + Time.time);
	}

	void Update(){
		print ("update");
		if (Input.GetKeyDown ("r")) {
			// start new thread
			StartCoroutine(WaitAndPrint(2.0F));
		}
	}
}
