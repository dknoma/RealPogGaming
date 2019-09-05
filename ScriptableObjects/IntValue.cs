using UnityEngine;

namespace ScriptableObjects {
	[CreateAssetMenu(fileName = "IntValue", menuName = "ScriptableObjects/IntValue", order = 1)]
	public class IntValue : ScriptableObject {
		public int initialValue;
	}
}
