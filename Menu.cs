using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu<T> {

	public enum Type { Horizontal, Vertical, Both };
	public enum Direction { Up, Down, Left, Right };

	public int optionNum;
	public Type menuType;

	[SerializeField]
	private T[] singleAxisMenuOptions;
	[SerializeField]
	private T[,] multiAxisMenuOptions;
	private int currentOption;
	private int size;
	private int width;
	private int height;

	public Menu(int size, Type type) {
		this.size = size;
		this.width = size;
		this.height = size;
		this.menuType = type;
	}

	public Menu(int width, int height, Type type) {
		this.size = width * height;
		this.width = width;
		this.height = height;
		this.menuType = type;
	}

	public void initSATestMenu(T[] arr) {
		if(this.menuType != Type.Horizontal || this.menuType != Type.Vertical) {
			
		}
		this.singleAxisMenuOptions = arr;
	}

	public void initMATestMenu(T[,] arr) {
		this.multiAxisMenuOptions = arr;
	}

	public void traverseTestMenu(int direction) {
//		Debug.Log("ma hor: " + (this.width + (this.width * (int)Mathf.Floor(this.currentOption/this.width))));
		switch(menuType) {
		case Type.Horizontal:
			if (direction == (int) Direction.Right) {
				this.currentOption = (this.currentOption + 1) % this.size;
			} else if (direction == (int) Direction.Left) {
				this.currentOption = (this.currentOption + this.size - 1) % this.size;
			}
			break;
		case Type.Vertical:
			if (direction == (int) Direction.Down) {
				this.currentOption = (this.currentOption + 1) % this.size;
			} else if (direction == (int) Direction.Up) {
				this.currentOption = (this.currentOption + this.size - 1) % this.size;
			}
			break;
		case Type.Both:
			if (direction == (int) Direction.Right) {
				this.currentOption = calculateMoveRight();
//				this.currentOption = (this.currentOption + (this.width * ((int)Mathf.Floor(this.currentOption/this.width)+1))) 
//					% (this.width + (this.width * (int)Mathf.Floor(this.currentOption/this.width)));
			} else if (direction == (int) Direction.Left) {
				this.currentOption = (this.currentOption + moveLeft()) 
					% (this.width + (this.width * (int)Mathf.Floor(this.currentOption/this.width)));
			} else  if (direction == (int) Direction.Down) {
				this.currentOption = (this.currentOption + this.width) % this.size;
			} else if (direction == (int) Direction.Up) {
				this.currentOption = (this.currentOption + (this.width*2)) % this.size;
			}
			break;
		}
		Debug.Log ("Current option: " + this.currentOption);
	}

	private int calculateMoveRight() {
		return this.currentOption < (this.width-1 + (this.width * (int)Mathf.Floor (this.currentOption / this.width))) 
			? ((this.currentOption + (((int)Mathf.Floor (this.currentOption / this.width)+1)*moveRight()) + 1) % (this.width + (this.width * (int)Mathf.Floor(this.currentOption/this.width))))
			: ((this.currentOption + moveRight() + 1) % (this.width + (this.width * (int)Mathf.Floor(this.currentOption/this.width))));
	}

	private int moveRight() {
		int modifier = (this.width * (int)Mathf.Floor (this.currentOption / this.width));
		return modifier;
	}

	private int moveLeft() {
		int modifier = (this.width*2) - 1 + (this.width * (int)Mathf.Floor (this.currentOption / this.width));
		return modifier;
	}

//	Use this code for multi-directional menus if you want <-> to loop to the next/previous line instead of beginning of line
//	if (direction == (int) Direction.Right) {
//		this.currentOption = (this.currentOption + 1) % this.size;
//	} else if (direction == (int) Direction.Left) {
//		this.currentOption = (this.currentOption + this.size - 1) % this.size;
//	} else  if (direction == (int) Direction.Down) {
//		this.currentOption = (this.currentOption + this.width) % this.size;
//	} else if (direction == (int) Direction.Up) {
//		this.currentOption = (this.currentOption + (this.width*2)) % this.size;
//	}
}
