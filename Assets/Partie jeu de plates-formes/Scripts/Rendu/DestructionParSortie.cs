using UnityEngine;
using System.Collections;



public class DestructionParSortie : MonoBehaviour {

	void OnTriggerExit2D(Collider2D other) {
		string tag = other.gameObject.tag;
		if (tag == "Player")
			Application.LoadLevel(Application.loadedLevelName);
		else if (tag != "Boundary" && tag != "Enemy")
			DestroyObject(other.gameObject);

	}

}