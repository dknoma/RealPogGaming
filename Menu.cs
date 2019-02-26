using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu<T> {

	public enum Type { Horizontal, Vertical, Both };
	public enum Direction { Up, Down, Left, Right };

	public int optionNum;
	public Type menuType;

	// Init menu items via inspector, or add them in via code
	[SerializeField]
	private T[] menuItems;
	private MenuNode[] menuNodes;
	private int currentOptionIndex;
	private int size;
	private int width;
	private int height;

	public Menu(int size, Type type) {
		this.size = size;
		this.width = size;
		this.height = size;
		this.menuType = type;
		this.menuNodes = new MenuNode[size];
//		for(int i = 0; i < size; i++) {
//			this.menuNodes [i] = new MenuNode (i);
//		}
	}

	public Menu(int width, int height, Type type) {
		this.size = width * height;
		this.width = width;
		this.height = height;
		this.menuType = type;
		this.menuNodes = new MenuNode[this.size];
//		for(int i = 0; i < this.size; i++) {
//			this.menuNodes [i] = new MenuNode (i);
//		}
	}

	public void initTestMenu(T[] arr) {
		this.menuItems = arr;
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

	public T getItem() {
		return this.menuItems [this.currentOptionIndex];
	}

	public void addItem(T item, int index) {
		this.menuItems [index] = item;
	}

	// Node based graph traversal for neighbor system
	public void traverseOptions(int direction) {
		switch(menuType) {
		case Type.Horizontal:
			// If the node at the current index is null, create a new one
			if(this.menuNodes [this.currentOptionIndex] == null) {
				this.menuNodes [this.currentOptionIndex] = new MenuNode (this.currentOptionIndex);
			}
			switch(direction){
			case (int) Direction.Right:
				MenuNode rightNeighbor = this.menuNodes [this.currentOptionIndex].rightNeighbor;
				if(rightNeighbor == null) {
					int newNeighborIndex = (this.currentOptionIndex + 1) % this.size;
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if (this.menuNodes [newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.leftNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].rightNeighbor = newNode;
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].rightNeighbor = menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].rightNeighbor.leftNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].rightNeighbor = rightNeighbor;
					this.currentOptionIndex = rightNeighbor.index;
				}
				break;
			case (int) Direction.Left:
				MenuNode leftNeighbor = this.menuNodes [this.currentOptionIndex].leftNeighbor;
				if(leftNeighbor == null) {
					int newNeighborIndex = (this.currentOptionIndex + this.size - 1) % this.size;
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if (this.menuNodes [newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.rightNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].leftNeighbor = newNode;
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].leftNeighbor = menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].leftNeighbor.rightNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].leftNeighbor = leftNeighbor;
					this.currentOptionIndex = leftNeighbor.index;
				}
				break;
			}
			break;
		case Type.Vertical:
			// If the node at the current index is null, create a new one
			if(this.menuNodes [this.currentOptionIndex] == null) {
				this.menuNodes [this.currentOptionIndex] = new MenuNode (this.currentOptionIndex);
			}
			switch(direction){
			case (int) Direction.Down:
				MenuNode bottomNeighbor = this.menuNodes [this.currentOptionIndex].bottomNeighbor;
				if(bottomNeighbor == null) {
					int newNeighborIndex = (this.currentOptionIndex + 1) % this.size;
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if (this.menuNodes [newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.topNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].bottomNeighbor = newNode;
//						this.menuNodes [this.currentOptionIndex].bottomNeighbor.topNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].bottomNeighbor = menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].bottomNeighbor.topNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].bottomNeighbor = bottomNeighbor;
					this.currentOptionIndex = bottomNeighbor.index;
				}
				break;
			case (int) Direction.Up:
				MenuNode topNeighbor = this.menuNodes [this.currentOptionIndex].topNeighbor;
				if(topNeighbor == null) {
					int newNeighborIndex = (this.currentOptionIndex + this.size - 1) % this.size;
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if (this.menuNodes [newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].topNeighbor = newNode;
						this.menuNodes [this.currentOptionIndex].topNeighbor.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].topNeighbor = menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].topNeighbor.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].topNeighbor = topNeighbor;
					this.currentOptionIndex = topNeighbor.index;
				}
				break;
			}
			break;
		/*** Multi axis menu: horizontal+vetical ***/
		case Type.Both:
			// If the node at the current index is null, create a new one
			if(this.menuNodes [this.currentOptionIndex] == null) {
				this.menuNodes [this.currentOptionIndex] = new MenuNode (this.currentOptionIndex);
			}
			switch(direction){
			case (int) Direction.Right:
				MenuNode rightNeighbor = this.menuNodes [this.currentOptionIndex].rightNeighbor;
				if(rightNeighbor == null) {
					int newNeighborIndex = calculateMoveRight ();
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if(this.menuNodes[newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.leftNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].rightNeighbor = newNode;
						this.menuNodes [this.currentOptionIndex].rightNeighbor.leftNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].rightNeighbor = this.menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].rightNeighbor.leftNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].rightNeighbor = rightNeighbor;
					this.currentOptionIndex = rightNeighbor.index;
				}
				break;
			case (int) Direction.Left:
				MenuNode leftNeighbor = this.menuNodes [this.currentOptionIndex].leftNeighbor;
				if(leftNeighbor == null) {
					int newNeighborIndex = calculateMoveLeft ();
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if(this.menuNodes[newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.rightNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].leftNeighbor = newNode;
						this.menuNodes [this.currentOptionIndex].leftNeighbor.rightNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].leftNeighbor = this.menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].leftNeighbor.rightNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].leftNeighbor = leftNeighbor;
					this.currentOptionIndex = leftNeighbor.index;
				}
				break;
			case (int) Direction.Down:
				MenuNode bottomNeighbor = this.menuNodes [this.currentOptionIndex].bottomNeighbor;
				if(bottomNeighbor == null) {
					int newNeighborIndex = (this.currentOptionIndex + this.width) % this.size;
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if(this.menuNodes[newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.topNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].bottomNeighbor = newNode;
						this.menuNodes [this.currentOptionIndex].bottomNeighbor.topNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].bottomNeighbor = this.menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].bottomNeighbor.topNeighbor = this.menuNodes [this.currentOptionIndex];
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].bottomNeighbor = bottomNeighbor;
					this.currentOptionIndex = bottomNeighbor.index;
				}
				break;
			case (int) Direction.Up:
				MenuNode topNeighbor = this.menuNodes [this.currentOptionIndex].topNeighbor;
				if(topNeighbor == null) {
					int newNeighborIndex = (this.currentOptionIndex + (this.size-this.width)) % this.size;
					this.menuNodes [this.currentOptionIndex].index = this.currentOptionIndex;
					// If the desired neighbor doesnt exist -> create it, else set using existing node
					if(this.menuNodes[newNeighborIndex] == null) {
						MenuNode newNode = new MenuNode (newNeighborIndex);
						newNode.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
						this.menuNodes [this.currentOptionIndex].topNeighbor = newNode;
						this.menuNodes [this.currentOptionIndex].topNeighbor.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
						this.menuNodes [this.currentOptionIndex] = newNode;
					} else {
						this.menuNodes [this.currentOptionIndex].topNeighbor = this.menuNodes[newNeighborIndex];
						this.menuNodes [this.currentOptionIndex].topNeighbor.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
						// Change current index and update the next node
						this.currentOptionIndex = newNeighborIndex;
					}
				} else {
					this.menuNodes [this.currentOptionIndex].topNeighbor = topNeighbor;
					this.currentOptionIndex = topNeighbor.index;
				}
				break;
			}
			break;
		}
		Debug.Log ("node: " + this.menuNodes[this.currentOptionIndex]);
	}

	private int calculateMoveRight() {
		int firstRowIndex = getFirstIndexOfRow ();
		return this.currentOptionIndex < (this.width-1 + firstRowIndex) 
			// if not the last option in the row
			? ((this.currentOptionIndex + (((int)Mathf.Floor (this.currentOptionIndex / this.width)+1)*firstRowIndex) + 1) % (this.width + firstRowIndex))
			// else
				: ((this.currentOptionIndex + firstRowIndex + 1) % (this.width + firstRowIndex));
	}

	private int calculateMoveLeft() {
		int firstRowIndex = getFirstIndexOfRow ();
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

	// Gets the first index in the current options column
	private int getFirstIndexOfColumn() {
		int first = this.currentOptionIndex % this.width;
		return first;
	}

	// Inner node class that determines an items index and its neighbors
	private class MenuNode {

		public int index = -1;
		public MenuNode topNeighbor;
		public MenuNode bottomNeighbor;
		public MenuNode leftNeighbor;
		public MenuNode rightNeighbor;

		public MenuNode(int index) {
			this.index = index;
		}

		public override string ToString () {
			return string.Format ("[index: {0}, top neighbor: {1}, bottom neighbor: {2}, left neighbor: {3}, right neighbor: {4}]",
				this.index, this.topNeighbor != null ? this.topNeighbor.index : -1, 
				this.bottomNeighbor != null ? this.bottomNeighbor.index : -1, 
				this.leftNeighbor != null ? this.leftNeighbor.index : -1, 
				this.rightNeighbor != null ? this.rightNeighbor.index : -1);
		}
	}
	//			Debug.Log ("this.menuNodes [this.currentOptionIndex]: " + this.menuNodes [this.currentOptionIndex]);
	//		case Type.Horizontal:
	//			if (direction == (int) Direction.Right) {
	//				this.currentOptionIndex = (this.currentOptionIndex + 1) % this.size;
	//			} else if (direction == (int) Direction.Left) {
	//				this.currentOptionIndex = (this.currentOptionIndex + this.size - 1) % this.size;
	//			}
	//			break;
	//		case Type.Vertical:
	//			if (direction == (int) Direction.Down) {
	//				this.currentOptionIndex = (this.currentOptionIndex + 1) % this.size;
	//			} else if (direction == (int) Direction.Up) {
	//				this.currentOptionIndex = (this.currentOptionIndex + this.size - 1) % this.size;
	//			}
	//			break;
	//			if (direction == (int) Direction.Right) {
	//				int rightNeighbor = this.neighborOptions [this.currentOptionIndex].rightNeighbor;
	//				if(rightNeighbor < 0) {
	//					int newNeighbor = calculateMoveRight ();
	//					this.neighborOptions [this.currentOptionIndex].rightNeighbor = newNeighbor;
	//					this.currentOptionIndex = newNeighbor;
	//				} else {
	//					this.currentOptionIndex = rightNeighbor;
	//				}
	//			} else if (direction == (int) Direction.Left) {
	//				int leftNeighbor = this.neighborOptions [this.currentOptionIndex].leftNeighbor;
	//				if(leftNeighbor < 0) {
	//					int newNeighbor = calculateMoveLeft ();
	//					this.neighborOptions [this.currentOptionIndex].leftNeighbor = newNeighbor;
	//					this.currentOptionIndex = newNeighbor;
	//				} else {
	//					this.currentOptionIndex = leftNeighbor;
	//				}
	//			} else  if (direction == (int) Direction.Down) {
	//				int bottomNeighbor = this.neighborOptions [this.currentOptionIndex].bottomNeighbor;
	//				if(bottomNeighbor < 0) {
	//					int newNeighbor = (this.currentOptionIndex + this.width) % this.size;
	//					this.neighborOptions [this.currentOptionIndex].bottomNeighbor = newNeighbor;
	//					this.currentOptionIndex = newNeighbor;
	//				} else {
	//					this.currentOptionIndex = bottomNeighbor;
	//				}
	//			} else if (direction == (int) Direction.Up) {
	//				int topNeighbor = this.neighborOptions [this.currentOptionIndex].topNeighbor;
	//				if(topNeighbor < 0) {
	//					int newNeighbor = (this.currentOptionIndex + (this.size-this.width)) % this.size;
	//					this.neighborOptions [this.currentOptionIndex].topNeighbor = newNeighbor;
	//					this.currentOptionIndex = newNeighbor;
	//				} else {
	//					this.currentOptionIndex = topNeighbor;
	//				}
	//			}
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
//		public int topNeighbor = -1;
//		public int bottomNeighbor = -1;
//		public int leftNeighbor = -1;
//		public int rightNeighbor = -1;
}
