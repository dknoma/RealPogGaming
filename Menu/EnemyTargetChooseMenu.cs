using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTargetChooseMenu", menuName = "Menu/EnemyTargets", order = 1)]
public class EnemyTargetChooseMenu : MenuObject<GameObject> {
	
	public void InitTargetMenu(int options) {
		menu.InitMenu(options, menuType);
		menu.InitMenuItems(new GameObject[options]);
	}
}
