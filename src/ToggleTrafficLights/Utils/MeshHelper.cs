//using System;
//using System.Linq;
//using UnityEngine;
//
//namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
//{
//    public static class MeshHelper
//    {
//        public static Mesh CreateCylinder(int numberOfSides)
//        {
//            //http://wiki.unity3d.com/index.php/ProceduralPrimitives#C.23_-_Tube
//            //http://jacksondunstan.com/articles/1924
//            //http://gamedev.stackexchange.com/questions/17214/create-vertices-indices-for-cylinder-with-triangle-strips
//
//            var mesh = new Mesh();
//
//            //BuildingManager.CreateHighlightMesh
//            //(-0.5,0,-0.5) to (0.5,1,0.5)
//
//            //diameter: 1
//            //center: 0
//            //radius: 0.5
//            var r = 0.5f;
//            //height: 1
//            var h = 1f;
//
//
//            //no caps
//            //top,bottom,top,bottom,top,bottom,...
//            var vertices = Enumerable.Range(0, numberOfSides)
//                .Select(i => (2.0f*Mathf.PI)/numberOfSides*i)
//                .Select(
//                    rad =>
//                        new[]
//                        {
//                            new Vector3(Mathf.Cos(rad)*0.5f, 0.5f, Mathf.Sin(rad)*0.5f),
//                            new Vector3(Mathf.Cos(rad)*0.5f, -0.5f, Mathf.Sin(rad)*0.5f)
//                        })
//                .SelectMany(v => v)
//                .ToArray();
//
//            checked
//            {
//                for (int i = 0; i < numberOfSides; i++)
//                {
//                    var vi = 2*i;
//
//                    var t1 = vi;
//                    var b1 = vi + 1;
//                    var t2 = (vi + 2)%vertices.Length;
//                    var b2 = (vi + 3)%vertices.Length;
//
//                    mesh.triangles
//
//                    var tri1 = new Vector3(t1, b2, b1);
//                    var tri2 = new Ve
//
//                }
//            }
//
//            mesh.triangles
//
//
//
//
////            var numberOfCapVertices = numberOfSides + 1;
////            //increase for each step while iteration circle
////            var stepTheta = (Math.PI*2.0)/numberOfSides;
////
////            //verticies
////            var vs = new Vector3[numberOfSides + 2*numberOfCapVertices + 2];
////            //triangles
////            var ts = new int[0];
//
//
////            //vertices index
////            var vi = 0;
////            //triangles index
////            var ti = 0;
////
////            //TODO: sind caps überhaupt notwendig?
////            //top cap
////            for (int i = 0; i < numberOfCapVertices; i++)
////            {
////                var rad = (2.0f*Mathf.PI)/numberOfSides*i;
////                vs[vi++] = new Vector3(Mathf.Cos(rad)*0.5f, 0.5f, Mathf.Sin(rad)*0.5f);
////            }
////            //bottom cap
////            for (int i = 0; i < numberOfCapVertices; i++)
////            {
////                var rad = (2.0f*Mathf.PI)/numberOfSides*i;
////                vs[vi++] = new Vector3(Mathf.Cos(rad)*0.5f, -0.5f, Mathf.Sin(rad)*0.5f);
////            }
////            //sides
////            for (int i = 0; i < numberOfSides; i++)
////            {
////                //every side has 2 triangles
////
////                //fetch points for side
////                var top1 = vs[i];
////                var top2 = vs[(i + 1) % numberOfCapVertices];
////
////                var bot1 = vs[i + numberOfCapVertices];
////                var bot2 = vs[(i + 1   numberOfCapVertices];
////            }
//
//            return mesh;
//        }
//    }
//}