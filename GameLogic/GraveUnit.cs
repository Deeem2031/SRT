namespace SRT.GameLogic
{
    public class GraveUnit: BaseUnit
    {
        public GraveUnit(Tile location) : base(location) { }

        public override string Content(double tileSize) => $"<text fill=\"#444444\" font-size=\"120\" dominant-baseline=\"middle\" text-anchor=\"middle\">✟</text>";

        public override double CostPerTick() => 0;

        public override int Strength() => 0;
    }
}
