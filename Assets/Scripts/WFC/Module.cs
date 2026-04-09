using UnityEngine.Assertions;

namespace WFC
{
    public class Module
    {
        public int id;

        // Whether each of the four edges is solid
        public readonly bool[] edges;

        // Relative rarity of the module to others
        public float weight;

        public string prefabName;
        public float angle;

        public Module(int id, bool[] edges, float weight, string prefabName, float angle)
        {
            Assert.AreEqual(Direction.COUNT, edges.Length);

            this.id = id;
            this.edges = (bool[])edges.Clone();
            this.weight = weight;
            this.prefabName = prefabName;
            this.angle = angle;
        }
    }
}