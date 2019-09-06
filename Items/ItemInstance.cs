namespace Items {
	[System.Serializable]
	public class ItemInstance {
		public Item item;
		public Quality.QualityGrade quality;

		public ItemInstance(Item item, Quality.QualityGrade quality) {
			this.item = item;
			this.quality = quality;
		}

		public override string ToString() {
			return string.Format("{{Name: \"{0}\", Quality: \"{1}\"}}, ", 
			                     item != null ? item.itemName : "N/A", 
			                     item != null ? quality.ToString() : "N/A");
		}
	}
}
