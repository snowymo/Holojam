using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class m3piComm : SerialCommunication
{
	char m_speed;

	int m_waitTime;

	string m_command;

	string m_name;

	public string m_returnMsg;

	public bool m_bRtn;

	//protected SerialPort stream;

	public m3piComm ()
	{
		//m_speed = 0.2f;
		//m_waitTime = 0.5f;
		m_returnMsg = "";
		m_bRtn = true;
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

	public void clear ()
	{
		m_command = "";
	}

	public Thread receiveThread;
	public void run ()
	{
		if (stream != null) {
			if (stream.getStream().IsOpen) {
				//Debug.Log ("time bf:\t" + Time.time);
				m_command = m_name + m_command + "E";
				stream.getStream().Write (m_command);
				Debug.Log ("command:\t" + m_command);

				// if robot is not power on then it will die
				//m_returnMsg = stream.ReadLine ();
				//Debug.Log ("time af:\t" + Time.time);
				m_bRtn = false;

				if (receiveThread != null) {
					Debug.Log ("before abort state:\t" + receiveThread.ThreadState);
					receiveThread.Abort ();
				}
				receiveThread = new Thread (receive);
				receiveThread.Name = m_name;
				Debug.Log("after new state :\t" +receiveThread.Name + "\t" + receiveThread.ThreadState);
				//if (receiveThread.ThreadState == ThreadState.Stopped
				//	|| receiveThread.ThreadState == ThreadState.Unstarted) {
				receiveThread.Start ();
				Debug.Log("after start state:\t" + receiveThread.ThreadState);
				//}
			}
		}
	}

//	public void stopThread() {
//		receiveThread.Join ();
//	}

	bool match(){
		// check if command is match with the receive msg
		if (m_command.Length >= 5
		   && m_returnMsg.Length > 10) {
			if (m_returnMsg.Substring (2, m_command.Length-1).Equals (m_command.Substring(0,m_command.Length-1))) {
				// clear the command
				m_command = "";
				return true;
			}
		}
		return false;
	}

	public void receive ()
	{
		do {
			Debug.Log ("in receive");
			m_returnMsg = stream.getStream().ReadLine ();
		} while(!match());
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
