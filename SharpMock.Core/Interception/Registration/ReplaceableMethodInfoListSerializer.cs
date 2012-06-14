using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SharpMock.Core.Interception.Registration
{
    public class ReplaceableMethodInfoListSerializer
    {
        private readonly string path;

        public ReplaceableMethodInfoListSerializer(string path)
        {
            this.path = path;
        }

        public List<ReplaceableMethodInfo> DeserializeAllSpecifiedMethods()
        {
            var assemblyPath = Path.GetDirectoryName(path);
            var files = Directory.GetFiles(assemblyPath, "*.SharpMock.SerializedSpecifications.xml");

            var aggregateList = new List<ReplaceableMethodInfo>();
            foreach (var specList in files)
            {
                var serializer = new XmlSerializer(typeof(List<ReplaceableMethodInfo>));
                using (var fileStream = File.Open(specList, FileMode.Open))
                {
                    var deserializedList = serializer.Deserialize(fileStream) as List<ReplaceableMethodInfo>;
                    aggregateList.AddRange(deserializedList);
                    fileStream.Close();
                }
            }

            return aggregateList;
        }

        public void SerializeSpecifications(string filename, IList<ReplaceableMethodInfo> specs)
        {
            var specList = new List<ReplaceableMethodInfo>(specs);
            var serializer = new XmlSerializer(typeof(List<ReplaceableMethodInfo>));
            using (var binFile = File.Create(Path.Combine(path, filename)))
            {
                serializer.Serialize(binFile, specList);
                binFile.Close();
            }
        }
    }
}