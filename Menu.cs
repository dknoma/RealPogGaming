using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu<T> {

	public enum Type { Horizontal, Vertical, Both };
	public enum Direction { Up, Down, Left, Right };

	public int optionNum;
	public Type menuType;

	[SerializeField]
	private T[] menuOptions;
	private DirectionContainer[] neighborOptions;
//	private T[] singleAxisMenuOptions;
//	[SerializeField]
//	private T[,] multiAxisMenuOptions;
	private int currentOptionIndex;
	private int size;
	private int width;
	private int height;

	public Menu(int size, Type type) {
		this.size = size;
		this.width = size;
		this.height = size;
		this.menuType = type;
		this.neighborOptions = new DirectionContainer[size];
		for(int i = 0; i < size; i++) {
			this.neighborOptions [i] = new DirectionContainer ();
		}
	}

	public Menu(int width, int height, Type type) {
		this.size = width * height;
		this.width = width;
		this.height = height;
		this.menuType = type;
		this.neighborOptions = new DirectionContainer[this.size];
		for(int i = 0; i < this.size; i++) {
			this.neighborOptions [i] = new DirectionContainer ();
		}
	}

	public void initTestMenu(T[] arr) {
		if(this.menuType != Type.Horizontal || this.menuType != Type.Vertical) {

		}
		this.menuOptions = arr;
	}

//	public void initSATestMenu(T[] arr) {
//		if(this.menuType != Type.Horizontal || this.menuType != Type.Vertical) {
//			
//		}
//		this.singleAxisMenuOptions = arr;
//	}

//	public void initMATestMenu(T[,] arr) {
//		this.multiAxisMenuOptions = arr;
//	}

	public void traverseTestMenu(int direction) {
//		Debug.Log("ma hor: " + (this.width + (this.width * (int)Mathf.Floor(this.currentOptionIndex/this.width))));
		switch(menuType) {
		case Type.Horizontal:
			if (direction == (int) Direction.Right) {
				this.currentOptionIndex = (this.currentOptionIndex + 1) % this.size;
			} else if (direction == (int) Direction.Left) {
				this.currentOptionIndex = (this.currentOptionIndex + this.size - 1) % this.size;
			}
			break;
		case Type.Vertical:
			if (direction == (int) Direction.Down) {
				this.currentOptionIndex = (this.currentOptionIndex + 1) % this.size;
			} else if (direction == (int) Direction.Up) {
				this.currentOptionIndex = (this.currentOptionIndex + this.size - 1) % this.size;
			}
			break;
		case Type.Both:
			if (direction == (int) Direction.Right) {
				int rightNeighbor = this.neighborOptions [this.currentOptionIndex].rightNeighbor;
				if(rightNeighbor < 0) {
					int newNeighbor = calculateMoveRight ();
					this.neighborOptions [this.currentOptionIndex].rightNeighbor = newNeighbor;
					this.currentOptionIndex = newNeighbor;
				} else {
					this.currentOptionIndex = rightNeighbor;
				}
//				return rightNeighbor < 0 ?
//					this.currentOptionIndex = calculateMoveRight()
//						: rightNeighbor;
//				this.currentOptionIndex = calculateMoveRight()
			} else if (direction == (int) Direction.Left) {
				int leftNeighbor = this.neighborOptions [this.currentOptionIndex].leftNeighbor;
				if(leftNeighbor < 0) {
					int newNeighbor = calculateMoveLeft ();
					this.neighborOptions [this.currentOptionIndex].leftNeighbor = newNeighbor;
					this.currentOptionIndex = newNeighbor;
				} else {
					this.currentOptionIndex = leftNeighbor;
				}
//				this.currentOptionIndex = calculateMoveLeft ();
			} else  if (direction == (int) Direction.Down) {
				int bottomNeighbor = this.neighborOptions [this.currentOptionIndex].bottomNeighbor;
				if(bottomNeighbor < 0) {
					int newNeighbor = (this.currentOptionIndex + this.width) % this.size;
					this.neighborOptions [this.currentOptionIndex].bottomNeighbor = newNeighbor;
					this.currentOptionIndex = newNeighbor;
				} else {
					this.currentOptionIndex = bottomNeighbor;
				}
//				this.currentOptionIndex = (this.currentOptionIndex + this.width) % this.size;
			} else if (direction == (int) Direction.Up) {
				int topNeighbor = this.neighborOptions [this.currentOptionIndex].topNeighbor;
				if(topNeighbor < 0) {
					int newNeighbor = (this.currentOptionIndex + (this.width*2)) % this.size;
					this.neighborOptions [this.currentOptionIndex].topNeighbor = newNeighbor;
					this.currentOptionIndex = newNeighbor;
				} else {
					this.currentOptionIndex = topNeighbor;
				}
//				this.currentOptionIndex = (this.currentOptionIndex + (this.width*2)) % this.size;
			}
			break;
		}
		Debug.Log ("Current option: " + this.currentOptionIndex);
	}

	private int calculateMoveRight() {
		int firstRowIndex = getFirstIndexOfRow ();
		Debug.Log ("rowCorrection: " + firstRowIndex);
		return this.currentOptionIndex < (this.width-1 + firstRowIndex) 
			// if not the last option in the row
			? ((this.currentOptionIndex + (((int)Mathf.Floor (this.currentOptionIndex / this.width)+1)*firstRowIndex) + 1) % (this.width + firstRowIndex))
			// else
				: ((this.currentOptionIndex + firstRowIndex + 1) % (this.width + firstRowIndex));
	}

	private int calculateMoveLeft() {
		int firstRowIndex = getFirstIndexOfRow ();
		Debug.Log ("rowCorrection: " + firstRowIndex);
		Debug.Log("adad: " + (this.width * (firstRowIndex/this.width)+1));
		return this.currentOptionIndex == 0 || this.currentOptionIndex > firstRowIndex
			// if not the first option in the row
			? (this.currentOptionIndex + (this.width-1 + firstRowIndex)) % (this.width + firstRowIndex)
			// else
				: ((((int)Mathf.Floor (this.currentOptionIndex / this.width)+1)*firstRowIndex) - 1) % (this.width + firstRowIndex);
	}

	// Gets the first index in the current options row
	private int getFirstIndexOfRow() {
		int first = (this.width * (int)Mathf.Floor (this.currentOptionIndex / this.width));
		return first;
	}

//	Use this code for multi-directional menus if you want <-> to loop to the next/previous line instead of beginning of line
//	if (direction == (int) Direction.Right) {
//		this.currentOptionIndex = (this.currentOptionIndex + 1) % this.size;
//	} else if (direction == (int) Direction.Left) {
//		this.currentOptionIndex = (this.currentOptionIndex + this.size - 1) % this.size;
//	} else  if (direction == (int) Direction.Down) {
//		this.currentOptionIndex = (this.currentOptionIndex + this.width) % this.size;
//	} else if (direction == (int) Direction.Up) {
//		this.currentOptionIndex = (this.currentOptionIndex + (this.width*2)) % this.size;
//	}
	private class DirectionContainer {

		public int topNeighbor = -1;
		public int bottomNeighbor = -1;
		public int leftNeighbor = -1;
		public int rightNeighbor = -1;

//		public DirectionContainer() {
//			this.topNeighbor = -1;
//			this.bottomeNeighbor = -1;
//			this.leftNeighbor = -1;
//			this.rightNeighbor = -1;
//		}
	}
}
