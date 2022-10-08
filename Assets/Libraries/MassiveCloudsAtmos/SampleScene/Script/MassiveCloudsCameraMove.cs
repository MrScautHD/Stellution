using UnityEngine;

public class MassiveCloudsCameraMove : MonoBehaviour
{

	public float velocity = 100;
	public float current = 0;

	private bool moving = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		moving = false;
		if (Input.GetKey("w")) Move(transform.forward);
		if (Input.GetKey("a")) Move(-transform.right);
		if (Input.GetKey("s")) Move(-transform.forward);
		if (Input.GetKey("d")) Move(transform.right);
		if (Input.GetKey("e")) Move(transform.up);
		if (Input.GetKey("q")) Move(-transform.up);
		if (moving)
		{
			if (Input.GetKey(KeyCode.LeftShift))
				current = velocity;
			else
				current = Mathf.Clamp(current + 100f * Time.deltaTime, 0, velocity);
		}
		else
			current = Mathf.Clamp(current - 100f * Time.deltaTime, 0, velocity);
	}

	void Move(Vector3 dir)
	{
		var moveAmount = current * Time.deltaTime;
		transform.position += dir * moveAmount;
		moving = true;
	}
}