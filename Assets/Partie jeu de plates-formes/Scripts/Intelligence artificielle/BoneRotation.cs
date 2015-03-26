using UnityEngine;
using System.Collections;

public class BoneRotation : MonoBehaviour {

	public float speed = 1.0f;

	private float angle;

	// Use this for initialization
	void Start () {
		angle = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		angle += speed;
		var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = rotation;
	}
}
