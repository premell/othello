    public class GameRequest
    {
        public int TimeLimitSeconds { get; set; }
        public int TimeIncrementSeconds { get; set; }
    }

    public class MoveRequest
    {
        public Color PlayerColor { get; set; }
        public int TargetSquare { get; set; }
    }