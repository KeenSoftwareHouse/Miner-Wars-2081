#region Using Statements

using System.Collections.Generic;
using MinerWarsMath;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// group mask used for collision filtering. If 2 elements have the same bit in mask then they wont collide.
    /// </summary>
    internal struct MyGroupMask
    {
        public System.UInt32 m_Mask0;
        public System.UInt32 m_Mask1;
        public System.UInt32 m_Mask2;
        public System.UInt32 m_Mask3;

        public static MyGroupMask Empty = new MyGroupMask();

        public static MyGroupMask operator |(MyGroupMask groupMask0, MyGroupMask groupMask1)
        {
            MyGroupMask groupMask = new MyGroupMask();
            groupMask.m_Mask0 = groupMask0.m_Mask0 | groupMask1.m_Mask0;
            groupMask.m_Mask1 = groupMask0.m_Mask1 | groupMask1.m_Mask1;
            groupMask.m_Mask2 = groupMask0.m_Mask2 | groupMask1.m_Mask2;
            groupMask.m_Mask3 = groupMask0.m_Mask3 | groupMask1.m_Mask3;
            return groupMask;
        }

        public static MyGroupMask operator &(MyGroupMask groupMask0, MyGroupMask groupMask1)
        {
            MyGroupMask groupMask = new MyGroupMask();
            groupMask.m_Mask0 = groupMask0.m_Mask0 & groupMask1.m_Mask0;
            groupMask.m_Mask1 = groupMask0.m_Mask1 & groupMask1.m_Mask1;
            groupMask.m_Mask2 = groupMask0.m_Mask2 & groupMask1.m_Mask2;
            groupMask.m_Mask3 = groupMask0.m_Mask3 & groupMask1.m_Mask3;
            return groupMask;
        }

        public static MyGroupMask operator ~(MyGroupMask groupMask0)
        {
            MyGroupMask groupMask = new MyGroupMask();
            groupMask.m_Mask0 = ~groupMask0.m_Mask0;
            groupMask.m_Mask1 = ~groupMask0.m_Mask1;
            groupMask.m_Mask2 = ~groupMask0.m_Mask2;
            groupMask.m_Mask3 = ~groupMask0.m_Mask3;
            return groupMask;
        }

        public bool Equals(MyGroupMask other)
        {
            return other.m_Mask0 == m_Mask0 && other.m_Mask1 == m_Mask1 && other.m_Mask2 == m_Mask2 && other.m_Mask3 == m_Mask3;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (MyGroupMask)) return false;
            return Equals((MyGroupMask) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = m_Mask0.GetHashCode();
                result = (result*397) ^ m_Mask1.GetHashCode();
                result = (result*397) ^ m_Mask2.GetHashCode();
                result = (result*397) ^ m_Mask3.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(MyGroupMask left, MyGroupMask right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MyGroupMask left, MyGroupMask right)
        {
            return !left.Equals(right);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// manager giving new mask so 2 same masks are not used. Currently only 128 masks can be since we are using 128bits.
    /// </summary>
    class MyGroupMaskManager
    {
        public MyGroupMaskManager()
        {
            m_BaseMask.m_Mask0 = 0;
            m_BaseMask.m_Mask1 = 0;
            m_BaseMask.m_Mask2 = 0;
            m_BaseMask.m_Mask3 = 0;
        }       
        private MyGroupMask m_BaseMask;

        #region implementation

        public  bool GetGroupMask(ref MyGroupMask mask)
        {            
            System.UInt32 fullMask = 0xffffffff;
            System.UInt32 halfMask = 0x0000ffff;
            System.UInt32 lowMask = 0x000000ff;
            System.UInt32 lowestMask = 0x0000000f;
            System.UInt32 ulowestMask = 0x00000003;
            System.UInt32 uulowestMask = 0x00000001;
            mask.m_Mask0 = 0;
            mask.m_Mask1 = 0;
            mask.m_Mask2 = 0;
            mask.m_Mask3 = 0;

            System.UInt32 bits = 0;
            System.UInt32 index = 0;
            int offset = 0;

            if ((m_BaseMask.m_Mask0 & fullMask) != fullMask)
            {
                bits = m_BaseMask.m_Mask0;
                index = 0;
            }
            else
            {
                if ((m_BaseMask.m_Mask1 & fullMask) != fullMask)
                {
                    bits = m_BaseMask.m_Mask1;
                    index = 1;
                }
                else
                {
                    if ((m_BaseMask.m_Mask2 & fullMask) != fullMask)
                    {
                        bits = m_BaseMask.m_Mask2;
                        index = 2;
                    }
                    else
                    {
                        if ((m_BaseMask.m_Mask3 & fullMask) != fullMask)
                        {
                            bits = m_BaseMask.m_Mask3;
                            index = 3;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if ((bits & halfMask) == halfMask)
            {
                bits = bits >> 16;
                offset += 16;
            }

            if ((bits & lowMask) == lowMask)
            {
                bits = bits >> 8;
                offset += 8;
            }

            if ((bits & lowestMask) == lowestMask)
            {
                bits = bits >> 4;
                offset += 4;
            }

            if ((bits & ulowestMask) == ulowestMask)
            {
                bits = bits >> 2;
                offset += 2;
            }

            if ((bits & uulowestMask) == uulowestMask)
            {
                bits = bits >> 1;
                offset += 1;
            }

            switch (index)
            {
                case 0:
                    mask.m_Mask0 = ((uint)1 << offset);
                    m_BaseMask.m_Mask0 |= mask.m_Mask0;
                    break;
                case 1:
                    mask.m_Mask1 = ((uint)1 << offset);
                    m_BaseMask.m_Mask1 |= mask.m_Mask1;
                    break;
                case 2:
                    mask.m_Mask2 = ((uint)1 << offset);
                    m_BaseMask.m_Mask2 |= mask.m_Mask2;
                    break;
                case 3:
                    mask.m_Mask3 = ((uint)1 << offset);
                    m_BaseMask.m_Mask3 |= mask.m_Mask3;
                    break;
            }

            return true;
        }

        public void PushBackGroupMask(MyGroupMask mask)
        {
            System.UInt32 fullMask = 0xffffffff;

            if ((mask.m_Mask0 & fullMask) != 0)
            {
                // here we go
                m_BaseMask.m_Mask0 &= ~mask.m_Mask0;
            }
            else
            {
                if ((mask.m_Mask1 & fullMask) != 0)
                {
                    // here we go
                    m_BaseMask.m_Mask1 &= ~mask.m_Mask1;
                }
                else
                {
                    if ((mask.m_Mask2 & fullMask) != 0)
                    {
                        // here we go
                        m_BaseMask.m_Mask2 &= ~mask.m_Mask2;
                    }
                    else
                    {
                        if ((mask.m_Mask3 & fullMask) != 0)
                        {
                            // here we go
                            m_BaseMask.m_Mask3 &= ~mask.m_Mask3;
                        }
                    }
                }
            }
        }

        #endregion
    }
}