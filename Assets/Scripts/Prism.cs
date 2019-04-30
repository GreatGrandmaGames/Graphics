//	Copyright (c) 2018 Elliot Winch
//  From https://github.com/elliot-winch/ProcJam2018
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Grandma.Graphics
{
    public class Prism : Renderable
    {

        public enum FaceType
        {
            Flat,
            Round
        }


        [Header("Geometry")]
        [Range(3, 60)]
        [Tooltip("The number of vertices of the end face")]
        public int vertCount = 6;
        [Tooltip("The offset applied to the bottom end face")]
        public Vector2 shear = Vector2.zero;
        [Tooltip("The factor by which the radius is multiplied when creating the bottom end face")]
        public Vector2 frustumScale = Vector2.one;
        [Tooltip("The incline of the bottom end face")]
        public float truncationAngle = 0f;
        [Tooltip("The ratio between the semi-major axis and the semi-minor axis of the end faces")]
        public float verticalSquash = 1f;

        [Header("Shape")]
        [Tooltip("The round face type produces one smoothed side face; the flat face type will produced many, defined faces")]
        public FaceType faceType = FaceType.Flat;
        [Tooltip("The end face will have a pointy top or a flat top?")]
        public GeometryUtility.TopType topType = GeometryUtility.TopType.Flat;
        public float thickness = 0.5f;
        [Tooltip("The distance at which vertices are positioned from the center of the end face")]
        public float radius = 1f;

        [Header("Prism Options")]
        [Tooltip("Don’t render the top end face")]
        public bool hideTop;
        [Tooltip("Don’t render the bottom end face")]
        public bool hideBottom;
        [Tooltip("Render the inside faces of the prism?")]
        public bool showInsides;
        [Tooltip("Don’t render the ith side faces")]
        public List<int> hiddenSideFaces;



        protected override Mesh GenerateMesh()
        {
            int normalsPerVert = 0;

            //When we have flat faces, we need three normals per point
            if (faceType == FaceType.Flat)
            {
                normalsPerVert = 3;
            }
            //When we have round faces, we only need two normals per point
            else if (faceType == FaceType.Round)
            {
                normalsPerVert = 2;
            }

            //The number of geometric vertices for a prism is 2 * vertCount. We multiple by vertsPerPoint for normals
            Vector3[] vertices = new Vector3[2 * vertCount * normalsPerVert];

            var corners = GeometryUtility.PointsAboutEllipse(vertCount, topType, verticalSquash);

            for (int i = 0; i < vertCount; i++)
            {
                for (int j = 0; j < normalsPerVert; j++)
                {
                    int k1 = (i * normalsPerVert) + j;
                    int k2 = k1 + (vertCount * normalsPerVert);

                    vertices[k1] = new Vector3((corners[i].x) * radius, 0f, corners[i].y * radius);

                    float bottomVertHeight = Mathf.Tan(truncationAngle) * corners[i].x * radius;

                    vertices[k2] = new Vector3((corners[i].x + shear.x) * frustumScale.x * radius, -thickness - bottomVertHeight, (corners[i].y + shear.y) * frustumScale.y * radius);
                }
            }

            List<int> triangles = new List<int>();

            if (hideTop == false)
            {
                triangles.AddRange(GeometryUtility.FillSurface(Enumerable.Range(0, (vertices.Length / 2) - 1).Where(x => (x % normalsPerVert == 0)).ToArray(), true));
            }

            if(hideBottom == false)
            {
                triangles.AddRange(GeometryUtility.FillSurface(Enumerable.Range((vertices.Length / 2), (vertices.Length / 2) - 1).Where(x => (x % normalsPerVert == 0)).ToArray(), false));
            }

            Vector3[] normals = new Vector3[vertices.Length];

            for (int i = 0; i < vertCount; i++)
            {
                int cornerVertIndex = i * normalsPerVert;
                int bottomFaceEquivalent = (vertCount * normalsPerVert);

                normals[cornerVertIndex] = Vector3.up;
                normals[cornerVertIndex + bottomFaceEquivalent] = new Vector3(-Mathf.Sin(truncationAngle), -Mathf.Cos(truncationAngle), 0).normalized;

                int topLeft = cornerVertIndex + 1;
                int bottomLeft = topLeft + bottomFaceEquivalent;

                //Couldn't see a meaningful function for this
                int normalSkip = (normalsPerVert == 3) ? 4 : 2;

                int topRight = (topLeft + normalSkip) % (vertices.Length / 2);
                int bottomRight = topRight + bottomFaceEquivalent;

                int[] sideFace = new int[]
                {
                topLeft,
                bottomLeft,
                bottomRight,
                topRight
                };

                //if we have elected to hide this face, don't create triangles
                //else, do
                if (hiddenSideFaces != null && hiddenSideFaces.Contains(i) == false)
                {
                    triangles.AddRange(GeometryUtility.FillSurface(sideFace, true));
                }

                if (faceType == FaceType.Flat)
                {
                    float angle = (2f * Mathf.PI) * (i / (float)vertCount) + (Mathf.PI / vertCount) + (topType == GeometryUtility.TopType.Flat ? (Mathf.PI / vertCount) : 0f);

                    var circlePoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    Vector3 normalVec = new Vector3(circlePoint.x, 0f, circlePoint.y).normalized;

                    foreach (int s in sideFace)
                    {
                        normals[s] = normalVec;
                    }

                }
                else if (faceType == FaceType.Round)
                {
                    //If we want rounded edges, use the vector for this corner
                    //The line that travels through the center of the prism face to the corner
                    Vector3 normalVec = new Vector3(corners[i].x, 0f, corners[i].y).normalized;

                    normals[topLeft] = normalVec;
                    normals[bottomLeft] = normalVec;
                }
            }

            if (showInsides)
            {
                int[] insideTriangles = new int[triangles.Count];

                for (int i = 0; i < triangles.Count; i += 3)
                {
                    insideTriangles[i] = triangles[i];
                    insideTriangles[i + 1] = triangles[i + 2];
                    insideTriangles[i + 2] = triangles[i + 1];
                }

                triangles.AddRange(insideTriangles);
            }

            Mesh mesh = new Mesh
            {
                name = "Prism (" + vertCount + ")",
                vertices = vertices,
                normals = normals,
                triangles = triangles.ToArray()
            };

            return mesh;
        }

    }
}