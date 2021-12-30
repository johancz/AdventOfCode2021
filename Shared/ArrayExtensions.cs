using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<IEnumerable<string>> Split(this string[] array, string separator)
        {
            for (var i = 0; i < array.Length; i++)
            {
                var subArray = array.Skip(i).TakeWhile(s => s != separator);
                i += subArray.Count();
                yield return subArray;
            }
        }
    }
}
