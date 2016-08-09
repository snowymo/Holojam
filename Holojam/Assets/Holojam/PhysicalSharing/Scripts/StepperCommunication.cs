using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class StepperCommunication : SerialCommunication {

	static StepperCommunication m_Instance = null;

	private StepperCommunication(){
		
	}

	public static StepperCommunication getInstance(){
		if (m_Instance == null) {
			m_Instance = new StepperCommunication ();
		}
		return m_Instance;
	}
		

//	public void open(){
//		if (stream == null) {
//			stream = new SerialPort ("/dev/cu.usbserial-AH01KCPQ", 57600);
//			stream.Open ();
//		}
//	}

	string encode(int left, int right){
		char chl = (char)('A' + left);
		char chr = (char)('A' + right);
		char[] ch = { '*', chl, chr, '#' };
		return new string(ch);
	}

	public void forward(int step){
		if (step > 20)
			step = 20;
		open ();
		stream.Write (encode(step,step));
	}

	public void backward(int step){
		if (step > 20)
			step = 20;
		open ();
		stream.Write (encode(-step,-step));
	}

	public void left(int step){
		if (step > 10)
			step = 10;
		open ();
		stream.Write (encode(0,step));
	}

	public void right(int step){
		if (step > 10)
			step = 10;
		open ();
		stream.Write (encode(step,0));
	}
}
