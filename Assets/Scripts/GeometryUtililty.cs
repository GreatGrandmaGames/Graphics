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
using UnityEngine;

namespace Grandma.Graphics
{
    public static class GeometryUtility
    {
        public enum TopType
        {
            Pointy,
            Flat
        }

        public static Vector2[] PointsAboutEllipse(int numIncrements, TopType topType, float verticalSquash = 1f)
        {
            var corners = new Vector2[numIncrements];

            float aOffset = 0;

            if (topType == TopType.Flat)
            {
                aOffset = 0.5f;
            }

            for (int inc = 0; inc < numIncrements; inc++)
            {
                float theta = ((2f * Mathf.PI) * ((inc + aOffset) / (float)numIncrements));

                corners[inc] = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta) * verticalSquash);
            }

            return corners;
        }

        public static int[] FillSurface(int[] verts, bool clockwise)
        {
            int numVerts = verts.Length;
            int[] triangles = new int[0];

            //Case where we have a square in the previous call
            if (numVerts <= 2)
            {
                return new int[0];
            }

            if (numVerts == 3)
            {

                if (clockwise)
                {
                    return new int[]
                    {
                    verts[0], verts[2], verts[1]
                    };
                }
                else
                {
                    return verts;
                }
            }

            int[] unfinished = new int[((numVerts + 1) / 2)];

            for (int i = 0; i < numVerts; i += 2)
            {
                unfinished[i / 2] = verts[i];

                if (i + 1 < numVerts)
                {
                    int[] triangle;

                    if (clockwise)
                    {
                        triangle = new int[]
                        {
                    verts[i], (verts[(i+ 2) % numVerts]), verts[i + 1]
                        };
                    }
                    else
                    {
                        triangle = new int[]
                        {
                    verts[i], verts[i + 1], (verts[(i + 2) % numVerts])
                        };
                    }

                    triangles = triangles.Concat(triangle).ToArray();
                }
            }


            return triangles.Concat(FillSurface(unfinished, clockwise)).ToArray();
        }
    }
}