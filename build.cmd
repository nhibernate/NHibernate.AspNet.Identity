rem install: choco install nuget.commandline
nuget restore source\NHibernate.AspNet.Identity.sln
"%PROGRAMFILES(x86)%\MSBuild\12.0\bin\msbuild.exe" "source\NHibernate.AspNet.Identity.sln" /t:Clean;Rebuild /p:Configuration=Release