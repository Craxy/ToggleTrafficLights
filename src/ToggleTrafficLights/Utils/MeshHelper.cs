using System;
using System.Linq;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public static class MeshHelper
    {
        public static Mesh CreateCylinder(int numberOfSides)
        {
            var mesh = new Mesh();

            //BuildingManager.CreateHighlightMesh

            //top,bottom,top,bottom,top,bottom,...
            var vertices = Enumerable.Range(0, numberOfSides)
                .Select(i => (2.0f*Mathf.PI)/numberOfSides*i)
                .Select(
                    rad =>
                        new[]
                        {
                            new Vector3(Mathf.Cos(rad)*0.5f, 1, Mathf.Sin(rad)*0.5f),
                            new Vector3(Mathf.Cos(rad)*0.5f, 0, Mathf.Sin(rad)*0.5f),
                        })
                .SelectMany(v => v)
                .ToArray();

            //no caps
            var triangles = Enumerable.Range(0, numberOfSides)
                .Select(i =>
                {
                    var vi = 2*i; //vertices index
                    var t1 = vi;
                    var b1 = vi + 1;
                    var t2 = (vi + 2)%vertices.Length;
                    var b2 = (vi + 3)%vertices.Length;

                    return new []
                    {
                        t1, b2, b1,
                        t1, t2, b2,
                    };
                })
                .SelectMany(t => t)
                .ToArray();


            mesh.vertices = vertices;
            mesh.triangles = triangles;

            return mesh;
        }


        public static Mesh CreateCircle(int numberOfSides)
        {




            throw new NotImplementedException();
        }
    }
}