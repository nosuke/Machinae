using UnityEngine;
using System.Collections;

[System.Serializable]
public class Weapon {
	public GameObject munition;
	public Transform fireSpawn;
	public AudioClip sound;

	public float firingWaitTime = 1.3f;

	public float speed = 5.0f;
}

public class FiringEnemyAI : MonoBehaviour {
	
	public float enemyRange = 10.0f;
	public float speed = 1.0f;
	public float life = 10.0f;

	public Animator animator;
	public AnimationClip attackAnimation;

	public Weapon weapon;
	
	private float nextTimeFire;
	private float nextTimeMove;

	// Use this for initialization
	void Start () {
		nextTimeFire = Time.time;
		nextTimeMove = Time.time;
	}
	
	void Update() {
		Vector3 playerPosition = GameObject.FindWithTag ("Player").transform.position;
		Vector2 heading = playerPosition - weapon.fireSpawn.position;

		if (heading.sqrMagnitude < enemyRange * enemyRange) {
			if (nextTimeFire <= Time.time) {
				animator.SetBool("Walk", false);
				animator.SetBool("Attack", true);
				rigidbody2D.velocity = new Vector2(0, 0);

				StartCoroutine(fire (heading, playerPosition));
				nextTimeFire = Time.time + weapon.firingWaitTime;
				nextTimeMove = Time.time + attackAnimation.length;

			} else if (Time.time >= nextTimeMove) {
				heading.y = 0;
				animator.SetBool("Attack", false);
				animator.SetBool("Walk", true);

				float distance = Vector2.Distance(playerPosition, transform.position);
				rigidbody2D.velocity = heading.normalized * speed;
				if (distance > 0.3f &&
				    ((heading.normalized.x <= 0 && transform.localScale.x < 0) ||
				    (heading.normalized.x > 0 && transform.localScale.x > 0))) {
					Vector3 scale = transform.localScale;
					scale.x *= -1;
					transform.localScale = scale;
				}
			}
		} else {
			animator.SetBool("Walk", false);
			animator.SetBool("Attack", false);
			rigidbody2D.velocity = new Vector2(0, 0);
		}
	}

	IEnumerator fire(Vector2 heading, Vector3 playerPosition) {
		yield return new WaitForSeconds(0.3f);
		GameObject cloneMunition = Instantiate(weapon.munition, weapon.fireSpawn.position, weapon.fireSpawn.rotation) as GameObject;
		cloneMunition.rigidbody2D.velocity = heading.normalized * weapon.speed;
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