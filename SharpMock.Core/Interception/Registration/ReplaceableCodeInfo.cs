using System;
using System.Collections.Generic;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableCodeInfo
    {
        public List<ReplaceableMethodInfo> Methods { get; set; }
        public List<ReplaceableFieldAccessorInfo> FieldAccessors { get; set; }
        public List<ReplaceableFieldAccessorInfo> FieldAssignments { get; set; } 

        public ReplaceableCodeInfo()
        {
            Methods = new List<ReplaceableMethodInfo>();
            FieldAccessors = new List<ReplaceableFieldAccessorInfo>();
            FieldAssignments = new List<ReplaceableFieldAccessorInfo>();
        }

        public ReplaceableCodeInfo Merge(ReplaceableCodeInfo other)
        {
            var mergeResult = new ReplaceableCodeInfo();

            var methodList = new List<ReplaceableMethodInfo>();
            var fieldAccessorList = new List<ReplaceableFieldAccessorInfo>();
            var fieldAssignmentList = new List<ReplaceableFieldAccessorInfo>();

            methodList.AddRange(Methods);
            methodList.AddRange(other.Methods);

            fieldAccessorList.AddRange(FieldAccessors);
            fieldAccessorList.AddRange(other.FieldAccessors);

            fieldAssignmentList.AddRange(FieldAssignments);
            fieldAssignmentList.AddRange(other.FieldAssignments);

            mergeResult.Methods = methodList;
            mergeResult.FieldAccessors = fieldAccessorList;
            mergeResult.FieldAssignments = fieldAssignmentList;

            return mergeResult;
        }
    }
}