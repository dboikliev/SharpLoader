using System;
using System.Threading;

namespace SharpLoader.Utils
{
    public static class EventUtils
    {
        public static void RaiseEvent<T>(object sender, T args, ref EventHandler<T> eventHandler) where T : EventArgs
        {
            var handler = Interlocked.CompareExchange(ref eventHandler, null, null);
            handler?.Invoke(sender, args);
        }
    }
}
