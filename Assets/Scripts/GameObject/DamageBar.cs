using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public class DamageBar : MonoBehaviour
	{
		[SerializeField] protected Vector2 size = new Vector2(100, 20);
		[SerializeField] protected float offset = 10f;

		protected Texture2D background;
		protected Texture2D damage;
		protected GUIStyle backgroundStyle = new GUIStyle();
		protected GUIStyle damageStyle = new GUIStyle();

		private IDamageable<float> damageable;

		// Initialization
		void Start()
		{
			background = new Texture2D(1, 1);
			damage = new Texture2D(1, 1);

			// Create style for red and green bar
			background.SetPixel(0, 0, Color.green);
			damage.SetPixel(0, 0, Color.red);

			background.wrapMode = TextureWrapMode.Repeat;
			damage.wrapMode = TextureWrapMode.Repeat;

			background.Apply();
			damage.Apply();

			// Add style to groups
			backgroundStyle.normal.background = background;
			damageStyle.normal.background = damage;

			// Get damage from object
			damageable = (IDamageable<float>)GetComponent(typeof(IDamageable<float>));
		}

		void OnGUI()
		{
			Vector3 barPos = Camera.main.WorldToScreenPoint(transform.position);

			// Position on screen
			float posX = barPos.x - (size.x / 2f);
			float posY = Screen.height - (barPos.y - (size.y / 2f)) - offset;

			// Draw health bar
			GUI.BeginGroup(new Rect(posX, posY, size.x, size.y), backgroundStyle);

			float damageProg = size.x * (Mathf.Clamp(damageable.Damage, 0f, 100f) / 100f);

			GUI.BeginGroup(new Rect(size.x - damageProg, 0, damageProg, size.y), damageStyle);
			GUI.EndGroup();

			GUI.EndGroup();
		}
	}
}
