using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

	public static DialogueManager dm;

	private Queue<string> sentences = new Queue<string>();

	private void OnEnable() {
		if (dm == null) {
			dm = this;
		} else if (dm != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	public void StartDialogue(Dialogue dialogue) {
		sentences.Clear();
		foreach (string sentence in dialogue.sentences) {
			sentences.Enqueue(sentence);
		}
	}
}
