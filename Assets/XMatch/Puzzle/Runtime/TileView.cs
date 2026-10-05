using UnityEngine;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public sealed class TileView : MonoBehaviour
    {
        public const float BaseScale = 0.86f;

        private SpriteRenderer glowRenderer;
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

            var glowObject =
                new GameObject("Glow");
            glowObject.transform.SetParent(
                transform,
                worldPositionStays: false);
            glowObject.transform.localPosition =
                new Vector3(0f, 0f, 0.04f);
            glowObject.transform.localScale =
                new Vector3(1.12f, 1.12f, 1f);

            glowRenderer =
                glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = sprite;
            glowRenderer.sortingOrder = 0;

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
                new Vector3(0f, -0.015f, -0.03f);

            label = labelObject.AddComponent<TextMesh>();
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.18f;
            label.fontSize = 64;
            label.fontStyle = FontStyle.Bold;
            label.color = Color.white;

            var renderer =
                labelObject.GetComponent<MeshRenderer>();
            renderer.sortingOrder = 2;

            RefreshAppearance();
            SetScaleFactor(1f);
        }

        private void Update()
        {
            if (glowRenderer == null)
            {
                return;
            }

            if (PowerUp == PowerUpKind.None)
            {
                return;
            }

            float pulse =
                0.16f +
                ((Mathf.Sin(
                    (Time.unscaledTime * 5f) +
                    (transform.position.x * 0.7f)) +
                  1f) * 0.07f);

            Color glow =
                SpecialGlowColor(PowerUp);
            glow.a = pulse;
            glowRenderer.color = glow;

            float scale =
                1.10f +
                ((Mathf.Sin(Time.unscaledTime * 4f) + 1f) *
                 0.025f);

            glowRenderer.transform.localScale =
                new Vector3(scale, scale, 1f);
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

            if (glowRenderer != null)
            {
                Color glow =
                    PowerUp == PowerUpKind.None
                        ? new Color(0f, 0f, 0f, 0.22f)
                        : SpecialGlowColor(PowerUp);

                if (PowerUp != PowerUpKind.None)
                {
                    glow.a = 0.20f;
                }

                glowRenderer.color = glow;
            }

            if (label != null)
            {
                label.text =
                    ShortName(Kind, PowerUp);

                label.characterSize =
                    PowerUp == PowerUpKind.None
                        ? 0.18f
                        : 0.14f;
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
                    return "BOMB";
                case PowerUpKind.ColorOrb:
                    return "ORB";
                case PowerUpKind.Seeker:
                    return "GO";
            }

            switch (kind)
            {
                case TileKind.Heart:
                    return "♥";
                case TileKind.Lips:
                    return "LIP";
                case TileKind.Diamond:
                    return "◆";
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
                float hue =
                    Mathf.Repeat(
                        Time.unscaledTime * 0.08f,
                        1f);

                return Color.HSVToRGB(
                    hue,
                    0.68f,
                    0.95f);
            }

            Color baseColor;

            switch (kind)
            {
                case TileKind.Heart:
                    baseColor =
                        new Color(1.00f, 0.20f, 0.38f);
                    break;
                case TileKind.Lips:
                    baseColor =
                        new Color(0.95f, 0.23f, 0.68f);
                    break;
                case TileKind.Diamond:
                    baseColor =
                        new Color(0.18f, 0.76f, 1.00f);
                    break;
                case TileKind.Perfume:
                    baseColor =
                        new Color(1.00f, 0.72f, 0.20f);
                    break;
                case TileKind.Rose:
                    baseColor =
                        new Color(0.72f, 0.29f, 0.93f);
                    break;
                default:
                    baseColor =
                        new Color(0.50f, 0.50f, 0.55f);
                    break;
            }

            if (powerUp == PowerUpKind.None)
            {
                return baseColor;
            }

            return Color.Lerp(
                baseColor,
                Color.white,
                0.24f);
        }

        private static Color SpecialGlowColor(
            PowerUpKind powerUp)
        {
            switch (powerUp)
            {
                case PowerUpKind.RowBlast:
                    return new Color(0.25f, 0.95f, 1f, 0.2f);
                case PowerUpKind.ColumnBlast:
                    return new Color(1f, 0.62f, 0.18f, 0.2f);
                case PowerUpKind.Bomb:
                    return new Color(1f, 0.20f, 0.16f, 0.2f);
                case PowerUpKind.ColorOrb:
                    return new Color(0.95f, 0.95f, 1f, 0.2f);
                case PowerUpKind.Seeker:
                    return new Color(0.40f, 1f, 0.50f, 0.2f);
                default:
                    return new Color(0f, 0f, 0f, 0.22f);
            }
        }
    }
}
