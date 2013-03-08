using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Collections.Generic;

//  This class is used for changing voxel maps (creating voxel sphere or boxes, or on the other hand cutting out spheres and boxes)

namespace MinerWars.AppCode.Game.Voxels
{
    static class MyVoxelGenerator
    {
        public static bool InvalidateCache = false;

        //  This is class because I want to pass reference to it and not copy it like structs
        class MyVoxelGeneratorCachedContent
        {
            public byte Content;
        }

        //  Here we will hold temporary content during explosion and clearing pass
        //  The thing is, we never empty/zero this array. We hold and m_notCompressedIndex of last pass.
        static MyVoxelGeneratorCachedContent[][][] m_cachedContents;

        //  Number of voxel we have in all direction in our cached array as a boundary or when needed beughbor voxels
        //  It's three because clearing pass accesses neighbor voxels to explosion, plus their neighbor, which makes two, but I added one as a protection.
        const int CACHED_BOUNDARY_IN_VOXELS = 4;


        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelGenerator.LoadData");

            MyMwcLog.WriteLine("MyVoxelGenerator.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            InvalidateCache = true;

            //  Size of cache array is determined by max possible explosion radius (or diameter), plus boundary one both sides of explosion
            int explosionContentsVoxelsCount = 2 * CACHED_BOUNDARY_IN_VOXELS + 2 * (int)((MyExplosionsConstants.EXPLOSION_RADIUS_MAX + MyVoxelConstants.VOXEL_SIZE_IN_METRES) / MyVoxelConstants.VOXEL_SIZE_IN_METRES);

            m_cachedContents = new MyVoxelGeneratorCachedContent[explosionContentsVoxelsCount][][];
            for (int x = 0; x < explosionContentsVoxelsCount; x++)
            {
                m_cachedContents[x] = new MyVoxelGeneratorCachedContent[explosionContentsVoxelsCount][];
                for (int y = 0; y < explosionContentsVoxelsCount; y++)
                {
                    m_cachedContents[x][y] = new MyVoxelGeneratorCachedContent[explosionContentsVoxelsCount];
                    for (int z = 0; z < explosionContentsVoxelsCount; z++)
                    {
                        m_cachedContents[x][y][z] = new MyVoxelGeneratorCachedContent();
                    }
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelGenerator.LoadData() - END");
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  Overload for case where we don't care about material or percentage, etc
        public static void UnloadData()
        {
        }

        public static void CutOutSphereFast(MyVoxelMap voxelMap, BoundingSphere sphere, float removeRatio = 1)
        {
            MyMwcVoxelMaterialsEnum? voxelMaterial_unused;
            float voxelContentRemovedInPercent_unused;
            MyVoxelGenerator.CutOutSphereFast(voxelMap, sphere, out voxelContentRemovedInPercent_unused, out voxelMaterial_unused, false, removeRatio);
        }

        public static Dictionary<MyMwcVoxelMaterialsEnum, int> CutOutSphereFastWithMaterials(MyVoxelMap voxelMap, BoundingSphere sphere, float removeRatio = 1)
        {
            var exactCutOutMaterials = new Dictionary<MyMwcVoxelMaterialsEnum, int>();
            MyMwcVoxelMaterialsEnum? voxelMaterial_unused;
            float voxelContentRemovedInPercent_unused;
            MyVoxelGenerator.CutOutSphereFast(voxelMap, sphere, out voxelContentRemovedInPercent_unused, out voxelMaterial_unused, false, removeRatio, exactCutOutMaterials);
            return exactCutOutMaterials;
        }

        //  Cut outs a sphere from voxel map (e.g. explosion in a voxel map). We modify only voxels inside the sphere - set them to full or partialy full.
        //  Other voxel are untouched. Sphere coordinates are in world space, not relative to voxel map and is in metres.
        //
        //  Method returns percent of how much voxels were removed by this explosion (value 0.0 means no voxels; value 1.0 means all voxels)
        //
        //  IMPORTANT:
        //  This is optimized version that uses cache for accessing voxels. But the cache has limits so it can be used only for not extremely large cut-outs.
        //  Non-optimized version is: CutOutSphere - but it doesn't mean it's slow... the difference is probably just 10-20%
        //
        //  Returns true if indestructible voxels has been hit (otherwise false)
        public static bool CutOutSphereFast(MyVoxelMap voxelMap, BoundingSphere explosion, out float voxelsCountInPercent, out MyMwcVoxelMaterialsEnum? voxelMaterial, bool isPlayerExplosion = false, float removeRatio = 1, Dictionary<MyMwcVoxelMaterialsEnum, int> exactCutOutMaterials = null)
        {
            explosion.Radius = System.Math.Min(explosion.Radius, MyExplosionsConstants.EXPLOSION_RADIUS_MAX);

            InvalidateCache = true; 

            voxelMaterial = null;

            MyMwcVoxelMaterialsEnum newVoxelMaterialTemp;
            byte tempminContentValueByte;
            MyMwcVector3Int exactCenterOfExplosion = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(explosion.Center.X, explosion.Center.Y, explosion.Center.Z));
            voxelMap.FixVoxelCoord(ref exactCenterOfExplosion);
            voxelMap.GetMaterialAndIndestructibleContent(ref exactCenterOfExplosion, out newVoxelMaterialTemp, out tempminContentValueByte);
            if (voxelMaterial == null)
                voxelMaterial = newVoxelMaterialTemp;   //  We need to replace this only once


            int originalVoxelContentsSum = 0;       //  Sum of all voxel contents affected by this explosion before we extract any voxel. This value is increasing per every voxel, no matter if we realy extract it.
            int removedVoxelContentsSum = 0;        //  Sum of all voxel contents we removed by this explosion. This value increases only if we extract something from voxel map.

            //  Get min corner of the explosion
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                explosion.Center.X - explosion.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                explosion.Center.Y - explosion.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                explosion.Center.Z - explosion.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the explosion
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                explosion.Center.X + explosion.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                explosion.Center.Y + explosion.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                explosion.Center.Z + explosion.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int cachedSubtract;
            cachedSubtract.X = minCorner.X - CACHED_BOUNDARY_IN_VOXELS;
            cachedSubtract.Y = minCorner.Y - CACHED_BOUNDARY_IN_VOXELS;
            cachedSubtract.Z = minCorner.Z - CACHED_BOUNDARY_IN_VOXELS;

            //  We are tracking which voxels were changed, so we can invalidate only needed cells in the cache
            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            MyMwcVector3Int tempVoxelCoord;
            bool indestructible = true;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        float dist = (voxelPosition - explosion.Center).Length();
                        float diff = dist - explosion.Radius;

                        //  This number will tell us how much of this voxel we will remove (can be zero, can be full voxel, or between)
                        int contentToRemove;
                        if (diff > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            contentToRemove = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                        }
                        else if (diff < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            contentToRemove = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else
                        {
                            //  This formula will work even if diff is positive or negative
                            contentToRemove = (int)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                        }

                        contentToRemove = (int)(contentToRemove * removeRatio);

                        //  Only if we have to remove something (e.g. voxels that are outside of the radius aren't affected, or voxels that are 
                        //  in cornes are always out of the radius, even if they are in bounding box)
                        if (contentToRemove > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                        {

                            byte minContentValueByte;
                            MyMwcVoxelMaterialsEnum voxelMaterialTemp;
                            voxelMap.GetMaterialAndIndestructibleContent(ref tempVoxelCoord, out voxelMaterialTemp, out minContentValueByte);

                            int minContentValue = minContentValueByte;

                            if (minContentValue != MyVoxelConstants.VOXEL_CONTENT_FULL)
                            {
                                indestructible = false;
                            }

                            //  Alter only non-empty voxels (because we can't remove empty voxel...)
                            int originalContent = GetCachedVoxelContent(voxelMap, ref tempVoxelCoord, ref cachedSubtract);
                            if (originalContent > minContentValue)
                            {
                                //  IMPORTANT: When doing transformations on 'content' value, cast it to int always!!!
                                //  It's because you can easily forget that result will be negative and if you put negative into byte, it will
                                //  be overflown and you will be surprised by results!!

                                int newVal = originalContent - contentToRemove;
                                if (newVal < minContentValue) newVal = minContentValue;

                                voxelMap.SetVoxelContent((byte)newVal, ref tempVoxelCoord);
                                SetCachedVoxelContent(voxelMap, ref tempVoxelCoord, ref cachedSubtract, (byte)newVal);

                                //  Sum of all voxel contents affected by this explosion before we extract any voxel. This value is increasing per every voxel, no matter if we realy extract it.
                                originalVoxelContentsSum += originalContent;

                                //  Sum of all voxel contents we removed by this explosion. This value increases only if we extract something from voxel map.
                                removedVoxelContentsSum += originalContent - newVal;

                                if (exactCutOutMaterials != null)
                                {
                                    int oldValue = 0;
                                    exactCutOutMaterials.TryGetValue(voxelMaterialTemp, out oldValue);
                                    exactCutOutMaterials[voxelMaterialTemp] = oldValue + removedVoxelContentsSum;
                                }

                                if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                                if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                                if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                                if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                                if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                                if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                            }
                        }
                    }
                }
            }

            InvalidateCache = false;

            if (removedVoxelContentsSum > 0)
            {
                //  Extend borders for cleaning, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                voxelMap.FixVoxelCoord(ref minChanged);
                voxelMap.FixVoxelCoord(ref maxChanged);

                //  Clear all small voxel that may have been created during explosion. They can be created even outside the range of
                //  explosion sphere, e.g. if you have three voxels in a row A, B, C, where A is 255, B is 60, and C is 255. During the
                //  explosion you change C to 0, so now we have 255, 60, 0. Than another explosion that will change A to 0, so we 
                //  will have 0, 60, 0. But B was always outside the range of the explosion. So this is why we need to do -1/+1 and remove
                //  B voxels too.
                RemoveSmallVoxelsUsingChachedVoxels(voxelMap, ref minCorner, ref maxCorner, ref cachedSubtract);

                //  Extend borders for invalidating the cache, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                voxelMap.FixVoxelCoord(ref minChanged);
                voxelMap.FixVoxelCoord(ref maxChanged);

                //  Invalidate cache for voxels cells covered by explosion and some boundary voxels too
                voxelMap.InvalidateCache(minCorner, maxCorner);

                voxelMap.AddExplosion(explosion);
            }

            if (originalVoxelContentsSum > 0f)
            {
                voxelsCountInPercent = (float)removedVoxelContentsSum / (float)originalVoxelContentsSum;
            }
            else
            {
                voxelsCountInPercent = 0f;
            }

            return indestructible;
        }

        //  Return cached voxel content. Voxel coord0 is defined by 'tempVoxelCoord', and it is absolute voxel coordinate in a voxel map.
        //  Because cache isn't large as voxel map, we have to make subtracttion - so use 'relativeSubtract'.
        //  IMPORTANT: This method doesn't check if voxel lies in the voxel map. That must be done by caller!
        static byte GetCachedVoxelContent(MyVoxelMap voxelMap, ref MyMwcVector3Int voxelCoord, ref MyMwcVector3Int relativeSubtract)
        {
            MyMwcVector3Int relativeVoxelCoord;
            relativeVoxelCoord.X = voxelCoord.X - relativeSubtract.X;
            relativeVoxelCoord.Y = voxelCoord.Y - relativeSubtract.Y;
            relativeVoxelCoord.Z = voxelCoord.Z - relativeSubtract.Z;

            MyVoxelGeneratorCachedContent cachedContent = m_cachedContents[relativeVoxelCoord.X][relativeVoxelCoord.Y][relativeVoxelCoord.Z];

            if (InvalidateCache)
            {
                //  Content of this voxel isn't yet cached, so we get it from voxel map and then store in cache
                cachedContent.Content = voxelMap.GetVoxelContent(ref voxelCoord);
            }

            return cachedContent.Content;
        }

        //  Return cached voxel content. Voxel coord0 is defined by 'tempVoxelCoord', and it is absolute voxel coordinate in a voxel map.
        //  Because cache isn't large as voxel map, we have to make subtracttion - so use 'relativeSubtract'.
        //  IMPORTANT: This method doesn't check if voxel lies in the voxel map. That must be done by caller!
        static void SetCachedVoxelContent(MyVoxelMap voxelMap, ref MyMwcVector3Int voxelCoord, ref MyMwcVector3Int relativeSubtract, byte content)
        {
            MyMwcVector3Int relativeVoxelCoord;
            relativeVoxelCoord.X = voxelCoord.X - relativeSubtract.X;
            relativeVoxelCoord.Y = voxelCoord.Y - relativeSubtract.Y;
            relativeVoxelCoord.Z = voxelCoord.Z - relativeSubtract.Z;

            MyVoxelGeneratorCachedContent cachedContent = m_cachedContents[relativeVoxelCoord.X][relativeVoxelCoord.Y][relativeVoxelCoord.Z];

            //  Passes must match. Otherwise we are overwriting content not set in this pass and that is strange!
            cachedContent.Content = content;
        }

        //  This method is similar to RemoveSmallVoxels, but used cached voxels, so is faster
        public static void RemoveSmallVoxelsUsingChachedVoxels(MyVoxelMap voxelMap, ref MyMwcVector3Int minVoxel, ref MyMwcVector3Int maxVoxel, ref MyMwcVector3Int relativeSubtract)
        {
            MyMwcVector3Int voxel;
            for (voxel.X = minVoxel.X; voxel.X <= maxVoxel.X; voxel.X++)
            {
                for (voxel.Y = minVoxel.Y; voxel.Y <= maxVoxel.Y; voxel.Y++)
                {
                    for (voxel.Z = minVoxel.Z; voxel.Z <= maxVoxel.Z; voxel.Z++)
                    {
                        //  IMPORTANT: When doing transformations on 'content' value, cast it to int always!!!
                        //  It's because you can easily forget that result will be negative and if you put negative into byte, it will
                        //  be overflown and you will be surprised by results!!

                        //int content = voxelMap.GetVoxelContent(ref voxel);
                        int content = GetCachedVoxelContent(voxelMap, ref voxel, ref relativeSubtract);

                        //  Check if this is small/invisible voxel (less than 127), but still not empty (more than 0)
                        if ((content > 0) && (content < MyVoxelConstants.VOXEL_ISO_LEVEL))
                        {
                            MyMwcVector3Int neighborVoxel;

                            MyMwcVector3Int neighborVoxelMin;
                            neighborVoxelMin.X = voxel.X - 1;
                            neighborVoxelMin.Y = voxel.Y - 1;
                            neighborVoxelMin.Z = voxel.Z - 1;

                            MyMwcVector3Int neighborVoxelMax;
                            neighborVoxelMax.X = voxel.X + 1;
                            neighborVoxelMax.Y = voxel.Y + 1;
                            neighborVoxelMax.Z = voxel.Z + 1;

                            voxelMap.FixVoxelCoord(ref neighborVoxelMin);
                            voxelMap.FixVoxelCoord(ref neighborVoxelMax);

                            bool foundNonEmptyVoxel = false;
                            for (neighborVoxel.X = neighborVoxelMin.X; neighborVoxel.X <= neighborVoxelMax.X; neighborVoxel.X++)
                            {
                                for (neighborVoxel.Y = neighborVoxelMin.Y; neighborVoxel.Y <= neighborVoxelMax.Y; neighborVoxel.Y++)
                                {
                                    for (neighborVoxel.Z = neighborVoxelMin.Z; neighborVoxel.Z <= neighborVoxelMax.Z; neighborVoxel.Z++)
                                    {
                                        //int neighborContent = voxelMap.GetVoxelContent(ref neighborVoxel);
                                        int neighborContent = GetCachedVoxelContent(voxelMap, ref neighborVoxel, ref relativeSubtract);

                                        //  Check if this is small/invisible voxel
                                        if (neighborContent >= MyVoxelConstants.VOXEL_ISO_LEVEL)
                                        {
                                            foundNonEmptyVoxel = true;
                                            goto END_NEIGHBOR_LOOP;
                                        }
                                    }
                                }
                            }

                        END_NEIGHBOR_LOOP:

                            if (foundNonEmptyVoxel == false)
                            {
                                voxelMap.SetVoxelContent(MyVoxelConstants.VOXEL_CONTENT_EMPTY, ref voxel);
                            }
                        }
                    }
                }
            }
        }

        //  Removes voxels that have content less than half of iso value (so less than 0.5 or in byte it's 127). It's because
        //  sometimes after many explosions we left the world with small voxels spread around that have this small values, but
        //  doesn't have neighborhod full voxels, so we can't render them, but they occupy memory and are detected by col/det. So we don't want them.
        //  We check only -1 and +1 neigbor!
        //  IMPORTANT: This method isn't remover for 'floater' or 'small voxel islands'.
        //  IMPORTANT: Use this method only offline, e.g. for clearing the levels. CutOutSphereFast has its own version optimized of clearing build into the algorithm
        public static void RemoveSmallVoxels(MyVoxelMap voxelMap, MyMwcVector3Int minVoxel, MyMwcVector3Int maxVoxel)
        {
            MyMwcVector3Int voxel;
            for (voxel.X = minVoxel.X; voxel.X <= maxVoxel.X; voxel.X++)
            {
                for (voxel.Y = minVoxel.Y; voxel.Y <= maxVoxel.Y; voxel.Y++)
                {
                    for (voxel.Z = minVoxel.Z; voxel.Z <= maxVoxel.Z; voxel.Z++)
                    {
                        //  IMPORTANT: When doing transformations on 'content' value, cast it to int always!!!
                        //  It's because you can easily forget that result will be negative and if you put negative into byte, it will
                        //  be overflown and you will be surprised by results!!

                        int content = voxelMap.GetVoxelContent(ref voxel);

                        //  Check if this is small/invisible voxel (less than 127), but still not empty (more than 0)
                        if ((content > 0) && (content < MyVoxelConstants.VOXEL_ISO_LEVEL))
                        {
                            MyMwcVector3Int neighborVoxel;

                            MyMwcVector3Int neighborVoxelMin;
                            neighborVoxelMin.X = voxel.X - 1;
                            neighborVoxelMin.Y = voxel.Y - 1;
                            neighborVoxelMin.Z = voxel.Z - 1;

                            MyMwcVector3Int neighborVoxelMax;
                            neighborVoxelMax.X = voxel.X + 1;
                            neighborVoxelMax.Y = voxel.Y + 1;
                            neighborVoxelMax.Z = voxel.Z + 1;

                            voxelMap.FixVoxelCoord(ref neighborVoxelMin);
                            voxelMap.FixVoxelCoord(ref neighborVoxelMax);

                            bool foundNonEmptyVoxel = false;
                            for (neighborVoxel.X = neighborVoxelMin.X; neighborVoxel.X <= neighborVoxelMax.X; neighborVoxel.X++)
                            {
                                for (neighborVoxel.Y = neighborVoxelMin.Y; neighborVoxel.Y <= neighborVoxelMax.Y; neighborVoxel.Y++)
                                {
                                    for (neighborVoxel.Z = neighborVoxelMin.Z; neighborVoxel.Z <= neighborVoxelMax.Z; neighborVoxel.Z++)
                                    {
                                        int neighborContent = voxelMap.GetVoxelContent(ref neighborVoxel);

                                        //  Check if this is small/invisible voxel
                                        if (neighborContent >= MyVoxelConstants.VOXEL_ISO_LEVEL)
                                        {
                                            foundNonEmptyVoxel = true;
                                            goto END_NEIGHBOR_LOOP;
                                        }
                                    }
                                }
                            }

                        END_NEIGHBOR_LOOP:

                            if (foundNonEmptyVoxel == false)
                            {
                                voxelMap.SetVoxelContent(MyVoxelConstants.VOXEL_CONTENT_EMPTY, ref voxel);
                            }
                        }
                    }
                }
            }
        }

        //  Create box in a voxel map. Voxels inside the box are set to full. All voxels outside are ignored.
        //  Box corners are in voxels, not metres.
        //  Only full voxels are set, we don't care about partialy inside/outside.
        public static void CreateBox(MyVoxelMap voxelMap, BoundingBox box)
        {
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Min);
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Max);

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        voxelMap.SetVoxelContent(MyVoxelConstants.VOXEL_CONTENT_FULL, ref tempVoxelCoord);
                    }
                }
            }
        }

        // This is same as normal CreateBox method, but cache is invalidated (so that voxel changes be immediately visible)
        public static void CreateBoxInvalidateCache(MyVoxelMap voxelMap, BoundingBox box, ref bool changed)
        {
            //  Get min corner of the box
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Min);

            //  Get max corner of the box
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Max);

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            //  We are tracking which voxels were changed, so we can invalidate only needed cells in the cache
            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            bool sphereAdded = false;

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        byte newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;

                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent > originalContent)
                        {
                            changed = true;
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                            sphereAdded = true;

                            if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                            if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                        }
                    }
                }
            }

            if (sphereAdded == true)
            {
                //  Extend borders for cleaning, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                voxelMap.FixVoxelCoord(ref minChanged);
                voxelMap.FixVoxelCoord(ref maxChanged);

                voxelMap.InvalidateCache(minChanged, maxChanged);
            }
        }

        //  Cut out box from a voxel map. Voxels inside the box are set to empty. All voxels outside are ignored.
        //  Box corners are in voxels, not metres.
        //  Only empty voxels are set, we don't care about partialy inside/outside.
        public static void CutOutBox(MyVoxelMap voxelMap, BoundingBox box)
        {
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Min);
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Max);

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        voxelMap.SetVoxelContent(MyVoxelConstants.VOXEL_CONTENT_EMPTY, ref tempVoxelCoord);
                    }
                }
            }
        }

        //  This is same as normal cut-out box, but cache is invalidated (so that voxel changes be immediately visible)
        public static void CutOutBoxInvalidateCache(MyVoxelMap voxelMap, BoundingBox box, ref bool changed)
        {
            //  Get min corner of the box
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Min);

            //  Get max corner of the box
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(box.Max);

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            //  We are tracking which voxels were changed, so we can invalidate only needed cells in the cache
            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            int removedVoxelContent = 0;

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        byte contentToRemove = MyVoxelConstants.VOXEL_CONTENT_FULL;

                        int originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);
                        if (originalContent > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                        {
                            changed = true;

                            int newVal = originalContent - contentToRemove;
                            if (newVal < MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                                newVal = MyVoxelConstants.VOXEL_CONTENT_EMPTY;

                            voxelMap.SetVoxelContent((byte)newVal, ref tempVoxelCoord);

                            removedVoxelContent += originalContent - newVal;

                            if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                            if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                        }
                    }
                }
            }

            if (removedVoxelContent > 0)
            {
                //  Extend borders for cleaning, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                voxelMap.FixVoxelCoord(ref minChanged);
                voxelMap.FixVoxelCoord(ref maxChanged);

                voxelMap.InvalidateCache(minChanged, maxChanged);
            }
        }

        //  Clears voxel map, set every voxel to empty.
        //  IMPORTANT: Don't use this method, because it destroys idea of not remembering full cells.
        public static void ClearWholeVoxelMap(MyVoxelMap voxelMap)
        {
            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = 0; tempVoxelCoord.X < voxelMap.Size.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = 0; tempVoxelCoord.Y < voxelMap.Size.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = 0; tempVoxelCoord.Z < voxelMap.Size.Z; tempVoxelCoord.Z++)
                    {
                        voxelMap.SetVoxelContent(MyVoxelConstants.VOXEL_CONTENT_EMPTY, ref tempVoxelCoord);
                    }
                }
            }
        }

        //  Generates sphere in a voxel map. We modify only voxels inside the sphere - set them to full or partialy full.
        //  Other voxel are untouched. Center is relative to voxel map (not world coordinates). Radius is in metres.
        public static void CreateSphere(MyVoxelMap voxelMap, BoundingSphere sphere)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelGenerator.CreateSphere");

            //  Get min corner of the explosion
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the explosion
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        float dist = (voxelPosition - sphere.Center).Length();
                        float diff = dist - sphere.Radius;

                        byte newContent;
                        if (diff > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                        }
                        else if (diff < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else
                        {
                            //  This formula will work even if diff is positive or negative
                            newContent = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                        }

                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent > originalContent)
                        {
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                        }
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        // This is same as normal CreateSphere method, but cache is invalidated (so that voxel changes be immediately visible)
        public static void CreateSphereInvalidateCache(MyVoxelMap voxelMap, BoundingSphere sphere, ref bool changed, MyMwcVoxelMaterialsEnum? material)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelGenerator.CreateSphereInvalidateCache");

            //  Get min corner of the explosion
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the explosion
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            //  We are tracking which voxels were changed, so we can invalidate only needed cells in the cache
            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            bool sphereAdded = false;

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        float dist = (voxelPosition - sphere.Center).Length();
                        float diff = dist - sphere.Radius;

                        byte newContent;
                        if (diff > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                        }
                        else if (diff < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else
                        {
                            //  This formula will work even if diff is positive or negative
                            newContent = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                        }

                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent > originalContent)
                        {
                            if (material.HasValue)
                            {
                                MyMwcVoxelMaterialsEnum originalMaterial;
                                byte originalIndestructibleContent;

                                voxelMap.GetMaterialAndIndestructibleContent(ref tempVoxelCoord, out originalMaterial, out originalIndestructibleContent);
                                voxelMap.SetVoxelMaterialAndIndestructibleContent(material.Value, originalIndestructibleContent, ref tempVoxelCoord);
                            }

                            changed = true;
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                            sphereAdded = true;

                            if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                            if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                        }
                    }
                }
            }

            if (sphereAdded == true)
            {
                //  Extend borders for cleaning, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                voxelMap.FixVoxelCoord(ref minChanged);
                voxelMap.FixVoxelCoord(ref maxChanged);

                voxelMap.InvalidateCache(minChanged, maxChanged);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  Cut-outs a sphere from a voxel map.
        //  Generates inverse sphere in a voxel map (extract matters, create hole, etc - it's like explosion). We modify only voxels inside the sphere - set them to full or partialy full.
        //  Other voxel are untouched. Center is relative to voxel map (not world coordinates). Radius is in metres.
        //
        //  IMPORTANT:
        //  This is non-optimized version (faster one is CutOutSphereFast). But it doesn't mean this one is slow... the difference is probably just 10-20%
        public static void CutOutSphere(MyVoxelMap voxelMap, BoundingSphere sphere)
        {
            //  Get min corner of the explosion
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the explosion
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        float dist = (voxelPosition - sphere.Center).Length();
                        float diff = dist - sphere.Radius;

                        byte contentToRemove;
                        if (diff > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            contentToRemove = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                        }
                        else if (diff < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            contentToRemove = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else
                        {
                            //  This formula will work even if diff is positive or negative
                            contentToRemove = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                        }

                        int originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);
                        if (originalContent > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                        {
                            int newVal = originalContent - contentToRemove;
                            if (newVal < MyVoxelConstants.VOXEL_CONTENT_EMPTY) newVal = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            voxelMap.SetVoxelContent((byte)newVal, ref tempVoxelCoord);
                        }

                    }
                }
            }
        }

        //  This is same as normal cut-out sphere, but cache is invalidated like in cut-out sphere fast(so that voxel cut-out will be immediately visible)
        public static void CutOutSphereInvalidateCache(MyVoxelMap voxelMap, BoundingSphere sphere, ref bool changed)
        {
            //  Get min corner of the explosion
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the explosion
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            //  We are tracking which voxels were changed, so we can invalidate only needed cells in the cache
            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            int removedVoxelContent = 0;

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        float dist = (voxelPosition - sphere.Center).Length();
                        float diff = dist - sphere.Radius;

                        byte contentToRemove;
                        if (diff > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            contentToRemove = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                        }
                        else if (diff < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            contentToRemove = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else
                        {
                            //  This formula will work even if diff is positive or negative
                            contentToRemove = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                        }

                        int originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);
                        if (originalContent > MyVoxelConstants.VOXEL_CONTENT_EMPTY && contentToRemove > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                        {
                            changed = true;

                            int newVal = originalContent - contentToRemove;
                            if (newVal < MyVoxelConstants.VOXEL_CONTENT_EMPTY) newVal = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            voxelMap.SetVoxelContent((byte)newVal, ref tempVoxelCoord);

                            removedVoxelContent += originalContent - newVal;

                            if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                            if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                        }
                    }
                }
            }

            if (removedVoxelContent > 0)
            {
                //  Extend borders for cleaning, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                voxelMap.FixVoxelCoord(ref minChanged);
                voxelMap.FixVoxelCoord(ref maxChanged);

                voxelMap.InvalidateCache(minChanged, maxChanged);
            }
        }


        public static void CutOutBoxRelative(MyVoxelMap voxelMap, Vector3 relativeMin, Vector3 relativeMax)
        {
            BoundingBox boundingBox = voxelMap.WorldAABB;

            //  Get min and max cell coordinate where camera bounding box can fit
            MyMwcVector3Int cellCoordMin = voxelMap.GetVoxelCoordinateFromMeters(boundingBox.Min);
            MyMwcVector3Int cellCoordMax = voxelMap.GetVoxelCoordinateFromMeters(boundingBox.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            voxelMap.FixVoxelCoord(ref cellCoordMin);
            voxelMap.FixVoxelCoord(ref cellCoordMax);

            cellCoordMin.X = (int)(cellCoordMax.X * relativeMin.X);
            cellCoordMin.Y = (int)(cellCoordMax.Y * relativeMin.Y);
            cellCoordMin.Z = (int)(cellCoordMax.Z * relativeMin.Z);

            cellCoordMax.X = (int)(cellCoordMax.X * relativeMax.X);
            cellCoordMax.Y = (int)(cellCoordMax.Y * relativeMax.Y);
            cellCoordMax.Z = (int)(cellCoordMax.Z * relativeMax.Z);

            MyMwcVector3Int cellCoord;

            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        voxelMap.SetVoxelContent(0, ref cellCoord);
                    }
                }
            }

            voxelMap.InvalidateCache(cellCoordMin, cellCoordMax);
            voxelMap.CalcAverageDataCellMaterials();

            //MyVoxelMaps.RecalcVoxelMaps();
        }


        public static void ChangeMaterialInBoxRelative(MyVoxelMap voxelMap, Vector3 relativeMin, Vector3 relativeMax, MyMwcVoxelMaterialsEnum material)
        {
            BoundingBox boundingBox = voxelMap.WorldAABB;

            //  Get min and max cell coordinate where camera bounding box can fit
            MyMwcVector3Int cellCoordMin = voxelMap.GetVoxelCoordinateFromMeters(boundingBox.Min);
            MyMwcVector3Int cellCoordMax = voxelMap.GetVoxelCoordinateFromMeters(boundingBox.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            voxelMap.FixVoxelCoord(ref cellCoordMin);
            voxelMap.FixVoxelCoord(ref cellCoordMax);

            cellCoordMin.X = (int)(cellCoordMax.X * relativeMin.X);
            cellCoordMin.Y = (int)(cellCoordMax.Y * relativeMin.Y);
            cellCoordMin.Z = (int)(cellCoordMax.Z * relativeMin.Z);

            cellCoordMax.X = (int)(cellCoordMax.X * relativeMax.X);
            cellCoordMax.Y = (int)(cellCoordMax.Y * relativeMax.Y);
            cellCoordMax.Z = (int)(cellCoordMax.Z * relativeMax.Z);

            MyMwcVector3Int cellCoord;

            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        voxelMap.SetVoxelMaterialAndIndestructibleContent(material, 0, ref cellCoord);
                    }
                }
            }

            voxelMap.InvalidateCache(cellCoordMin, cellCoordMax);
            voxelMap.CalcAverageDataCellMaterials();

            //MyVoxelMaps.RecalcVoxelMaps();
        }


        public static void CreateOrientedBox(MyVoxelMap voxelMap, MyOrientedBoundingBox box, MyMwcVoxelMaterialsEnum? material)
        {
            BoundingBox aabb = box.GetAABB();

            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Min - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Max + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 position = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        BoundingBox voxelAABB = new BoundingBox(position - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF), position + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF));

                        byte newContent = 0;
                        ContainmentType ct = box.Contains(ref voxelAABB);
                        if (ct == ContainmentType.Contains)
                        {
                            newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else
                            if (ct == ContainmentType.Disjoint)
                            {
                                newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            }
                            else
                            {

                                // Transform the point into box-local space and check against
                                // our extents.
                                Quaternion qinv = Quaternion.Conjugate(box.Orientation);
                                Vector3 plocal = Vector3.Transform(position - box.Center, qinv);

                                //MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF 

                                float distX = 0;
                                float distY = 0;
                                float distZ = 0;

                                int sX = System.Math.Sign(plocal.X);
                                int sY = System.Math.Sign(plocal.Y);
                                int sZ = System.Math.Sign(plocal.Z);

                                if (sX > 0)
                                {
                                    distX = plocal.X - box.HalfExtent.X;
                                }
                                else
                                {
                                    distX = plocal.X + box.HalfExtent.X;
                                }

                                if (sY > 0)
                                {
                                    distY = plocal.Y - box.HalfExtent.Y;
                                }
                                else
                                {
                                    distY = plocal.Y + box.HalfExtent.Y;
                                }

                                if (sZ > 0)
                                {
                                    distZ = plocal.Z - box.HalfExtent.Z;
                                }
                                else
                                {
                                    distZ = plocal.Z + box.HalfExtent.Z;
                                }

                                //float diff = (distX + distY + distZ) / 3;
                                //float diff = (sX*sY*sZ) * System.Math.Min(System.Math.Min(distX,distY), distZ);

                                int contentX;
                                int contentY;
                                int contentZ;

                                if (sX > 0)
                                {
                                    if (distX > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentX = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                    }
                                    else if (distX < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentX = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {
                                        //  This formula will work even if diff is positive or negative
                                        contentX = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distX / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                    }
                                }
                                else
                                {
                                    if (distX < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentX = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                    }
                                    else if (distX > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentX = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {
                                        //  This formula will work even if diff is positive or negative
                                        contentX = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distX / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                    }
                                }



                                if (sY > 0)
                                {
                                    if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                    }
                                    else if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {
                                        //  This formula will work even if diff is positive or negative
                                        contentY = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                    }
                                }
                                else
                                {
                                    if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                    }
                                    else if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {
                                        //  This formula will work even if diff is positive or negative
                                        contentY = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                    }
                                }


                                if (sZ > 0)
                                {
                                    if (distZ > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentZ = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                    }
                                    else if (distZ < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentZ = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {
                                        //  This formula will work even if diff is positive or negative
                                        contentZ = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distZ / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                    }
                                }
                                else
                                {
                                    if (distZ < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentZ = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                    }
                                    else if (distZ > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                    {
                                        contentZ = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {
                                        //  This formula will work even if diff is positive or negative
                                        contentZ = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distZ / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                    }
                                }

                                //voxelMap.SetVoxelContent( (byte)((contentX + contentY + contentZ)/3.0f), ref tempVoxelCoord);
                                newContent = (byte)(System.Math.Min(System.Math.Min(contentX, contentY), contentZ));
                            }


                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent > originalContent)
                        {
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                            if (material.HasValue)
                                voxelMap.SetVoxelMaterialAndIndestructibleContent(material.Value, 0, ref tempVoxelCoord);
                        }


                    }
                }
            }

            voxelMap.InvalidateCache(minCorner, maxCorner);
            voxelMap.CalcAverageDataCellMaterials();
        }


        public static void CutOutOrientedBox(MyVoxelMap voxelMap, MyOrientedBoundingBox box, ref bool changed)
        {
            BoundingBox aabb = box.GetAABB();

            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Min - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Max + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            System.Threading.Tasks.Parallel.For(minCorner.X, maxCorner.X, i =>
                {
                    System.Threading.Tasks.Parallel.For(minCorner.Y, maxCorner.Y, j =>
                        {
                            System.Threading.Tasks.Parallel.For(minCorner.Z, maxCorner.Z, k =>
                            {
                                MyMwcVector3Int tempVoxelCoord = new MyMwcVector3Int(i, j, k);
                                Vector3 position = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                                BoundingBox voxelAABB = new BoundingBox(position - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF), position + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF));

                                byte newContent = 0;
                                ContainmentType ct = box.Contains(ref voxelAABB);
                                if (ct == ContainmentType.Contains)
                                {
                                    newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                }
                                else
                                    if (ct == ContainmentType.Disjoint)
                                    {
                                        newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {

                                        // Transform the point into box-local space and check against
                                        // our extents.
                                        Quaternion qinv = Quaternion.Conjugate(box.Orientation);
                                        Vector3 plocal = Vector3.Transform(position - box.Center, qinv);

                                        //MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF 

                                        float distX = 0;
                                        float distY = 0;
                                        float distZ = 0;

                                        int sX = System.Math.Sign(plocal.X);
                                        int sY = System.Math.Sign(plocal.Y);
                                        int sZ = System.Math.Sign(plocal.Z);

                                        if (sX > 0)
                                        {
                                            distX = plocal.X - box.HalfExtent.X;
                                        }
                                        else
                                        {
                                            distX = plocal.X + box.HalfExtent.X;
                                        }

                                        if (sY > 0)
                                        {
                                            distY = plocal.Y - box.HalfExtent.Y;
                                        }
                                        else
                                        {
                                            distY = plocal.Y + box.HalfExtent.Y;
                                        }

                                        if (sZ > 0)
                                        {
                                            distZ = plocal.Z - box.HalfExtent.Z;
                                        }
                                        else
                                        {
                                            distZ = plocal.Z + box.HalfExtent.Z;
                                        }

                                        //float diff = (distX + distY + distZ) / 3;
                                        //float diff = (sX*sY*sZ) * System.Math.Min(System.Math.Min(distX,distY), distZ);

                                        int contentX;
                                        int contentY;
                                        int contentZ;

                                        if (sX < 0)
                                        {
                                            if (distX > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentX = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                            }
                                            else if (distX < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentX = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                            }
                                            else
                                            {
                                                //  This formula will work even if diff is positive or negative
                                                contentX = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distX / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                            }
                                        }
                                        else
                                        {
                                            if (distX < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentX = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                            }
                                            else if (distX > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentX = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                            }
                                            else
                                            {
                                                //  This formula will work even if diff is positive or negative
                                                contentX = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distX / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                            }
                                        }



                                        if (sY < 0)
                                        {
                                            if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                            }
                                            else if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                            }
                                            else
                                            {
                                                //  This formula will work even if diff is positive or negative
                                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                            }
                                        }
                                        else
                                        {
                                            if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                            }
                                            else if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                            }
                                            else
                                            {
                                                //  This formula will work even if diff is positive or negative
                                                contentY = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                            }
                                        }


                                        if (sZ < 0)
                                        {
                                            if (distZ > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentZ = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                            }
                                            else if (distZ < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentZ = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                            }
                                            else
                                            {
                                                //  This formula will work even if diff is positive or negative
                                                contentZ = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distZ / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                            }
                                        }
                                        else
                                        {
                                            if (distZ < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentZ = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                            }
                                            else if (distZ > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                                            {
                                                contentZ = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                            }
                                            else
                                            {
                                                //  This formula will work even if diff is positive or negative
                                                contentZ = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distZ / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                            }
                                        }

                                        //voxelMap.SetVoxelContent( (byte)((contentX + contentY + contentZ)/3.0f), ref tempVoxelCoord);
                                        newContent = (byte)(System.Math.Min(System.Math.Min(contentX, contentY), contentZ));
                                    }


                                byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                                if (newContent < originalContent)
                                {
                                    voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord, true);
                                    //changed = true;
                                }
                            });
                        });
                });

            /*
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        
                       
                    }
                }
            }*/

            changed = true;
            voxelMap.InvalidateCache(minCorner, maxCorner);
            voxelMap.CalcAverageDataCellMaterials();
        }



        public static void CreateCylinder(MyVoxelMap voxelMap, float radius1, float radius2, MyOrientedBoundingBox box, MyMwcVoxelMaterialsEnum? material, ref bool changed)
        {
            //box.HalfExtent x = radius1
            //box.HalfExtent y = length/2
            //box.HalfExtent z = radius2


            BoundingBox aabb = box.GetAABB();

            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Min - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Max + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 position = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);

                        //BoundingBox voxelAABB = new BoundingBox(position - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF), position + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF));

                        byte newContent = 0;

                        // Transform the point into box-local space and check against
                        // our extents.
                        Quaternion qinv = Quaternion.Conjugate(box.Orientation);
                        Vector3 plocal = Vector3.Transform(position - box.Center, qinv);

                        //MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF 

                        float distY = 0;
                        int sY = System.Math.Sign(plocal.Y);

                        if (sY > 0)
                        {
                            distY = plocal.Y - box.HalfExtent.Y;
                        }
                        else
                        {
                            distY = plocal.Y + box.HalfExtent.Y;
                        }

                        float distRatio = 1;
                        distRatio = MathHelper.Clamp((plocal.Y + box.HalfExtent.Y) / (2*box.HalfExtent.Y), 0, 1);

                        int contentY = -1;

                        if (sY > 0)
                        {
                            if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            }
                            else if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                            }
                            else
                            {
                                //  This formula will work even if diff is positive or negative
                                contentY = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                            }
                        }
                        else
                        {
                            if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            }
                            else if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                            }
                            else
                            {
                                //  This formula will work even if diff is positive or negative
                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                            }
                        }

                               
                        Matrix boxMatrix = Matrix.CreateFromQuaternion(box.Orientation);
                        Vector3 hY1 = box.Center + boxMatrix.Up * box.HalfExtent.Y;
                        Vector3 hY2 = box.Center + boxMatrix.Down * box.HalfExtent.Y;

                        float dist = MyUtils.GetPointLineDistance(ref hY1, ref hY2, ref position);
                        float diff = dist - MathHelper.Lerp(radius2, radius1, distRatio);
                        //float diff = dist - box.HalfExtent.Z;

                        byte newContent2;
                        if (diff > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent2 = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                        }
                        else if (diff < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent2 = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else
                        {
                            //  This formula will work even if diff is positive or negative
                            newContent2 = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                        }  

                          
                        newContent = (byte)System.Math.Min(contentY, newContent2);
                        //newContent = (byte)contentY; 

                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent > originalContent)
                        {
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                            if (material.HasValue)
                                voxelMap.SetVoxelMaterialAndIndestructibleContent(material.Value, 0, ref tempVoxelCoord);
                        }

                    }
                }
            }

            voxelMap.InvalidateCache(minCorner, maxCorner);
            voxelMap.CalcAverageDataCellMaterials();

        } //CreateCylinder


        public static void CutOutCylinder(MyVoxelMap voxelMap, float radius1, float radius2, MyOrientedBoundingBox box, MyMwcVoxelMaterialsEnum? material, ref bool changed)
        {
            //box.HalfExtent x = radius1
            //box.HalfExtent y = length/2
            //box.HalfExtent z = radius2


            BoundingBox aabb = box.GetAABB();

            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Min - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Max + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 position = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);

                        //BoundingBox voxelAABB = new BoundingBox(position - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF), position + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF));

                        byte newContent = 0;

                        // Transform the point into box-local space and check against
                        // our extents.
                        Quaternion qinv = Quaternion.Conjugate(box.Orientation);
                        Vector3 plocal = Vector3.Transform(position - box.Center, qinv);

                        //MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF 

                        float distY = 0;
                        int sY = System.Math.Sign(plocal.Y);

                        if (sY > 0)
                        {
                            distY = plocal.Y - box.HalfExtent.Y;
                        }
                        else
                        {
                            distY = plocal.Y + box.HalfExtent.Y;
                        }

                        float distRatio = 1;
                        distRatio = MathHelper.Clamp((plocal.Y + box.HalfExtent.Y) / (2 * box.HalfExtent.Y), 0, 1);

                        int contentY = -1;

                        if (sY > 0)
                        {
                            if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                            }
                            else if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            }
                            else
                            {
                                //  This formula will work even if diff is positive or negative
                                //contentY = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                            }
                        }
                        else
                        {
                            if (distY < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_FULL;
                            }
                            else if (distY > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                contentY = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            }
                            else
                            {
                                //  This formula will work even if diff is positive or negative
                                //contentY = MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                contentY = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - distY / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                            }
                        }


                        Matrix boxMatrix = Matrix.CreateFromQuaternion(box.Orientation);
                        Vector3 hY1 = box.Center + boxMatrix.Up * box.HalfExtent.Y;
                        Vector3 hY2 = box.Center + boxMatrix.Down * box.HalfExtent.Y;

                        float dist = MyUtils.GetPointLineDistance(ref hY1, ref hY2, ref position);
                        float diff = dist - MathHelper.Lerp(radius2, radius1, distRatio);
                        //float diff = dist - box.HalfExtent.Z;

                        byte newContent2;
                        if (diff > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent2 = MyVoxelConstants.VOXEL_CONTENT_FULL;
                        }
                        else if (diff < -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                        {
                            newContent2 = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                        }
                        else
                        {
                            //  This formula will work even if diff is positive or negative
                            //newContent2 = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                            newContent2 = (byte)(MyVoxelConstants.VOXEL_CONTENT_FULL - (int)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL));
                        }


                        newContent = (byte)System.Math.Max(contentY, newContent2);
                        //newContent = (byte)contentY; 

                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent < originalContent)
                        {
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                            if (material.HasValue)
                                voxelMap.SetVoxelMaterialAndIndestructibleContent(material.Value, 0, ref tempVoxelCoord);
                        }

                    }
                }
            }

            voxelMap.InvalidateCache(minCorner, maxCorner);
            voxelMap.CalcAverageDataCellMaterials();

        } //CutoutCylinder


        //  6 - 7  
        // /   /|
        //4|- 5 |
        //|2 -|3
        //|/  |/
        //0 - 1                
        public static void CreateCuboid(MyVoxelMap voxelMap, MyCuboid cuboid, MyMwcVoxelMaterialsEnum? material)
        {
            BoundingBox aabb = cuboid.GetAABB();


            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Min - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Max + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 position = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        BoundingBox voxelAABB = new BoundingBox(position - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF), position + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF));

                        bool isInside = true;

                        for (int i = 0; i < cuboid.Sides.Length; i++)
                        {
                            MyPlane side = cuboid.Sides[i].Plane;
                            float distance = MyUtils.GetDistanceFromPointToPlane(ref position, ref side);
                            if (distance > 0)
                            {
                                isInside = false;
                                break;
                            }
                        }

                        float minDistance = float.MaxValue;

                        for (int i = 0; i < cuboid.Sides.Length; i++)
                        {
                            /*
                            for (int j = 0; j < cuboid.Sides[i].Lines.Length; j++)
                            {
                                MyLine line = cuboid.Sides[i].Lines[j];
                                float distance = 0;
                                Vector3 point = MyUtils.GetClosestPointOnLine(ref line, ref position, out distance);
                                distance = System.Math.Abs(Vector3.Distance(position, point));
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                }
                            } */

                            MyQuad quad = new MyQuad();
                            quad.Point0 = cuboid.Sides[i].Lines[0].From;
                            quad.Point1 = cuboid.Sides[i].Lines[1].From;
                            quad.Point2 = cuboid.Sides[i].Lines[3].From;
                            quad.Point3 = cuboid.Sides[i].Lines[2].From;

                            float distance = MyUtils.GetDistancePointToQuad(ref position, ref quad);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                            }
                        }


                        byte newContent = 0;


                        if (isInside)
                        {
                            if (minDistance > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                            }
                            else
                            {
                                //  This formula will work even if diff is positive or negative
                                //newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                newContent = (byte)(MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - minDistance / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL));
                            }
                        }
                        else
                        {
                            if (minDistance > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            }
                            else
                            {
                                //newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                //  This formula will work even if diff is positive or negative
                                newContent = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - minDistance / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                
                            }
                        }

                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent > originalContent)
                        {
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                            if (material.HasValue)
                                voxelMap.SetVoxelMaterialAndIndestructibleContent(material.Value, 0, ref tempVoxelCoord);
                        }
                    }
                }
            }

            voxelMap.InvalidateCache(minCorner, maxCorner);
            voxelMap.CalcAverageDataCellMaterials();
        } //Create Cuboid


        public static void CutOutCuboid(MyVoxelMap voxelMap, MyCuboid cuboid, MyMwcVoxelMaterialsEnum? material)
        {
            BoundingBox aabb = cuboid.GetAABB();


            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Min - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(aabb.Max + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        Vector3 position = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                        BoundingBox voxelAABB = new BoundingBox(position - new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF), position + new Vector3(MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF));

                        bool isInside = true;

                        for (int i = 0; i < cuboid.Sides.Length; i++)
                        {
                            MyPlane side = cuboid.Sides[i].Plane;
                            float distance = MyUtils.GetDistanceFromPointToPlane(ref position, ref side);
                            if (distance > 0)
                            {
                                isInside = false;
                                break;
                            }
                        }

                        float minDistance = float.MaxValue;

                        for (int i = 0; i < cuboid.Sides.Length; i++)
                        {
                            MyQuad quad = new MyQuad();
                            quad.Point0 = cuboid.Sides[i].Lines[0].From;
                            quad.Point1 = cuboid.Sides[i].Lines[1].From;
                            quad.Point2 = cuboid.Sides[i].Lines[3].From;
                            quad.Point3 = cuboid.Sides[i].Lines[2].From;

                            float distance = MyUtils.GetDistancePointToQuad(ref position, ref quad);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                            }
                        }


                        byte newContent = 0;


                        if (isInside)
                        {
                            if (minDistance > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                            }
                            else
                            {
                                //  This formula will work even if diff is positive or negative
                                //newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                newContent = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - minDistance / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                //newContent = (byte)(MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - minDistance / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL));
                            }
                        }
                        else
                        {
                            if (minDistance > MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)
                            {
                                newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                            }
                            else
                            {
                                //newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                //  This formula will work even if diff is positive or negative
                                //newContent = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - minDistance / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                newContent = (byte)(MyVoxelConstants.VOXEL_CONTENT_FULL - (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - minDistance / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL));
                            }
                        }

                        byte originalContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        if (newContent < originalContent)
                        {
                            voxelMap.SetVoxelContent(newContent, ref tempVoxelCoord);
                            if (material.HasValue)
                                voxelMap.SetVoxelMaterialAndIndestructibleContent(material.Value, 0, ref tempVoxelCoord);
                        }
                    }
                }
            }

            voxelMap.InvalidateCache(minCorner, maxCorner);
            voxelMap.CalcAverageDataCellMaterials();
        } //CutOut Cuboid

    }
}       
