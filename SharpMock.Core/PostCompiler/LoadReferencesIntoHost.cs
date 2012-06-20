namespace SharpMock.Core.PostCompiler
{
    public class LoadReferencesIntoHost : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            LoadReferencedUnits(context.AssemblyToAlter, context.Host);
        }

        private static void LoadReferencedUnits(Microsoft.Cci.IUnit unit, Microsoft.Cci.IMetadataHost host)
        {
            foreach (var reference in unit.UnitReferences)
            {
                if (reference != null)
                {
                    var loadedUnit = host.LoadUnit(reference.UnitIdentity);    
                    LoadReferencedUnits(loadedUnit, host);
                }
            }            
        }
    }
}