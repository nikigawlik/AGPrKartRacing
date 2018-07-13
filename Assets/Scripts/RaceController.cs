using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceController : MonoBehaviour {
	public GameObject[] karts;
	public GameObject[] checkpoints;
	public int numberOfRounds = 3;

	[Header("UI References")]
	public Image[] startSequenceImages;
	public float normalImageTime = 1;
	public float lastImageTime = 2;
	public Text roundsCounterText;
	public Text timeText;

	private int currentCheckpointIndex = 1;
    private int currentRound;
	private float secondsSinceStart;

	private bool raceStarted = false;

    public int CurrentRound
    {
        get
        {
            return currentRound;
        }

        set
        {
            currentRound = value;
			roundsCounterText.text = "Round " + (currentRound + 1) + "/" + numberOfRounds;
        }
    }

    // Use this for initialization
    void Start () {
		StartCoroutine("StartSequence");
		CurrentRound = 0;
		secondsSinceStart = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(raceStarted) {
			secondsSinceStart += Time.deltaTime;
		}

		int minutes = (int) secondsSinceStart / 60;
		int seconds = (int) secondsSinceStart % 60;
		int milliseconds = (int) ((secondsSinceStart % 1) * 1000);

		timeText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + milliseconds.ToString("D3");
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
					raceStarted = true;
				}
				yield return new WaitForSeconds(lastImageTime);
			}
			startSequenceImages[i].gameObject.SetActive(false);
		}
	}

	public void HitCheckpoint(GameObject checkpointObject) {
		if(checkpoints[currentCheckpointIndex] == checkpointObject) {
			currentCheckpointIndex++;
			currentCheckpointIndex = currentCheckpointIndex % checkpoints.Length;
			if(currentCheckpointIndex == 1) {
				CurrentRound++;
			}
		}
	}
}
