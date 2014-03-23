using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ProcessHit() {
		animator.SetTrigger("hit");
		gameObject.layer = LayerMask.NameToLayer("Default");
		StartCoroutine("Rebuild");
	}

	IEnumerator Rebuild() {
		yield return new WaitForSeconds(3);
		animator.SetTrigger("rebuild");

		yield return new WaitForSeconds(0.5f);
		gameObject.layer = LayerMask.NameToLayer("Ground");
	}
}
