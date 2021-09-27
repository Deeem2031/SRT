namespace SRT.GameLogic
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Point((int X, int Y) coords) => new Point(coords.X, coords.Y);
        public static implicit operator (int X, int Y)(Point coords) => (coords.X, coords.Y);
    }
}
