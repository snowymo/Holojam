using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

public class StreamSingleton
{

	SerialPort m_stream;

	Thread m_rcvThread;
	bool thread_running;

	Object m_rcvMsgLock;
	//lock

	bool m_exStop;

	IList<string> m_rcvMsgs;

	public int m_linkCnt;

	public SerialPort getStream ()
	{
		return m_stream;
	}

	public void setExstop (bool b)
	{
		m_exStop = b;
	}

	public StreamSingleton ()
	{
		m_stream = new SerialPort ("/dev/cu.usbserial-AH01KCPQ", 57600);
		m_rcvMsgs = new List<string> ();
		m_rcvMsgLock = new Object ();
		m_linkCnt = 0;
		//m_rcvThread = new Thread ();
	}

	public void newRcvThread (ThreadStart func)
	{
		m_rcvThread = new Thread (func);
	}

	public void addReceive ()
	{
		//if link is 0 so that i need to start a new one, otherwise i just continue
		Debug.Log ("addReceive:\t" + m_linkCnt);
		if (m_linkCnt == 0) {
			m_rcvThread = new Thread (receiveRobots);
			m_rcvThread.Start ();
			thread_running = true;
		}
		++m_linkCnt;
	}

	public void minusThread (bool stop = false)
	{
		if (m_rcvThread == null)
			return;
		Debug.Log ("minusThread:\t" + m_linkCnt);
		if (m_linkCnt > 0) {
			--m_linkCnt;
		}
		if (m_linkCnt == 0) {
			//thread_running = false;
			m_exStop = true;
			Debug.Log (m_rcvThread.ThreadState);
		}
		if (stop) {
			Debug.Log ("Before join: " + m_rcvThread.ThreadState);

			m_rcvThread.Join ();
			Debug.Log ("After join");
			Debug.Log (m_rcvThread.ThreadState);
		}
	}

	bool validateRtn(string cmd, string rtnMsg){
		if (rtnMsg.Length > cmd.Length) {
			if (rtnMsg.StartsWith ("t:")) {
				if (rtnMsg [2] == 'A' ||
				   rtnMsg [2] == 'B') {
					return true;
				}
			}
		} 
		return false;
	}

	bool validateCmd(string cmd){
		if (cmd != null && cmd.Length >= 5) {
				if (cmd [0] == 'A' ||
					cmd [0] == 'B') {
					return true;
				}
		} 
		return false;
	}

	public int match (string cmd)
	{
		//lock (m_rcvMsgLock) {
		List<int> removeList = new List<int> ();
		int ret = 2;
		for (int i = 0; i < m_rcvMsgs.Count; i++) {
			string curMsg = m_rcvMsgs [i];
			// check if command is match with the receive msg
			if (validateCmd(cmd)) {
				if (validateRtn (cmd, curMsg)) {
					if (curMsg.Substring (2, cmd.Length - 1).Equals (cmd.Substring (0, cmd.Length - 1))) {
						Debug.Log ("matched:\t" + cmd + "\t" + curMsg);
						removeList.Add (i);
						ret = 0;
						break;
					} 
					// external stop leads to later receive msg
					else if (cmd [0] == curMsg [2]) {
						Debug.Log ("discard:\t" + cmd + "\t" + curMsg);
						removeList.Add (i);
						ret = 1;
					}
				} else {
					Debug.Log ("discard:\t" + cmd + "\t" + curMsg);
					removeList.Add (i);
					ret = 1;
				}
			}
		}
		removeList.Sort ();
		for (int i = removeList.Count - 1; i >= 0; i--)
			m_rcvMsgs.RemoveAt (removeList[i]);
		return ret;
		//}
	}

	void receiveRobots ()
	{
//		if (!thread_running)
//			return;
		if (m_exStop) {
			Debug.Log ("stop external:\t" + m_linkCnt);
			--m_linkCnt;
			m_exStop = false;
		}
		while(m_linkCnt > 0){
			Debug.Log ("in receive:" + m_linkCnt);
			//lock (m_rcvMsgLock) {
			string returnMsg = m_stream.ReadLine ();
			// if it returns too slow, it has already got stop externally, so that the return msg is not match to the current command
			if (returnMsg.Length > 0) {
				Debug.Log ("add msg\t" + returnMsg);
				m_rcvMsgs.Add (returnMsg);
			}
		}
		// clear the command
		//clear ();
		Debug.Log ("after receive");
		//m_bRtn = true;
		return;
	}

	public Thread getReceiveThread ()
	{
		return m_rcvThread;
	}

	static StreamSingleton stream_inst = null;

	static public StreamSingleton getInst ()
	{
		if (stream_inst == null)
			stream_inst = new StreamSingleton ();
		return stream_inst;
	}
}
