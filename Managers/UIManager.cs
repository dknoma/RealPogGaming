using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

	public static UIManager um;
	
	[SerializeField]
	private GameObject battleUI;
//    private static GameObject _battleUI;

	[SerializeField]
	private GameObject mainActionMenuPrefab;
	private static GameObject _mainActionMenu;
	
	private void OnEnable() {
		if (um == null) {
			um = this;
		} else if (um != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	public void InitBattleUI() {
		Vector3 cameraPos = Camera.main.transform.position;
		battleUI.transform.position = new Vector3(cameraPos.x, cameraPos.y, transform.position.z);
		battleUI.SetActive(true);
		if (_mainActionMenu == null) {
			_mainActionMenu = Instantiate(mainActionMenuPrefab, BattleManager.battleCanvas.transform, false);
		}
	}
	
	public void DisableBattleUI() {
		battleUI.SetActive(false);
		_mainActionMenu.SetActive(false);
	}

	public GameObject GetBattleUI() {
		return battleUI;
	}
	
	public GameObject GetMainActionMenu() {
		return _mainActionMenu;
	}
}
