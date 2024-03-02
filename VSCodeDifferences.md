# Changes to solution to allow it to be edited and run in VSCode

In a Visual Studio Code (VSCode) environment, the **directory.build.props** file is not automatically recognized or used by the build system like it is in Visual Studio with MSBuild. Therefore the contents of directory.build.props was copied into EACH .csproj file within the solution.  Another untested approach would be to create build tasks or build scripts within VSCode that implement all the content of the **directory.build.props** file.

Reference the FAATests.csproj for settings needed to make this work properly in VSCode.

A **.vscode** folder containing a **launch.json** and **tasks.json** will be used to build, publish and launch programs
