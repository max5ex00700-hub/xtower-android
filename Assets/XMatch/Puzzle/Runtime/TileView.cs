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
        public PowerUpKind PowerUp { get; private set; }

        public void Initialize(
            TileKind kind,
            Sprite sprite)
        {
            Initialize(
                kind,
                PowerUpKind.None,
                sprite);
        }

        public void Initialize(
            TileKind kind,
            PowerUpKind powerUp,
            Sprite sprite)
        {
            Kind = kind;
            PowerUp = powerUp;

            spriteRenderer =
                gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = 1;

            var labelObject =
                new GameObject("Label");
            labelObject.transform.SetParent(
                transform,
                worldPositionStays: false);
            labelObject.transform.localPosition =
                new Vector3(0f, -0.02f, -0.02f);

            label = labelObject.AddComponent<TextMesh>();
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.19f;
            label.fontSize = 64;
            label.color = Color.white;

            var renderer =
                labelObject.GetComponent<MeshRenderer>();
            renderer.sortingOrder = 2;

            RefreshAppearance();
            SetScaleFactor(1f);
        }

        public void SetPowerUp(
            TileKind kind,
            PowerUpKind powerUp)
        {
            Kind = kind;
            PowerUp = powerUp;
            RefreshAppearance();
        }

        public void SetScaleFactor(float factor)
        {
            float scale =
                BaseScale * Mathf.Max(0f, factor);

            transform.localScale =
                new Vector3(scale, scale, 1f);
        }

        private void RefreshAppearance()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color =
                    ColorFor(Kind, PowerUp);
            }

            if (label != null)
            {
                label.text =
                    ShortName(Kind, PowerUp);
            }
        }

        private static string ShortName(
            TileKind kind,
            PowerUpKind powerUp)
        {
            switch (powerUp)
            {
                case PowerUpKind.RowBlast:
                    return "ROW";
                case PowerUpKind.ColumnBlast:
                    return "COL";
                case PowerUpKind.Bomb:
                    return "B";
                case PowerUpKind.ColorOrb:
                    return "ORB";
                case PowerUpKind.Seeker:
                    return "GO";
            }

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
                case TileKind.Wild:
                    return "ORB";
                default:
                    return "?";
            }
        }

        private static Color ColorFor(
            TileKind kind,
            PowerUpKind powerUp)
        {
            if (powerUp == PowerUpKind.ColorOrb ||
                kind == TileKind.Wild)
            {
                return new Color(0.16f, 0.16f, 0.22f);
            }

            Color baseColor;

            switch (kind)
            {
                case TileKind.Heart:
                    baseColor = new Color(0.95f, 0.18f, 0.35f);
                    break;
                case TileKind.Lips:
                    baseColor = new Color(0.88f, 0.20f, 0.65f);
                    break;
                case TileKind.Diamond:
                    baseColor = new Color(0.15f, 0.68f, 0.98f);
                    break;
                case TileKind.Perfume:
                    baseColor = new Color(0.93f, 0.67f, 0.18f);
                    break;
                case TileKind.Rose:
                    baseColor = new Color(0.65f, 0.24f, 0.82f);
                    break;
                default:
                    baseColor = Color.gray;
                    break;
            }

            if (powerUp == PowerUpKind.None)
            {
                return baseColor;
            }

            return Color.Lerp(
                baseColor,
                Color.white,
                0.28f);
        }
    }
}
