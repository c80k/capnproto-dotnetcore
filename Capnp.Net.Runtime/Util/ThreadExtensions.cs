using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Capnp.Util
{
    internal static class ThreadExtensions
    {
        class ThreadExtensionsLoggingContext
        {
            public ILogger Logger { get; } = Logging.CreateLogger<ThreadExtensionsLoggingContext>();
        }

        static Lazy<ThreadExtensionsLoggingContext> LoggingContext = new Lazy<ThreadExtensionsLoggingContext>(
            () => new ThreadExtensionsLoggingContext(),
            LazyThreadSafetyMode.PublicationOnly);

        public static void SafeJoin(this Thread thread, ILogger? logger = null, int timeout = 5000)
        {
            if (!thread.Join(timeout))
            {
                logger ??= LoggingContext.Value.Logger;

                string name = thread.Name ?? thread.ManagedThreadId.ToString();

                try
                {
                    logger.LogError($"Unable to join thread {name}. Thread is in state {thread.ThreadState}.");
                    thread.Interrupt();
                    if (!thread.Join(timeout))
                    {
                        logger.LogError($"Still unable to join thread {name} after Interrupt(). Thread is in state {thread.ThreadState}.");
                    }
                }
                catch
                {
                }
            }
        }
    }
}
