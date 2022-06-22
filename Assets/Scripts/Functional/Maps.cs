namespace CardGrid
{
    //For a valid center map can be 1X1 or 3X3 or 5X5 only
    public  class DamageMaps
    {
        public static int[,] Claws =
        {
            {0, 0, 0},
            {1, 1, 1},
            {0, 0, 0}
        };

        public static int[,] Hammer =
        {
            {0, 0, 1, 0, 0},
            {0, 1, 1, 1, 0},
            {1, 1, 1, 1, 1},
            {0, 1, 1, 1, 0},
            {0, 0, 1, 0, 0}
        };

        public static int[,] Book =
        {
            {1, 0, 1},
            {0, 0, 0},
            {1, 0, 1}
        };
        
        public static int[,] Test =
        {
            {1}
        };
        
        public static int[,] Ghost =
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
        
        public static int[,] Demons =
        {
            {0, 1, 0},
            {1, 0, 1},
            {0, 1, 0}
        };
    }
    
    public class PushMaps
    {
        
    }

    public class PickUpItemsMaps
    {
        
    }
}