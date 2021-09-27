namespace SRT.GameLogic
{
    public interface IUnit
    {
        Tile Location { get; }
        int Strength();
        double CostPerTick();
        string Content(double tileSize);
        void OnTick();
    }

    public interface ICanMove
    {
        bool CanMove();
        bool CanMoveThere(Tile target);
        void MoveTo(Tile target);
        int Cooldown();
    }

    public interface IHasGold
    {
        double Gold { get; }
        void AddGold(double plus);
    }

    public interface ICanSpawn
    {
        bool CanSpawn();
        IUnit? SpawnUnit();
    }

    public interface ISpawnable
    {
        void AbortSpawn();
    }
}
