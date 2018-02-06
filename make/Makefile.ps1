Define-Step -Name 'Compile tutorial builder' -Target 'all' -Body {
	call dotnet build '-c' Release make\tutorial-builder\TutorialBuilder.sln
	call dotnet test '-c' Release --no-build make\tutorial-builder\TutorialBuilder.Tests\TutorialBuilder.Tests.csproj
	Copy-Item make\tutorial-builder\TutorialBuilder\bin\Release\net46\TutorialBuilder.exe make
}

Define-Step -Name 'Compile tutorials' -Target 'all' -Body {
	call make\TutorialBuilder.exe
}