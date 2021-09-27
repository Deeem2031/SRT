namespace SRT.GameLogic
{
    public class InfantryUnit : BaseUnit, ICanMove, ISpawnable
    {
        public InfantryUnit(Tile location) : base(location)
        {
            _strength = 1;
        }

        private int _strength;
        private const int MaxDistance = 5;
        private const int CooldownAfterMove = 5 * Settings.TicksPerSecond;
        private int _cooldown;

        public bool CanMove()
        {
            return _cooldown == 0;
        }

        public bool CanMoveThere(Tile target)
        {
            if (_cooldown > 0)
            {
                return false;
            }
            if (target == Location)
            {
                return false;
            }
            if (!(Location.Unit is HomeUnit) && Location.DistanceTo(target, t => t.PlayerId == this.PlayerId) > MaxDistance)
            {
                return false;
            }
            if (target.PlayerId == this.PlayerId)
            {
                return CanCombine(target.Unit);
            }
            return target.GetNeighbours().Append(target).All(t =>
            {
                if (target.PlayerId == t.PlayerId)
                {
                    return Strength() > (t.Unit?.Strength() ?? 0);
                }
                return true;
            });
        }

        public void MoveTo(Tile target)
        {
            if (Location != null && Location.Unit == this)
            {
                Location.Unit = null;
            }
            if (target.PlayerId != this.PlayerId)
            {
                Capture(target);
            }
            else if (target.Unit != null)
            {
                Combine(target.Unit);
            }
            target.Unit = this;
            Location = target;
            _cooldown = CooldownAfterMove;
        }

        public int Cooldown()
        {
            return _cooldown;
        }

        public override void OnTick()
        {
            if (_cooldown > 0)
            {
                _cooldown -= 1;
                if (_cooldown == 0)
                {
                    //UpdateDarkArea();
                }
            }
        }

        private bool CanCombine(IUnit? with)
        {
            if (with is InfantryUnit inf)
            {
                return _strength + inf.Strength() <= 4;
            }
            if (with is GraveUnit)
            {
                return true;
            }
            return with == null;
        }

        public void Combine(IUnit with)
        {
            _strength += with.Strength();
        }

        public override string Content(double tileSize)
        {
            var gradientId = this.GetHashCode().ToString();
            var cooldownGradient = $"<defs>"
                + $"<linearGradient id=\"{gradientId}\" x1=\"0\" x2=\"0\" y1=\"0\" y2=\"1\">"
                + $"<stop offset=\"0%\" stop-color=\"black\" stop-opacity=\"0\"/>"
                + $"<stop offset=\"{(int)(_cooldown * 100 / CooldownAfterMove)}%\" stop-color=\"black\" stop-opacity=\"0.8\"/>"
                + $"<stop offset=\"100%\" stop-color=\"black\"/>"
                + $"</linearGradient>"
                + $"</defs>";
            var result = string.Empty;
            var fill = "#444444";
            if (_cooldown > 0)
            {
                result += cooldownGradient;
                fill = $"url(#{gradientId})";
            }

            result += $"<text fill=\"{fill}\" font-size=\"120\" dominant-baseline=\"middle\" text-anchor=\"middle\">{Strength().ToString()}</text>";

            return result;
        }

        public override int Strength() => _strength;

        private static readonly double[] UnitCostPerTick = 
            { 2.0 * Settings.GoldRateMultiplier / Settings.TicksPerSecond, 
            6.0 * Settings.GoldRateMultiplier / Settings.TicksPerSecond, 
            18.0 * Settings.GoldRateMultiplier / Settings.TicksPerSecond, 
            54.0 * Settings.GoldRateMultiplier / Settings.TicksPerSecond };

        public override double CostPerTick() => UnitCostPerTick[_strength - 1];

        private void CombineAreas(IEnumerable<Tile> tiles)
        {
            var areas = new List<IEnumerable<Tile>>();
            foreach (var tile in tiles)
            {
                if (areas.Any(a => a.Contains(tile)))
                {
                    continue;
                }
                areas.Add(tile.GetArea());
            }

            var areahomes = areas.Select(a => (a, a.Select(t => t.Unit).OfType<HomeUnit>().FirstOrDefault())).Where(e => !(e.Item2 == null)).ToList();
            areahomes.Sort((e1, e2) => e2.a.Count() - e1.a.Count());
            foreach (var secondary in areahomes.Skip(1))
            {
                if (secondary.Item2 is null)
                {
                    continue;
                }
                areahomes.First().Item2?.AddGold(secondary.Item2.Gold);
                secondary.Item2.Location.Unit = null;
            }
        }

        private void SplitAreas(Tile tileLeft, Tile tileCaptured)
        {
            var newArea = tileLeft.GetArea(t => t != tileCaptured);
            if (newArea.Count() == 1)
            {
                if (newArea.First().Unit is HomeUnit)
                {
                    newArea.First().Unit = null;
                }
                return;
            }
            if (!newArea.Any(t => t.Unit is HomeUnit))
            {
                var tile = newArea.OrderBy(t => t.Unit?.Strength() ?? 0).First();
                tile.Unit = new HomeUnit(tile);
            }
        }

        private void Capture(Tile target)
        {
            CombineAreas(target.GetNeighbours().Where(t => t.PlayerId == this.PlayerId));

            foreach (var neighbour in target.GetNeighbours())
            {
                if (neighbour.PlayerId == target.PlayerId)
                {
                    SplitAreas(neighbour, target);
                }
            }
            target.PlayerId = this.PlayerId;
        }

        public void AbortSpawn()
        {
            (Location.Unit as IHasGold)?.AddGold(10);
        }
    }
}
