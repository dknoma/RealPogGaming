using System;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class MenuObject<T> : MonoBehaviour {

	public int optionCount;
	public MenuType menuType;
	public bool symmetricalRectMenu;
	public int width;
	public int height;
	public Canvas canvas;
	
	private MenuGraph<T> menu = new MenuGraph<T>();

	private void OnEnable() {
		switch (menuType) {
			case MenuType.Horizontal:
				menu.InitMenu(optionCount, menuType);
				break;
			case MenuType.Vertical:
				menu.InitMenu(optionCount, menuType);
				break;
			case MenuType.Both:
				if (symmetricalRectMenu) {
					// Symmetrical rectangular menu
					menu.InitMenu(width, height);
				} else {
					// Asymmetrical x by 1 or 1 by y menus
					menu.InitMenu(optionCount, menuType);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void NavigateMenu(Direction direction) {
		menu.TraverseOptions(direction);
	}
}
