using UnityEngine.Assertions;

namespace WFC
{
    public class Module
    {
        public int id;

        // Whether each of the four edges is solid
        public readonly bool[] edges;

        public string prefabName;
        public float angle;

        public Module(int id, bool[] edges, string prefabName, float angle)
        {
            Assert.AreEqual(Direction.COUNT, edges.Length);

            this.id = id;
            this.edges = (bool[])edges.Clone();
            this.prefabName = prefabName;
            this.angle = angle;
        }
    }
}