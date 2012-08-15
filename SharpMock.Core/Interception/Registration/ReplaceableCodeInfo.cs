using System;
using System.Collections.Generic;
using SharpMock.Core.Utility;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableCodeInfo
    {
        public List<ReplaceableMethodInfo> Methods { get; set; }
        public List<ReplaceableFieldInfo> FieldAccessors { get; set; }
        public List<ReplaceableFieldInfo> FieldAssignments { get; set; } 

        public ReplaceableCodeInfo()
        {
            Methods = new List<ReplaceableMethodInfo>();
            FieldAccessors = new List<ReplaceableFieldInfo>();
            FieldAssignments = new List<ReplaceableFieldInfo>();
        }

        public ReplaceableCodeInfo Merge(ReplaceableCodeInfo other)
        {
            var mergeResult = new ReplaceableCodeInfo();

            var methodList = new List<ReplaceableMethodInfo>();
            var fieldAccessorList = new List<ReplaceableFieldInfo>();
            var fieldAssignmentList = new List<ReplaceableFieldInfo>();

            methodList.AddRange(Methods);
            var otherMethodList = other.Methods.Where(m => !methodList.Contains(m));
            methodList.AddRange(otherMethodList);

            fieldAccessorList.AddRange(FieldAccessors);
            var otherFieldAccessorList = other.FieldAccessors.Where(f => !fieldAccessorList.Contains(f));
            fieldAccessorList.AddRange(otherFieldAccessorList);

            fieldAssignmentList.AddRange(FieldAssignments);
            var otherFieldAssignmentList = other.FieldAssignments.Where(f => !fieldAssignmentList.Contains(f));
            fieldAssignmentList.AddRange(otherFieldAssignmentList);

            mergeResult.Methods = methodList;
            mergeResult.FieldAccessors = fieldAccessorList;
            mergeResult.FieldAssignments = fieldAssignmentList;

            return mergeResult;
        }
    }
}