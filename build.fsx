#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

let version = "0.1.1"

Target "Clean" (fun _ ->
    !! "*/bin/"
    |> CleanDirs
)

Target "Build" (fun _ ->
    !! "*/*.csproj"
      |> MSBuildRelease null "Build"
      |> Log "AppBuild-Output: "
)

Target "Package" (fun _ ->
    "MonoTouch.SlideoutNavigation/MonoTouch.SlideoutNavigation.nuspec"
    |> NuGet (fun p -> 
        {p with 
            Project = "MonoTouch.SlideoutNavigation"
            WorkingDir = "MonoTouch.SlideoutNavigation/bin"
            OutputPath = "MonoTouch.SlideoutNavigation/bin"
            Files = [("Release/*.dll", Some "lib/Xamarin.iOS10", None)]
            Version = version
            ToolPath = "nuget"
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey"
        }))

"Clean"
  ==> "Build"
  ==> "Package"

RunTargetOrDefault "Build"
