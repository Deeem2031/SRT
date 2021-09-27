namespace SRT.GameLogic
{
    public class HomeUnit : BaseUnit, IHasGold, ICanSpawn
    {
        public HomeUnit(Tile location) : base(location)
        {
        }

        public override double CostPerTick() => 0;

        public override string Content(double tileSize) => $"<image href=\"house.svg\" x=\"{(int)(-tileSize / 2)}\" y=\"{(int)(-tileSize / 2)}\" width=\"{(int)(tileSize)}\" height=\"{(int)(tileSize)}\" />";

        public override int Strength() => 1;

        public double Gold { get; private set; } = 0;

        public void AddGold(double plus)
        {
            Gold += plus;
            if (Gold < 0)
            {
                KillAllUnits();
                Gold = 0;
            }
        }

        private void KillAllUnits()
        {
            foreach (var tile in Location.GetArea())
            {
                if (tile.Unit?.CostPerTick() > 0)
                {
                    tile.Unit = new GraveUnit(tile);
                }
            }
        }

        public bool CanSpawn() => Gold >= 10;

        public IUnit? SpawnUnit()
        {
            Gold -= 10;
            return new InfantryUnit(Location);
        }

        public override void OnTick()
        {
            AddGold(Location.GetArea().Sum(t =>
            {
                var gain = 1.0 * Settings.GoldRateMultiplier / Settings.TicksPerSecond;
                gain -= t.Unit?.CostPerTick() ?? 0.0;
                return gain;
            }));
        }
    }
}
