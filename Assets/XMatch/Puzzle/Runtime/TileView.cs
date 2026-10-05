using UnityEngine;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public sealed class TileView : MonoBehaviour
    {
        public const float BaseScale = 0.90f;

        private SpriteRenderer glowRenderer;
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
                new Vector3(0f, 0f, 0.04f);
            glowObject.transform.localScale =
                new Vector3(1.12f, 1.12f, 1f);

            glowRenderer =
                glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sortingOrder = 0;

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
                0.14f +
                ((Mathf.Sin(phase) + 1f) * 0.08f);

            glowRenderer.color =
                new Color(1f, 0.75f, 0.95f, alpha);

            float scale =
                1.08f +
                ((Mathf.Sin(phase * 0.8f) + 1f) *
                 0.035f);

            glowRenderer.transform.localScale =
                new Vector3(scale, scale, 1f);

            if (PowerUp == PowerUpKind.ColorOrb)
            {
                transform.Rotate(
                    0f,
                    0f,
                    10f * Time.unscaledDeltaTime);
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
                BaseScale * Mathf.Max(0f, factor);

            transform.localScale =
                new Vector3(scale, scale, 1f);
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

            glowRenderer.sprite = art;

            if (PowerUp == PowerUpKind.None)
            {
                glowRenderer.color =
                    new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                glowRenderer.color =
                    new Color(1f, 0.72f, 0.95f, 0.18f);
            }
        }
    }
}
