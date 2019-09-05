using UnityEngine;

namespace TopDown {
	public class TopDownBlock : MonoBehaviour {
		public Sprite topRender;
		public Sprite frontRender;
//		public Sprite bottomRender;
//		public Sprite backRender;

		[SerializeField] private int height;
		private GameObject top;
		private GameObject front;
		private GameObject bottom;
		private GameObject back;

		public int Height {
			get { return height; }
			set { height = value; }
		}

		private void Awake() {
			top = transform.GetChild(0).gameObject;
			front = transform.GetChild(1).gameObject;
			bottom = transform.GetChild(2).gameObject;
			back = transform.GetChild(3).gameObject;
		}

		/// <summary>
		/// Allows the object to change the children sprite renders when inspector value is changed.
		/// </summary>
		private void OnValidate() {
			top.GetComponent<SpriteRenderer>().sprite = topRender;
			front.GetComponent<SpriteRenderer>().sprite = frontRender;
//			bottom.GetComponent<SpriteRenderer>().sprite = bottomRender;
//			back.GetComponent<SpriteRenderer>().sprite = backRender;
		}
	}
}
