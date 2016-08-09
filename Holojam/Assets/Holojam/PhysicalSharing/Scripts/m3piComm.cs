using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class m3piComm : SerialCommunication
{
	float m_speed;

	float m_waitTime;

	//protected SerialPort stream;

	public m3piComm ()
	{
		m_speed = 0.2f;
		m_waitTime = 0.5f;
	}

	static private m3piComm m_inst = null;

	static public m3piComm getInst ()
	{
		if (m_inst == null)
			m_inst = new m3piComm ();
		return m_inst;
	}

	public override void setSpeed(float sp){
		m_speed = sp;

	}

	public override void setWaitTime(float wt){
		m_waitTime = wt;
	}

	public override void forward ()
	{
		open ();
		stream.Write ("f" + (m_speed * 10.0f).ToString () + (m_waitTime * 10.0f).ToString () + "e");
	}

	public override void backward ()
	{
		open ();
		stream.Write ("b" + (m_speed * 10.0f).ToString () + (m_waitTime * 10.0f).ToString () + "e");
	}

	public override void left ()
	{
		open ();
		stream.Write ("l" + (m_speed * 10.0f).ToString () + (m_waitTime * 10.0f).ToString () + "e");
	}

	public override void right ()
	{
		open ();
		stream.Write ("r" + (m_speed * 10.0f).ToString () + (m_waitTime * 10.0f).ToString () + "e");
	}

	public override void stop ()
	{
		open ();
		stream.Write ("se");
	}
}
