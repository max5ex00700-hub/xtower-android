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
        private static Texture2D headerPanelTexture;
        private static Texture2D missionPanelTexture;
        private static Texture2D boosterPanelTexture;
        private static Texture2D boosterButtonTexture;
        private static Texture2D boosterSelectedButtonTexture;
        private static Sprite boardFrameSprite;
        private static bool attemptedLoad;

        private const string EmbeddedBoosterAtlasBase64 =
            "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";

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

        public static Texture2D GetHeaderPanelTexture()
        {
            if (headerPanelTexture == null)
            {
                headerPanelTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_HeaderPanel",
                        new Color(
                            0.26f,
                            0.15f,
                            0.40f,
                            0.99f),
                        new Color(
                            0.075f,
                            0.085f,
                            0.22f,
                            0.99f),
                        new Color(
                            0.78f,
                            0.63f,
                            0.34f,
                            1f),
                        18f);
            }

            return headerPanelTexture;
        }

        public static Texture2D GetMissionPanelTexture()
        {
            if (missionPanelTexture == null)
            {
                missionPanelTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_MissionPanel",
                        new Color(
                            0.20f,
                            0.12f,
                            0.32f,
                            0.98f),
                        new Color(
                            0.060f,
                            0.075f,
                            0.18f,
                            0.98f),
                        new Color(
                            0.52f,
                            0.45f,
                            0.33f,
                            1f),
                        16f);
            }

            return missionPanelTexture;
        }

        public static Texture2D GetBoosterPanelTexture()
        {
            if (boosterPanelTexture == null)
            {
                boosterPanelTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_BoosterPanel",
                        new Color(
                            0.16f,
                            0.10f,
                            0.27f,
                            0.99f),
                        new Color(
                            0.050f,
                            0.055f,
                            0.14f,
                            0.99f),
                        new Color(
                            0.64f,
                            0.52f,
                            0.30f,
                            1f),
                        16f);
            }

            return boosterPanelTexture;
        }

        public static Texture2D GetBoosterButtonTexture()
        {
            if (boosterButtonTexture == null)
            {
                boosterButtonTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_BoosterButton",
                        new Color(
                            0.22f,
                            0.13f,
                            0.34f,
                            1f),
                        new Color(
                            0.065f,
                            0.075f,
                            0.18f,
                            1f),
                        new Color(
                            0.70f,
                            0.57f,
                            0.33f,
                            1f),
                        15f);
            }

            return boosterButtonTexture;
        }

        public static Texture2D GetBoosterSelectedButtonTexture()
        {
            if (boosterSelectedButtonTexture == null)
            {
                boosterSelectedButtonTexture =
                    CreateLuxuryUiTexture(
                        "XMatch_BoosterSelectedButton",
                        new Color(
                            0.23f,
                            0.18f,
                            0.38f,
                            1f),
                        new Color(
                            0.095f,
                            0.095f,
                            0.20f,
                            1f),
                        new Color(
                            0.96f,
                            0.76f,
                            0.32f,
                            1f),
                        15f);
            }

            return boosterSelectedButtonTexture;
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

            const int width = 360;
            const int height = 640;

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
                    0.075f,
                    0.105f,
                    0.245f,
                    1f);

            Color middle =
                new Color(
                    0.235f,
                    0.105f,
                    0.290f,
                    1f);

            Color bottom =
                new Color(
                    0.105f,
                    0.070f,
                    0.185f,
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
                            0.82f,
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
                78,
                20261006);

            AddHotelScene(
                pixels,
                width,
                height);

            AddBokeh(
                pixels,
                width,
                height,
                42,
                202610061);

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

            if (boosterAtlas == null)
            {
                boosterAtlas =
                    LoadTextureFromBase64(
                        EmbeddedBoosterAtlasBase64,
                        "XMatch_BoosterAtlas_Embedded");
            }

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

        private static Texture2D LoadTextureFromBase64(
            string base64,
            string textureName)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return null;
            }

            byte[] bytes;

            try
            {
                bytes =
                    Convert.FromBase64String(
                        base64);
            }
            catch
            {
                return null;
            }

            var texture =
                new Texture2D(
                    2,
                    2,
                    TextureFormat.RGBA32,
                    false);

            texture.name =
                textureName;

            texture.filterMode =
                FilterMode.Bilinear;

            texture.wrapMode =
                TextureWrapMode.Clamp;

            if (!ImageConversion.LoadImage(
                    texture,
                    bytes,
                    false))
            {
                UnityEngine.Object.Destroy(
                    texture);

                return null;
            }

            return texture;
        }

        public static Sprite GetBoardFrameSprite()
        {
            if (boardFrameSprite != null)
            {
                return boardFrameSprite;
            }

            const int size = 128;
            const float radius = 13f;
            const float thickness = 4f;

            var texture =
                new Texture2D(
                    size,
                    size,
                    TextureFormat.RGBA32,
                    false);

            texture.name =
                "XMatch_BoardGoldFrame";

            texture.filterMode =
                FilterMode.Bilinear;

            texture.wrapMode =
                TextureWrapMode.Clamp;

            var pixels =
                new Color[size * size];

            Color clear =
                new Color(
                    0f,
                    0f,
                    0f,
                    0f);

            Color darkGold =
                new Color(
                    0.48f,
                    0.32f,
                    0.12f,
                    1f);

            Color gold =
                new Color(
                    0.90f,
                    0.68f,
                    0.28f,
                    1f);

            for (int y = 0;
                 y < size;
                 y++)
            {
                for (int x = 0;
                     x < size;
                     x++)
                {
                    float edge =
                        RoundedRectEdgeDistance(
                            x,
                            y,
                            size,
                            size,
                            radius);

                    float inside =
                        -edge;

                    Color value =
                        clear;

                    if (edge <= 0f &&
                        inside <= thickness)
                    {
                        float t =
                            Mathf.Clamp01(
                                y /
                                (float)(size - 1));

                        value =
                            Color.Lerp(
                                darkGold,
                                gold,
                                t);
                    }

                    pixels[
                        (y * size) + x] =
                        value;
                }
            }

            texture.SetPixels(
                pixels);

            texture.Apply();

            boardFrameSprite =
                Sprite.Create(
                    texture,
                    new Rect(
                        0f,
                        0f,
                        size,
                        size),
                    new Vector2(
                        0.5f,
                        0.5f),
                    size,
                    0,
                    SpriteMeshType.FullRect,
                    new Vector4(
                        16f,
                        16f,
                        16f,
                        16f));

            boardFrameSprite.name =
                "XMatch_BoardGoldFrameSprite";

            return boardFrameSprite;
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

                    if (distanceToBorder < 2.2f)
                    {
                        pixels[index] =
                            Color.Lerp(
                                border,
                                Color.white,
                                0.20f);

                        continue;
                    }

                    if (distanceToBorder < 5.0f)
                    {
                        pixels[index] =
                            Color.Lerp(
                                border,
                                new Color(
                                    0.32f,
                                    0.18f,
                                    0.08f,
                                    1f),
                                0.48f);

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

        private static void AddHotelScene(
            Color[] pixels,
            int width,
            int height)
        {
            Color deepNavy =
                new Color(
                    0.035f,
                    0.045f,
                    0.13f,
                    1f);

            Color violet =
                new Color(
                    0.15f,
                    0.075f,
                    0.22f,
                    1f);

            Color gold =
                new Color(
                    0.95f,
                    0.67f,
                    0.25f,
                    1f);

            Color warm =
                new Color(
                    1f,
                    0.55f,
                    0.12f,
                    1f);

            Color moon =
                new Color(
                    0.68f,
                    0.79f,
                    1f,
                    0.78f);

            // Moon and cool halo.
            BlendDisk(
                pixels,
                width,
                height,
                width * 0.50f,
                height * 0.78f,
                width * 0.16f,
                new Color(
                    0.30f,
                    0.42f,
                    0.95f,
                    0.11f));

            BlendDisk(
                pixels,
                width,
                height,
                width * 0.50f,
                height * 0.79f,
                width * 0.070f,
                moon);

            // Distant castle.
            int castleBase =
                Mathf.RoundToInt(
                    height * 0.45f);

            int castleTop =
                Mathf.RoundToInt(
                    height * 0.67f);

            FillRect(
                pixels,
                width,
                height,
                Mathf.RoundToInt(
                    width * 0.29f),
                castleBase,
                Mathf.RoundToInt(
                    width * 0.42f),
                castleTop -
                castleBase,
                deepNavy);

            FillRect(
                pixels,
                width,
                height,
                Mathf.RoundToInt(
                    width * 0.41f),
                castleTop,
                Mathf.RoundToInt(
                    width * 0.18f),
                Mathf.RoundToInt(
                    height * 0.075f),
                deepNavy);

            DrawTriangle(
                pixels,
                width,
                height,
                Mathf.RoundToInt(
                    width * 0.50f),
                Mathf.RoundToInt(
                    height * 0.82f),
                Mathf.RoundToInt(
                    width * 0.12f),
                Mathf.RoundToInt(
                    height * 0.09f),
                deepNavy);

            int leftTowerX =
                Mathf.RoundToInt(
                    width * 0.24f);

            int rightTowerX =
                Mathf.RoundToInt(
                    width * 0.70f);

            FillRect(
                pixels,
                width,
                height,
                leftTowerX,
                Mathf.RoundToInt(
                    height * 0.48f),
                Mathf.RoundToInt(
                    width * 0.12f),
                Mathf.RoundToInt(
                    height * 0.19f),
                deepNavy);

            FillRect(
                pixels,
                width,
                height,
                rightTowerX,
                Mathf.RoundToInt(
                    height * 0.48f),
                Mathf.RoundToInt(
                    width * 0.12f),
                Mathf.RoundToInt(
                    height * 0.19f),
                deepNavy);

            DrawTriangle(
                pixels,
                width,
                height,
                leftTowerX +
                Mathf.RoundToInt(
                    width * 0.06f),
                Mathf.RoundToInt(
                    height * 0.76f),
                Mathf.RoundToInt(
                    width * 0.085f),
                Mathf.RoundToInt(
                    height * 0.09f),
                deepNavy);

            DrawTriangle(
                pixels,
                width,
                height,
                rightTowerX +
                Mathf.RoundToInt(
                    width * 0.06f),
                Mathf.RoundToInt(
                    height * 0.76f),
                Mathf.RoundToInt(
                    width * 0.085f),
                Mathf.RoundToInt(
                    height * 0.09f),
                deepNavy);

            // Warm castle windows.
            int[] windowXs =
            {
                Mathf.RoundToInt(
                    width * 0.36f),
                Mathf.RoundToInt(
                    width * 0.46f),
                Mathf.RoundToInt(
                    width * 0.54f),
                Mathf.RoundToInt(
                    width * 0.64f)
            };

            for (int i = 0;
                 i < windowXs.Length;
                 i++)
            {
                for (int row = 0;
                     row < 2;
                     row++)
                {
                    int y =
                        Mathf.RoundToInt(
                            height *
                            (0.51f +
                             (row * 0.075f)));

                    BlendDisk(
                        pixels,
                        width,
                        height,
                        windowXs[i],
                        y,
                        width * 0.025f,
                        new Color(
                            warm.r,
                            warm.g,
                            warm.b,
                            0.22f));

                    FillRect(
                        pixels,
                        width,
                        height,
                        windowXs[i] - 3,
                        y - 7,
                        6,
                        14,
                        gold);
                }
            }

            // Side balcony columns.
            int columnWidth =
                Mathf.Max(
                    10,
                    Mathf.RoundToInt(
                        width * 0.065f));

            int columnY =
                Mathf.RoundToInt(
                    height * 0.20f);

            int columnHeight =
                Mathf.RoundToInt(
                    height * 0.58f);

            int leftColumn =
                Mathf.RoundToInt(
                    width * 0.035f);

            int rightColumn =
                width -
                leftColumn -
                columnWidth;

            FillRect(
                pixels,
                width,
                height,
                leftColumn,
                columnY,
                columnWidth,
                columnHeight,
                new Color(
                    0.24f,
                    0.12f,
                    0.26f,
                    0.90f));

            FillRect(
                pixels,
                width,
                height,
                rightColumn,
                columnY,
                columnWidth,
                columnHeight,
                new Color(
                    0.24f,
                    0.12f,
                    0.26f,
                    0.90f));

            DrawLine(
                pixels,
                width,
                height,
                leftColumn,
                columnY,
                leftColumn,
                columnY + columnHeight,
                2.0f,
                new Color(
                    0.76f,
                    0.50f,
                    0.24f,
                    0.65f));

            DrawLine(
                pixels,
                width,
                height,
                rightColumn +
                columnWidth,
                columnY,
                rightColumn +
                columnWidth,
                columnY + columnHeight,
                2.0f,
                new Color(
                    0.76f,
                    0.50f,
                    0.24f,
                    0.65f));

            // Two prominent lanterns.
            int lampY =
                Mathf.RoundToInt(
                    height * 0.64f);

            int[] lampXs =
            {
                Mathf.RoundToInt(
                    width * 0.14f),
                Mathf.RoundToInt(
                    width * 0.86f)
            };

            for (int i = 0;
                 i < lampXs.Length;
                 i++)
            {
                int x =
                    lampXs[i];

                BlendDisk(
                    pixels,
                    width,
                    height,
                    x,
                    lampY,
                    width * 0.11f,
                    new Color(
                        1f,
                        0.43f,
                        0.06f,
                        0.13f));

                BlendDisk(
                    pixels,
                    width,
                    height,
                    x,
                    lampY,
                    width * 0.055f,
                    new Color(
                        1f,
                        0.67f,
                        0.16f,
                        0.31f));

                DrawLine(
                    pixels,
                    width,
                    height,
                    x,
                    lampY +
                    Mathf.RoundToInt(
                        height * 0.08f),
                    x,
                    lampY +
                    Mathf.RoundToInt(
                        height * 0.14f),
                    2.0f,
                    gold);

                FillRect(
                    pixels,
                    width,
                    height,
                    x -
                    Mathf.RoundToInt(
                        width * 0.035f),
                    lampY -
                    Mathf.RoundToInt(
                        height * 0.035f),
                    Mathf.RoundToInt(
                        width * 0.070f),
                    Mathf.RoundToInt(
                        height * 0.070f),
                    new Color(
                        0.40f,
                        0.18f,
                        0.12f,
                        0.92f));

                FillRect(
                    pixels,
                    width,
                    height,
                    x -
                    Mathf.RoundToInt(
                        width * 0.024f),
                    lampY -
                    Mathf.RoundToInt(
                        height * 0.025f),
                    Mathf.RoundToInt(
                        width * 0.048f),
                    Mathf.RoundToInt(
                        height * 0.050f),
                    new Color(
                        1f,
                        0.61f,
                        0.18f,
                        0.92f));
            }

            // Rose clusters on the edges.
            for (int side = 0;
                 side < 2;
                 side++)
            {
                float x =
                    side == 0
                        ? width * 0.035f
                        : width * 0.965f;

                for (int i = 0;
                     i < 7;
                     i++)
                {
                    float y =
                        height *
                        (0.18f +
                         (i * 0.10f));

                    BlendDisk(
                        pixels,
                        width,
                        height,
                        x,
                        y,
                        width * 0.045f,
                        new Color(
                            0.45f,
                            0.015f,
                            0.15f,
                            0.75f));

                    BlendDisk(
                        pixels,
                        width,
                        height,
                        x,
                        y,
                        width * 0.023f,
                        new Color(
                            0.90f,
                            0.06f,
                            0.30f,
                            0.85f));
                }
            }

            // Terrace floor at the bottom.
            int floorTop =
                Mathf.RoundToInt(
                    height * 0.17f);

            FillRect(
                pixels,
                width,
                height,
                0,
                0,
                width,
                floorTop,
                new Color(
                    0.055f,
                    0.065f,
                    0.16f,
                    1f));

            for (int row = 1;
                 row < 5;
                 row++)
            {
                int y =
                    floorTop *
                    row /
                    5;

                DrawLine(
                    pixels,
                    width,
                    height,
                    0,
                    y,
                    width - 1,
                    y,
                    1.0f,
                    new Color(
                        0.30f,
                        0.24f,
                        0.40f,
                        0.45f));
            }

            for (int x = 0;
                 x < width;
                 x += Mathf.Max(
                     18,
                     width / 8))
            {
                DrawLine(
                    pixels,
                    width,
                    height,
                    x,
                    0,
                    width / 2,
                    floorTop,
                    1.0f,
                    new Color(
                        0.30f,
                        0.24f,
                        0.40f,
                        0.30f));
            }

            // Subtle violet atmospheric haze.
            BlendDisk(
                pixels,
                width,
                height,
                width * 0.50f,
                height * 0.44f,
                width * 0.40f,
                new Color(
                    violet.r,
                    violet.g,
                    violet.b,
                    0.10f));
        }

        private static void BlendDisk(
            Color[] pixels,
            int width,
            int height,
            float cx,
            float cy,
            float radius,
            Color color)
        {
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
                        color.a *
                        Mathf.Pow(
                            1f -
                            (distance /
                             radius),
                            1.8f);

                    int index =
                        (y * width) + x;

                    pixels[index] =
                        Color.Lerp(
                            pixels[index],
                            new Color(
                                color.r,
                                color.g,
                                color.b,
                                1f),
                            alpha);
                }
            }
        }

        private static void DrawTriangle(
            Color[] pixels,
            int width,
            int height,
            int centerX,
            int topY,
            int halfWidth,
            int triangleHeight,
            Color color)
        {
            int bottomY =
                topY -
                triangleHeight;

            for (int y =
                     Mathf.Max(
                         0,
                         bottomY);
                 y <=
                 Mathf.Min(
                     height - 1,
                     topY);
                 y++)
            {
                float t =
                    triangleHeight <= 0
                        ? 1f
                        : (y - bottomY) /
                          (float)triangleHeight;

                int rowHalfWidth =
                    Mathf.RoundToInt(
                        halfWidth * t);

                int startX =
                    Mathf.Max(
                        0,
                        centerX -
                        rowHalfWidth);

                int endX =
                    Mathf.Min(
                        width - 1,
                        centerX +
                        rowHalfWidth);

                for (int x = startX;
                     x <= endX;
                     x++)
                {
                    pixels[
                        (y * width) + x] =
                        color;
                }
            }
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
