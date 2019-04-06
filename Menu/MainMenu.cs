using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MainMenu", menuName = "Menu/MainMenu", order = 1)]
public class MainMenu : MenuObject<ActionCategoryContainer> {
	
	public void InitMainMenu(int options) {
		menu.InitMenu(options, menuType);
		menu.InitMenuItems(new ActionCategoryContainer[options]);
	}
	
	public override void NavigateMenu(Direction direction) {
		DeactivateCurrentButton();
		menu.TraverseOptions(direction);
		ActivateCurrentButton();
	}
	
	public override void AddItem(ActionCategoryContainer item, int index) {
		menu.AddItem(item, index);
	}
	
	public void DeactivateButton(int index) {
		menu.GetItem(index).OptionRender().GetComponent<Image>().color = Color.grey;
	}
	
	public void DeactivateCurrentButton() {
		menu.GetCurrentItem().OptionRender().GetComponent<Image>().color = Color.grey;
	}
	
	public void ActivateCurrentButton() {
		menu.GetCurrentItem().OptionRender().GetComponent<Image>().color = Color.white;
	}
}
