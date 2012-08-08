msbuild "C:\Projects\github\SharpMock\SharpMock.VisualStudio.Debugging\SharpMock.VisualStudio.Debugging.csproj" /p:Configuration=Release;Platform=AnyCPU /t:Rebuild

COPY /Y "C:\Projects\github\SharpMock\SharpMock.VisualStudio.Debugging\bin\Release\SharpMock.VisualStudio.Debugging.dll" "C:\Users\Ryan\My Documents\Visual Studio 2010\Visualizers\"