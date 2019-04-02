using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sorting {
	
	/*
	 * In-place List sorting
	 */

	// Ascending merge sort. Useful if want slowest characters to move first
	public static void MergeSort(List<GameObject> list, IComparer customComparer) {
		List<GameObject> temp = new List<GameObject> (new GameObject[list.Count]);
		//loop 2^i times, inc m which is how many times we merge
		for(int i = 1, m = 1; i < list.Count; i *= 2, m++) {
			//2^m
			int exp = (int)Math.Pow(2, m);
			//loop through 2^m times
			for(int j = 0; j < list.Count; j += exp) {
				//high index is going to be the last ele of the second sublist to compare
				int high = exp - 1 + j;
				//midpoint between the two sublists
				int mid = (j + high) / 2;
				//to prevent ArrayOutOfBoundsException
				if(high > list.Count - 1) {
					high = list.Count - 1;
				}
				//sort and merge the two sublists
				Merge(list, temp, j, mid, high, customComparer);
			}
		}
	}
	// Helper method to do in place sorting
	private static void Merge(List<GameObject> list, List<GameObject> temp, int low, int mid, int high, IComparer customComparer) {
		int k = low;
		int i = low;
		int j = mid + 1;
		while (k <= high) {
			if (i > mid) {                  // ran out of elements in the i sublist
				temp [k] = list [j];
				k++;
				j++;
			} else if (j > high) {          // ran out of elements in the j sublist
				temp [k] = list [i];
				k++;
				i++;
			} else if (customComparer.Compare(list [i], (list [j])) < 0) { // place list[i] in temp, move i
				temp [k] = list [i];
				k++;
				i++;
			} else {
				temp [k] = list [j];           // place list[j] in temp, move j
				k++;
				j++;
			}
		}
		// copy the result from temp back to arr
		for (k = low; k <= high; k++) {
			list[k] = temp[k];
		}
	}

	// Descending order sort. Useful for checking who has a greater stat
	public static void DescendingMergeSort(List<GameObject> list, IComparer customComparer) {
		List<GameObject> temp = new List<GameObject> (new GameObject[list.Count]);
		//loop 2^i times, inc m which is how many times we merge
		for(int i = 1, m = 1; i < list.Count; i *= 2, m++) {
			//2^m
			int exp = (int)Math.Pow(2, m);
			//loop through 2^m times
			for(int j = 0; j < list.Count; j += exp) {
				//high index is going to be the last ele of the second sublist to compare
				int high = exp - 1 + j;
				//midpoint between the two sublists
				int mid = (j + high) / 2;
				//to prevent ArrayOutOfBoundsException
				if(high > list.Count - 1) {
					high = list.Count - 1;
				}
				//sort and merge the two sublists
				DescendingMerge(list, temp, j, mid, high, customComparer);
			}
		}
	}

	// Helper method to do in place sorting
	private static void DescendingMerge(List<GameObject> list, List<GameObject> temp, int low, int mid, int high, IComparer customComparer) {
		int k = low;
		int i = low;
		int j = mid + 1;
		while (k <= high) {
			if (i > mid) {                  // ran out of elements in the i sublist
				temp [k] = list [j];
				k++;
				j++;
			} else if (j > high) {          // ran out of elements in the j sublist
				temp [k] = list [i];
				k++;
				i++;
			} else if (customComparer.Compare(list [i], (list [j])) > 0) { // place list[i] in temp, move i
				temp [k] = list [i];
				k++;
				i++;
			} else {
				temp [k] = list [j];           // place list[j] in temp, move j
				k++;
				j++;
			}
		}
		// copy the result from temp back to arr
		for (k = low; k <= high; k++) {
			list[k] = temp[k];
		}
	}

	/*
	 * In-place array sorting
	 */

	public static IComparable[] MergeSort(IComparable[] array) {
		IComparable[] temp = new IComparable[array.Length];
		//loop 2^i times, inc m which is how many times we merge
		for(int i = 1, m = 1; i < array.Length; i *= 2, m++) {
			//2^m
			int exp = (int)Math.Pow(2, m);
			//loop through 2^m times
			for(int j = 0; j < array.Length; j += exp) {
				//high index is going to be the last ele of the second sublist to compare
				int high = exp - 1 + j;
				//midpoint between the two sublists
				int mid = (j + high) / 2;
				//to prevent ArrayOutOfBoundsException
				if(high > array.Length - 1) {
					high = array.Length - 1;
				}
				Debug.Log ("mid: " + mid + "\nhigh: " + high);
				//sort and merge the two sublists
				Merge(array, temp, j, mid, high);
			}
		}
		return array;
	}

	// Helper method to do in place sorting
	private static void Merge(IComparable[] arr, IComparable[] temp, int low, int mid, int high) {
		Debug.Log ("low: " + low + ", mid: " + mid + ", high: " + high + "\n\tlen: " +  arr.Length);
		for(int r = 0; r < arr.Length; r++) {
			Debug.Log ("\t\tr: " + arr[r]);		
		}

		int k = low;
		int i = low;
		int j = mid + 1;
		bool kTest = k <= high;
		Debug.Log ("\tk " + k + ", i: " + i + ", j: " + j);
		//		Debug.Log ("\t\ttest: " + (0 > 5));
		Debug.Log ("\t\tk<=high: " + kTest);
		while (kTest) {
			if (i > mid) {                  // ran out of elements in the i sublist
				Debug.Log ("\tRan out of ele in i sublist.");
				temp [k] = arr [j];
				k++;
				j++;
			} else if (j > high) {          // ran out of elements in the j sublist
				Debug.Log ("\tRan out of ele in j sublist");
				temp [k] = arr [i];
				k++;
				i++;
			} else if (arr [i].CompareTo(arr [j]) < 0) { // place list[i] in temp, move i
				Debug.Log ("\tplace list[i] in temp, move i");
				temp [k] = arr [i];
				k++;
				i++;
			} else {
				temp [k] = arr [j];           // place list[j] in temp, move j
				Debug.Log ("\tPlace list[j] in temp, move j");
				k++;
				j++;
			}
			kTest = k <= high;
		}
		// copy the result from temp back to arr
		for (k = low; k <= high; k++) {
			arr[k] = temp[k];
			Debug.Log ("\t\t\tarr[k]: " + arr [k]);
		}
	}

	public static IComparable[] DescendingMergeSort(IComparable[] array) {
		IComparable[] temp = new IComparable[array.Length];
		//loop 2^i times, inc m which is how many times we merge
		for(int i = 1, m = 1; i < array.Length; i *= 2, m++) {
			//2^m
			int exp = (int)Math.Pow(2, m);
			//loop through 2^m times
			for(int j = 0; j < array.Length; j += exp) {
				//high index is going to be the last ele of the second sublist to compare
				int high = exp - 1 + j;
				//midpoint between the two sublists
				int mid = (j + high) / 2;
				//to prevent ArrayOutOfBoundsException
				if(high > array.Length - 1) {
					high = array.Length - 1;
				}
				//sort and merge the two sublists
				DescendingMerge(array, temp, j, mid, high);
			}
		}
		return array;
	}

	// Helper method to do in place sorting
	private static void DescendingMerge(IComparable[] arr, IComparable[] temp, int low, int mid, int high) {
		int k = low;
		int i = low;
		int j = mid + 1;
		while (k <= high) {
			if (i > mid) {                  // ran out of elements in the i sublist
				temp [k] = arr [j];
				k++;
				j++;
			} else if (j > high) {          // ran out of elements in the j sublist
				temp [k] = arr [i];
				k++;
				i++;
			} else if (arr [i].CompareTo(arr [j]) > 0) { // place list[i] in temp, move i
				temp [k] = arr [i];
				k++;
				i++;
			} else {
				temp [k] = arr [j];           // place list[j] in temp, move j
				k++;
				j++;
			}
		}
		// copy the result from temp back to arr
		for (k = low; k <= high; k++) {
			arr[k] = temp[k];
		}
	}
}
