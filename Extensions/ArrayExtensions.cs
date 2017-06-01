using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul.Extensions
{
    public static class ArrayExtensions
    {
        public static bool TryGetFromBuffer<T>(this Array16<T> array, uint index, out T result)
        {
            if (index >= array.m_buffer.Length)
            {
                result = default(T);
                return false;
            }

            result = array.m_buffer[index];
            return true;
        }
    }
}
