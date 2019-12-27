using System.Collections.Generic;

namespace Tilemaps {
	public class TiledRenderOrder {
		public enum RenderOrder {
			RIGHT_DOWN,
			RIGHT_UP,
			LEFT_DOWN,
			LEFT_UP,
		}

		private static readonly IDictionary<string, RenderOrder> ORDERS = new Dictionary<string, RenderOrder>();

		static TiledRenderOrder() {
			ORDERS.Add("right-down", RenderOrder.RIGHT_DOWN);
			ORDERS.Add("right-up", RenderOrder.RIGHT_UP);
			ORDERS.Add("left-down", RenderOrder.LEFT_DOWN);
			ORDERS.Add("left-up", RenderOrder.LEFT_UP);
		}

		public static RenderOrder GetRenderOrder(string order) {
			return ORDERS.GetOrDefault(order);
		}

		public bool IsDefault(RenderOrder order) {
			return order == default;
		}
	}

	public static class TiledExtensions {
		public static V GetOrDefault<K,V>(this IDictionary<K, V> dictionary, K key) {
			V ret;
			// Ignore return value
			dictionary.TryGetValue(key, out ret);
			return ret;
		}
	}
}