using System;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction
{
    internal static class AccessibilityExtensions
    {
        internal static TypeMemberVisibility ToTypeMemberVisibility(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Public:
                    return TypeMemberVisibility.Public;
                case Accessibility.Private:
                    return TypeMemberVisibility.Private;
                case Accessibility.Internal:
                    return TypeMemberVisibility.Assembly;
                case Accessibility.Protected:
                    return TypeMemberVisibility.Family;
                case Accessibility.ProtectedInternal:
                    return TypeMemberVisibility.FamilyOrAssembly;
                default:
                    throw new ArgumentException("Unrecognized Accessibility value.", "accessibility");
            }
        }
    }
}