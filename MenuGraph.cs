using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable SwitchStatementMissingSomeCases

public enum MenuType {
	Horizontal, 
	Vertical, 
	Both
}

public class MenuGraph<T> {

	public int optionNum;
	private MenuType menuType;

	// Init menu items via inspector, or add them in via code
	private T[] menuItems;
	private MenuNode[] menuNodes;
	private int size;
	private int width;
	private int height;
	private int currentOptionIndex;
	private bool symmetricalRectMenu;

	public MenuGraph() {
		menuNodes = new MenuNode[0];
	}
	
	public MenuGraph(int size, MenuType type) {
		this.size = size;
		width = size;
		height = size;
		menuType = type;
		menuNodes = new MenuNode[size];
		symmetricalRectMenu = false;
	}

	public MenuGraph(int width, int height) {
		size = width * height;
		this.width = width;
		this.height = height;
		menuType = MenuType.Both;
		menuNodes = new MenuNode[size];
		symmetricalRectMenu = true;
	}

	public void InitMenu(int menuSize, MenuType type) {
		size = menuSize;
		width = size;
		height = size;
		menuType = type;
		menuNodes = new MenuNode[size];
		symmetricalRectMenu = false;
	}
	
	public void InitMenu(int menuWidth, int menuHeight) {
		size = menuWidth * menuHeight;
		width = menuWidth;
		height = menuHeight;
		menuType = MenuType.Both;
		menuNodes = new MenuNode[size];
		symmetricalRectMenu = true;
	}

	public void InitMenuItems(T[] arr) {
		menuItems = arr;
	}

	public T GetCurrentItem() {
		return menuItems [currentOptionIndex];
	}
	
	public T GetItem(int index) {
		return menuItems [index];
	}

	public void AddItem(T item, int index) {
		menuItems[index] = item;
	}

	public MenuType GetMenuType() {
		return menuType;
	}

	public int Size() {
		return size;
	}

