namespace XMatch.Core
{
    public interface ITileSource
    {
        TileKind NextTile(BoardPosition target);
    }
}
