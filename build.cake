// //*** Description ***

// // How to run the build script ?
// // Default target from powershell: .\build.ps1
// // Specific target: .\build.ps1 --target="Test"
// // Specific target without dependencies: .\build.ps1 --target="Test" --exclusive

#load nuget:?package=PDS.Cake.Recipe&version=1.5.0

Build.SetParameters(
    context: Context,
    name: "PADS Space App",
    solutionFile: "Space.sln",
    sonarKey: "PADS-Space-App"
    );

RunTarget(Build.Target);
