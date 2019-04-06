using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuObject<T> : ScriptableObject {

	public int optionCount;
	public MenuType menuType;
	public bool symmetricalRectMenu;
	public int width;
	public int height;
	public Canvas canvas;
	
	protected readonly MenuGraph<T> menu = new MenuGraph<T>();

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

	public MenuGraph<T> Menu() {
		return menu;
	}

	public virtual void NavigateMenu(Direction direction) {
		menu.TraverseOptions(direction);
	}
	
	public virtual T GetCurrentItem() {
		return menu.GetCurrentItem();
	}
	
	public virtual T GetItem(int index) {
		return menu.GetItem(index);
	}
	
	public virtual void AddItem(T item, int index) {
		menu.AddItem(item, index);
	}
	
	public virtual int Size() {
		return menu.Size();
	}
}
