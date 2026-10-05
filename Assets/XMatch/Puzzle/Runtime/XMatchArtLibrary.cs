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

        public static void Warmup()
        {
            EnsureAtlases();

            GetTileSprite(TileKind.Heart, PowerUpKind.None);
            GetTileSprite(TileKind.Lips, PowerUpKind.None);
            GetTileSprite(TileKind.Diamond, PowerUpKind.None);
            GetTileSprite(TileKind.Perfume, PowerUpKind.None);
            GetTileSprite(TileKind.Rose, PowerUpKind.None);

            for (int i = 1; i <= 5; i++)
            {
                GetTileSprite(
                    TileKind.Heart,
                    (PowerUpKind)i);
            }

            for (int i = 0; i < 10; i++)
            {
                GetVfxSprite((XMatchVfxKind)i);
            }
        }

        public static Sprite GetTileSprite(
            TileKind kind,
            PowerUpKind powerUp)
        {
            EnsureAtlases();

            if (tilesAtlas == null)
            {
                return null;
            }

            int index = TileAtlasIndex(kind, powerUp);

            if (index < 0)
            {
                return null;
            }

            Sprite sprite;

            if (tileSprites.TryGetValue(index, out sprite))
            {
                return sprite;
            }

            sprite = CreateAtlasSprite(
                tilesAtlas,
                index);

            tileSprites[index] = sprite;
            return sprite;
        }

        public static Sprite GetVfxSprite(
            XMatchVfxKind kind)
        {
            EnsureAtlases();

            if (vfxAtlas == null)
            {
                return null;
            }

            int index = (int)kind;

            Sprite sprite;

            if (vfxSprites.TryGetValue(index, out sprite))
            {
                return sprite;
            }

            sprite = CreateAtlasSprite(
                vfxAtlas,
                index);

            vfxSprites[index] = sprite;
            return sprite;
        }

        private static void EnsureAtlases()
        {
            if (tilesAtlas == null)
            {
                tilesAtlas =
                    Resources.Load<Texture2D>(
                        "XMatch/Art/TilesAtlas");
            }

            if (vfxAtlas == null)
            {
                vfxAtlas =
                    Resources.Load<Texture2D>(
                        "XMatch/Art/VfxAtlas");
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
            int column = index % Columns;
            int topRow = index / Columns;
            int unityRow = (Rows - 1) - topRow;

            Rect rect = new Rect(
                column * CellSize,
                unityRow * CellSize,
                CellSize,
                CellSize);

            Sprite sprite = Sprite.Create(
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
