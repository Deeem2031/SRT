namespace SRT.GameLogic
{
    public abstract class BaseUnit: IUnit
    {
        public BaseUnit(Tile location)
        {
            Location = location;
        }
        public Tile Location { get; protected set; }
        public int PlayerId => Location?.PlayerId ?? -1;
        public abstract int Strength();
        public abstract double CostPerTick();
        public abstract string Content(double tileSize);
        public virtual void OnTick() { }
    }
}
