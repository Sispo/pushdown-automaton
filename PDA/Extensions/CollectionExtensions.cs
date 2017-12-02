using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class CollectionExtensions
    {
        public static T[] Copy<T>(this T[] array)
        {
            T[] copy = new T[array.Length];
            array.CopyTo(copy, 0);
            return copy;
        }

        public static T[] CopyExcludingFirst<T>(this T[] array)
        {
            T[] copy = new T[array.Length - 1];
            for(int i = 1; i < array.Length; i++)
            {
                copy[i - 1] = array[i];
            }
            return copy;
        }

        public static Stack<T> Clone<T>(this Stack<T> stack)
        {
            return new Stack<T>(new Stack<T>(stack));
        }
    }
}
