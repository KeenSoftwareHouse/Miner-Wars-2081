#region Using Statements

using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Ring buffer    
    /// </summary>
    class MyRingBuffer<T>
    {
	    public MyRingBuffer( uint size ) 
	    {
            MyCommonDebugUtils.AssertDebug((size & (size - 1)) == 0);

          	m_Capacity = size;
	        m_Get = 0;
	        m_Put = 0;
            
	        m_Buffer = new T[size];
	    }

        public uint Size()
	    {
		    return ( m_Put - m_Get ) & ( m_Capacity - 1 );
	    }

	    public uint GetCapacity()
	    {
		    return m_Capacity;
	    }

	    public bool Put(T item )
	    {
		    if ( Inc(m_Put) == m_Get ) 
			    return false;

		    m_Buffer[m_Put] = item;
		    m_Put = Inc(m_Put);

		    return true;
	    }

        public T Bottom()
	    {
            MyCommonDebugUtils.AssertDebug(m_Put != m_Get);
		    return m_Buffer[m_Get];
	    }

	    public T Pop()
	    {
		    if ( m_Put == m_Get )
			    return default(T);

		    uint	get	= m_Get;

		    m_Get						= Inc(m_Get);

		    return m_Buffer[get];
	    }

        public bool CanPut()
	    {
		    return Inc(m_Put) != m_Get;
	    }  
  
       	public void Dispose()
        {
	        MyCommonDebugUtils.AssertDebug( m_Get != m_Put );	        
	        m_Get = Inc(m_Get);
        }

	    private uint Inc( uint idx )
	    {
		    return (idx+1)&(m_Capacity-1);
	    }

	    private uint    m_Capacity;
	    private uint    m_Get;
	    private uint    m_Put;
	    private T[]     m_Buffer;
    };

}