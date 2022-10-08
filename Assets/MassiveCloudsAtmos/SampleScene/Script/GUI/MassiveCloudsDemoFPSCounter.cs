using System;
using UnityEngine;
using UnityEngine.UI;

public class MassiveCloudsDemoFPSCounter : MonoBehaviour
{
	private Text text;
	private float t;
	private long lastTicks;
	private int count;

	// Use this for initialization
	private void Start ()
	{
		text 	  = GetComponent<Text>();
		t         = 0f;
		count 	  = 0;
		lastTicks  = DateTime.Now.Ticks;
		text.text = "---FPS";
	}
	
	// Update is called once per frame
	private void Update ()
	{
		var ticks = DateTime.Now.Ticks;
		t += (ticks - lastTicks);
		lastTicks = ticks;
		count++;
		if (t >= 10000000)
		{
			UpdateCounter();
			t %= 10000000;
		}
	}

	private void UpdateCounter()
	{
		text.text = count + "FPS";
		count = 0;
	}
}
