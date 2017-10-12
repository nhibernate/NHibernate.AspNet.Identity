rem install: choco install nuget.commandline
nuget restore source\NHibernate.AspNet.Identity.sln
"%PROGRAMFILES(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\bin\msbuild.exe" "source\NHibernate.AspNet.Identity.sln" /t:Clean;Rebuild /p:Configuration=Release
packages\NUnit.Runners.2.6.4\tools\nunit-console-x86.exe source\NHibernate.AspNet.Identity.Tests\bin\Release\NHibernate.AspNet.Identity.Tests.dll source\NHibernate.AspNet.Web.Specs\bin\Release\NHibernate.AspNet.Web.Specs.dll
nuget pack source\NHibernate.AspNet.Identity\NHibernate.AspNet.Identity.csproj -Prop Configuration=Release
pause