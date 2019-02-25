using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class old : MonoBehaviour {



	//	public static void mergeSort(List<IComparable> list) {
	//		//		GameObject[] listCount = new GameObject[list.Count];
	//		List<IComparable> temp = new List<IComparable>(new IComparable[list.Count]);
	//		Debug.Log ("temp size: " + temp.Count);
	//		//loop 2^i times, inc m which is how many times we merge
	//		for(int i = 1, m = 1; i < list.Count; i *= 2, m++) {
	//			//2^m
	//			int exp = (int)Math.Pow(2, m);
	//			//loop through 2^m times
	//			for(int j = 0; j < list.Count; j += exp) {
	//				//high index is going to be the last ele of the second sublist to compare
	//				int high = exp - 1 + j;
	//				//midpoint between the two sublists
	//				int mid = (j + high) / 2;
	//				//to prevent ArrayOutOfBoundsException
	//				if(high > list.Count - 1) {
	//					high = list.Count - 1;
	//				}
	//				//sort and merge the two sublists
	//				merge(list, temp, j, mid, high);
	//			}
	//		}
	//	}
	//
	//	/**
	//	 * Merge two sorted sublists together, one that goes from low to mid another
	//	 * goes from mid+1 to high. Uses a temporary array.
	//	 */
	//	private static void merge(List<IComparable> list, List<IComparable> temp, int low, int mid, int high) {
	//		Debug.Log ("low: " + low + ", mid: " + mid + ", high: " + high);
	//		int k = low;
	//		int i = low;
	//		int j = mid + 1;
	//		Debug.Log ("\tsize: " + list.Count);
	//		Debug.Log ("\tk: " + k + ", i: " + i + ", j: " + j);
	//		while (k <= high) {
	//			if (i > mid) {                  // ran out of elements in the i sublist
	//				Debug.Log("Ran out of ele in i sublist.");
	//				temp[k] = list[j];
	//				k++;
	//				j++;
	//			} else if (j > high) {          // ran out of elements in the j sublist
	//				Debug.Log("Ran out of ele in j sublist");
	//				temp[k] = list[i];
	//				k++;
	//				i++;
	//			} else if (list[i].CompareTo(list[j]) < 0) { // place list[i] in temp, move i
	//				Debug.Log("place list[i] in temp, move i");
	//				temp[k] = list[i];
	//				k++;
	//				i++;
	//			} else {
	//				temp[k] = list[j];           // place list[j] in temp, move j
	//				Debug.Log("Place list[j] in temp, move j");
	//				k++;
	//				j++;
	//			}
	//		}
	//		// copy the result from temp back to arr
	//		for (k = low; k <= high; k++) {
	//			list[k] = temp[k];
	//		}
	//	}
	//	public static void mergeSort(List<GameObject> list, IComparer customComparer) {
	////		GameObject[] listCount = new GameObject[list.Count];
	//		List<GameObject> temp = new List<GameObject>(new GameObject[list.Count]);
	//		List<GameObject> newList = new List<GameObject>(new GameObject[list.Count]);
	//		Debug.Log ("temp size: " + temp.Count);
	//		//loop 2^i times, inc m which is how many times we merge
	//		for(int i = 1, m = 1; i < list.Count; i *= 2, m++) {
	//			//2^m
	//			int exp = (int)Math.Pow(2, m);
	//			//loop through 2^m times
	//			for(int j = 0; j < list.Count; j += exp) {
	//				//high index is going to be the last ele of the second sublist to compare
	//				int high = exp - 1 + j;
	//				//midpoint between the two sublists
	//				int mid = (j + high) / 2;
	//				//to prevent ArrayOutOfBoundsException
	//				if(high > list.Count - 1) {
	//					high = list.Count - 1;
	//				}
	//				//sort and merge the two sublists
	//				merge(newList, list, temp, j, mid, high, customComparer);
	//			}
	//		}
	//		list = new List<GameObject> (newList);
	//	}
	//
	//	/**
	//	 * Merge two sorted sublists together, one that goes from low to mid another
	//	 * goes from mid+1 to high. Uses a temporary array.
	//	 */
	//	private static void merge(List<GameObject> newList, List<GameObject> list, List<GameObject> temp, int low, int mid, int high, IComparer customComparer) {
	//		Debug.Log ("low: " + low + ", mid: " + mid + ", high: " + high);
	//		int k = low;
	//		int i = low;
	//		int j = mid + 1;
	//		Debug.Log ("\tsize: " + list.Count);
	//		Debug.Log ("\tk: " + k + ", i: " + i + ", j: " + j);
	//		while (k <= high) {
	//			if (i > mid) {                  // ran out of elements in the i sublist
	//				Debug.Log("Ran out of ele in i sublist.");
	//				temp[k] = list[j];
	//				k++;
	//				j++;
	//			} else if (j > high) {          // ran out of elements in the j sublist
	//				Debug.Log("Ran out of ele in j sublist");
	//				temp[k] = list[i];
	//				k++;
	//				i++;
	//			} else if (customComparer.Compare(list[i],list[j]) < 0) { // place list[i] in temp, move i
	//				Debug.Log("place list[i] in temp, move i");
	//				temp[k] = list[i];
	//				k++;
	//				i++;
	//			} else {
	//				temp[k] = list[j];           // place list[j] in temp, move j
	//				Debug.Log("Place list[j] in temp, move j");
	//				k++;
	//				j++;
	//			}
	//		}
	//		// copy the result from temp back to arr
	//		for (k = low; k <= high; k++) {
	//			newList[k] = temp[k];
	//		}
	//	}
}
