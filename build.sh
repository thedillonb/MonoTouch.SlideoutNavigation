clear
nuget "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
mono packages/FAKE/tools/FAKE.exe build.fsx
