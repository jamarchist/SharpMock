using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SharpMock.Core.Interception.Registration
{
    public class ReplaceableCodeInfoSerializer
    {
        private readonly string path;

        public ReplaceableCodeInfoSerializer(string path)
        {
            this.path = path;
        }

        public ReplaceableCodeInfo DeserializeAllSpecifications()
        {
            var assemblyPath = Path.GetDirectoryName(path);
            var files = Directory.GetFiles(assemblyPath, "*.SharpMock.SerializedSpecifications.xml");

            var aggregate = new ReplaceableCodeInfo();
            foreach (var specList in files)
            {
                var serializer = new XmlSerializer(typeof(ReplaceableCodeInfo));
                using (var fileStream = File.Open(specList, FileMode.Open))
                {
                    var deserializedInfo = serializer.Deserialize(fileStream) as ReplaceableCodeInfo;
                    aggregate = deserializedInfo.Merge(aggregate);
                    fileStream.Close();
                }
            }

            return aggregate;
        }

        public void SerializeSpecifications(string filename, ReplaceableCodeInfo specs)
        {
            var serializer = new XmlSerializer(typeof(ReplaceableCodeInfo));
            using (var binFile = File.Create(Path.Combine(path, filename)))
            {
                serializer.Serialize(binFile, specs);
                binFile.Close();
            }
        }
    }
}