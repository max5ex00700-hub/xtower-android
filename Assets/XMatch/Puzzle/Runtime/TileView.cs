using UnityEngine;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public sealed class TileView : MonoBehaviour
    {
        public const float BaseScale = 0.86f;

        private SpriteRenderer spriteRenderer;
        private TextMesh label;

        public TileKind Kind { get; private set; }

        public void Initialize(
            TileKind kind,
            Sprite sprite)
        {
            Kind = kind;

            spriteRenderer =
                gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = ColorFor(kind);
            spriteRenderer.sortingOrder = 1;

            var labelObject =
                new GameObject("Label");
            labelObject.transform.SetParent(
                transform,
                worldPositionStays: false);
            labelObject.transform.localPosition =
                new Vector3(0f, -0.02f, -0.02f);

            label = labelObject.AddComponent<TextMesh>();
            label.text = ShortName(kind);
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.22f;
            label.fontSize = 64;
            label.color = Color.white;

            var renderer =
                labelObject.GetComponent<MeshRenderer>();
            renderer.sortingOrder = 2;

            SetScaleFactor(1f);
        }

        public void SetScaleFactor(float factor)
        {
            float scale =
                BaseScale * Mathf.Max(0f, factor);

            transform.localScale =
                new Vector3(scale, scale, 1f);
        }

        private static string ShortName(TileKind kind)
        {
            switch (kind)
            {
                case TileKind.Heart:
                    return "H";
                case TileKind.Lips:
                    return "L";
                case TileKind.Diamond:
                    return "D";
                case TileKind.Perfume:
                    return "P";
                case TileKind.Rose:
                    return "R";
                default:
                    return "?";
            }
        }

        private static Color ColorFor(TileKind kind)
        {
            switch (kind)
            {
                case TileKind.Heart:
                    return new Color(0.95f, 0.18f, 0.35f);
                case TileKind.Lips:
                    return new Color(0.88f, 0.20f, 0.65f);
                case TileKind.Diamond:
                    return new Color(0.15f, 0.68f, 0.98f);
                case TileKind.Perfume:
                    return new Color(0.93f, 0.67f, 0.18f);
                case TileKind.Rose:
                    return new Color(0.65f, 0.24f, 0.82f);
                default:
                    return Color.gray;
            }
        }
    }
}
