using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class m3piComm : SerialCommunication
{
	char m_speed;

	int m_waitTime;

	public string m_command;

	string m_name;

	public string m_returnMsg;

	public bool m_bRtn;

	public float m_cmdTime;

	public float m_runTime;

	public bool m_exStop;

	//protected SerialPort stream;

	public m3piComm ()
	{
		//m_speed = 0.2f;
		//m_waitTime = 0.5f;
		m_command = "";
		m_returnMsg = "";
		m_bRtn = true;
		m_exStop = false;
	}

	static private m3piComm m_inst = null;

	static public m3piComm getInst ()
	{
		if (m_inst == null)
			m_inst = new m3piComm ();
		return m_inst;
	}

	public override void setSpeed (int sp)
	{
		m_speed = (char)('a' + (char)sp);
	}

	public override void setWaitTime (int wt)
	{
		m_waitTime = wt;
	}

	public override void forward ()
	{
		open ();
		if (stream != null) {
			m_command += "f" + (m_speed).ToString () + (m_waitTime).ToString ();
		}
	}

    public bool isOpen()
    {
        stream = StreamSingleton.getInst();
        return stream.getStream().IsOpen;
    }

	public void clear ()
	{
		m_command = "";
	}

	void assignRunTime ()
	{
		m_cmdTime = 0;
		//calculate estimated running time based on command
		for (int i = 4; i < m_command.Length; i += 3) {
			m_cmdTime += m_command [i] - '0';
		}
		m_cmdTime /= 10.0f;
		Debug.Log ("estimated time:\t" + m_command + "\t" + m_cmdTime);
	}

	bool verifyCommand ()
	{
		if (m_command.Length % 3 != 0)
			return false;
		return true;
	}

	//	public Thread receiveThread;
	//	public void run (float curTime = 0)
	//	{
	//		m_runTime = curTime;
	//		if (stream != null) {
	//			if (stream.getStream().IsOpen) {
	//				//Debug.Log ("time bf:\t" + Time.time);
	//				// verify command
	//				if (verifyCommand ()) {
	//					m_command = m_name + m_command + "E";
	//					assignRunTime ();
	//					stream.getStream ().Write (m_command);
	//					//Debug.Log ("command:\t" + m_command);
	//
	//					// if robot is not power on then it will die
	//					//m_returnMsg = stream.ReadLine ();
	//					//Debug.Log ("time af:\t" + Time.time);
	//					m_bRtn = false;
	//
	//					if (StreamSingleton.getInst().getReceiveThread() != null) {
	////						Debug.Log ("before abort state:\t" + StreamSingleton.getInst().getReceiveThread().ThreadState);
	//						//					while (receiveThread.ThreadState != ThreadState.Stopped)
	//						//						Thread.Sleep (500);
	//						StreamSingleton.getInst().getReceiveThread().Abort ();
	//					}
	//					StreamSingleton.getInst ().newRcvThread (receive);
	//					StreamSingleton.getInst().getReceiveThread().Name = m_name;
	//					//Debug.Log("after new state :\t" +receiveThread.Name + "\t" + receiveThread.ThreadState);
	//					//if (receiveThread.ThreadState == ThreadState.Stopped
	//					//	|| receiveThread.ThreadState == ThreadState.Unstarted) {
	//					StreamSingleton.getInst().getReceiveThread().Start ();
	//					//Debug.Log("after start state:\t" + receiveThread.ThreadState);
	//					//}
	//				} else {
	//					clear ();
	//				}
	//			}
	//		}
	//	}

	bool sameCmd (string cmda, string cmdb)
	{
		if (cmda.Length == cmdb.Length)
		if (cmda [0] == cmdb [0]) {
			int dis = cmda [1] - cmdb [1];
			if (Mathf.Abs (dis) < 5)
			if (cmda.Substring (2).Equals (cmdb.Substring (2)))
				return true;
		}
		return false;
	}

	string m_lastCmd = "";
	// new version for sharing thread
	public void run2 (float curTime = 0)
	{
		m_runTime = curTime;
		if (stream != null) {
			if (stream.getStream ().IsOpen) {
//				Debug.Log ("time:\t" + curTime + "\t" +Time.time);
				// verify command
				if (verifyCommand ()) {
					m_command = m_name + Utility.getInst ().getMyTimeStamp () + m_command + "E";
					if (!sameCmd (m_command, m_lastCmd)) {
						assignRunTime ();
						stream.getStream ().Write (m_command);
						//stream.addReceive ();
						// if robot is not power on then it will die
						m_bRtn = false;
						m_lastCmd = m_command;
						//clear ();
					} else
						m_lastCmd = "";
				} else {
					clear ();
				}
			}
		}
	}

	//	public void stopThread() {
	//		receiveThread.Join ();
	//	}

	//unused
	bool match ()
	{
		// check if command is match with the receive msg
		if (m_command.Length >= 5
		    && m_returnMsg.Length > 10) {
			//if (m_returnMsg.Substring (2, m_command.Length-1).Equals (m_command.Substring(0,m_command.Length-1))) {
			if (m_returnMsg [0].Equals (m_command [1])) {
				return true;
			}
			Debug.Log ("cmd:\t" + m_command + "\trtnMsg:\t" + m_returnMsg);
		}
		return false;
	}

	// unused
	public void receive ()
	{
		do {
			Debug.Log ("in receive");
			if (m_exStop) {
				Debug.Log ("stop external");
				m_exStop = false;
				break;
			}
			m_returnMsg = stream.getStream ().ReadLine ();
			// if it returns too slow, it has already got stop externally, so that the return msg is not match to the current command
			if (m_returnMsg.Length > 0 && !match ()) {
				Debug.Log ("when return NOT match:cmd\t" + m_command + "\rret\t" + m_returnMsg);
				// TODO
				m_returnMsg = "";
			}
		} while(!match ());
		// clear the command
		clear ();
		Debug.Log ("after receive");
		m_bRtn = true;
		return;
	}

	public override void backward ()
	{
		open ();
		if (stream != null) {
			m_command += "b" + (m_speed).ToString () + (m_waitTime).ToString ();
		}
	}

	public override void left ()
	{
		open ();
		if (stream != null) {
			m_command += "l" + (m_speed).ToString () + (m_waitTime).ToString ();
		}
	}

	public override void right ()
	{
		open ();
		if (stream != null) {
			m_command += "r" + (m_speed).ToString () + (m_waitTime).ToString ();
		}
	}

	public override void stop ()
	{
		open ();
		if (stream != null) {
			m_command += m_name + "sE";
		}
	}

	public void setName (string name)
	{
		m_name = name;
	}
}
