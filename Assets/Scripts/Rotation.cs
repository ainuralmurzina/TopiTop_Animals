using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {


	public Transform rotateAround;
	public float speed = -20f;

	void Update () {
		transform.RotateAround(rotateAround.position, Vector3.forward, speed * Time.deltaTime);
	}
}
