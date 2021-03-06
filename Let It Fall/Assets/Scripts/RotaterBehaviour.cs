using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaterBehaviour : MonoBehaviour {

	Vector3 pos;
	float prevAng;
	float ang;
	float initAngle;
	//BallBehaviour ballScript;
	bool autoMove = false;
	//bool isMoving = true;
	//bool isClicked = false;
	bool fadeAwayInstruction = false;
	float alphaLevel = 1f;

	float rotateLimitTop = 90f;
	float rotateLimitBottom = -90f;

	bool soundPlayed = false;
	int nextStop;
	bool isNextStopDefined = false;


	void Start () {
		//ballScript = GameObject.FindObjectOfType (typeof(BallBehaviour)) as BallBehaviour;
		initAngle = transform.rotation.eulerAngles.z;
		//print ("Init Ang: " + initAngle);
		//audioSource.clip = rotaterSound;
		if (transform.root.gameObject.name.Contains("Fake")) {
			rotateLimitTop = 180f;
			rotateLimitBottom = 0f;
		}

	}

	void Update(){

		//determine if game is stopped or paused
		//isMoving = !ballScript.getStopMovementFlag () && !ballScript.getGamePausedFlag ();

		if (autoMove && GameManager.IsBallFalling()) {

			if (!isNextStopDefined) {
				//print ("currAng: " + ang);
				nextStop = CommonFunctions.FindNextStop (initAngle, ang);
				isNextStopDefined = true;
				//print ("Go To: " + nextStop);
			}

			if (ang < nextStop) {
				if (ang > nextStop - 5f) {
					ang = nextStop;
					autoMove = false;
					initAngle = ang;
					if (initAngle == 360)
						initAngle = 0;
				} else
					ang += Time.deltaTime * 300f;
			} else {
				if (ang < nextStop + 5f) {
					ang = nextStop;
					autoMove = false;
					initAngle = ang;
					if (initAngle == 360)
						initAngle = 0;
				} else
					ang -= Time.deltaTime * 300f;
			}

			transform.rotation = Quaternion.AngleAxis (ang, Vector3.forward);

		}

		if (fadeAwayInstruction && GameManager.IsBallFalling()) {
			if (alphaLevel > 0.0f) {
				alphaLevel -= Time.deltaTime * 5;
				transform.root.FindChild ("Instruction").gameObject.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, alphaLevel);
			}

			if (alphaLevel <= 0f) {
				transform.root.FindChild ("Instruction").gameObject.SetActive(false);
				fadeAwayInstruction = false;
			}
		}
	}

	void OnMouseDrag () {
		
		if (GameManager.IsBallFalling()) {

			//play sound
			if (!soundPlayed) {
				//GetComponent<AudioSource> ().Play();
				FindObjectOfType<AudioManager>().Play("Rotater");
				soundPlayed = true;
			}

			pos = Camera.main.WorldToScreenPoint (transform.position);
			pos = Input.mousePosition - pos;

			//Store previous angle
			prevAng = ang;
			//ang = Mathf.Atan2 (pos.y, pos.x) * Mathf.Rad2Deg;
			ang = CommonFunctions.GetAngle (pos);
			
			if ( Mathf.Abs(ang) > Mathf.Abs(initAngle) + 10f || Mathf.Abs(ang) < Mathf.Abs(initAngle) - 10f) {
				autoMove = true;
				isNextStopDefined = false;
			}
			
			transform.rotation = Quaternion.AngleAxis (ang, Vector3.forward);
		}

		if (!transform.root.gameObject.name.Contains("Fake")) {
			if (transform.root.FindChild ("Instruction").gameObject.activeSelf) {
				fadeAwayInstruction = true;
			}
		}

	}

	//void OnMouseDown(){
		//isClicked = true;
	//}
		
}
