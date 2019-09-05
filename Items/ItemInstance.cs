namespace Items {
	[System.Serializable]
	public class ItemInstance {
		public Item item;
		public Quality.QualityGrade quality;

		public ItemInstance(Item item, Quality.QualityGrade quality) {
			this.item = item;
			this.quality = quality;
		}
	}
}
