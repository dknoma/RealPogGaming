using System.Collections.Generic;
using UnityEngine;

public class BattleBackgroundManager : MonoBehaviour {

	public static BattleBackgroundManager bbm;

	[SerializeField]
	private GameObject grasslandBGPrefab;
	private GameObject grasslandBG;

//	private readonly Camera mainCamera = Camera.main;
	private Dictionary<WorldArea, GameObject> areaBackgrounds = new Dictionary<WorldArea, GameObject>();
	private GameObject currentBackground;
	private WorldArea currentArea;

	private void OnEnable() {
		if (bbm == null) {
			bbm = this;
		} else if (bbm != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		currentBackground = null;
	}

	private void Awake() {
		if (grasslandBG == null) {
			grasslandBG = grasslandBGPrefab;
			areaBackgrounds[WorldArea.PaltryPlains] = grasslandBG;
			currentArea = WorldArea.PaltryPlains;
		}
	}

	public void LoadBackground() {
		if (currentBackground == null) {
//			currentBackground = areaBackgrounds[currentArea];
			Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
			// TODO: If area changes, change current area and change current background
			//			Destroy currentbackground, and instantiate the new current
			currentBackground = Instantiate(areaBackgrounds[currentArea], cameraPos, Quaternion.identity);
			Debug.LogFormat("Intantiating {0}", currentBackground);
		}
//		Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
//		Instantiate(currentBackground, cameraPos, Quaternion.identity);
		ShowBackground(true);
	}

	public WorldArea GetArea() {
		return currentArea;
	}

	public void SetArea(WorldArea area) {
		currentArea = area;
//		if (areaBackgrounds[area] != null) {
//			currentBackground = areaBackgrounds[area];
//		}
	}
	
	public void SetBackground(WorldArea area) {
		currentBackground = areaBackgrounds[area];
	}

	public void ShowBackground(bool isActive) {
		currentBackground.SetActive(isActive);
	}
}
