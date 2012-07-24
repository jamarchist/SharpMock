using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Syntax
{
    public static class ReplacementOptionExtensions
    {
        public static IReplacementOptions CallOriginal(this IReplacementOptions replace)
        {
            var replaceAsInterceptor = replace.AsInterceptor();
            replaceAsInterceptor.With((IInvocation i) =>
            {
                i.Return = i.OriginalCall.SafeInvoke(i.Arguments);
            });

            return replaceAsInterceptor;
        }
    }
}