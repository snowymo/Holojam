using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class SerialCommunication
{

	protected StreamSingleton stream;

	public SerialCommunication ()
	{
		stream = null;
	}

	static SerialCommunication m_Instance = null;

	static public SerialCommunication getInstance ()
	{
		if (m_Instance == null) {
			m_Instance = new SerialCommunication ();
		}
		return m_Instance;
	}

	public void open ()
	{
		if (stream == null)
			stream = StreamSingleton.getStreamInst ();
		if (stream != null) {
			if (!stream.getStream().IsOpen) {
				stream.getStream().Open ();
			}
		}
	}

	public void forward (float dis)
	{
		open ();
		if (stream != null) {
			int round = (int)(dis / 0.07);
			string command = "h";
			for (int r = 0; r < round; r++) {
				command += "f";
				if (r % 3 == 0)
					command += "my";
			}
			float left = dis - round * 0.06f;
			if (left > 0.035) {
				command += "mf";
				left -= 0.035f;
			}
			if (left > 0)
				command += "lf";
			stream.getStream().Write (command);
//		if (dis > 0.05)
//			stream.Write ("h");
//		else if (dis > 0.025)
//			stream.Write ("m");
//		else
//			stream.Write ("l");
//		stream.Write ("f");
			//stream.Write ("f");
		}
	}

	public virtual void forward ()
	{
		open ();
		if (stream != null) {
			stream.getStream().Write ("f");
		}
		//stream.Write ("f");
	}

	public void forwardTest (int round)
	{
		open ();
		if (stream != null) {
			string command = "";
			for (int r = 0; r < round; r++)
				command += "f";
			stream.getStream().Write (command);
		}
		//stream.Write ("f");
	}

	public void backwardTest (int round)
	{
		open ();
		if (stream != null) {
			string command = "";
			for (int r = 0; r < round; r++)
				command += "b";
			stream.getStream().Write (command);
		}
		//stream.Write ("f");
	}

	public virtual void backward ()
	{
		open ();
		if (stream != null) {
			stream.getStream().Write ("b");
		}
	}

	public void backward (float dis)
	{
		open ();
		if (stream != null) {
			int round = (int)(dis / 0.06);
			string command = "h";
			for (int r = 0; r < round; r++) {
				command += "b";
				if ((r % 5 == 2) || (r % 5 == 0))
					command += "mz";
			}
			float left = dis - round * 0.07f;
			if (left > 0.028) {
				command += "mb";
				left -= 0.035f;
			}
			if (left > 0)
				command += "lb";
			stream.getStream().Write (command);
			//		if (dis > 0.05)
			//			stream.Write ("h");
			//		else if (dis > 0.025)
			//			stream.Write ("m");
			//		else
			//			stream.Write ("l");
			//		stream.Write ("f");
			//stream.Write ("f");
		}
	}

	public void left (float angle)
	{
		if (angle < 0)
			right (-angle);
		open ();
		if (stream != null) {
			int round = (int)(angle / 15);
			string command = "h";
			for (int i = 0; i < round; i++)
				command += "z";
			angle = angle - round * 15;
			if (angle > 6) {
				command += "mz";
				angle -= 6;
			}
			if (angle > 0)
				command += "lz";
			stream.getStream().Write (command);
		}
		//stream.Write ("z");
	}

	public virtual void left ()
	{
		open ();
		if (stream != null) {
			stream.getStream().Write ("z");
		}
		//stream.Write ("z");
	}

	public virtual void right ()
	{
		open ();
		if (stream != null) {
			stream.getStream().Write ("y");
		}
		//stream.Write ("z");
	}

	public virtual void stop ()
	{
	}

	public virtual void setSpeed (int f)
	{
	}

	public virtual  void setWaitTime (int f)
	{
	}

	public void right (float angle)
	{
		if (angle < 0)
			left (-angle);
		open ();
		if (stream != null) {
			int round = (int)(angle / 15);
			string command = "h";
			for (int i = 0; i < round; i++)
				command += "y";
			angle = angle - round * 15;
			if (angle > 6) {
				command += "my";
				angle -= 6;
			}
			if (angle > 0)
				command += "ly";
			stream.getStream().Write (command);
		}
	}

	public void median ()
	{
		stream.getStream().Write ("m");
	}

	public void high ()
	{
		stream.getStream().Write ("h");
	}

	public void low ()
	{
		stream.getStream().Write ("l");
	}
}
