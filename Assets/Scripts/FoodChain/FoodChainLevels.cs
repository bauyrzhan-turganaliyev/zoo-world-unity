namespace ZooWorld.FoodChain
{
    public static class FoodChainLevels
    {
        public const int Prey = 0;

        public static bool IsPredator(int level) => level > Prey;
    }
}
