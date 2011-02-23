using System.Diagnostics;
using System.Globalization;

namespace Abilitics.SearchPoint.Engine.Infrastructure
{
    internal static class CultureEx
    {
        public static CultureInfo Current
        {
            [DebuggerStepThrough]
            get
            {
                return CultureInfo.CurrentUICulture;
            }
        }

        public static CultureInfo Invariant
        {
            [DebuggerStepThrough]
            get
            {
                return CultureInfo.InvariantCulture;
            }
        }
    }
}
