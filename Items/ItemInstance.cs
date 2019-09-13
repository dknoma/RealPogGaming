using UnityEngine;

namespace Items {
	[System.Serializable]
	public class ItemInstance {
		public Item item;
		public int quantity = 1;
		public Quality.QualityGrade quality;

		public ItemInstance(Item item, int quantity, Quality.QualityGrade quality) {
			this.item = item;
			this.quantity = quantity;
			this.quality = quality;
		}
		
		public void AddQuantity(int amount) {
			quantity = Mathf.Min(item.stackLimit, quantity + amount);
		}

		public string GetItemName() {
			return item.itemName;
		}
		
		public string GetItemInfo() {
			string grade = quality.ToString();
			string gradeColor = "#" + ColorUtility.ToHtmlStringRGB(Quality.GradeToColor(quality));
			return string.Format("Quality: <color={0}>{1}</color> | Quantity: <color=white>{2}</color>{3}",
			                           gradeColor, grade, quantity, this.item.GetItemInfo());
		}
		
		public override string ToString() {
			return string.Format("{{Name: \"{0}\", Quality: \"{1}\"}}, ", 
			                     item != null ? item.itemName : "N/A", 
			                     item != null ? quality.ToString() : "N/A");
		}
	}
}
