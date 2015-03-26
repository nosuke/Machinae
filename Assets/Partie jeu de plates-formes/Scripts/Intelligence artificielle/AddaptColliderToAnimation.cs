using UnityEngine;
using System.Collections;

public class AddaptColliderToAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		BoxCollider2D collider = collider2D as BoxCollider2D;
		collider.size = renderer.bounds.size;

		if (Mathf.Abs (Mathf.Round(transform.rotation.eulerAngles.z)) == 90.0f) {
			collider.size = new Vector2(renderer.bounds.size.y, renderer.bounds.size.x);
		}
	}
}
