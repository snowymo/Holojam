using UnityEngine;
using System.Threading;

public class MyThread
{
	public int count;
	string thrdName;
	public MyThread(string nam)
	{
		count = 0;
		thrdName = nam;
	}
	public void run()
	{
		Debug.Log("start run a thread");
		bool receved = false;
		while(!receved)
		{
			//Thread.Sleep(1000);
			if (count == 20)
				receved = true;
			//Debug.Log("in child thread-------count="+count);
			count++;   
		}
		count = 0;
		Debug.Log("end thread");
	}

}