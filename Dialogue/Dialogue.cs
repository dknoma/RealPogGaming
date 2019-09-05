using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue : MonoBehaviour {

	public string unitName;
	[TextArea(3, 10)]
	public string[] sentences;

}
