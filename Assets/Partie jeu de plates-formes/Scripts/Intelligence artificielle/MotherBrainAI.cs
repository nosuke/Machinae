using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Animation {
	public Animator			animator;
	public SpriteRenderer	rightBowlRenderer;
	public SpriteRenderer	leftBowlRenderer;
	public AnimationClip 	dieAnimation;
}

[System.Serializable]
public class MotherBrainWeapon {
	public GameObject	basicWeaponFire;
	public GameObject	roomWeaponFire;
	public float		basicSpeed = 5.0f;
	public float		roomSpeed = 2.0f;
	public float 		coolDownBasicShoot = 2.0f;
	public float 		coolDownRoomShoot = 6.0f;
	public float 		coolDownBombShoot = 10.0f;
}

[System.Serializable]
public class Stats {
	public float motherBrainMaxLife = 500.0f;
	public float bowlMaxLife = 200.0f;
	public float enemyRange = 5.0f;
}

public class MotherBrainAI : MonoBehaviour {

	public Stats					stats;
	public MotherBrainWeapon		weapon;
	public Animation				motherBrainAnimation;

	private float		motherBrainCurrentLife;
	private float		leftBowlCurrentLife;
	private float		rightBowlCurrentLife;

	private float		lastNormalShoot;
	private float		lastRoomShoot;
	private float		lastBombShoot;

	private bool		isTopSpawnAnimated = false;
	private bool		isBottomSpawnAnimated = false;

	private Dictionary<string, Sprite>	motherBrainSpriteByName;

	// Use this for initialization
	void Start () {
		motherBrainCurrentLife = stats.motherBrainMaxLife;
		rightBowlCurrentLife = stats.bowlMaxLife;
		leftBowlCurrentLife = stats.bowlMaxLife;

		lastNormalShoot = Time.time;
		lastRoomShoot = Time.time;
		lastBombShoot = Time.time;

		GameObject.Find("MotherBrainTopRightSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainBottomRightSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainTopLeftSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainBottomLeftSpawnShoot").GetComponent<Animator>().enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 playerPosition = GameObject.FindWithTag ("Player").transform.position;
		Vector2 heading = playerPosition - transform.position;

		if (heading.sqrMagnitude < stats.enemyRange * stats.enemyRange) {
			if (motherBrainCurrentLife <= 0) return;

			float lifePercentage = GetMotherBrainLifePercentage();

			if (lastBombShoot + weapon.coolDownBombShoot < Time.time && lifePercentage <= 25.0f) {
				BombShoot(heading.normalized);
			} else if (lastRoomShoot + weapon.coolDownRoomShoot < Time.time && lifePercentage <= 50.0f) {

				StartCoroutine(DelayRoomShoot(heading.normalized, 0.5f, "Bottom"));
				RoomShoot(heading.normalized, "Top");

				if (isBottomSpawnAnimated == false) {
					GameObject.Find("MotherBrainBottomRightSpawnShoot").GetComponent<Animator>().enabled = true;
					GameObject.Find("MotherBrainBottomLeftSpawnShoot").GetComponent<Animator>().enabled = true;
					isBottomSpawnAnimated = true;
				}
			} else if (lastRoomShoot + weapon.coolDownRoomShoot < Time.time && lifePercentage <= 75.0f) {
				RoomShoot(heading.normalized, "Top");

				if (isTopSpawnAnimated == false) {
					GameObject.Find("MotherBrainTopRightSpawnShoot").GetComponent<Animator>().enabled = true;
					GameObject.Find("MotherBrainTopLeftSpawnShoot").GetComponent<Animator>().enabled = true;
					isTopSpawnAnimated = true;
				}
			} else if (lastNormalShoot + weapon.coolDownBasicShoot < Time.time) {
				BasicShoot(heading.normalized);
			}	
		} else if (motherBrainCurrentLife != stats.motherBrainMaxLife) {
			Reset();
		}
	}

