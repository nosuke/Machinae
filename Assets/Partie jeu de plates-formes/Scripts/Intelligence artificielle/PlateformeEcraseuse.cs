using UnityEngine;
using System.Collections;



[System.Serializable]
public class ParametresPlateformeEcraseuse {
	public float tempsAttente = 1;
	public float vitesseMouvementHaut = 3;
	public float vitesseMouvementBas = 30;
}



public class PlateformeEcraseuse : MonoBehaviour {

	public Transform 	positionVerticaleMaximum;
	public Transform 	positionVerticaleMinimum;
	public Transform 	plateformeBougeantVerticalement;

	public ParametresPlateformeEcraseuse 	paramètresOptionnels;
	
	private Transform 	destination;
	private Vector2 	direction = - Vector2.up;
	private float 		attente;


	void Start() {
		destination = positionVerticaleMinimum;
		attente = Time.time;
	}

	void FixedUpdate() {
		float vitesseMouvement = direction == Vector2.up ? paramètresOptionnels.vitesseMouvementHaut : paramètresOptionnels.vitesseMouvementBas;

		if (attente <= Time.time) {
			plateformeBougeantVerticalement.rigidbody2D.MovePosition (plateformeBougeantVerticalement.rigidbody2D.position + direction * vitesseMouvement * Time.fixedDeltaTime);
		}

		Vector2 v2 = plateformeBougeantVerticalement.rigidbody2D.position;

		if ((v2.y >= destination.position.y && direction == Vector2.up) || (v2.y <= destination.position.y && direction == - Vector2.up)) {
			destination = direction == Vector2.up ? positionVerticaleMinimum : positionVerticaleMaximum;
			direction = direction == Vector2.up ? - Vector2.up : Vector2.up;
			if (direction != Vector2.up)
				attente = Time.time + paramètresOptionnels.tempsAttente;
		}
	}

}