using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField] private Transform follow = null;
	[SerializeField] private bool lockPitch = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private Vector3 velocity = Vector3.zero;
	void Update ()
	{
		var toTarget = follow.position - transform.position;
		var target = follow.position - toTarget.normalized * 6f;
		var currentLookat = transform.position + transform.forward * 5f;
		var lookat = follow.position + follow.forward * 5f;
		if (lockPitch)
			lookat.y = transform.position.y;
		transform.LookAt(Vector3.Lerp(currentLookat, lookat, 0.1f * Time.deltaTime));
		transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 1f);
		
	}
}
