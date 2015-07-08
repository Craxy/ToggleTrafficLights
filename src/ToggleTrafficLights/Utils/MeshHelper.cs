using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public static class MeshHelper
    {
        public static Mesh CreateCylinder3(float height, float radius, int numberOfSides)
        {
            //todo: bottom cap is useless

            //y: from 0 to heigh
            //x,z: +/- radius with center at 0,0
            //ergo bottom center at 0,0,0; top center at 0,height,0

            var step = (2.0f*Mathf.PI)/numberOfSides;

            //side vertices
            var sideVertices = Enumerable.Range(0, numberOfSides)
                .Select(i => step*i)
                .Select(rad =>
                    new[]
                    {
                        //top
                        new Vector3(Mathf.Cos(rad)*radius, height, Mathf.Sin(rad)*radius),
                        //bottom
                        new Vector3(Mathf.Cos(rad)*radius, 0.0f, Mathf.Sin(rad)*radius),
                    }
                )
                .SelectMany(v => v);
            //cap vertices
            var capVertices = new[]
            {
                new Vector3(0.0f, height, 0.0f),
                new Vector3(0.0f, 0.0f, 0.0f),

            };
            //all vertices
            var vertices = sideVertices.Concat(capVertices).ToArray();

            var sideVerticesLength = vertices.Length - capVertices.Length;


            //side triangles
            var sideTriangles = Enumerable.Range(0, numberOfSides)
                .Select(i =>
                {
                    //two rows for two triangles
                    var startIdx = 2*i;
                    var row1Top = startIdx;
                    var row1Bot = startIdx + 1;
                    // mod for circular
                    var row2Top = (startIdx + 2)%sideVerticesLength;
                    var row2Bot = (startIdx + 3)%sideVerticesLength;

//                    return new[]
//                    {
//                        row1Top, row1Bot, row2Top,
//                        row1Bot, row2Bot, row2Top,
//                    };
                    return new[]
                    {
                        row1Top, row2Top, row1Bot,
                        row1Bot, row2Top, row2Bot,
                    };
                })
                .SelectMany(ts => ts);
            //cap triangles
            var topCenterIdx = vertices.Length - 2;
            var botCenterIdx = vertices.Length - 1;
            var capTriangles = Enumerable.Range(0, numberOfSides)
                .Select(i =>
                {
                    //two rows for two triangles
                    var startIdx = 2*i;
                    var row1Top = startIdx;
                    var row1Bot = startIdx + 1;
                    // mod for circular
                    var row2Top = (startIdx + 2)%sideVerticesLength;
                    var row2Bot = (startIdx + 3)%sideVerticesLength;

                    return new[]
                    {
                        //top triangles
                        topCenterIdx, row2Top, row1Top,
                        //bottom triangle
                        botCenterIdx, row2Bot, row1Bot,
                    };
                })
                .SelectMany(ts => ts);

            var triangles = sideTriangles.Concat(capTriangles).ToArray();

            return new Mesh
            {
                name = "CylinderMesh",
                vertices = vertices,
                triangles = triangles,
            };
        }

        public static Mesh CreateCylinder(float height, float radius, int numberOfSides)
        {
            //y: from 0 to heigh
            //x,z: +/- radius with center at 0,0
            //ergo bottom center at 0,0,0; top center at 0,height,0

            var mesh = new Mesh();
            mesh.name = "CylinderMesh";

            //top,bottom,top,bottom,top,bottom,...
            var vertices = Enumerable.Range(0, numberOfSides)
                .Select(i => (2.0f*Mathf.PI)/numberOfSides*i)
                .Select(
                    rad =>
                        new[]
                        {
                            new Vector3(Mathf.Cos(rad)*radius, height, Mathf.Sin(rad)*radius),
                            new Vector3(Mathf.Cos(rad)*radius, 0.0f, Mathf.Sin(rad)*radius),
                        })
                .SelectMany(v => v)
                // caps (top, bottom not needed)
                .Concat(Enumerable.Repeat(new Vector3(0.0f, height, 0.0f), 1))
                .ToArray();

            //no caps
            var sides = Enumerable.Range(0, numberOfSides)
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

            //caps (top)
            var ci = vertices.Length - 1;
            var tops = Enumerable.Range(0, numberOfSides)
                        .Select(i =>
                        {
                            var vi = 2*i; //vertices index
                            var t1 = vi;
                            var t2 = (vi + 2)%vertices.Length;
                            
                            return new[]
                            {
                                ci, t1, t2,
                            };
                        })
                        .SelectMany(t => t)
                        .ToArray();

            var triangles = sides.Concat(tops).ToArray();

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}