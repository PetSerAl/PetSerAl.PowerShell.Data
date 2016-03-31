#if !NETv35Plus
using System;
using System.Collections;
using System.Collections.Generic;
namespace System {
    public delegate TResult Func<in T, out TResult>(T arg);
}
namespace System.Linq {
    public static class Enumerable {
        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source) {
            if(source==null) {
                throw new ArgumentNullException(nameof(source));
            }
            return source as IEnumerable<TResult>??CastIterator<TResult>(source);
        }
        private static IEnumerable<TResult> CastIterator<TResult>(this IEnumerable source) {
            foreach(object current in source) {
                yield return (TResult)current;
            }
        }
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) {
            if(source==null) {
                throw new ArgumentNullException(nameof(source));
            }
            if(selector==null) {
                throw new ArgumentNullException(nameof(selector));
            }
            return SelectIterator(source, selector);
        }
        private static IEnumerable<TResult> SelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector) {
            foreach(TSource current in source) {
                yield return selector(current);
            }
        }
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source) {
            if(source==null) {
                throw new ArgumentNullException(nameof(source));
            }
            return new List<TSource>(source).ToArray();
        }
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            if(source==null) {
                throw new ArgumentNullException(nameof(source));
            }
            if(predicate==null) {
                throw new ArgumentNullException(nameof(predicate));
            }
            return WhereIterator(source, predicate);
        }
        private static IEnumerable<TSource> WhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach(TSource current in source) {
                if(predicate(current)) {
                    yield return current;
                }
            }
        }
    }
}
namespace System.Runtime.CompilerServices {
    [AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Class|AttributeTargets.Method)]
    public class ExtensionAttribute : Attribute { }
}
#endif
