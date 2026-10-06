using System;
using System.Collections.Generic;
using UnityEngine;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public enum XMatchVfxKind
    {
        PopSmall = 0,
        PopBig = 1,
        RowBlast = 2,
        ColumnBlast = 3,
        BombBurst = 4,
        ColorOrbBurst = 5,
        HeartBurst = 6,
        RoseBurst = 7,
        MagicCircle = 8,
        SeekerDash = 9
    }

    public static class XMatchArtLibrary
    {
        private const int CellSize = 64;
        private const int Columns = 5;
        private const int Rows = 2;
        private const int BoosterSize = 96;
        private const int BoosterAssetCellSize = 64;
        private const int BoosterAssetColumns = 6;
        private const int BoosterAssetRows = 2;

        private static readonly Dictionary<int, Sprite>
            tileSprites =
                new Dictionary<int, Sprite>();

        private static readonly Dictionary<int, Sprite>
            vfxSprites =
                new Dictionary<int, Sprite>();

        private static readonly Dictionary<int, Sprite>
            boosterSprites =
                new Dictionary<int, Sprite>();

        private static Texture2D tilesAtlas;
        private static Texture2D vfxAtlas;
        private static Texture2D boosterAtlas;
        private static Sprite backgroundSprite;
        private static Texture2D resultPanelTexture;
        private static Texture2D primaryButtonTexture;
        private static Texture2D secondaryButtonTexture;
        private static bool attemptedLoad;

        public static string LoadError { get; private set; }

        public static void Warmup()
        {
            try
            {
                EnsureAtlases();

                GetBackgroundSprite();

                GetBoosterSprite(BoosterKind.Hammer);
                GetBoosterSprite(BoosterKind.RowClear);
                GetBoosterSprite(BoosterKind.ColumnClear);
                GetBoosterSprite(BoosterKind.Shuffle);
                GetBoosterSprite(BoosterKind.GiftBox);
                GetBoosterSprite(BoosterKind.MagicWand);
            }
            catch (Exception exception)
            {
                LoadError = exception.Message;
                Debug.LogException(exception);
            }
        }

        public static Sprite GetTileSprite(
            TileKind kind,
            PowerUpKind powerUp)
        {
            try
            {
                EnsureAtlases();

                if (tilesAtlas == null)
                {
                    return null;
                }

                int index =
                    TileAtlasIndex(
                        kind,
                        powerUp);

                if (index < 0)
                {
                    return null;
                }

                Sprite sprite;

                if (tileSprites.TryGetValue(
                        index,
                        out sprite))
                {
                    return sprite;
                }

                sprite =
                    CreateTintedTileSprite(
                        tilesAtlas,
                        index);

                tileSprites[index] = sprite;
                return sprite;
            }
            catch (Exception exception)
            {
                LoadError = exception.Message;
                Debug.LogException(exception);
                return null;
            }
        }

        public static Sprite GetVfxSprite(
            XMatchVfxKind kind)
        {
            try
            {
                EnsureAtlases();

                if (vfxAtlas == null)
                {
                    return null;
                }

                int index = (int)kind;

                Sprite sprite;

                if (vfxSprites.TryGetValue(
                        index,
                        out sprite))
                {
                    return sprite;
                }

                sprite =
                    CreateAtlasSprite(
                        vfxAtlas,
                        index);

                vfxSprites[index] = sprite;
                return sprite;
            }
            catch (Exception exception)
            {
                LoadError = exception.Message;
                Debug.LogException(exception);
                return null;
            }
        }

        public static Sprite GetBoosterSprite(
            BoosterKind booster,
            bool selected = false)
        {
            try
            {
                EnsureAtlases();

                int column =
                    BoosterAtlasColumn(
                        booster);

                int cacheKey =
                    (column * 2) +
                    (selected ? 1 : 0);

                Sprite sprite;

                if (boosterSprites.TryGetValue(
                        cacheKey,
                        out sprite))
                {
                    return sprite;
                }

                if (boosterAtlas != null &&
                    column >= 0)
                {
                    sprite =
                        CreateBoosterAtlasSprite(
                            boosterAtlas,
                            column,
                            selected);
                }
                else
                {
                    sprite =
                        CreateBoosterSprite(
                            booster);
                }

                boosterSprites[cacheKey] =
                    sprite;

                return sprite;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                return
                    CreateBoosterSprite(
                        booster);
            }
        }

        public static Texture2D GetResultPanelTexture()
        {
            if (resultPanelTexture == null)
            {
                resultPanelTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_ResultPanel",
                        new Color(
                            0.055f,
                            0.060f,
                            0.070f,
                            0.98f),
                        new Color(
                            0.105f,
                            0.090f,
                            0.085f,
                            0.98f),
                        new Color(
                            0.64f,
                            0.53f,
                            0.36f,
                            1f),
                        12f);
            }

            return resultPanelTexture;
        }

        public static Texture2D GetPrimaryButtonTexture()
        {
            if (primaryButtonTexture == null)
            {
                primaryButtonTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_PrimaryButton",
                        new Color(
                            0.36f,
                            0.30f,
                            0.22f,
                            1f),
                        new Color(
                            0.20f,
                            0.18f,
                            0.16f,
                            1f),
                        new Color(
                            0.82f,
                            0.68f,
                            0.42f,
                            1f),
                        16f);
            }

            return primaryButtonTexture;
        }

        public static Texture2D GetSecondaryButtonTexture()
        {
            if (secondaryButtonTexture == null)
            {
                secondaryButtonTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_SecondaryButton",
                        new Color(
                            0.18f,
                            0.22f,
                            0.25f,
                            1f),
                        new Color(
                            0.10f,
                            0.12f,
                            0.15f,
                            1f),
                        new Color(
                            0.46f,
                            0.51f,
                            0.55f,
                            1f),
                        16f);
            }

            return secondaryButtonTexture;
        }

        public static Sprite GetBackgroundSprite()
        {
            if (backgroundSprite != null)
            {
                return backgroundSprite;
            }

            const int width = 256;
            const int height = 512;

            var texture =
                new Texture2D(
                    width,
                    height,
                    TextureFormat.RGBA32,
                    false);

            texture.name =
                "XMatch_HotelNight_Background";
            texture.filterMode =
                FilterMode.Bilinear;
            texture.wrapMode =
                TextureWrapMode.Clamp;

            var pixels =
                new Color[width * height];

            Color top =
                new Color(
                    0.045f,
                    0.060f,
                    0.080f,
                    1f);

            Color middle =
                new Color(
                    0.095f,
                    0.072f,
                    0.075f,
                    1f);

            Color bottom =
                new Color(
                    0.115f,
                    0.085f,
                    0.060f,
                    1f);

            for (int y = 0;
                 y < height;
                 y++)
            {
                float t =
                    y / (float)(height - 1);

                Color row =
                    t < 0.55f
                        ? Color.Lerp(
                            bottom,
                            middle,
                            t / 0.55f)
                        : Color.Lerp(
                            middle,
                            top,
                            (t - 0.55f) / 0.45f);

                for (int x = 0;
                     x < width;
                     x++)
                {
                    float centerDistance =
                        Mathf.Abs(
                            (x / (float)(width - 1)) -
                            0.5f) *
                        2f;

                    float vignette =
                        Mathf.Lerp(
                            1f,
                            0.74f,
                            centerDistance *
                            centerDistance);

                    pixels[
                        (y * width) + x] =
                        row * vignette;
                }
            }

            AddBokeh(
                pixels,
                width,
                height,
                38,
                20261006);

            texture.SetPixels(pixels);
            texture.Apply();

            backgroundSprite =
                Sprite.Create(
                    texture,
                    new Rect(
                        0f,
                        0f,
                        width,
                        height),
                    new Vector2(
                        0.5f,
                        0.5f),
                    100f,
                    0,
                    SpriteMeshType.FullRect);

            backgroundSprite.name =
                "XMatch_HotelNight_BackgroundSprite";

            return backgroundSprite;
        }

        private static void EnsureAtlases()
        {
            if (attemptedLoad)
            {
                return;
            }

            attemptedLoad = true;

            tilesAtlas =
                LoadTextureFromBytes(
                    "XMatch/Art/TilesAtlasBytes",
                    "XMatch_TilesAtlas_Runtime");

            vfxAtlas =
                LoadTextureFromBytes(
                    "XMatch/Art/VfxAtlasBytes",
                    "XMatch_VfxAtlas_Runtime");

            boosterAtlas =
                LoadTextureFromBytes(
                    "XMatch/Art/BoosterIconsAtlas64",
                    "XMatch_BoosterAtlas_Runtime");

            if (tilesAtlas == null)
            {
                LoadError =
                    "Tiles atlas bytes could not be loaded.";
            }
            else if (vfxAtlas == null)
            {
                LoadError =
                    "VFX atlas bytes could not be loaded.";
            }
        }

        private static Texture2D LoadTextureFromBytes(
            string resourcePath,
            string textureName)
        {
            TextAsset asset =
                Resources.Load<TextAsset>(
                    resourcePath);

            if (asset == null ||
                asset.bytes == null ||
                asset.bytes.Length == 0)
            {
                return null;
            }

            var texture =
                new Texture2D(
                    2,
                    2,
                    TextureFormat.RGBA32,
                    false);

            texture.name = textureName;
            texture.filterMode =
                FilterMode.Bilinear;
            texture.wrapMode =
                TextureWrapMode.Clamp;

            if (!ImageConversion.LoadImage(
                    texture,
                    asset.bytes,
                    false))
            {
                UnityEngine.Object.Destroy(texture);
                return null;
            }

            return texture;
        }

        private static Sprite CreateTintedTileSprite(
            Texture2D texture,
            int index)
        {
            int column =
                index % Columns;
            int topRow =
                index / Columns;
            int unityRow =
                (Rows - 1) - topRow;

            int startX =
                column * CellSize;
            int startY =
                unityRow * CellSize;

            Color[] pixels =
                texture.GetPixels(
                    startX,
                    startY,
                    CellSize,
                    CellSize);

            bool recolor =
                index != 8;

            if (recolor)
            {
                float targetHue =
                    TargetHue(index);

                float targetSaturation =
                    TargetSaturation(index);

                for (int i = 0;
                     i < pixels.Length;
                     i++)
                {
                    Color color = pixels[i];

                    if (color.a <= 0.01f)
                    {
                        continue;
                    }

                    float h;
                    float s;
                    float v;

                    Color.RGBToHSV(
                        color,
                        out h,
                        out s,
                        out v);

                    if (s < 0.08f)
                    {
                        continue;
                    }

                    float softenedValue =
                        Mathf.Lerp(
                            v,
                            Mathf.Clamp01(
                                0.18f +
                                (v * 0.82f)),
                            0.55f);

                    float softenedSaturation =
                        Mathf.Lerp(
                            s,
                            targetSaturation,
                            0.76f);

                    Color recolored =
                        Color.HSVToRGB(
                            targetHue,
                            softenedSaturation,
                            softenedValue);

                    recolored.a =
                        color.a;

                    pixels[i] =
                        Color.Lerp(
                            color,
                            recolored,
                            0.82f);
                }
            }

            var tileTexture =
                new Texture2D(
                    CellSize,
                    CellSize,
                    TextureFormat.RGBA32,
                    false);

            tileTexture.name =
                "XMatch_Tile_" + index;
            tileTexture.filterMode =
                FilterMode.Bilinear;
            tileTexture.wrapMode =
                TextureWrapMode.Clamp;

            tileTexture.SetPixels(pixels);
            tileTexture.Apply();

            Sprite sprite =
                Sprite.Create(
                    tileTexture,
                    new Rect(
                        0f,
                        0f,
                        CellSize,
                        CellSize),
                    new Vector2(
                        0.5f,
                        0.5f),
                    CellSize,
                    0,
                    SpriteMeshType.FullRect);

            sprite.name =
                tileTexture.name + "_Sprite";

            return sprite;
        }

        private static float TargetHue(int index)
        {
            switch (index)
            {
                case 0:
                    return 0.065f;
                case 1:
                    return 0.77f;
                case 2:
                    return 0.54f;
                case 3:
                    return 0.115f;
                case 4:
                    return 0.39f;
                case 5:
                    return 0.52f;
                case 6:
                    return 0.105f;
                case 7:
                    return 0.82f;
                case 9:
                    return 0.37f;
                default:
                    return 0f;
            }
        }

        private static float TargetSaturation(
            int index)
        {
            switch (index)
            {
                case 0:
                    return 0.58f;
                case 1:
                    return 0.45f;
                case 2:
                    return 0.62f;
                case 3:
                    return 0.55f;
                case 4:
                    return 0.50f;
                default:
                    return 0.58f;
            }
        }

        private static int TileAtlasIndex(
            TileKind kind,
            PowerUpKind powerUp)
        {
            switch (powerUp)
            {
                case PowerUpKind.RowBlast:
                    return 5;
                case PowerUpKind.ColumnBlast:
                    return 6;
                case PowerUpKind.Bomb:
                    return 7;
                case PowerUpKind.ColorOrb:
                    return 8;
                case PowerUpKind.Seeker:
                    return 9;
            }

            switch (kind)
            {
                case TileKind.Heart:
                    return 0;
                case TileKind.Lips:
                    return 1;
                case TileKind.Diamond:
                    return 2;
                case TileKind.Perfume:
                    return 3;
                case TileKind.Rose:
                    return 4;
                case TileKind.Wild:
                    return 8;
                default:
                    return -1;
            }
        }

        private static Sprite CreateAtlasSprite(
            Texture2D texture,
            int index)
        {
            int column =
                index % Columns;
            int topRow =
                index / Columns;
            int unityRow =
                (Rows - 1) - topRow;

            Rect rect =
                new Rect(
                    column * CellSize,
                    unityRow * CellSize,
                    CellSize,
                    CellSize);

            Sprite sprite =
                Sprite.Create(
                    texture,
                    rect,
                    new Vector2(
                        0.5f,
                        0.5f),
                    CellSize,
                    0,
                    SpriteMeshType.FullRect);

            sprite.name =
                texture.name +
                "_" +
                index;

            return sprite;
        }

        private static int BoosterAtlasColumn(
            BoosterKind booster)
        {
            switch (booster)
            {
                case BoosterKind.Hammer:
                    return 0;
                case BoosterKind.RowClear:
                    return 1;
                case BoosterKind.ColumnClear:
                    return 2;
                case BoosterKind.Shuffle:
                    return 3;
                case BoosterKind.GiftBox:
                    return 4;
                case BoosterKind.MagicWand:
                    return 5;
                default:
                    return -1;
            }
        }

        private static Sprite CreateBoosterAtlasSprite(
            Texture2D atlas,
            int column,
            bool selected)
        {
            int startX =
                column *
                BoosterAssetCellSize;

            int startY =
                selected
                    ? 0
                    : BoosterAssetCellSize;

            Color[] pixels =
                atlas.GetPixels(
                    startX,
                    startY,
                    BoosterAssetCellSize,
                    BoosterAssetCellSize);

            var texture =
                new Texture2D(
                    BoosterAssetCellSize,
                    BoosterAssetCellSize,
                    TextureFormat.RGBA32,
                    false);

            texture.name =
                "XMatch_BoosterArt_" +
                column +
                (selected
                    ? "_Selected"
                    : "_Normal");

            texture.filterMode =
                FilterMode.Bilinear;

            texture.wrapMode =
                TextureWrapMode.Clamp;

            texture.SetPixels(pixels);
            texture.Apply();

            Sprite sprite =
                Sprite.Create(
                    texture,
                    new Rect(
                        0f,
                        0f,
                        BoosterAssetCellSize,
                        BoosterAssetCellSize),
                    new Vector2(
                        0.5f,
                        0.5f),
                    BoosterAssetCellSize,
                    0,
                    SpriteMeshType.FullRect);

            sprite.name =
                texture.name +
                "_Sprite";

            return sprite;
        }

        private static Sprite CreateBoosterSprite(
            BoosterKind booster)
        {
            var texture =
                new Texture2D(
                    BoosterSize,
                    BoosterSize,
                    TextureFormat.RGBA32,
                    false);

            texture.name =
                "XMatch_Booster_" +
                booster;
            texture.filterMode =
                FilterMode.Bilinear;
            texture.wrapMode =
                TextureWrapMode.Clamp;

            var pixels =
                new Color[
                    BoosterSize *
                    BoosterSize];

            Color clear =
                new Color(
                    0f,
                    0f,
                    0f,
                    0f);

            for (int i = 0;
                 i < pixels.Length;
                 i++)
            {
                pixels[i] = clear;
            }

            Color plate =
                new Color(
                    0.105f,
                    0.085f,
                    0.105f,
                    0.97f);

            Color champagne =
                new Color(
                    0.78f,
                    0.64f,
                    0.40f,
                    1f);

            Color accent =
                BoosterAccent(
                    booster);

            DrawDisk(
                pixels,
                BoosterSize,
                BoosterSize,
                48f,
                48f,
                43f,
                plate);

            DrawRing(
                pixels,
                BoosterSize,
                BoosterSize,
                48f,
                48f,
                43f,
                3.1f,
                champagne);

            DrawBoosterGlyph(
                pixels,
                BoosterSize,
                BoosterSize,
                booster,
                accent,
                champagne);

            texture.SetPixels(pixels);
            texture.Apply();

            return
                Sprite.Create(
                    texture,
                    new Rect(
                        0f,
                        0f,
                        BoosterSize,
                        BoosterSize),
                    new Vector2(
                        0.5f,
                        0.5f),
                    BoosterSize,
                    0,
                    SpriteMeshType.FullRect);
        }

        private static Color BoosterAccent(
            BoosterKind booster)
        {
            switch (booster)
            {
                case BoosterKind.RowClear:
                    return new Color(
                        0.36f,
                        0.68f,
                        0.72f,
                        1f);
                case BoosterKind.ColumnClear:
                    return new Color(
                        0.82f,
                        0.66f,
                        0.35f,
                        1f);
                case BoosterKind.Shuffle:
                    return new Color(
                        0.40f,
                        0.62f,
                        0.50f,
                        1f);
                case BoosterKind.GiftBox:
                    return new Color(
                        0.72f,
                        0.58f,
                        0.42f,
                        1f);
                case BoosterKind.MagicWand:
                    return new Color(
                        0.48f,
                        0.57f,
                        0.78f,
                        1f);
                default:
                    return new Color(
                        0.78f,
                        0.64f,
                        0.40f,
                        1f);
            }
        }

        private static void DrawBoosterGlyph(
            Color[] pixels,
            int width,
            int height,
            BoosterKind booster,
            Color accent,
            Color gold)
        {
            switch (booster)
            {
                case BoosterKind.Hammer:
                    DrawLine(
                        pixels,
                        width,
                        height,
                        37,
                        61,
                        59,
                        34,
                        7f,
                        gold);
                    FillRect(
                        pixels,
                        width,
                        height,
                        32,
                        57,
                        27,
                        13,
                        accent);
                    break;

                case BoosterKind.RowClear:
                    DrawLine(
                        pixels,
                        width,
                        height,
                        24,
                        48,
                        72,
                        48,
                        7f,
                        accent);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        24,
                        48,
                        37,
                        37,
                        6f,
                        gold);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        24,
                        48,
                        37,
                        59,
                        6f,
                        gold);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        72,
                        48,
                        59,
                        37,
                        6f,
                        gold);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        72,
                        48,
                        59,
                        59,
                        6f,
                        gold);
                    break;

                case BoosterKind.ColumnClear:
                    DrawLine(
                        pixels,
                        width,
                        height,
                        48,
                        24,
                        48,
                        72,
                        7f,
                        accent);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        48,
                        24,
                        37,
                        37,
                        6f,
                        gold);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        48,
                        24,
                        59,
                        37,
                        6f,
                        gold);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        48,
                        72,
                        37,
                        59,
                        6f,
                        gold);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        48,
                        72,
                        59,
                        59,
                        6f,
                        gold);
                    break;

                case BoosterKind.Shuffle:
                    DrawLine(
                        pixels,
                        width,
                        height,
                        25,
                        35,
                        69,
                        61,
                        6f,
                        accent);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        25,
                        61,
                        69,
                        35,
                        6f,
                        gold);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        69,
                        61,
                        59,
                        61,
                        5f,
                        accent);
                    DrawLine(
                        pixels,
                        width,
                        height,
                        69,
                        61,
                        65,
                        51,
                        5f,
                        accent);
                    break;

                case BoosterKind.GiftBox:
                    FillRect(
                        pixels,
                        width,
                        height,
                        27,
                        37,
                        42,
                        31,
                        accent);
                    FillRect(
                        pixels,
                        width,
                        height,
                        24,
                        31,
                        48,
                        9,
                        gold);
                    FillRect(
                        pixels,
                        width,
                        height,
                        45,
                        31,
                        7,
                        37,
                        gold);
                    DrawRing(
                        pixels,
                        width,
                        height,
                        39f,
                        27f,
                        11f,
                        4f,
                        accent);
                    DrawRing(
                        pixels,
                        width,
                        height,
                        57f,
                        27f,
                        11f,
                        4f,
                        accent);
                    break;

                case BoosterKind.MagicWand:
                    DrawLine(
                        pixels,
                        width,
                        height,
                        31,
                        64,
                        62,
                        31,
                        7f,
                        gold);
                    DrawStar(
                        pixels,
                        width,
                        height,
                        67,
                        27,
                        12f,
                        accent);
                    break;
            }
        }

        private static void DrawDisk(
            Color[] pixels,
            int width,
            int height,
            float cx,
            float cy,
            float radius,
            Color color)
        {
            float radiusSq =
                radius * radius;

            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
                {
                    float dx =
                        x - cx;
                    float dy =
                        y - cy;

                    if ((dx * dx) +
                        (dy * dy) <=
                        radiusSq)
                    {
                        pixels[
                            (y * width) +
                            x] = color;
                    }
                }
            }
        }

        private static void DrawRing(
            Color[] pixels,
            int width,
            int height,
            float cx,
            float cy,
            float radius,
            float thickness,
            Color color)
        {
            float inner =
                radius - thickness;
            float outerSq =
                radius * radius;
            float innerSq =
                inner * inner;

            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
                {
                    float dx =
                        x - cx;
                    float dy =
                        y - cy;

                    float distance =
                        (dx * dx) +
                        (dy * dy);

                    if (distance <=
                            outerSq &&
                        distance >=
                            innerSq)
                    {
                        pixels[
                            (y * width) +
                            x] = color;
                    }
                }
            }
        }

        private static void DrawLine(
            Color[] pixels,
            int width,
            int height,
            int x0,
            int y0,
            int x1,
            int y1,
            float thickness,
            Color color)
        {
            float dx =
                x1 - x0;
            float dy =
                y1 - y0;

            float lengthSq =
                (dx * dx) +
                (dy * dy);

            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
                {
                    float t =
                        lengthSq <= 0.001f
                            ? 0f
                            : Mathf.Clamp01(
                                (((x - x0) * dx) +
                                 ((y - y0) * dy)) /
                                lengthSq);

                    float px =
                        x0 + (dx * t);
                    float py =
                        y0 + (dy * t);

                    float ddx =
                        x - px;
                    float ddy =
                        y - py;

                    if ((ddx * ddx) +
                        (ddy * ddy) <=
                        thickness * thickness)
                    {
                        pixels[
                            (y * width) +
                            x] = color;
                    }
                }
            }
        }

        private static void FillRect(
            Color[] pixels,
            int width,
            int height,
            int x,
            int y,
            int rectWidth,
            int rectHeight,
            Color color)
        {
            int maxX =
                Mathf.Min(
                    width,
                    x + rectWidth);
            int maxY =
                Mathf.Min(
                    height,
                    y + rectHeight);

            for (int yy =
                     Mathf.Max(0, y);
                 yy < maxY;
                 yy++)
            {
                for (int xx =
                         Mathf.Max(0, x);
                     xx < maxX;
                     xx++)
                {
                    pixels[
                        (yy * width) +
                        xx] = color;
                }
            }
        }

        private static void DrawStar(
            Color[] pixels,
            int width,
            int height,
            int cx,
            int cy,
            float radius,
            Color color)
        {
            DrawLine(
                pixels,
                width,
                height,
                cx - (int)radius,
                cy,
                cx + (int)radius,
                cy,
                3.2f,
                color);

            DrawLine(
                pixels,
                width,
                height,
                cx,
                cy - (int)radius,
                cx,
                cy + (int)radius,
                3.2f,
                color);

            DrawLine(
                pixels,
                width,
                height,
                cx - 8,
                cy - 8,
                cx + 8,
                cy + 8,
                2.5f,
                color);

            DrawLine(
                pixels,
                width,
                height,
                cx - 8,
                cy + 8,
                cx + 8,
                cy - 8,
                2.5f,
                color);
        }

        private static Texture2D CreateLuxuryUiTexture(
            string name,
            Color top,
            Color bottom,
            Color border,
            float cornerRadius)
        {
            const int width = 160;
            const int height = 80;

            var texture =
                new Texture2D(
                    width,
                    height,
                    TextureFormat.RGBA32,
                    false);

            texture.name = name;
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            var pixels =
                new Color[width * height];

            for (int y = 0;
                 y < height;
                 y++)
            {
                float t =
                    y / (float)(height - 1);

                Color fill =
                    Color.Lerp(
                        bottom,
                        top,
                        t);

                for (int x = 0;
                     x < width;
                     x++)
                {
                    float edgeDistance =
                        RoundedRectEdgeDistance(
                            x,
                            y,
                            width,
                            height,
                            cornerRadius);

                    int index =
                        (y * width) + x;

                    if (edgeDistance > 0f)
                    {
                        pixels[index] =
                            new Color(
                                0f,
                                0f,
                                0f,
                                0f);

                        continue;
                    }

                    float distanceToBorder =
                        -edgeDistance;

                    if (distanceToBorder < 2.4f)
                    {
                        pixels[index] =
                            border;

                        continue;
                    }

                    float highlight =
                        Mathf.Clamp01(
                            (y -
                             (height * 0.60f)) /
                            (height * 0.40f));

                    Color value =
                        Color.Lerp(
                            fill,
                            Color.Lerp(
                                fill,
                                Color.white,
                                0.08f),
                            highlight);

                    float sideShade =
                        Mathf.Abs(
                            (x /
                             (float)(width - 1)) -
                            0.5f) *
                        2f;

                    value =
                        Color.Lerp(
                            value,
                            value * 0.80f,
                            sideShade *
                            sideShade *
                            0.38f);

                    pixels[index] =
                        value;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        private static float RoundedRectEdgeDistance(
            float x,
            float y,
            float width,
            float height,
            float radius)
        {
            float halfWidth =
                width * 0.5f;

            float halfHeight =
                height * 0.5f;

            float px =
                Mathf.Abs(
                    x - halfWidth) -
                (halfWidth - radius);

            float py =
                Mathf.Abs(
                    y - halfHeight) -
                (halfHeight - radius);

            float outsideX =
                Mathf.Max(px, 0f);

            float outsideY =
                Mathf.Max(py, 0f);

            float outside =
                Mathf.Sqrt(
                    (outsideX * outsideX) +
                    (outsideY * outsideY));

            float inside =
                Mathf.Min(
                    Mathf.Max(px, py),
                    0f);

            return
                outside +
                inside -
                radius;
        }

        private static void AddBokeh(
            Color[] pixels,
            int width,
            int height,
            int count,
            int seed)
        {
            var random =
                new System.Random(seed);

            for (int i = 0;
                 i < count;
                 i++)
            {
                float cx =
                    (float)random.NextDouble() *
                    width;

                float cy =
                    (float)random.NextDouble() *
                    height;

                float radius =
                    2.5f +
                    ((float)random.NextDouble() *
                     8.5f);

                float strength =
                    0.025f +
                    ((float)random.NextDouble() *
                     0.060f);

                Color glow =
                    i % 3 == 0
                        ? new Color(
                            0.73f,
                            0.59f,
                            0.38f,
                            1f)
                        : new Color(
                            0.48f,
                            0.50f,
                            0.58f,
                            1f);

                int minX =
                    Mathf.Max(
                        0,
                        Mathf.FloorToInt(
                            cx - radius));

                int maxX =
                    Mathf.Min(
                        width - 1,
                        Mathf.CeilToInt(
                            cx + radius));

                int minY =
                    Mathf.Max(
                        0,
                        Mathf.FloorToInt(
                            cy - radius));

                int maxY =
                    Mathf.Min(
                        height - 1,
                        Mathf.CeilToInt(
                            cy + radius));

                for (int y = minY;
                     y <= maxY;
                     y++)
                {
                    for (int x = minX;
                         x <= maxX;
                         x++)
                    {
                        float dx =
                            x - cx;
                        float dy =
                            y - cy;

                        float distance =
                            Mathf.Sqrt(
                                (dx * dx) +
                                (dy * dy));

                        if (distance >
                            radius)
                        {
                            continue;
                        }

                        float alpha =
                            (1f -
                             (distance /
                              radius)) *
                            strength;

                        int index =
                            (y * width) +
                            x;

                        pixels[index] =
                            Color.Lerp(
                                pixels[index],
                                glow,
                                alpha);
                    }
                }
            }
        }
    }
}
