using UnityEngine;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public sealed class TileView : MonoBehaviour
    {
        public const float BaseScale = 0.90f;

        private SpriteRenderer glowRenderer;
        private SpriteRenderer plateRenderer;
        private SpriteRenderer spriteRenderer;
        private Sprite fallbackSprite;

        public TileKind Kind { get; private set; }
        public PowerUpKind PowerUp { get; private set; }

        public void Initialize(
            TileKind kind,
            Sprite fallback)
        {
            Initialize(
                kind,
                PowerUpKind.None,
                fallback);
        }

        public void Initialize(
            TileKind kind,
            PowerUpKind powerUp,
            Sprite fallback)
        {
            Kind = kind;
            PowerUp = powerUp;
            fallbackSprite = fallback;

            var glowObject =
                new GameObject("Glow");
            glowObject.transform.SetParent(
                transform,
                worldPositionStays: false);
            glowObject.transform.localPosition =
                new Vector3(0f, 0f, 0.06f);
            glowObject.transform.localScale =
                new Vector3(1.16f, 1.16f, 1f);

            glowRenderer =
                glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sortingOrder = -1;

            var plateObject =
                new GameObject("Contrast Plate");
            plateObject.transform.SetParent(
                transform,
                worldPositionStays: false);
            plateObject.transform.localPosition =
                new Vector3(0f, 0f, 0.035f);
            plateObject.transform.localScale =
                new Vector3(1.02f, 1.02f, 1f);

            plateRenderer =
                plateObject.AddComponent<SpriteRenderer>();
            plateRenderer.sprite = fallbackSprite;
            plateRenderer.sortingOrder = 0;

            spriteRenderer =
                gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;

            RefreshAppearance();
            SetScaleFactor(1f);
        }

        private void Update()
        {
            if (PowerUp == PowerUpKind.None ||
                glowRenderer == null)
            {
                return;
            }

            float phase =
                Time.unscaledTime * 5.5f +
                (transform.position.x * 0.6f);

            float alpha =
                0.16f +
                ((Mathf.Sin(phase) + 1f) * 0.10f);

            Color glow =
                PowerGlowColor(PowerUp);

            glow.a = alpha;
            glowRenderer.color = glow;

            float scale =
                1.10f +
                ((Mathf.Sin(phase * 0.8f) + 1f) *
                 0.045f);

            glowRenderer.transform.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f);

            if (PowerUp == PowerUpKind.ColorOrb)
            {
                transform.Rotate(
                    0f,
                    0f,
                    12f *
                    Time.unscaledDeltaTime);
            }
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
                BaseScale *
                Mathf.Max(0f, factor);

            transform.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f);
        }

        private void RefreshAppearance()
        {
            Sprite art =
                XMatchArtLibrary.GetTileSprite(
                    Kind,
                    PowerUp);

            if (art == null)
            {
                art = fallbackSprite;
            }

            spriteRenderer.sprite = art;
            spriteRenderer.color = Color.white;

            if (plateRenderer != null)
            {
                plateRenderer.sprite =
                    fallbackSprite;
                plateRenderer.color =
                    PlateColor(
                        Kind,
                        PowerUp);
            }

            glowRenderer.sprite = art;

            if (PowerUp == PowerUpKind.None)
            {
                glowRenderer.color =
                    new Color(
                        0f,
                        0f,
                        0f,
                        0f);
            }
            else
            {
                Color glow =
                    PowerGlowColor(PowerUp);
                glow.a = 0.20f;
                glowRenderer.color = glow;
            }
        }

        private static Color PlateColor(
            TileKind kind,
            PowerUpKind powerUp)
        {
            if (powerUp != PowerUpKind.None)
            {
                return
                    new Color(
                        0.20f,
                        0.08f,
                        0.24f,
                        0.84f);
            }

            switch (kind)
            {
                case TileKind.Heart:
                    return
                        new Color(
                            0.28f,
                            0.015f,
                            0.045f,
                            0.78f);

                case TileKind.Lips:
                    return
                        new Color(
                            0.20f,
                            0.015f,
                            0.24f,
                            0.78f);

                case TileKind.Diamond:
                    return
                        new Color(
                            0.015f,
                            0.12f,
                            0.28f,
                            0.78f);

                case TileKind.Perfume:
                    return
                        new Color(
                            0.16f,
                            0.04f,
                            0.28f,
                            0.78f);

                case TileKind.Rose:
                    return
                        new Color(
                            0.015f,
                            0.22f,
                            0.10f,
                            0.78f);

                default:
                    return
                        new Color(
                            0.08f,
                            0.06f,
                            0.12f,
                            0.78f);
            }
        }

        private static Color PowerGlowColor(
            PowerUpKind powerUp)
        {
            switch (powerUp)
            {
                case PowerUpKind.RowBlast:
                    return
                        new Color(
                            0.25f,
                            0.95f,
                            1f,
                            1f);

                case PowerUpKind.ColumnBlast:
                    return
                        new Color(
                            1f,
                            0.70f,
                            0.18f,
                            1f);

                case PowerUpKind.Bomb:
                    return
                        new Color(
                            1f,
                            0.18f,
                            0.26f,
                            1f);

                case PowerUpKind.ColorOrb:
                    return
                        new Color(
                            0.90f,
                            0.50f,
                            1f,
                            1f);

                case PowerUpKind.Seeker:
                    return
                        new Color(
                            0.42f,
                            1f,
                            0.58f,
                            1f);

                default:
                    return Color.white;
            }
        }
    }
}
