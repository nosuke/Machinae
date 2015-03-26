using UnityEngine;
using System.Collections;



public class DestructionParContact : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "Limites")
			return;
		if (!(tag == "Projectile" && other.gameObject.tag == "BossProjectile")) {
			if (tag != "BossProjectile"
				|| (tag == "BossProjectile" && other.tag != "Projectile"))
					Destroy (gameObject);
		}
		if (tag != "BossProjectile" && (other.gameObject.tag == "EnemyProjectile" || other.gameObject.tag == "Projectile"))
			Destroy (other.gameObject);
	}
}