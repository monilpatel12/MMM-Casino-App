using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibrary
{
    public static class ExtensionMethods
    {
        public static byte[] CombineWith(this byte[] original, params byte[][] arrays)
        {
            var combined = new byte[original.Length + arrays.Sum(arr => arr.Length)];
            Buffer.BlockCopy(original, 0, combined, 0, original.Length);
            var offset = original.Length;
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, combined, offset, array.Length);
                offset += array.Length;
            }
            return combined;
        }
    }
}
