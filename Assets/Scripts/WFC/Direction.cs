namespace WFC
{
    public class Direction
    {
        public const int LEFT = 0;
        public const int DOWN = 1;
        public const int RIGHT = 2;
        public const int UP = 3;

        public const int COUNT = 4;

        public static int Opposite(int d)
        {
            return d switch
            {
                LEFT => RIGHT,
                DOWN => UP,
                RIGHT => LEFT,
                UP => DOWN,
                _ => -1
            };
        }

        public static int OffsetX(int d)
        {
            return d switch
            {
                LEFT => -1,
                RIGHT => 1,
                _ => 0
            };
        }

        public static int OffsetY(int d)
        {
            return d switch
            {
                DOWN => -1,
                UP => 1,
                _ => 0
            };
        }
    }
}