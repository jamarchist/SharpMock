using System;
using System.Collections.Generic;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableCodeInfo
    {
        public List<ReplaceableMethodInfo> Methods { get; set; }
        public List<ReplaceableFieldAccessorInfo> FieldAccessors { get; set; }

        public ReplaceableCodeInfo()
        {
            Methods = new List<ReplaceableMethodInfo>();
            FieldAccessors = new List<ReplaceableFieldAccessorInfo>();
        }

        public ReplaceableCodeInfo Merge(ReplaceableCodeInfo other)
        {
            var mergeResult = new ReplaceableCodeInfo();

            var methodList = new List<ReplaceableMethodInfo>();
            var fieldAccessorList = new List<ReplaceableFieldAccessorInfo>();

            methodList.AddRange(Methods);
            methodList.AddRange(other.Methods);

            fieldAccessorList.AddRange(FieldAccessors);
            fieldAccessorList.AddRange(other.FieldAccessors);

            mergeResult.Methods = methodList;
            mergeResult.FieldAccessors = fieldAccessorList;

            return mergeResult;
        }
    }
}