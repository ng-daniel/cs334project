using UnityEngine;

namespace WFC
{
    public class WFC : MonoBehaviour
    {
        public GameObject cube;

        public float tileScale = 1;
        public float tileHeight = 0.2f;

        void Start()
        {
            PlaceCube(0, 0);
            PlaceCube(1, 0);
            PlaceCube(2, 0);
            PlaceCube(3, 0);
            PlaceCube(4, 0);
            PlaceCube(2, 1);
        }

        void Update()
        {

        }

        private void PlaceCube(int x, int y)
        {
            GameObject go = GameObject.Instantiate(cube);
            go.transform.position = new Vector3(x * tileScale, 0, y * tileScale);
            go.transform.localScale = new Vector3(tileScale, tileHeight, tileScale);
        }
    }
}