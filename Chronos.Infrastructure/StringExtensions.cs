using System.Linq;

namespace Chronos.Infrastructure
{
    public static class StringExtensions
    {
        public static int HashString(this string text)
        {
            unchecked
            {
                return text.Aggregate(23, (current, c) => current * 31 + c);
            }
        }
    }
}