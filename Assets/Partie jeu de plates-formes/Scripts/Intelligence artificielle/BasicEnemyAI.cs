using UnityEngine;
using System.Collections;

public class BasicEnemyAI : MonoBehaviour {

	public float enemyRange = 10.0f;
	public float speed = 1.0f;
	public float enemyDamage = 5.0f;

	public float life = 20.0f;

	public Animator animator;
	public AnimationClip dieAnimation;

	// Use this for initialization
	void Start () {
	
	}

	void Update() {
		Vector3 playerPosition = GameObject.FindWithTag ("Player").transform.position;
		Vector2 heading = playerPosition - transform.position;

		heading.y = 0;
		if (heading.sqrMagnitude < enemyRange * enemyRange) {
			rigidbody2D.velocity = heading.normalized * speed;

			if ((heading.normalized.x < 0 && transform.localScale.x <0) ||
			    (heading.normalized.x > 0 && transform.localScale.x > 0)) {
				Vector3 scale = transform.localScale;
				scale.x *= -1;
				transform.localScale = scale;
			}
			animator.SetFloat("Speed", speed);
		} else {
			animator.SetFloat("Speed", 0);
			rigidbody2D.velocity = new Vector2(0, 0);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Projectile") {
			Damage d = other.gameObject.GetComponent<Damage> ();
			life -= d.damage;
			
			if (life <= 0) {
				if (dieAnimation != null) {
					animator.Play(dieAnimation.name);
					Destroy(this.gameObject, dieAnimation.length);
				} else {
					Destroy(this.gameObject);
				}
			}
			Destroy(other.gameObject);
		}
	}
}
