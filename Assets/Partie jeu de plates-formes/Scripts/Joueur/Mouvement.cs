using UnityEngine;
using System.Collections;



public class Mouvement : MonoBehaviour {

	public enum			Direction { DROITE, GAUCHE };

	public bool			débutDéplacement;
	public float		vitesse;

	private Direction	_direction = Direction.DROITE;


	void Start() {
		if (débutDéplacement)
			CommencerDéplacement();
	}

	public void ChangerDirection(Direction direction) {
		_direction = direction;
	}

	public void CommencerDéplacement() {
		if (_direction == Direction.DROITE)
			rigidbody2D.velocity = new Vector2(vitesse, 0.0f);
		else if (_direction == Direction.GAUCHE)
			rigidbody2D.velocity = new Vector2(vitesse * -1, 0.0f);
	}

}