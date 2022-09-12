cmake_minimum_required(VERSION 3.5)

project(tf2_dotnet)

find_package(ament_cmake_export_assemblies REQUIRED)

find_package(ament_cmake REQUIRED)
find_package(rcl REQUIRED)
find_package(rcl_interfaces REQUIRED)
find_package(rmw REQUIRED)
find_package(rmw_implementation REQUIRED)
find_package(rmw_implementation_cmake REQUIRED)
find_package(rosidl_generator_c REQUIRED)

find_package(rclcpp REQUIRED)
find_package(tf2 REQUIRED)
find_package(tf2_ros REQUIRED)
find_package(std_msgs REQUIRED)
find_package(geometry_msgs REQUIRED)
find_package(tf2_msgs REQUIRED)
find_package(builtin_interfaces REQUIRED)
find_package(rcldotnet REQUIRED)

find_package(dotnet_cmake_module REQUIRED)
find_package(DotNETExtra REQUIRED)

find_package(rclcpp REQUIRED)
find_package(rcldotnet_common REQUIRED)

if(NOT WIN32)
  if(CMAKE_CXX_COMPILER_ID STREQUAL "GNU")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -Wl,--no-undefined")
  elseif(CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -Wl,-undefined,error")
  endif()
endif()

set(CSHARP_TARGET_FRAMEWORK "netstandard2.0")

set(CS_SOURCES
  Exceptions.cs
  Interop.cs
  SafeBufferCoreHandle.cs
  tf2_dotnet.cs
  Tf2ExceptionHelper.cs
  TransformBuffer.cs
  TransformListener.cs
)

find_package(rcldotnet_common REQUIRED)
#foreach(_assembly_dep ${rcldotnet_common_ASSEMBLIES_DLL})
#  list(APPEND _assembly_deps_dll "${_assembly_dep}")
#endforeach()

set(_assembly_deps_dll
    ${rcldotnet_common_ASSEMBLIES_DLL}
    ${rcldotnet_ASSEMBLIES_DLL}
    ${std_msgs_ASSEMBLIES_DLL}
    ${geometry_msgs_ASSEMBLIES_DLL}
    ${tf2_msgs_ASSEMBLIES_DLL}
    ${test_msgs_ASSEMBLIES_DLL}
    ${builtin_interfaces_ASSEMBLIES_DLL}
    ${rcldotnet_ASSEMBLIES_DLL}
)


add_dotnet_library(${PROJECT_NAME}_assemblies
  SOURCES
  ${CS_SOURCES}
  INCLUDE_DLLS
  ${_assembly_deps_dll}
)

install_dotnet(${PROJECT_NAME}_assemblies DESTINATION lib/${PROJECT_NAME}/dotnet)
ament_export_assemblies_dll("lib/${PROJECT_NAME}/dotnet/${PROJECT_NAME}_assemblies.dll")

set(_native_sources "tf2_dotnet")

foreach(_target_name ${_native_sources})
  add_library(${_target_name} SHARED
    ${_target_name}.cpp
  )
  set_target_properties(${_target_name}
    PROPERTIES
    OUTPUT_NAME ${_target_name}_native)
  ament_target_dependencies(${_target_name}
    "builtin_interfaces"
    "rcl"
    "rclcpp"
    "rosidl_generator_c"
    "rosidl_typesupport_c"
    "tf2"
    "tf2_ros"
    "std_msgs"
    "geometry_msgs"
    "builtin_interfaces"
  )
  ament_export_libraries(${_target_name}_native)

  install(TARGETS ${_target_name}
    ARCHIVE DESTINATION lib
    LIBRARY DESTINATION lib
    RUNTIME DESTINATION bin
  )
endforeach()

set(_assemblies_dep_dlls
    ${rcldotnet_common_ASSEMBLIES_DLL}
    ${rcldotnet_ASSEMBLIES_DLL}
    ${std_msgs_ASSEMBLIES_DLL}
    ${geometry_msgs_ASSEMBLIES_DLL}
    ${test_msgs_ASSEMBLIES_DLL}
    ${builtin_interfaces_ASSEMBLIES_DLL}
    ${rcldotnet_ASSEMBLIES_DLL}
)


ament_export_dependencies(ament_cmake)
ament_export_dependencies(builtin_interfaces)
ament_export_dependencies(geometry_msgs)
ament_export_dependencies(tf2_msgs)
ament_export_dependencies(rcl)
ament_export_dependencies(rosidl_generator_c)
ament_export_dependencies(rosidl_typesupport_c)

if(BUILD_TESTING)
  find_package(ament_lint_auto REQUIRED)
  ament_lint_auto_find_test_dependencies()
endif()

ament_package()
