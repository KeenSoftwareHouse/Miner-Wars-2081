using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XACT3;
using SharpDX;
using System.IO;

namespace MinerWars.AppCode.Game.Audio
{
    /// <summary>
    /// In memory wave bank
    /// We must hold reference to DataStream
    /// </summary>
    public class MyInMemoryWaveBank : WaveBank
    {
        private DataStream m_backingStore;

        private MyInMemoryWaveBank(AudioEngine engine, DataStream dataStream)
            : base(engine, dataStream)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                m_backingStore.Dispose();
            }
        }

        public static MyInMemoryWaveBank Create(AudioEngine engine, string filename)
        {
            DataStream ds;
            using (FileStream fs = File.OpenRead(filename))
            {
                ds = new DataStream((int)fs.Length, true, true);
                fs.CopyTo(ds);
                ds.Position = 0;
            }
            var result = new MyInMemoryWaveBank(engine, ds);
            result.m_backingStore = ds;
            return result;
        }
    }
}
