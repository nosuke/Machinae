using UnityEngine;
using System.Collections;



public class Redemarrage : MonoBehaviour {

	void Start() {
	
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player")
			Application.LoadLevel (Application.loadedLevel);
		else if (other.gameObject.tag == "Enemy") {
			Destroy(other.gameObject);
		}
	}
}