using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class SetDepthPassToCamera : MonoBehaviour
{
	public DepthTextureMode mode;
	// Use this for initialization
	void Start ()
	{
		GetComponent<Camera>().depthTextureMode = mode;
	}
	
#if UNITY_EDITOR
	private void OnValidate()
	{
		GetComponent<Camera>().depthTextureMode = mode;
	}
#endif

}
