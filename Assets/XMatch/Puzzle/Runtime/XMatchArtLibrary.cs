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

        private static readonly Dictionary<int, Sprite>
            tileSprites = new Dictionary<int, Sprite>();

        private static readonly Dictionary<int, Sprite>
            vfxSprites = new Dictionary<int, Sprite>();

        private static Texture2D tilesAtlas;
        private static Texture2D vfxAtlas;
        private static bool attemptedLoad;

        public static string LoadError { get; private set; }

        public static void Warmup()
        {
            try
            {
                EnsureAtlases();
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
                    TileAtlasIndex(kind, powerUp);

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
                    CreateAtlasSprite(
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
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

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
            int column = index % Columns;
            int topRow = index / Columns;
            int unityRow =
                (Rows - 1) - topRow;

            Rect rect = new Rect(
                column * CellSize,
                unityRow * CellSize,
                CellSize,
                CellSize);

            Sprite sprite =
                Sprite.Create(
                    texture,
                    rect,
                    new Vector2(0.5f, 0.5f),
                    CellSize,
                    0,
                    SpriteMeshType.FullRect);

            sprite.name =
                texture.name + "_" + index;

            return sprite;
        }
    }
}