	IEnumerator DelayRoomShoot(Vector2 playerDirection, float time, string type) {
		yield return new WaitForSeconds(time);
		float lastTime = lastRoomShoot;
		RoomShoot(playerDirection, type);
		lastRoomShoot = lastTime;
	}

	void BasicShoot(Vector2 playerDirection) {
		Transform spawnPosition;

		if (playerDirection.x > 0) {
			spawnPosition = GameObject.Find("MotherBrainRightSpawnShoot").transform;
		} else {
			spawnPosition = GameObject.Find("MotherBrainLeftSpawnShoot").transform;
		}

		GameObject cloneMunition = Instantiate(weapon.basicWeaponFire, spawnPosition.position, spawnPosition.rotation) as GameObject;
		cloneMunition.rigidbody2D.velocity = playerDirection * weapon.basicSpeed;

		Vector2 forward = playerDirection;
		forward.y = 0;

		float angle = Vector2.Angle (playerDirection, forward);
		bool revertSprite = forward.x > 0 ? true : false;

		angle = forward.x > 0 ? angle : -angle;
		angle = playerDirection.y > 0 ? angle : -angle;

		cloneMunition.transform.Rotate(new Vector3(0, 0, angle));
		if (revertSprite == true)
			cloneMunition.renderer.transform.Rotate (new Vector3(0, 0, 180f));

		lastNormalShoot = Time.time;
	}

	void RoomShoot(Vector2 playerDirection, string type) {
		Transform spawnPosition;
		
		if (playerDirection.x > 0) {
			spawnPosition = GameObject.Find("MotherBrain" + type + "RightSpawnShoot").transform;
		} else {
			spawnPosition = GameObject.Find("MotherBrain" + type + "LeftSpawnShoot").transform;
		}

		if (type == "Bottom")
			Debug.Log ("Bottom shoot");

		Vector2 direction = type == "Top" ? new Vector2(0, -1.0f) : playerDirection.x > 0 ? new Vector2(-1.0f, 0) : new Vector2(1.0f, 0);

		GameObject cloneMunition = Instantiate(weapon.roomWeaponFire, spawnPosition.position, spawnPosition.rotation) as GameObject;
		cloneMunition.rigidbody2D.velocity = direction * weapon.roomSpeed;
		cloneMunition.renderer.transform.Rotate (new Vector3(0, 0, type == "Top" ? 90f : playerDirection.x > 0 ? 0f : 180f));

		lastRoomShoot = Time.time;
		lastNormalShoot = Time.time;
	}

