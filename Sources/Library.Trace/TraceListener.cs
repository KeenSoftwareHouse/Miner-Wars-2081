using System.Diagnostics;
#if DEBUG || DEVELOP
    using TraceTool;
#endif

namespace KeenSoftwareHouse.Library.Trace
{
    

    /// <summary>
    /// Advanced TTrace listener with watches and flush support.
    /// </summary>
    public class TraceListener : 
#if DEBUG || DEVELOP
         TTraceListener
#else
        System.Diagnostics.TraceListener 
#endif

    {
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TraceListener"/> is reclaimed by garbage collection.
        /// </summary>
        ~TraceListener()
        {
            Dispose(true);
        }

        #region Overrides of TTraceListener

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Diagnostics.TraceListener"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected 
#if DEBUG || DEVELOP
            override 
#endif
            void Dispose(bool disposing)
        {
            Flush();

#if DEBUG || DEVELOP
            base.Dispose(disposing);
#endif
        }

        /// <summary>
        /// When overridden in a derived class, closes the output stream so it no longer receives tracing or debugging output.
        /// </summary>
        public 
#if DEBUG || DEVELOP
            override 
#endif
            void Close()
        {
#if DEBUG || DEVELOP
            TTrace.Flush();
            TTrace.Stop();
#endif
        }

        /// <summary>
        /// When overridden in a derived class, flushes the output buffer.
        /// </summary>
        public 
#if DEBUG || DEVELOP
            override 
#endif
            void Flush()
        {
#if DEBUG || DEVELOP
            TTrace.Flush();
#endif
        }

        /// <summary>
        /// Emits an error message to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        public 
#if DEBUG || DEVELOP
            override 
#endif
            void Fail(string message)
        {
#if DEBUG || DEVELOP
            Trace.Default.Error.Send(message);
#endif
        }

        /// <summary>
        /// Emits an error message and a detailed error message to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        public 
#if DEBUG || DEVELOP
            override 
#endif
            void Fail(string message, string detailMessage)
        {
#if DEBUG || DEVELOP
            Trace.Default.Send(message, detailMessage);
#endif
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Write(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// Writes the value of the object's <see cref="M:System.Object.ToString"/> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"/> whose fully qualified class name you want to write.</param>
        public
#if DEBUG || DEVELOP
            override
#endif
 void Write(object o)
        {
            WriteLine(o);
        }

        /// <summary>
        /// Writes a category name and a message to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public
#if DEBUG || DEVELOP
            override
#endif
 
            void Write(string message, string category)
        {
            WriteLine(message, category);
        }

        /// <summary>
        /// Writes a category name and the value of the object's <see cref="M:System.Object.ToString"/> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"/> whose fully qualified class name you want to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public
#if DEBUG || DEVELOP
            override
#endif
 void Write(object o, string category)
        {
            WriteLine(o, category);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void WriteLine(string message)
        {
#if DEBUG || DEVELOP
            if (message.StartsWith("W:"))
            {
                Trace.Default.Warning.Send(message.Substring(2));
            }
            else if (message.StartsWith("E:"))
            {
                Trace.Default.Error.Send(message.Substring(2));
            }
            else
            {
                Trace.Default.Debug.Send(message);
            }
#endif
        }

        /// <summary>
        /// Writes the value of the object's <see cref="M:System.Object.ToString"/> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class, followed by a line terminator.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"/> whose fully qualified class name you want to write.</param>
        public
#if DEBUG || DEVELOP
            override
#endif
 void WriteLine(object o)
        {
#if DEBUG || DEVELOP
            Trace.Default.Debug.SendObject(string.Format("{0}:{1}", o.GetType().Name, o.GetHashCode()), o);
#endif
        }

        /// <summary>
        /// Writes a category name and a message to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public
#if DEBUG || DEVELOP
            override
#endif
 
            void WriteLine(string message, string category)
        {
#if DEBUG || DEVELOP
            if (message.StartsWith("W:"))
            {
                Trace.Default.Warning.Send(category, message.Substring(2));
            }
            else if (message.StartsWith("E:"))
            {
                Trace.Default.Error.Send(category, message.Substring(2));
            }
            else
            {
                Trace.Default.Debug.Send(category, message);
            }
#endif
        }

        /// <summary>
        /// Writes a category name and the value of the object's <see cref="M:System.Object.ToString"/> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"/> class, followed by a line terminator.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"/> whose fully qualified class name you want to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public
#if DEBUG || DEVELOP
            override
#endif
 void WriteLine(object o, string category)
        {
#if DEBUG || DEVELOP
            Trace.Default.Debug.SendObject(category, o);
#endif
        }

        #endregion
    }
}