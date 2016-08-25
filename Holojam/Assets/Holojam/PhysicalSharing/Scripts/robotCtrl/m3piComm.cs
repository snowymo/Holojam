﻿using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class m3piComm : SerialCommunication
{
	char m_speed;

	int m_waitTime;

	string m_command;

	string m_name;

	public string m_returnMsg;

	//protected SerialPort stream;

	public m3piComm ()
	{
		//m_speed = 0.2f;
		//m_waitTime = 0.5f;
		m_returnMsg = "";
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
			m_command += m_name + "f" + (m_speed).ToString () + (m_waitTime).ToString () + "e";
		}
	}

	public void clear ()
	{
		m_command = "";
	}

	public void run ()
	{
		if (stream != null) {
			if (stream.IsOpen) {
				//Debug.Log ("time bf:\t" + Time.time);
				stream.Write (m_command);
				Debug.Log ("command:\t" + m_command);
				m_command = "";
				// if robot is not power on then it will die
				//m_returnMsg = stream.ReadLine ();
				//Debug.Log ("time af:\t" + Time.time);
			}
		}
	}

	public override void backward ()
	{
		open ();
		if (stream != null) {
			m_command += m_name + "b" + (m_speed).ToString () + (m_waitTime).ToString () + "e";
		}
	}

	public override void left ()
	{
		open ();
		if (stream != null) {
			m_command += m_name + "l" + (m_speed).ToString () + (m_waitTime).ToString () + "e";
		}
	}

	public override void right ()
	{
		open ();
		if (stream != null) {
			m_command += m_name + "r" + (m_speed).ToString () + (m_waitTime).ToString () + "e";
		}
	}

	public override void stop ()
	{
		open ();
		if (stream != null) {
			m_command += m_name + "se";
		}
	}

	public void setName (string name)
	{
		m_name = name;
	}
}
