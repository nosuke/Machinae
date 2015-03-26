using UnityEngine;
using System.Collections;

public class FlyingEnemyAI : MonoBehaviour {
	
	public float enemyRange = 10.0f;
	public float speed = 5.0f;

	public float life = 10.0f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void Update() {
		Vector3 playerPosition = GameObject.FindWithTag ("Player").transform.position;//playerTest.position;
		Vector2 heading = playerPosition - transform.position;

		if (heading.sqrMagnitude < enemyRange * enemyRange) {
			//transform.Translate(heading.normalized * speed * Time.deltaTime);
			rigidbody2D.velocity = heading.normalized * speed;
			if ((heading.normalized.x < 0 && transform.localScale.x <0) ||
			    (heading.normalized.x > 0 && transform.localScale.x > 0)) {
				Vector3 scale = transform.localScale;
				scale.x *= -1;
				transform.localScale = scale;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Projectile") {
			Damage d = other.gameObject.GetComponent<Damage> ();
			life -= d.damage;
			
			if (life <= 0) {
				//animator.Play(dieAnimation.name);
				//Destroy(this.gameObject, dieAnimation.length);
				Destroy(this.gameObject);
			}
			Destroy(other.gameObject);
		}
	}

}
