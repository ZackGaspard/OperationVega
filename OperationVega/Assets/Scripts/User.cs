
namespace Assets.Scripts
{
    /// <summary>
    /// The user class. Represents the user playing the game.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The max count of units.
        /// This references the max amount of each given unit the user is allowed on the field.
        /// </summary>
        public static readonly int MaxCountOfUnits = 30;

        /// <summary>
        /// The food count. This represents the total number of food
        /// in users inventory
        /// </summary>
        public static int FoodCount;

        /// <summary>
        /// The cooked food count. This represents the total number of cooked food
        /// in users inventory
        /// </summary>
        public static int CookedFoodCount;

        /// <summary>
        /// The minerals count. This represents the total number of minerals
        /// in users inventory
        /// </summary>
        public static int MineralsCount;

        /// <summary>
        /// The steel count. This represents the total number of steel
        /// in users inventory
        /// </summary>
        public static uint SteelCount;

        /// <summary>
        /// The fuel count. This represents the total number of fuel
        /// in users inventory
        /// </summary>
        public static uint FuelCount;

        /// <summary>
        /// The gas count. This represents the total number of gas
        /// in users inventory
        /// </summary>
        public static int GasCount;

        /// <summary>
        /// The harvester count.
        /// Reference to the number of harvesters on the field.
        /// </summary>
        public static int HarvesterCount;

        /// <summary>
        /// The miner count.
        /// Reference to the number of miners on the field.
        /// </summary>
        public static int MinerCount;

        /// <summary>
        /// The extractor count.
        /// Reference to the number of extractors on the field.
        /// </summary>
        public static int ExtractorCount;

        /// <summary>
        /// The upgrade points reference.
        /// Reference to the number of points the user can use to upgrade a units stats.
        /// </summary>
        public static int UpgradePoints;
    }
}