	// Node based graph traversal for neighbor system
	public void TraverseOptions(Direction direction) {
		if (menuItems.Length == 1) return;
		switch(menuType) {
		case MenuType.Horizontal:
			// If the node at the current index is null, create a new one
			if(menuNodes [currentOptionIndex] == null) {
				menuNodes [currentOptionIndex] = new MenuNode (currentOptionIndex);
			}
			switch(direction){
				case Direction.Right:
					MenuNode rightNeighbor = menuNodes [currentOptionIndex].rightNeighbor;
					if(rightNeighbor == null) {
						int newNeighborIndex = (currentOptionIndex + 1) % size;
						menuNodes [currentOptionIndex].index = currentOptionIndex;
						// If the desired neighbor doesnt exist -> create it, else set using existing node
						if (menuNodes [newNeighborIndex] == null) {
							MenuNode newNode = new MenuNode(newNeighborIndex) {leftNeighbor = menuNodes[currentOptionIndex]};
							// set the new nodes origin neighbor to this node
							menuNodes [currentOptionIndex].rightNeighbor = newNode;
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
							menuNodes [currentOptionIndex] = newNode;
						} else {
							menuNodes [currentOptionIndex].rightNeighbor = menuNodes[newNeighborIndex];
							menuNodes [currentOptionIndex].rightNeighbor.leftNeighbor = menuNodes [currentOptionIndex];
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
						}
					} else {
						// Set this nodes neighbor to the desired node, move index
						menuNodes [currentOptionIndex].rightNeighbor = rightNeighbor;
						currentOptionIndex = rightNeighbor.index;
					}
					break;
				case Direction.Left:
					MenuNode leftNeighbor = menuNodes [currentOptionIndex].leftNeighbor;
					if(leftNeighbor == null) {
						int newNeighborIndex = (currentOptionIndex + size - 1) % size;
						menuNodes [currentOptionIndex].index = currentOptionIndex;
						// If the desired neighbor doesnt exist -> create it, else set using existing node
						if (menuNodes [newNeighborIndex] == null) {
							MenuNode newNode = new MenuNode(newNeighborIndex) {rightNeighbor = menuNodes[currentOptionIndex]};
							// set the new nodes origin neighbor to this node
							menuNodes [currentOptionIndex].leftNeighbor = newNode;
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
							menuNodes [currentOptionIndex] = newNode;
						} else {
							menuNodes [currentOptionIndex].leftNeighbor = menuNodes[newNeighborIndex];
							menuNodes [currentOptionIndex].leftNeighbor.rightNeighbor = menuNodes [currentOptionIndex];
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
						}
					} else {
						// Set this nodes neighbor to the desired node, move index
						menuNodes [currentOptionIndex].leftNeighbor = leftNeighbor;
						currentOptionIndex = leftNeighbor.index;
					}
					break;
			}
			break;
		case MenuType.Vertical:
			// If the node at the current index is null, create a new one
			if(menuNodes [currentOptionIndex] == null) {
				menuNodes [currentOptionIndex] = new MenuNode (currentOptionIndex);
			}
			switch(direction){
				case Direction.Down:
					MenuNode bottomNeighbor = menuNodes [currentOptionIndex].bottomNeighbor;
					if(bottomNeighbor == null) {
						int newNeighborIndex = (currentOptionIndex + 1) % size;
						menuNodes [currentOptionIndex].index = currentOptionIndex;
						// If the desired neighbor doesnt exist -> create it, else set using existing node
						if (menuNodes [newNeighborIndex] == null) {
							MenuNode newNode = new MenuNode(newNeighborIndex) {topNeighbor = menuNodes[currentOptionIndex]};
							// set the new nodes origin neighbor to this node
							menuNodes [currentOptionIndex].bottomNeighbor = newNode;
	//						this.menuNodes [this.currentOptionIndex].bottomNeighbor.topNeighbor = this.menuNodes [this.currentOptionIndex];
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
							menuNodes [currentOptionIndex] = newNode;
						} else {
							menuNodes [currentOptionIndex].bottomNeighbor = menuNodes[newNeighborIndex];
							menuNodes [currentOptionIndex].bottomNeighbor.topNeighbor = menuNodes [currentOptionIndex];
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
						}
					} else {
						// Set this nodes neighbor to the desired node, move index
						menuNodes [currentOptionIndex].bottomNeighbor = bottomNeighbor;
						currentOptionIndex = bottomNeighbor.index;
					}
					break;
				case Direction.Up:
					MenuNode topNeighbor = menuNodes [currentOptionIndex].topNeighbor;
					if(topNeighbor == null) {
						int newNeighborIndex = (currentOptionIndex + size - 1) % size;
						menuNodes [currentOptionIndex].index = currentOptionIndex;
						// If the desired neighbor doesnt exist -> create it, else set using existing node
						if (menuNodes [newNeighborIndex] == null) {
							MenuNode newNode = new MenuNode(newNeighborIndex) {bottomNeighbor = menuNodes[currentOptionIndex]};
							// set the new nodes origin neighbor to this node
							menuNodes [currentOptionIndex].topNeighbor = newNode;
	//						this.menuNodes [this.currentOptionIndex].topNeighbor.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
							menuNodes [currentOptionIndex] = newNode;
						} else {
							menuNodes [currentOptionIndex].topNeighbor = menuNodes[newNeighborIndex];
							menuNodes [currentOptionIndex].topNeighbor.bottomNeighbor = menuNodes [currentOptionIndex];
							// Change current index and update the next node
							currentOptionIndex = newNeighborIndex;
						}
					} else {
						// Set this nodes neighbor to the desired node, move index
						menuNodes [currentOptionIndex].topNeighbor = topNeighbor;
						currentOptionIndex = topNeighbor.index;
					}
					break;
			}
			break;
		/*** Multi axis menu: horizontal+vetical ***/
		case MenuType.Both:
			// If the node at the current index is null, create a new one
			if(menuNodes [currentOptionIndex] == null) {
				menuNodes [currentOptionIndex] = new MenuNode (currentOptionIndex);
			}

			if (symmetricalRectMenu) {
				switch (direction) {
					case Direction.Right:
						MenuNode rightNeighbor = menuNodes[currentOptionIndex].rightNeighbor;
						if (rightNeighbor == null) {
							int newNeighborIndex = CalculateMoveRight();
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {leftNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].rightNeighbor = newNode;
								//						this.menuNodes [this.currentOptionIndex].rightNeighbor.leftNeighbor = this.menuNodes [this.currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].rightNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].rightNeighbor.leftNeighbor =
									menuNodes[currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							if (menuNodes[currentOptionIndex].rightNeighbor.index < 0) {
								menuNodes[currentOptionIndex].rightNeighbor = rightNeighbor;
							}

							currentOptionIndex = rightNeighbor.index;
						}

						break;
					case Direction.Left:
						MenuNode leftNeighbor = menuNodes[currentOptionIndex].leftNeighbor;
						if (leftNeighbor == null) {
							int newNeighborIndex = CalculateMoveLeft();
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {rightNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].leftNeighbor = newNode;
								//						this.menuNodes [this.currentOptionIndex].leftNeighbor.rightNeighbor = this.menuNodes [this.currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].leftNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].leftNeighbor.rightNeighbor =
									menuNodes[currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							menuNodes[currentOptionIndex].leftNeighbor = leftNeighbor;
							currentOptionIndex = leftNeighbor.index;
						}

						break;
					case Direction.Down:
						MenuNode bottomNeighbor = menuNodes[currentOptionIndex].bottomNeighbor;
						if (bottomNeighbor == null) {
							int newNeighborIndex = (currentOptionIndex + width) % size;
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {topNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].bottomNeighbor = newNode;
								//						this.menuNodes [this.currentOptionIndex].bottomNeighbor.topNeighbor = this.menuNodes [this.currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].bottomNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].bottomNeighbor.topNeighbor =
									menuNodes[currentOptionIndex];
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							menuNodes[currentOptionIndex].bottomNeighbor = bottomNeighbor;
							currentOptionIndex = bottomNeighbor.index;
						}

						break;
					case Direction.Up:
						MenuNode topNeighbor = menuNodes[currentOptionIndex].topNeighbor;
						if (topNeighbor == null) {
							int newNeighborIndex = (currentOptionIndex + (size - width)) % size;
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {bottomNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].topNeighbor = newNode;
								//						this.menuNodes [this.currentOptionIndex].topNeighbor.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].topNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].topNeighbor.bottomNeighbor =
									menuNodes[currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							menuNodes[currentOptionIndex].topNeighbor = topNeighbor;
							currentOptionIndex = topNeighbor.index;
						}

						break;
				}
				break;
			} else {
				// MenuObject is NOT a n x m menu, is only a one dimensional menu.
				switch (direction) {
					case Direction.Right:
						MenuNode rightNeighbor = menuNodes[currentOptionIndex].rightNeighbor;
						if (rightNeighbor == null) {
							int newNeighborIndex = (currentOptionIndex + 1) % size;
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {leftNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].rightNeighbor = newNode;
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].rightNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].rightNeighbor.leftNeighbor =
									menuNodes[currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							menuNodes[currentOptionIndex].rightNeighbor = rightNeighbor;
							currentOptionIndex = rightNeighbor.index;
						}

						break;
					case Direction.Left:
						MenuNode leftNeighbor = menuNodes[currentOptionIndex].leftNeighbor;
						if (leftNeighbor == null) {
							int newNeighborIndex = (currentOptionIndex + size - 1) % size;
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {rightNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].leftNeighbor = newNode;
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].leftNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].leftNeighbor.rightNeighbor =
									menuNodes[currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							menuNodes[currentOptionIndex].leftNeighbor = leftNeighbor;
							currentOptionIndex = leftNeighbor.index;
						}

						break;
					case Direction.Down:
						MenuNode bottomNeighbor = menuNodes[currentOptionIndex].bottomNeighbor;
						if (bottomNeighbor == null) {
							int newNeighborIndex = (currentOptionIndex + 1) % size;
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {topNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].bottomNeighbor = newNode;
								//						this.menuNodes [this.currentOptionIndex].bottomNeighbor.topNeighbor = this.menuNodes [this.currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].bottomNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].bottomNeighbor.topNeighbor =
									menuNodes[currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							menuNodes[currentOptionIndex].bottomNeighbor = bottomNeighbor;
							currentOptionIndex = bottomNeighbor.index;
						}

						break;
					case Direction.Up:
						MenuNode topNeighbor = menuNodes[currentOptionIndex].topNeighbor;
						if (topNeighbor == null) {
							int newNeighborIndex = (currentOptionIndex + size - 1) % size;
							menuNodes[currentOptionIndex].index = currentOptionIndex;
							// If the desired neighbor doesnt exist -> create it, else set using existing node
							if (menuNodes[newNeighborIndex] == null) {
								MenuNode newNode = new MenuNode(newNeighborIndex) {bottomNeighbor = menuNodes[currentOptionIndex]};
								// set the new nodes origin neighbor to this node
								menuNodes[currentOptionIndex].topNeighbor = newNode;
								//						this.menuNodes [this.currentOptionIndex].topNeighbor.bottomNeighbor = this.menuNodes [this.currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
								menuNodes[currentOptionIndex] = newNode;
							} else {
								menuNodes[currentOptionIndex].topNeighbor = menuNodes[newNeighborIndex];
								menuNodes[currentOptionIndex].topNeighbor.bottomNeighbor =
									menuNodes[currentOptionIndex];
								// Change current index and update the next node
								currentOptionIndex = newNeighborIndex;
							}
						} else {
							// Set this nodes neighbor to the desired node, move index
							menuNodes[currentOptionIndex].topNeighbor = topNeighbor;
							currentOptionIndex = topNeighbor.index;
						}
						break;
				}
				break;
			}
		default:
			throw new ArgumentOutOfRangeException();
		}
//		Debug.Log ("node: " + menuNodes[currentOptionIndex]);
	}

	private int CalculateMoveRight() {
		int firstRowIndex = GetFirstIndexOfRow ();
		return currentOptionIndex < (width-1 + firstRowIndex) 
			// if not the last option in the row
			? ((currentOptionIndex + ((int)Mathf.Floor (currentOptionIndex / width)+1)*firstRowIndex) + 1) % (width + firstRowIndex)
			// else
				: ((currentOptionIndex + firstRowIndex + 1) % (width + firstRowIndex));
	}

	private int CalculateMoveLeft() {
		int firstRowIndex = GetFirstIndexOfRow ();
		return currentOptionIndex == 0 || currentOptionIndex > firstRowIndex
			// if not the first option in the row
			? (currentOptionIndex + width-1 + firstRowIndex) % (width + firstRowIndex)
			// else
				: (((int)Mathf.Floor (currentOptionIndex / width)+1)*firstRowIndex - 1) % (width + firstRowIndex);
	}

	// Gets the first index in the current options row
	private int GetFirstIndexOfRow() {
		int first = width * (int)Mathf.Floor (currentOptionIndex / width);
		return first;
	}

	// Gets the first index in the current options column
	private int GetFirstIndexOfColumn() {
		int first = currentOptionIndex % width;
		return first;
	}

	// Inner node class that determines an items index and its neighbors
	private class MenuNode {

		public int index;
		public MenuNode topNeighbor;
		public MenuNode bottomNeighbor;
		public MenuNode leftNeighbor;
		public MenuNode rightNeighbor;

		public MenuNode(int index) {
			this.index = index;
		}

		public override string ToString () {
			return string.Format ("[index: {0}, top neighbor: {1}, bottom neighbor: {2}, left neighbor: {3}, right neighbor: {4}]",
				index, topNeighbor != null ? topNeighbor.index : -1, 
				bottomNeighbor != null ? bottomNeighbor.index : -1, 
				leftNeighbor != null ? leftNeighbor.index : -1, 
				rightNeighbor != null ? rightNeighbor.index : -1);
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
	//					int newNeighbor = (this.currentOptionIndex + this.menuWidth) % this.size;
	//					this.neighborOptions [this.currentOptionIndex].bottomNeighbor = newNeighbor;
	//					this.currentOptionIndex = newNeighbor;
	//				} else {
	//					this.currentOptionIndex = bottomNeighbor;
	//				}
	//			} else if (direction == (int) Direction.Up) {
	//				int topNeighbor = this.neighborOptions [this.currentOptionIndex].topNeighbor;
	//				if(topNeighbor < 0) {
	//					int newNeighbor = (this.currentOptionIndex + (this.size-this.menuWidth)) % this.size;
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
//		this.currentOptionIndex = (this.currentOptionIndex + this.menuWidth) % this.size;
//	} else if (direction == (int) Direction.Up) {
//		this.currentOptionIndex = (this.currentOptionIndex + (this.menuWidth*2)) % this.size;
//	}
//		public int topNeighbor = -1;
//		public int bottomNeighbor = -1;
//		public int leftNeighbor = -1;
//		public int rightNeighbor = -1;
}
