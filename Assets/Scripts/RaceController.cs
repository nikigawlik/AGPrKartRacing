using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceController : MonoBehaviour {
	public Image[] startSequenceImages;
	public float normalImageTime = 1;
	public float lastImageTime = 2;
	public GameObject[] karts;

	// Use this for initialization
	void Start () {
		StartCoroutine("StartSequence");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator StartSequence() {
		foreach (GameObject kart in karts)
		{
			kart.GetComponent<KartController>().enabled = false;
		}

		for(int i = 0; i < startSequenceImages.Length; i++) {
			startSequenceImages[i].gameObject.SetActive(true);
			if (i < startSequenceImages.Length - 1) {
				yield return new WaitForSeconds(normalImageTime);
			} else {
				foreach (GameObject kart in karts)
				{
					kart.GetComponent<KartController>().enabled = true;
				}
				yield return new WaitForSeconds(lastImageTime);
			}
			startSequenceImages[i].gameObject.SetActive(false);
		}
	}
}
