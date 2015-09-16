using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public static class Util
    {
        public static Mesh CreateMesh()
        {
            Vector3 p0 = new Vector3(-12, -1.5f, -64);
            Vector3 p1 = new Vector3(12, -1.5f, -64);
            Vector3 p2 = new Vector3(-12, -1.5f, 64);
            Vector3 p3 = new Vector3(12, -1.5f, 64);
            Vector3 p4 = new Vector3(-12, 0, -64);
            Vector3 p5 = new Vector3(12, 0, -64);
            Vector3 p6 = new Vector3(-12, 0, 64);
            Vector3 p7 = new Vector3(12, 0, 64);



            Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.vertices = new Vector3[]{
                p4,p5,p7,p6,
                //p0,p1,p5,p4,
                p1,p3,p7,p5,
                //p3,p2,p6, p7,
                p0,p4,p6, p2,
                //p0,p2,p3, p1
            };
                        mesh.triangles = new int[]{
                0,2,1,
                0,3,2,
                4,6,5,
                4,7,6,
                8,10,9,
                8,11,10,
//                12,14,13,
//                12,15,14,                        
//                16,18,17,
//                16,19,18,  
//                20,22,21,
//                20,23,22 
                        };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();
            return mesh;
        }


    }
}