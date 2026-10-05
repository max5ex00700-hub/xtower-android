using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public sealed class CascadeStep
    {
        public CascadeStep(
            int chainNumber,
            IEnumerable<ClearedTile> cleared,
            IEnumerable<TileMove> moved,
            IEnumerable<TileSpawn> spawned,
            PowerUpCreation? createdPowerUp = null)
        {
            if (chainNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chainNumber));
            }

            if (cleared == null)
            {
                throw new ArgumentNullException(nameof(cleared));
            }

            if (moved == null)
            {
                throw new ArgumentNullException(nameof(moved));
            }

            if (spawned == null)
            {
                throw new ArgumentNullException(nameof(spawned));
            }

            ChainNumber = chainNumber;
            Cleared = new List<ClearedTile>(cleared).AsReadOnly();
            Moved = new List<TileMove>(moved).AsReadOnly();
            Spawned = new List<TileSpawn>(spawned).AsReadOnly();
            CreatedPowerUp = createdPowerUp;
        }

        public int ChainNumber { get; }
        public IReadOnlyList<ClearedTile> Cleared { get; }
        public IReadOnlyList<TileMove> Moved { get; }
        public IReadOnlyList<TileSpawn> Spawned { get; }
        public PowerUpCreation? CreatedPowerUp { get; }
    }
}
