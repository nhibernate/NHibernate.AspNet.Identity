rem install: choco install nuget.commandline
nuget restore source\NHibernate.AspNet.Identity.sln
"%PROGRAMFILES(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\bin\msbuild.exe" "source\NHibernate.AspNet.Identity.sln" /t:Clean;Rebuild /p:Configuration=Release