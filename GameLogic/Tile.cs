namespace SRT.GameLogic
{
    public class Tile
    {
        public Tile(Map map)
        {
            Map = map;
        }

        public Map Map { get; }
        public int Q;
        public int R;
        public int PlayerId;
        public IUnit? Unit;

        public int X(double tileSize) => (int)(tileSize * 3.0 / 2.0 * Q);
        public int Y(double tileSize) => (int)(tileSize * (Math.Sqrt(3.0) / 2.0 * Q + Math.Sqrt(3.0) * R));

        public bool IsHighlighted = false;
        public bool IsDarkend = false;

        private static readonly string[] Colors = { "#BBFFBB", "#BBBBFF", "#FFBBBB" };
        private static readonly string[] HighlightColors = { "#EEFFEE", "#EEEEFF", "#FFEEEE" };
        private static readonly string[] DarkendColors = { "#557755", "#555577", "#775555" };
        private static readonly string[] SecondaryColors = { "#88FF88", "#8888FF", "#FF8888" };

        public string PrimaryColor => IsDarkend ? DarkendColors[PlayerId] : IsHighlighted ? HighlightColors[PlayerId] : Colors[PlayerId];
        public string SecondaryColor => SecondaryColors[PlayerId];

        public bool CanGrab()
        {
            if (Unit is ICanMove canMove && canMove.CanMove())
            {
                return true;
            }
            if (Unit is ICanSpawn canSpawn && canSpawn.CanSpawn())
            {
                return true;
            }
            return false;
        }

        public string Style()
        {
            if (CanGrab())
            {
                return "cursor:grab";
            }
            else
            {
                return "";
            }
        }

        public IEnumerable<Tile> GetNeighbours()
        {
            foreach (var tile in Map.Tiles)
            {
                if (tile.Q == this.Q)
                {
                    if (Math.Abs(tile.R - this.R) == 1)
                    {
                        yield return tile;
                    }
                }
                else if (tile.Q == this.Q + 1)
                {
                    if (tile.R == this.R || tile.R == this.R - 1)
                    {
                        yield return tile;
                    }
                }
                else if (tile.Q == this.Q - 1)
                {
                    if (tile.R == this.R || tile.R == this.R + 1)
                    {
                        yield return tile;
                    }
                }
            }
        }

        public IEnumerable<Tile> GetArea()
        {
            return GetArea(_ => true);
        }

        public IEnumerable<Tile> GetArea(Func<Tile, bool> canVisit)
        {
            yield return this;
            var visited = new List<Tile>();
            var fringe = new List<Tile>();
            var newFringe = new List<Tile>();
            fringe.Add(this);
            do
            {
                newFringe.Clear();
                foreach (var tile in fringe)
                {
                    foreach (var n in tile.GetNeighbours())
                    {
                        if (visited.Contains(n) || fringe.Contains(n) || newFringe.Contains(n))
                        {
                            continue;
                        }
                        if (n.PlayerId != PlayerId || !canVisit(n))
                        {
                            continue;
                        }
                        newFringe.Add(n);
                        yield return n;
                    }
                    visited.Add(tile);
                }
                fringe.Clear();
                fringe.AddRange(newFringe);
            } while (newFringe.Any());
        }

        public int DistanceTo(Tile target, Func<Tile, bool> canVisit)
        {
            if (target == this)
            {
                return 0;
            }
            var distance = 0;
            var visited = new List<Tile>();
            var fringe = new List<Tile>();
            var newFringe = new List<Tile>();
            fringe.Add(this);
            do
            {
                distance++;
                newFringe.Clear();
                foreach (var tile in fringe)
                {
                    foreach (var n in tile.GetNeighbours())
                    {
                        if (visited.Contains(n) || fringe.Contains(n))
                        {
                            continue;
                        }
                        if (n == target)
                        {
                            return distance;
                        }
                        if (!canVisit(n))
                        {
                            continue;
                        }
                        newFringe.Add(n);
                    }
                    visited.Add(tile);
                }
                fringe.Clear();
                fringe.AddRange(newFringe);
            } while (newFringe.Any());
            return int.MaxValue;
        }
    }
}
