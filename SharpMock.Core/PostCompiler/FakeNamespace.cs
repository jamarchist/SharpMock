using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Helpers;
using SharpMock.PostCompiler.Core.CciExtensions;

namespace SharpMock.Core.PostCompiler
{
    internal class FakeNamespace
    {
        private readonly IMetadataHost host;
        private readonly ILogger log;
        private readonly Module module;
        private readonly NestedUnitNamespace fake;
        private readonly IDictionary<string, NestedUnitNamespace> namespaces = new Dictionary<string, NestedUnitNamespace>();
        private readonly IDictionary<string, NamespaceTypeDefinition> classes = new Dictionary<string, NamespaceTypeDefinition>(); 

        public FakeNamespace(Module module, IMetadataHost host, ILogger log)
        {
            this.module = module;
            this.host = host;
            this.log = log;

            fake = module.UnitNamespaceRoot.AddNestedNamespace("<Fake>", host);
        }

        public IDictionary<string, NamespaceTypeDefinition> Classes { get { return classes; } } 

        public void AddNamespaces(string dotDelimitedNamespaces)
        {
            log.WriteTrace("Adding fake namespace: '{0}'.", dotDelimitedNamespaces);
            var allNamespaces = dotDelimitedNamespaces.Split(new char[] {'.'});
            AddNamespaces(allNamespaces);
        }

        public void AddNamespaces(ReverseStringBuilder reversedNamespaces)
        {
            log.WriteTrace("Adding fake namespace: '{0}'.", reversedNamespaces);
            var allNamespaces = reversedNamespaces.ToStringArray();
            AddNamespaces(allNamespaces);
        }

        private void AddNamespaces(string[] allNamespaces)
        {
            var stack = StackedNamespaces(allNamespaces);

            foreach (var ns in stack)
            {
                if (!namespaces.ContainsKey(ns.Key))
                {
                    NestedUnitNamespace root = null;
                    if (namespaces.ContainsKey(ns.Value.Root))
                        root = namespaces[ns.Value.Root];
                    else
                        root = fake.AddNestedNamespace(ns.Value.Root, host);

                    var newNamespace = root.AddNestedNamespace(ns.Value.LastElement, host);
                    namespaces.Add(ns.Key, newNamespace);
                }
            }            
        }

        public void AddClass(string fullNamespace, string className)
        {
            var fullyQualifiedName = String.Format("{0}.{1}", fullNamespace, className);
            log.WriteTrace("Adding fake class: '{0}'.", fullyQualifiedName);
            if (!classes.ContainsKey(fullyQualifiedName))
            {
                var ns = namespaces[fullNamespace];
                var newClass = ns.AddStaticClass(module, className, host);

                classes.Add(fullyQualifiedName, newClass);
            }
        }

        private IDictionary<string, NamespaceInfo> StackedNamespaces(string[] namespaceElements)
        {
            var stack = new Dictionary<string, NamespaceInfo>();
            for (var elementIndex = 0; elementIndex < namespaceElements.Length; elementIndex++)
            {
                var stackElement = StackedNamespace(namespaceElements, elementIndex);

                var namespaceInfo = new NamespaceInfo();
                namespaceInfo.FullNamespace = stackElement;
                namespaceInfo.LastElement = namespaceElements[elementIndex];
                namespaceInfo.Root = StackedNamespace(namespaceElements, elementIndex - 1);

                stack.Add(namespaceInfo.FullNamespace, namespaceInfo);
            }

            return stack;
        }

        private string StackedNamespace(string[] elements, int lastElementIndex)
        {
            var stackElement = new StringBuilder();
            for (var subIndex = 0; subIndex <= lastElementIndex; subIndex++)
            {
                stackElement.Append(elements[subIndex]);
                stackElement.Append('.');
            }

            var stack = stackElement.ToString();

            if (stack.Length > 0)
            {
                return stack.Trim('.');
            }

            return stack;
        }

        private class NamespaceInfo
        {
            public string FullNamespace { get; set; }
            public string LastElement { get; set; }
            public string Root { get; set; }
        }
    }
}