	void BombShoot(Vector2 playerDirection) {
		Transform spawnPosition;

		if (playerDirection.x > 0) {
			spawnPosition = GameObject.Find("MotherBrainRightSpawnShoot").transform;
		} else {
			spawnPosition = GameObject.Find("MotherBrainLeftSpawnShoot").transform;
		}

		GameObject cloneMunition = Instantiate(weapon.basicWeaponFire, spawnPosition.position, spawnPosition.rotation) as GameObject;
		cloneMunition.rigidbody2D.velocity = new Vector2(-2.0f, 7.0f) * weapon.basicSpeed;
		cloneMunition.rigidbody2D.isKinematic = false;
		
		lastRoomShoot = Time.time;
		lastNormalShoot = Time.time;
		lastBombShoot = Time.time;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Projectile") {
			Vector3 projectilePosition = other.transform.position;
			Vector2 heading = projectilePosition - transform.position;

			heading.y = 0;
			if (motherBrainCurrentLife > 0)
				TakeDamage(other.gameObject, heading.normalized);
		}
	}

	void TakeDamage(GameObject other, Vector2 heading) {
		Damage d = other.GetComponent<Damage> ();

		if (rightBowlCurrentLife > 0 && heading.x > 0) {
			rightBowlCurrentLife -= d.damage;
		} else if (leftBowlCurrentLife > 0 && heading.x < 0) {
			leftBowlCurrentLife -= d.damage;
		} else {
			motherBrainCurrentLife -= d.damage;
		}

		ChangeBowlSprite (GetBowlLifePercentage(rightBowlCurrentLife), GetBowlLifePercentage(leftBowlCurrentLife));
		
		if (motherBrainCurrentLife <= 0) {
			Die ();
		}
		Destroy(other);
	}

	void Awake() {
		Sprite[] motherBrainSprites = Resources.LoadAll<Sprite>("MotherBrain");
		motherBrainSpriteByName = new Dictionary<string, Sprite>();

		for (int i = 0; i != motherBrainSprites.Length; ++i) {
			Sprite sprite = motherBrainSprites[i];
			motherBrainSpriteByName.Add(sprite.name, sprite);
		}
	}

	void ChangeBowlSprite(float rightBowlPercentage, float leftBowlPercentage) {
		string rightSpriteName = "MotherBrainRight_100";
		string leftSpriteName = "MotherBrainLeft_100";
	
		if (rightBowlPercentage <= 75.0f && rightBowlPercentage > 50.0f) {
			rightSpriteName = "MotherBrainRight_75";
		} else if (rightBowlPercentage <= 50.0f && rightBowlPercentage > 25.0f) {
			rightSpriteName = "MotherBrainRight_50";
		} else if (rightBowlPercentage <= 25.0f && rightBowlPercentage > 0.0f) {
			rightSpriteName = "MotherBrainRight_25";
		} else if (rightBowlPercentage <= 0.0f) {
			rightSpriteName = "MotherBrainRight_0";
		}

		if (leftBowlPercentage <= 75.0f && leftBowlPercentage > 50.0f) {
			leftSpriteName = "MotherBrainLeft_75";
		} else if (leftBowlPercentage <= 50.0f && leftBowlPercentage > 25.0f) {
			leftSpriteName = "MotherBrainLeft_50";
		} else if (leftBowlPercentage <= 25.0f && leftBowlPercentage > 0.0f) {
			leftSpriteName = "MotherBrainLeft_25";
		} else if (leftBowlPercentage <= 0.0f) {
			leftSpriteName = "MotherBrainLeft_0";
		}

		motherBrainAnimation.rightBowlRenderer.sprite = motherBrainSpriteByName[rightSpriteName];
		motherBrainAnimation.leftBowlRenderer.sprite = motherBrainSpriteByName[leftSpriteName];
	}

	float GetMotherBrainLifePercentage() {
		return (motherBrainCurrentLife / stats.motherBrainMaxLife) * 100.0f;
	}

	float GetBowlLifePercentage(float life) {
		return (life / stats.bowlMaxLife) * 100.0f;
	}

	void Reset() {
		motherBrainCurrentLife = stats.motherBrainMaxLife;
		rightBowlCurrentLife = stats.bowlMaxLife;
		leftBowlCurrentLife = stats.bowlMaxLife;

		GameObject.Find("MotherBrainTopRightSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainBottomRightSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainTopLeftSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainBottomLeftSpawnShoot").GetComponent<Animator>().enabled = false;

		isBottomSpawnAnimated = false;
		isTopSpawnAnimated = false;

		ChangeBowlSprite (GetBowlLifePercentage(rightBowlCurrentLife), GetBowlLifePercentage(leftBowlCurrentLife));
	}

	void Die() {
		//animator.Play(dieAnimation.name);
		//Destroy(this.gameObject, dieAnimation.length);
		
		GameObject.Find("MotherBrainTopRightSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainBottomRightSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainTopLeftSpawnShoot").GetComponent<Animator>().enabled = false;
		GameObject.Find("MotherBrainBottomLeftSpawnShoot").GetComponent<Animator>().enabled = false;

		Destroy(GameObject.Find("MotherBrainSprite")); // Change ?
	}

}
