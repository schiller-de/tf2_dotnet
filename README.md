# TF2 for .NET

This is based on the work from [@fmrico](https://github.com/fmrico) in [this branch](https://github.com/IntelligentRoboticsLabs/geometry2/tree/rcldotnet).
The subfolder `tf2_dotnet` was extracted into this repository to make it separate from `ros2/geometry2`.

This works against the version of `ros2_dotnet` from [this PR](https://github.com/ros2-dotnet/ros2_dotnet/pull/94).

The package `tf2_msgs` must be built with `ros2_dotnet` support, so include the [geometry2 repository](https://github.com/ros2/geometry2/) (with the right branch for your ROS distribution) in your workspace.

This was tested with ROS2 Foxy.
