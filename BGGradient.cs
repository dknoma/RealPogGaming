using UnityEditor.Experimental.UIElements;
using UnityEngine;

[ExecuteInEditMode]
public class BGGradient : MonoBehaviour {

	public Gradient gradient;
	public bool generateGradient;

	private SpriteRenderer spriteRenderer;
	private Texture2D gradientTexture;
	private int ppu;
	private int width;
	private int height;
	private float locationPerPixel;

	private void OnEnable() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (!generateGradient) return;
		GenerateTexture();
	}

	private void GenerateTexture() {
		Vector2 size = spriteRenderer.size;
		ppu = Mathf.RoundToInt(spriteRenderer.sprite.pixelsPerUnit);
		width = (int) size.x * ppu;
		height = (int) size.y * ppu;
		locationPerPixel = height*0.01f;
		Debug.LogFormat("w,h: {0},{1}", width, height);
		gradientTexture = new Texture2D(width, height, TextureFormat.RGBA32, true);

//		GradientColorKey[] colorKeys = gradient.colorKeys;
//		Color[] colors = new Color[colorKeys.Length];
//		Debug.LogFormat("colors: {0}", colors.Length);
//		for (int i = 0; i < colorKeys.Length; i++) {
//			colors[i] = colorKeys[i].color;
//		}
//		for (int i = 0; i < height; i++) {
////			Color[] colors = {gradient.Evaluate(i * locationPerPixel)};
////			gradientTexture.SetPixels(i, i, width, height, colors);
//			gradientTexture.SetPixel();
//		}
//		for (int i = 0; i < width; i++) {
//			for (int j = 0; j < height; j++) {
//				float radius = Vector2.Distance(maskCenter, new Vector2(i, j));
//				float maskPixel = (0.5f - radius / width) * maskThreshold;
//				gradientTexture.SetPixels();
//			}
//		}
		for (int h = 0; h < height; h++) {
			Color col = gradient.Evaluate(h * locationPerPixel*0.01f);
//			Debug.LogFormat("col[{0}: {1}, {2}", h, col, h * locationPerPixel*0.01f);
			for (int w = 0; w < width; w++) {
				gradientTexture.SetPixel(w, h, col);
			}
		}
		gradientTexture.Apply();
		spriteRenderer.sprite = Sprite.Create(gradientTexture, 
		                                      new Rect(transform.position, new Vector2(width, height)),
		                                      new Vector2(width*0.5f, height*0.5f));
	}
}
