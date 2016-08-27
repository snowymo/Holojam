using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class StreamSingleton {

	public SerialPort m_stream;

	public SerialPort getStream(){
		return m_stream;
	}

//	static public SerialPort getStream {
//		get {
//			return m_stream;
//		}
//	}

	public StreamSingleton(){
		m_stream = new SerialPort ("/dev/cu.usbserial-AH01KCPQ", 57600);
	}

	static StreamSingleton stream_inst = null;

	static public StreamSingleton getStreamInst(){
		if (stream_inst == null)
			stream_inst = new StreamSingleton ();
		return stream_inst;
	}
}
