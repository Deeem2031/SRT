namespace SRT.GameLogic
{
    public class Map
    {
        private static Random rng = new Random();

        public List<Tile> Tiles = new List<Tile>();

        public void Add(Tile tile) => Tiles.Add(tile);

        public void AddHomes()
        {
            foreach (var tile in Tiles.Where(t => t.GetNeighbours().Any(n => n.PlayerId == t.PlayerId)).OrderBy(e => rng.Next()))
            {
                if (tile.GetArea().Where(t => t.PlayerId == tile.PlayerId).Any(t => t.Unit is HomeUnit))
                {
                    continue;
                }
                var newHome = new HomeUnit(tile);
                newHome.AddGold(10);
                tile.Unit = newHome;
            }
        }

        public void GenerateRandom(double tileSize, int minX, int maxX, int minY, int maxY)
        {
            for (var q = 0; q < 10; q++)
            {
                for (var r = -10; r < 10; r++)
                {
                    var tile = new Tile(this)
                    {
                        Q = q,
                        R = r,
                        PlayerId = rng.Next(0, 3)
                    };

                    if (tile.X(tileSize) > minX && tile.X(tileSize) < maxX && tile.Y(tileSize) > minY && tile.Y(tileSize) < maxY)
                    {
                        this.Add(tile);
                    }
                }
            }
            AddHomes();
        }
    }

}
