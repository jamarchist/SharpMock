using System.Diagnostics;
using Microsoft.VisualStudio.DebuggerVisualizers;
using SharpMock.Core.PostCompiler.Replacement;

[assembly: DebuggerVisualizer(
    typeof(SharpMock.VisualStudio.Debugging.RegistryDebuggerVisualizer),
    typeof(VisualizerObjectSource),
    Target = typeof(ReplacementRegistry),
    Description = "Replacement Registry Viewer")]
namespace SharpMock.VisualStudio.Debugging
{
    public class RegistryDebuggerVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var registry = objectProvider.GetObject() as ReplacementRegistry;
            
            var form = new RegistryForm(registry);
            windowService.ShowDialog(form);
        }
    }
}
