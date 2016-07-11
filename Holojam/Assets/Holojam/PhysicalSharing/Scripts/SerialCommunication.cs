﻿using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class SerialCommunication {

	protected SerialPort stream;

	public SerialCommunication(){
		
	}

	public void open(){
		stream = new SerialPort ("/dev/cu.usbserial-AH01KCPQ",57600);
		stream.Open ();
		median ();
	}

	public void forward(){
		if(!stream.IsOpen)
			open ();
		stream.Write ("f");
		//stream.Write ("f");
	}

	public void backward(){
		if(!stream.IsOpen)
			open ();
		stream.Write ("b");
	}

	public void left(){
		if(!stream.IsOpen)
			open ();
		stream.Write ("z");
		//stream.Write ("z");
	}

	public void right(){
		stream.Write ("y");
		//stream.Write ("y");
	}

	public void median(){
		if(!stream.IsOpen)
			open ();
		stream.Write ("m");
	}

	public void high(){
		stream.Write ("h");
	}

	public void low(){
		stream.Write ("l");
	}
}