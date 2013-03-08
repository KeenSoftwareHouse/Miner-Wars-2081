#region Using Statements

using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// Computes distance for various shapes
    /// </summary>
    sealed class Distance
    {
        private Distance() { }

        #region PointColDetVoxelTriangleDistanceSq
        /// <summary>
        /// point vs Voxel triangle distancesq
        /// </summary>
        public static float PointColDetVoxelTriangleDistanceSq(out float pfSParam, out float pfTParam, Vector3 rkPoint, MyColDetVoxelTriangle rkTri)
        {
            Vector3 kDiff = rkTri.Origin - rkPoint;
            float fA00 = rkTri.Edge0.LengthSquared();
            float fA01 = Vector3.Dot(rkTri.Edge0, rkTri.Edge1);
            float fA11 = rkTri.Edge1.LengthSquared();
            float fB0 = Vector3.Dot(kDiff, rkTri.Edge0);
            float fB1 = Vector3.Dot(kDiff, rkTri.Edge1);
            float fC = kDiff.LengthSquared();
            float fDet = System.Math.Abs(fA00 * fA11 - fA01 * fA01);
            float fS = fA01 * fB1 - fA11 * fB0;
            float fT = fA01 * fB0 - fA00 * fB1;
            float fSqrDist;

            if (fS + fT <= fDet)
            {
                if (fS < 0.0f)
                {
                    if (fT < 0.0f)  // region 4
                    {
                        if (fB0 < 0.0f)
                        {
                            fT = 0.0f;
                            if (-fB0 >= fA00)
                            {
                                fS = 1.0f;
                                fSqrDist = fA00 + 2.0f * fB0 + fC;
                            }
                            else
                            {
                                fS = -fB0 / fA00;
                                fSqrDist = fB0 * fS + fC;
                            }
                        }
                        else
                        {
                            fS = 0.0f;
                            if (fB1 >= 0.0f)
                            {
                                fT = 0.0f;
                                fSqrDist = fC;
                            }
                            else if (-fB1 >= fA11)
                            {
                                fT = 1.0f;
                                fSqrDist = fA11 + 2.0f * fB1 + fC;
                            }
                            else
                            {
                                fT = -fB1 / fA11;
                                fSqrDist = fB1 * fT + fC;
                            }
                        }
                    }
                    else  // region 3
                    {
                        fS = 0.0f;
                        if (fB1 >= 0.0f)
                        {
                            fT = 0.0f;
                            fSqrDist = fC;
                        }
                        else if (-fB1 >= fA11)
                        {
                            fT = 1.0f;
                            fSqrDist = fA11 + 2.0f * fB1 + fC;
                        }
                        else
                        {
                            fT = -fB1 / fA11;
                            fSqrDist = fB1 * fT + fC;
                        }
                    }
                }
                else if (fT < 0.0f)  // region 5
                {
                    fT = 0.0f;
                    if (fB0 >= 0.0f)
                    {
                        fS = 0.0f;
                        fSqrDist = fC;
                    }
                    else if (-fB0 >= fA00)
                    {
                        fS = 1.0f;
                        fSqrDist = fA00 + 2.0f * fB0 + fC;
                    }
                    else
                    {
                        fS = -fB0 / fA00;
                        fSqrDist = fB0 * fS + fC;
                    }
                }
                else  // region 0
                {
                    // minimum at interior point
                    float fInvDet = 1.0f / fDet;
                    fS *= fInvDet;
                    fT *= fInvDet;
                    fSqrDist = fS * (fA00 * fS + fA01 * fT + 2.0f * fB0) +
                      fT * (fA01 * fS + fA11 * fT + 2.0f * fB1) + fC;
                }
            }
            else
            {
                float fTmp0, fTmp1, fNumer, fDenom;

                if (fS < 0.0f)  // region 2
                {
                    fTmp0 = fA01 + fB0;
                    fTmp1 = fA11 + fB1;
                    if (fTmp1 > fTmp0)
                    {
                        fNumer = fTmp1 - fTmp0;
                        fDenom = fA00 - 2.0f * fA01 + fA11;
                        if (fNumer >= fDenom)
                        {
                            fS = 1.0f;
                            fT = 0.0f;
                            fSqrDist = fA00 + 2.0f * fB0 + fC;
                        }
                        else
                        {
                            fS = fNumer / fDenom;
                            fT = 1.0f - fS;
                            fSqrDist = fS * (fA00 * fS + fA01 * fT + 2.0f * fB0) +
                              fT * (fA01 * fS + fA11 * fT + 2.0f * fB1) + fC;
                        }
                    }
                    else
                    {
                        fS = 0.0f;
                        if (fTmp1 <= 0.0f)
                        {
                            fT = 1.0f;
                            fSqrDist = fA11 + 2.0f * fB1 + fC;
                        }
                        else if (fB1 >= 0.0f)
                        {
                            fT = 0.0f;
                            fSqrDist = fC;
                        }
                        else
                        {
                            fT = -fB1 / fA11;
                            fSqrDist = fB1 * fT + fC;
                        }
                    }
                }
                else if (fT < 0.0f)  // region 6
                {
                    fTmp0 = fA01 + fB1;
                    fTmp1 = fA00 + fB0;
                    if (fTmp1 > fTmp0)
                    {
                        fNumer = fTmp1 - fTmp0;
                        fDenom = fA00 - 2.0f * fA01 + fA11;
                        if (fNumer >= fDenom)
                        {
                            fT = 1.0f;
                            fS = 0.0f;
                            fSqrDist = fA11 + 2.0f * fB1 + fC;
                        }
                        else
                        {
                            fT = fNumer / fDenom;
                            fS = 1.0f - fT;
                            fSqrDist = fS * (fA00 * fS + fA01 * fT + 2.0f * fB0) +
                              fT * (fA01 * fS + fA11 * fT + 2.0f * fB1) + fC;
                        }
                    }
                    else
                    {
                        fT = 0.0f;
                        if (fTmp1 <= 0.0f)
                        {
                            fS = 1.0f;
                            fSqrDist = fA00 + 2.0f * fB0 + fC;
                        }
                        else if (fB0 >= 0.0f)
                        {
                            fS = 0.0f;
                            fSqrDist = fC;
                        }
                        else
                        {
                            fS = -fB0 / fA00;
                            fSqrDist = fB0 * fS + fC;
                        }
                    }
                }
                else  // region 1
                {
                    fNumer = fA11 + fB1 - fA01 - fB0;
                    if (fNumer <= 0.0f)
                    {
                        fS = 0.0f;
                        fT = 1.0f;
                        fSqrDist = fA11 + 2.0f * fB1 + fC;
                    }
                    else
                    {
                        fDenom = fA00 - 2.0f * fA01 + fA11;
                        if (fNumer >= fDenom)
                        {
                            fS = 1.0f;
                            fT = 0.0f;
                            fSqrDist = fA00 + 2.0f * fB0 + fC;
                        }
                        else
                        {
                            fS = fNumer / fDenom;
                            fT = 1.0f - fS;
                            fSqrDist = fS * (fA00 * fS + fA01 * fT + 2.0f * fB0) +
                              fT * (fA01 * fS + fA11 * fT + 2.0f * fB1) + fC;
                        }
                    }
                }
            }

            pfSParam = fS;
            pfTParam = fT;

            return System.Math.Abs(fSqrDist);
        }
        #endregion

    }
}
