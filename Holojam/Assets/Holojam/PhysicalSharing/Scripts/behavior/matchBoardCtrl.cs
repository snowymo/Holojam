using UnityEngine;
using System.Collections;

public class matchBoardCtrl : boardCtrl {

	public Material[] matchMat;
	MeshRenderer[] matchMeshes;

	void randomizeMesh(){
		matchMeshes = new MeshRenderer[16];
		for (int i = 0; i < 16; i++) {
			matchMeshes [i].material = matchMat [i / 2];
		}
		// randomize
		for (int i = 0; i < 16; i++) {
			int swapidx = Random.Range (0, 16);
			MeshRenderer mr = matchMeshes [i];
			matchMeshes [i] = matchMeshes [swapidx];
			matchMeshes [swapidx] = mr;
		}
	}

	// Use this for initialization
	void Start () {
		// randomize the texture first
		randomizeMesh();
		// create sixteen chess at the beginning
		chesses = new GameObject[16];
		for (int i = 0; i < chesses.Length; i++) {
			chesses[i] = (GameObject)GameObject.Instantiate (chessModel);
			chesses [i].name = "chess" + i.ToString ("00");
			chesses [i].AddComponent<chessCtrl> ();
			chesses [i].transform.parent = transform;
			chesses [i].transform.localPosition = new Vector3 (i % 4 - 1, 0, i / 4 - 1) * 0.075f;
			//chesses [i].GetComponent<Material> () = matchMeshes [i].material;
			//chesses [i].GetComponent<Renderer>().material = matchMeshes [i].material;
			//chesses [i].transform.localScale = new Vector3 (0.08f, 0.08f, 0.08f);
		}

		Transform[] ts = transform.GetComponentsInChildren<Transform> ();
		foreach (Transform t in ts) {
			if (t.gameObject.name == "controller") {
				//ctrler = t;
				ctrlGo = t.gameObject;
				break;
			}
		}

		ctrlGo = transform.Find ("controller").gameObject;
		remoteAssignFlag = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
