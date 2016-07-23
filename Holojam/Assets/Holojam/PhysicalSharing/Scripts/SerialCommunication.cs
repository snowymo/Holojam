using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class SerialCommunication {

	protected SerialPort stream;

	public SerialCommunication(){
		
	}

	static SerialCommunication m_Instance = null;

	static public SerialCommunication getInstance(){
		if (m_Instance == null) {
			m_Instance = new SerialCommunication ();
		}
		return m_Instance;
	}

	public void open(){
		if(stream == null)
			stream = new SerialPort ("/dev/cu.usbserial-AH01KCPQ",57600);
		if (!stream.IsOpen) {
			stream.Open ();
		}
	}

	public void forward(float dis){
		open ();
		int round = (int)(dis / 0.07);
		string command = "h";
		for (int r = 0; r < round; r++) {
			command += "f";
			if (r % 3 == 0)
				command += "my";
		}
		float left = dis - round * 0.07f;
		if (left > 0.035) {
			command += "mf";
			left -= 0.035f;
		}
		if (left > 0)
			command += "lf";
		stream.Write (command);
//		if (dis > 0.05)
//			stream.Write ("h");
//		else if (dis > 0.025)
//			stream.Write ("m");
//		else
//			stream.Write ("l");
//		stream.Write ("f");
		//stream.Write ("f");
	}

	public void forward(){
		open ();
		stream.Write ("f");
		//stream.Write ("f");
	}

	public void forwardTest(int round){
		open ();
		string command = "";
		for (int r = 0; r < round; r++)
			command += "f";
		stream.Write (command);
		//stream.Write ("f");
	}

	public void backward(){
		open ();
		stream.Write ("b");
	}

	public void left(float angle){
		open ();
		if (angle > 15)
			stream.Write ("h");
		else if (angle > 6)
			stream.Write ("m");
		else
			stream.Write ("l");
		stream.Write ("z");
		//stream.Write ("z");
	}

	public void left(){
		open ();
		stream.Write ("z");
		//stream.Write ("z");
	}

	public void right(float angle){
		open ();
		if (angle > 15)
			stream.Write ("h");
		else if (angle > 6)
			stream.Write ("m");
		else
			stream.Write ("l");
		stream.Write ("y");
		//stream.Write ("y");
	}

	public void median(){
		stream.Write ("m");
	}

	public void high(){
		stream.Write ("h");
	}

	public void low(){
		stream.Write ("l");
	}
}
