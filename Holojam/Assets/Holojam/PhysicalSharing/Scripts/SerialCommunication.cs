using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class SerialCommunication {

	protected SerialPort stream;

	public SerialCommunication(){
		
	}

	public void open(){
		if(stream == null)
			stream = new SerialPort ("/dev/cu.usbserial-AH01KCPQ",57600);
		if(!stream.IsOpen)
			stream.Open ();
		high ();
	}

	public void forward(){
		open ();
		stream.Write ("f");
		//stream.Write ("f");
	}

	public void backward(){
		open ();
		stream.Write ("b");
	}

	public void left(){
		open ();
		stream.Write ("z");
		//stream.Write ("z");
	}

	public void right(){
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
