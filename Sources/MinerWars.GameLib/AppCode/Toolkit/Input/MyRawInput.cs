using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX.Multimedia;
using SharpDX.RawInput;
using System.Security;
using System.Runtime.InteropServices;
using SharpDX;
using System.IO;
using SharpDX.Win32;

namespace MinerWars.AppCode.Toolkit.Input
{
    delegate void HidInputHandler(ref RawInputHeader header, int hidEventCount, int hidEventSize, MyRawInput.RawInputReader temporaryBuffer);

    static class MyRawInput
    {
        private enum RawInputDataType
        {
            Input = 268435459,
            Header = 268435461,
        }

        private class RawInputMessageFilter : IMessageFilter
        {
            private const int WmInput = 255;

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WmInput)
                    MyRawInput.HandleMessage(m.LParam);
                return false;
            }
        }

        public class RawInputReader
        {
            BinaryReader m_reader;

            public RawInputReader(BinaryReader reader)
            {
                this.m_reader = reader;
            }

            public BinaryReader InitReader()
            {
                m_reader.BaseStream.Position = 0;
                return m_reader;
            }
        }

        private static IntPtr m_filterWindow;
        private static RawInputMessageFilter m_filter;

        private static RawInputReader m_reader;
        private static MemoryStream m_memoryStream;

        public static event Action<MyKeyboardInputArgs> KeyboardInput;
        public static event Action<MyMouseInputArgs> MouseInput;

        /// <summary>
        /// Occures when new hid input raw message is received and handled.
        /// </summary>
        public static event HidInputHandler RawInput;

        static MyRawInput()
        {
            m_memoryStream = new MemoryStream(256); // 256B is good default size
            m_reader = new RawInputReader(new BinaryReader(m_memoryStream));
        }

        // This allocates, however it should be used only when launching app
        public static void RegisterDevice(UsagePage usagePage, UsageId usageId, DeviceFlags flags, IntPtr target)
        {
            Device.RegisterDevice(usagePage, usageId, flags, target, false);
            if (m_filter == null)
            {
                m_filterWindow = target;
                m_filter = new RawInputMessageFilter();
                MessageFilterHook.AddMessageFilter(m_filterWindow, m_filter);
            }
        }

        public static void ClearMessageFilter()
        {
            if (m_filter != null)
            {
                MessageFilterHook.RemoveMessageFilter(m_filterWindow, m_filter);
                m_filter = null;
                m_filterWindow = IntPtr.Zero;
            }
        }

        public static unsafe void HandleMessage(IntPtr rawInputMessagePointer)
        {
            int cbSizeRef = 0;
            GetRawInputData(rawInputMessagePointer, RawInputDataType.Input, IntPtr.Zero, ref cbSizeRef, Utilities.SizeOf<RawInputHeader>());
            if (cbSizeRef == 0)
                return;
            byte* numPtr = stackalloc byte[cbSizeRef];
            GetRawInputData(rawInputMessagePointer, RawInputDataType.Input, (IntPtr)((void*)numPtr), ref cbSizeRef, Utilities.SizeOf<RawInputHeader>());
            RawInput* rawInputPtr = (RawInput*)numPtr;
            switch (rawInputPtr->Header.Type)
            {
                case DeviceType.Mouse:
                    if (MouseInput != null)
                    {
                        MouseInput(new MyMouseInputArgs(ref *rawInputPtr));
                    }
                    break;

                case DeviceType.Keyboard:
                    if (KeyboardInput != null)
                    {
                        KeyboardInput(new MyKeyboardInputArgs(ref *rawInputPtr));
                    }
                    break;

                case DeviceType.HumanInputDevice:
                    ProcessRawInput(ref *rawInputPtr);
                    break;
            }
        }

        private static void ProcessRawInput(ref RawInput rawInput)
        {
            if (RawInput != null)
            {
                int size = rawInput.Data.Hid.Count * rawInput.Data.Hid.SizeHid;

                if (size > 0)
                {
                    if (m_memoryStream.Capacity < size)
                        m_memoryStream.Capacity = size;

                    var buffer = m_memoryStream.GetBuffer();
                    unsafe
                    {
                        fixed (void* to = buffer)
                        {
                            fixed (void* from = &rawInput.Data.Hid.RawData)
                            {
                                SharpDX.Utilities.CopyMemory((IntPtr)to, (IntPtr)from, size * sizeof(byte));
                            }
                        }
                    }
                }

                RawInput(ref rawInput.Header, rawInput.Data.Hid.Count, rawInput.Data.Hid.SizeHid, m_reader);
            }
        }

        private static unsafe int GetRawInputData(IntPtr hRawInput, RawInputDataType uiCommand, IntPtr dataRef, ref int cbSizeRef, int cbSizeHeader)
        {
            int rawInputData;
            fixed (int* numPtr = &cbSizeRef)
                rawInputData = GetRawInputData_((void*)hRawInput, (int)uiCommand, (void*)dataRef, (void*)numPtr, cbSizeHeader);
            return rawInputData;
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", EntryPoint = "GetRawInputData", CallingConvention = CallingConvention.StdCall)]
        private unsafe extern static int GetRawInputData_(void* arg0, int arg1, void* arg2, void* arg3, int arg4);
    }
}
