using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Balyrinth
{
    public static class Utilities
    {
        public enum LabyShape
        {
            Rectangle,
            HoneyComb,
            Octogonized,
            Hypermaze,
            Torus,
            Sphere,
        }

        public const float VIEW_SCALE = 2.0f;
        public static bool isInSight(float pMinimalViewvingAngle, float pMaximalViewingAngle, float pMinimalObjectAngle, float pMaximalObjectAngle, out float pMinimalOverlapAngle, out float pMaximalOverlapAngle)
        {
            pMinimalViewvingAngle = Mathf.Repeat(pMinimalViewvingAngle, 360);
            pMaximalViewingAngle = Mathf.Repeat(pMaximalViewingAngle, 360);

            pMinimalObjectAngle = Mathf.Repeat(pMinimalObjectAngle, 360);
            pMaximalObjectAngle = Mathf.Repeat(pMaximalObjectAngle, 360);

            if (pMaximalViewingAngle < pMinimalViewvingAngle)
            {
                pMaximalViewingAngle += 360;
            }

            if (pMaximalObjectAngle < pMinimalObjectAngle)
            {
                pMaximalObjectAngle += 360;
            }

            if (pMinimalViewvingAngle < pMinimalObjectAngle && pMaximalViewingAngle < pMinimalObjectAngle)
            {
                pMinimalViewvingAngle += 360;
                pMaximalViewingAngle += 360;
            }

            if (pMinimalObjectAngle < pMinimalViewvingAngle && pMaximalObjectAngle < pMinimalViewvingAngle)
            {
                pMinimalObjectAngle += 360;
                pMaximalObjectAngle += 360;
            }

            pMinimalOverlapAngle = Mathf.Max(pMinimalViewvingAngle, pMinimalObjectAngle);
            pMaximalOverlapAngle = Mathf.Min(pMaximalViewingAngle, pMaximalObjectAngle);

            return (pMaximalOverlapAngle >= pMinimalOverlapAngle);
        }

        //this method checks if direct angle is smaller than indirect angle (in degrees) - concave
        public static bool isMinimalAngleIsDirect(float pAngle1, float pAngle2)
        {
            pAngle1 = Mathf.Repeat(pAngle1, 360);
            pAngle2 = Mathf.Repeat(pAngle2, 360);

            if (pAngle2 < pAngle1)
            {
                pAngle2 += 360;
            }

            return (pAngle2 - pAngle1) < 180;
        }
    }
}
