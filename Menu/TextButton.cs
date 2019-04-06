using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class TextButton : Button {
	
	[SerializeField]
	private SpriteState SpriteState;
	
	private Transition m_Transition = Transition.SpriteSwap;
	
	//TODO: change state transition to let button know when to change text transform
    protected override void DoStateTransition(SelectionState state, bool instant) {
        Debug.LogFormat("DOING STATE TRANSITION");
        Color color;
        Sprite newSprite;
        string triggername;
        switch (state) {
	        case SelectionState.Normal:
		        newSprite = (Sprite) null;
		        break;
	        case SelectionState.Highlighted:
		        newSprite = SpriteState.highlightedSprite;
		        break;
	        case SelectionState.Pressed:
		        newSprite = SpriteState.pressedSprite;
		        break;
	        case SelectionState.Disabled:
		        newSprite = SpriteState.disabledSprite;
		        break;
	        default:
		        color = Color.black;
		        newSprite = (Sprite) null;
		        triggername = string.Empty;
		        break;
        }
        if (!this.gameObject.activeInHierarchy)
	        return;
        switch (this.m_Transition) {
	        case Transition.SpriteSwap:
		        this.DoSpriteSwap(newSprite);
		        break;
        }
    }
    
    private void DoSpriteSwap(Sprite newSprite) {
	    if ((UnityEngine.Object) this.image == (UnityEngine.Object) null)
		    return;
	    this.image.overrideSprite = newSprite;
    }
}
