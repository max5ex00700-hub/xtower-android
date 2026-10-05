using System;

namespace XMatch.Core
{
    public sealed class SeededTileSource : ITileSource
    {
        private static readonly TileKind[] DefaultTiles =
        {
            TileKind.Heart,
            TileKind.Lips,
            TileKind.Diamond,
            TileKind.Perfume,
            TileKind.Rose
        };

        private readonly TileKind[] choices;
        private uint state;

        public SeededTileSource(int seed)
            : this(seed, DefaultTiles)
        {
        }

        public SeededTileSource(int seed, params TileKind[] choices)
        {
            if (choices == null)
            {
                throw new ArgumentNullException(nameof(choices));
            }

            if (choices.Length == 0)
            {
                throw new ArgumentException(
                    "At least one non-empty tile kind is required.",
                    nameof(choices));
            }

            this.choices = new TileKind[choices.Length];

            for (int i = 0; i < choices.Length; i++)
            {
                if (choices[i] == TileKind.Empty)
                {
                    throw new ArgumentException(
                        "Tile sources cannot produce TileKind.Empty.",
                        nameof(choices));
                }

                this.choices[i] = choices[i];
            }

            state = unchecked((uint)seed);

            if (state == 0)
            {
                state = 0x6D2B79F5u;
            }
        }

        public TileKind NextTile(BoardPosition target)
        {
            uint value = NextUInt();
            int index = (int)(value % (uint)choices.Length);
            return choices[index];
        }

        private uint NextUInt()
        {
            uint value = state;
            value ^= value << 13;
            value ^= value >> 17;
            value ^= value << 5;
            state = value;
            return value;
        }
    }
}
