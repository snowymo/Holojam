using UnityEngine;
using System.Collections;

public class boardCtrl : MonoBehaviour {

	public GameObject chessModel;

	public string username;
	public GameObject[] chesses;

	public GameObject chessSyncGameObject;

	//Transform ctrler;
	GameObject ctrlGo;

	public bool isViewer;

	public GameObject enemyBoard;

	public string remoteAssignFlag;

	// Use this for initialization
	void Start () {
		// create sixteen chess at the beginning
		chesses = new GameObject[9];
		for (int i = 0; i < chesses.Length; i++) {
			chesses[i] = (GameObject)GameObject.Instantiate (chessModel);
			chesses [i].name = "chess" + i.ToString ("00");
			chesses [i].AddComponent<chessCtrl> ();
			chesses [i].transform.parent = transform;
			chesses [i].transform.localPosition = new Vector3 (i % 3 - 1, 0, i / 3 - 1) * 0.1f;
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
		//isViewer = false;
	}
	
	// Update is called once per frame
	void Update () {

		receiveAssignment ();

		assignChess ();

	}

	void assignChess(){

		// if ctrl hits one chess more than two seconds
		float dis = Vector3.Distance(chesses [0].transform.position,chesses [8].transform.position);
		int indexselect = -1;
		//print (ctrlGo.transform.position);
		// choose one of the highlights to set time
		for (int i = 0; i < chesses.Length; i++) {
			if (chesses [i].GetComponent<chessCtrl> ().isSelect()) {
				float d = Vector3.Distance(chesses [i].transform.position , ctrlGo.transform.position);
				if (d < dis) {
					indexselect = i;
					dis = d;
				}

			}
		}
		if (indexselect != -1) {
			if (remoteAssignFlag != "") {
				string[] msg = remoteAssignFlag.Split (splitChar, 2);
				if (indexselect == int.Parse (msg [0])) {
					chesses [int.Parse (msg [0])].GetComponent<chessCtrl> ().select (msg [1]);
					remoteAssignFlag = "";
				}
			}
			else if (GetComponent<boardCtrl> ().chesses [indexselect].GetComponent<chessCtrl> ().select (username)) {
				// select the same in other table
				if (!isViewer) {
					// manually select
					enemyBoard.GetComponent<boardCtrl> ().remoteAssignFlag = indexselect.ToString () + "-" + username;
					//later
					//enemyBoard.GetComponent<boardCtrl> ().chesses [indexselect].GetComponent<chessCtrl> ().select (username);
					// send msg
					chessSyncGameObject.GetComponent<chessSync> ().sentMsg = indexselect.ToString () + "-" + username;
				}
			} 
		}
	}

	char[] splitChar = { '-' };
	void receiveAssignment(){
		//TODO to be test
		if (isViewer) {
			// use msg 
			chessSync cs = chessSyncGameObject.GetComponent<chessSync> ();
			if (cs.text != "" && cs.text != null) {
				remoteAssignFlag = cs.text;
				//string[] msg = cs.text.Split (splitChar, 2);
				//chesses [int.Parse (msg [0])].GetComponent<chessCtrl> ().select (msg [1]);
			}
		}
	}

}
