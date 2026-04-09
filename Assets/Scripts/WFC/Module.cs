using UnityEngine.Assertions;

namespace WFC
{
    public class Module
    {
        public int id;
        public string name;

        // Whether each of the four edges is solid
        public readonly bool[] edges;

        // Relative rarity of the module to others
        public float weight;

        public Module(int id, string name, bool[] edges, float weight)
        {
            Assert.AreEqual(Direction.COUNT, edges.Length);

            this.id = id;
            this.name = name;
            this.edges = (bool[])edges.Clone();
            this.weight = weight;
        }
    }
}