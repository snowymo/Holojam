using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class m3piComm : SerialCommunication
{
	int m_speed;

	int m_waitTime;

	string m_command;

	//protected SerialPort stream;

	public m3piComm ()
	{
		//m_speed = 0.2f;
		//m_waitTime = 0.5f;
	}

	static private m3piComm m_inst = null;

	static public m3piComm getInst ()
	{
		if (m_inst == null)
			m_inst = new m3piComm ();
		return m_inst;
	}

	public override void setSpeed(int sp){
		m_speed = sp;

	}

	public override void setWaitTime(int wt){
		m_waitTime = wt;
	}

	public override void forward ()
	{
		open ();
		m_command += "Af" + (m_speed).ToString () + (m_waitTime).ToString () + "e";
	}

	public void clear(){
		m_command = "";
	}

	public void run(){
		stream.Write (m_command);
		m_command = "";
	}

	public override void backward ()
	{
		open ();
		m_command += "Ab" + (m_speed ).ToString () + (m_waitTime ).ToString () + "e";
	}

	public override void left ()
	{
		open ();
		m_command +="Al" + (m_speed ).ToString () + (m_waitTime).ToString () + "e";
	}

	public override void right ()
	{
		open ();
		m_command +="Ar" + (m_speed ).ToString () + (m_waitTime ).ToString () + "e";
	}

	public override void stop ()
	{
		open ();
		m_command +="Ase";
	}
}
