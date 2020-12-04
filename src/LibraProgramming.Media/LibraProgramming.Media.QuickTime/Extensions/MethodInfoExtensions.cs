using System;
using System.Reflection;

namespace LibraProgramming.Media.QuickTime.Extensions
{
    internal static class MethodInfoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
        {
            if (null == method)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return method.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }
    }
}