namespace SRT.GameLogic
{
    public struct AxialCoords
    {
        public double Q;
        public double R;

        public AxialCoords(double q, double r)
        {
            Q = q;
            R = r;
        }

        public static implicit operator AxialCoords((double Q, double R) coords) => new AxialCoords(coords.Q, coords.R);
        public static implicit operator (double Q, double R)(AxialCoords coords) => (coords.Q, coords.R);

        public Point ToPoint() => new Point((int)Math.Round(Q), (int)Math.Round(R));
    }
}
