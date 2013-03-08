using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.RawInput;
using System.Collections;

namespace MinerWars.AppCode.Toolkit.Input
{
    struct MyKeyboardState
    {   
        MyKeyboardBuffer m_buffer;

        public void GetPressedKeys(List<Keys> keys)
        {
            keys.Clear();

            for (int i = 1; i < 256; i++)
            {
                if (m_buffer.GetBit((byte)i))
                    keys.Add((Keys)i);
            }
        }

        public bool IsAnyKeyPressed()
        {
            return m_buffer.AnyBitSet();
        }
        
        void SetKey(Keys key, bool value)
        {
            m_buffer.SetBit((byte)key, value);
        }

        public static MyKeyboardState FromBuffer(MyKeyboardBuffer buffer)
        {
            return new MyKeyboardState() { m_buffer = buffer };
        }

        public void Update(MyKeyboardInputArgs args)
        {
            bool isDown = args.State == SharpDX.RawInput.KeyState.KeyDown || args.State == SharpDX.RawInput.KeyState.SystemKeyDown;
            bool isUp = args.State == SharpDX.RawInput.KeyState.KeyUp || args.State == SharpDX.RawInput.KeyState.SystemKeyUp;

            if (isDown || isUp)
            {
                SetKey(args.Key, isDown);

                // Translate Left/Right shift, alt, control
                var translation = MyKeyTranslationTable.Translate(args.Key, args.ScanCodeFlags, args.MakeCode);
                if (translation != Keys.None)
                    SetKey(translation, isDown);
            }
        }

        public bool IsKeyDown(Keys key)
        {
            return m_buffer.GetBit((byte)key);
        }

        public bool IsKeyUp(Keys key)
        {
            return !IsKeyDown(key);
        }
    }
}