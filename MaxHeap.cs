using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHeap : MonoBehaviour {
	
	private IComparable[] heap; // the array to store the sorting
	private int maxIndex; 		// the highest index of the sublist of the array
	private int lowIndex; 		// lowest index of the sublist of the array

	/**
	 * Constructor
	 * @param array the given array to sort
	 * @param lowindex the lowest index of the array
     * @param highindex the highest index of the array
	 */
	public MaxHeap(IComparable[] array, int lowindex, int highindex) {
		this.heap = array;
		this.maxIndex = highindex;
		this.lowIndex = lowindex;
	}

	/** 
	 * Return the index of the left child of the element at index pos
	 *
	 * @param pos the index of the element in the sorting array
	 * @return the index of the left child
	 */
	private int leftChild(int pos) {
		return 2 * pos + 1 - lowIndex;
	}

	/** 
	 * Return the index of the right child
	 *
	 * @param pos the index of the element in the sorting array
	 * @return the index of the right child
	 */
	private int rightChild(int pos) {
		return 2 * pos + 2 - lowIndex;
	}

	/** 
	 * Return the index of the parent
	 *
	 * @param pos the index of the element in the sorting array
	 * @return the index of the parent
	 */
	private int parent(int pos) {
		return pos / 2;
	}

	/** 
	 * Returns true if the node in a given position is a leaf
	 *
	 * @param pos the index of the element in the sorting array
	 * @return true if the node is a leaf, false otherwise
	 */
	private bool isLeaf(int pos, int maxIndex) {
		return ((pos < (maxIndex + lowIndex) / 2) && pos >= lowIndex);
	}

	/** 
	 * Swap given elements: one at index pos1, another at index pos2
	 *
	 * @param pos1 the index of the first element in the sorting
	 * @param pos2 the index of the second element in the sorting
	 */
	private void swap( int pos1, int pos2) {
		IComparable tmp = this.heap[pos1];
		this.heap[pos1] = this.heap[pos2];
		this.heap[pos2] = tmp;
	}

	/** 
	 * Builds a max heap from the given array
	 */
	public void buildHeap(int lastParent) {
		pushdown(lastParent, maxIndex);
	}

	/** 
	 * Sorts the max heap by moving the highest value to the end of the
	 * sublist. Will continue to bubble down if the root element has
	 * children larger than itself.
	 */
	public void sorting() {
		for (int i = maxIndex; i > lowIndex; i--) {
			swap(lowIndex, i);
			maxIndex--;
			pushdown(lowIndex, maxIndex);
		}
		//if the first two values are not sorted, switch places
		if(this.heap[lowIndex].CompareTo(this.heap[lowIndex +1]) > 0) {
			swap(lowIndex, lowIndex +1);
		}
	}
		
	public IComparable[] getHeap() {
		return this.heap;
	}

	/** 
	 * Push the value down the sorting if it does not satisfy the sorting property
	 *
	 * @param position the index of the element in the sorting
	 */
	private void pushdown(int position, int high) {
		int largestchild;
		//continue to compare value with its children as long as its not a leaf
		while(isLeaf(position, high)) {
			largestchild = leftChild(position); // set the largest child to left child
			if ((largestchild < high) && (this.heap[largestchild].CompareTo(this.heap[largestchild + 1]) < 0)) {
				largestchild = largestchild + 1; // right child was smaller, so smallest child = right child
			}
			// the value of the largest child is greater than value of current,
			// the sorting is already valid
			if (this.heap[position].CompareTo(this.heap[largestchild]) >= 0) {
				return;
			}
			swap(position, largestchild);
			position = largestchild;
		}
	}
}
