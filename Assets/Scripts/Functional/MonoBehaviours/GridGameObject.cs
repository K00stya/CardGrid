using UnityEngine;

namespace CardGrid
{
    public class GridGameObject : MonoBehaviour
    {
        public float OffsetX = 0.2f;
        public float OffsetZ = 0.2f;
        public int SizeX = 6;
        public int SizeZ = 6;

        public Vector3 CubeSize = new Vector3(1,1,1);    

        public Vector3 GetCellSpacePosition(Vector2 fieldPosition)
        {
            return transform.position + 
                   new Vector3(fieldPosition.x + fieldPosition.x * OffsetX, 0, -(fieldPosition.y + fieldPosition.y * OffsetZ));
        }

        public Vector3 GetSpawnPosition(int x)
        {
            return transform.position + new Vector3(x + x * OffsetX, 0, -(-3 * OffsetZ));
        }

        void OnDrawGizmos()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    Gizmos.DrawCube(GetCellSpacePosition(new Vector2(x,z)), CubeSize);
                }
            }
        }
    }
}
