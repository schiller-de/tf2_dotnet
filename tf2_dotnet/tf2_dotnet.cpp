#include <assert.h>
#include <stdlib.h>
#include <iostream>

#include <rcl/error_handling.h>
#include <rcl/node.h>
#include <rcl/rcl.h>
#include <rmw/rmw.h>

#include "tf2_dotnet.h"

extern "C" {

void tf2_convert_exception(Tf2DotnetExceptionType * exception_type, char * exception_message_buffer)
{
  // See https://stackoverflow.com/a/48036486, this is called a Lippincott function.
  try
  {
    // This re-throws the exception from the catch block in the calling function.
    // This allows to match again to the different types.
    throw;
  }
  catch (const tf2::LookupException& e)
  {
    *exception_type = TF2_DOTNET_LOOKUP_EXCEPTION;
    strncpy(exception_message_buffer, e.what(), TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
  catch (const tf2::ConnectivityException& e)
  {
    *exception_type = TF2_DOTNET_CONNECTIVITY_EXCEPTION;
    strncpy(exception_message_buffer, e.what(), TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
  catch (const tf2::ExtrapolationException& e)
  {
    *exception_type = TF2_DOTNET_EXTRAPOLATION_EXCEPTION;
    strncpy(exception_message_buffer, e.what(), TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
  catch (const tf2::InvalidArgumentException& e)
  {
    *exception_type = TF2_DOTNET_INVALID_ARGUMENT_EXCEPTION;
    strncpy(exception_message_buffer, e.what(), TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
  catch (const tf2::TimeoutException& e)
  {
    *exception_type = TF2_DOTNET_TIMEOUT_EXCEPTION;
    strncpy(exception_message_buffer, e.what(), TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
  catch (const tf2::TransformException& e)
  {
    *exception_type = TF2_DOTNET_TRANSFORM_EXCEPTION;
    strncpy(exception_message_buffer, e.what(), TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
  catch (const std::runtime_error& e)
  {
    *exception_type = TF2_DOTNET_EXCEPTION;
    strncpy(exception_message_buffer, e.what(), TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
  catch (...)
  {
    *exception_type = TF2_DOTNET_UNKNOWN_EXCEPTION;
    memset(exception_message_buffer, 0, TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH);
  }
}

tf2::BufferCore *
tf2_dotnet_native_buffer_core_create(
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer)
{
  try
  {
    return new tf2::BufferCore();
  }
  catch (...)
  {
    tf2_convert_exception(exception_type, exception_message_buffer);
    return nullptr;
  }
}

void
tf2_dotnet_native_buffer_core_destroy(
  tf2::BufferCore * buffer_core,
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer)
{
  try
  {
    delete buffer_core;
  }
  catch (...)
  {
    tf2_convert_exception(exception_type, exception_message_buffer);
  }
}

int32_t
tf2_dotnet_native_buffer_core_set_transform(
  tf2::BufferCore * buffer_core,
  int32_t sec,
  uint32_t nanosec,
  const char * frame_id,
  const char * child_frame_id,
  double translation_x,
  double translation_y,
  double translation_z,
  double rotation_x,
  double rotation_y,
  double rotation_z,
  double rotation_w,
  const char * authority,
  int32_t is_static,
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer)
{
  try
  {
    geometry_msgs::msg::TransformStamped transform;

    transform.header.stamp.sec = sec;
    transform.header.stamp.nanosec = nanosec;
    transform.header.frame_id = std::string(frame_id);
    transform.child_frame_id = std::string(child_frame_id);
    transform.transform.translation.x = translation_x;
    transform.transform.translation.y = translation_y;
    transform.transform.translation.z = translation_z;
    transform.transform.rotation.x = rotation_x;
    transform.transform.rotation.y = rotation_y;
    transform.transform.rotation.z = rotation_z;
    transform.transform.rotation.w = rotation_w;

    bool result = buffer_core->setTransform(transform, std::string(authority), is_static == 1);
    return result ? 1 : 0;
  }
  catch (...)
  {
    tf2_convert_exception(exception_type, exception_message_buffer);
    return 0;
  }
}

Tf2DotnetTransformStamped
tf2_dotnet_native_buffer_core_lookup_transform(
  tf2::BufferCore * buffer_core,
  const char * target_frame,
  const char * source_frame,
  int32_t sec,
  uint32_t nanosec,
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer)
{
  try
  {
    Tf2DotnetTransformStamped result;

    tf2::TimePoint time = tf2::TimePoint(std::chrono::seconds(sec) + std::chrono::nanoseconds(nanosec));

    geometry_msgs::msg::TransformStamped transform =
      buffer_core->lookupTransform(std::string(target_frame), std::string(source_frame), time);

    result.sec = transform.header.stamp.sec;
    result.nanosec = transform.header.stamp.nanosec;
    result.translation_x = transform.transform.translation.x;
    result.translation_y = transform.transform.translation.y;
    result.translation_z = transform.transform.translation.z;
    result.rotation_x = transform.transform.rotation.x;
    result.rotation_y = transform.transform.rotation.y;
    result.rotation_z = transform.transform.rotation.z;
    result.rotation_w = transform.transform.rotation.w;

    return result;
  }
  catch (...)
  {
    tf2_convert_exception(exception_type, exception_message_buffer);
    return Tf2DotnetTransformStamped();
  }
}

}
