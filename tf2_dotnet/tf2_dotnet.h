#ifndef TF2_DOTNET_H
#define TF2_DOTNET_H

#include "tf2_dotnet_macros.h"

#include "tf2_ros/transform_listener.h"

extern "C" {

struct Tf2DotnetTransformStamped {
  int32_t sec;
  uint32_t nanosec;
  double translation_x;
  double translation_y;
  double translation_z;
  double rotation_x;
  double rotation_y;
  double rotation_z;
  double rotation_w;
};

// Mirror of TF2ExceptionType enum in C#, keep in Sync!
enum Tf2DotnetExceptionType {
  TF2_DOTNET_NO_EXCEPTION = 0,
  TF2_DOTNET_LOOKUP_EXCEPTION = 1,
  TF2_DOTNET_CONNECTIVITY_EXCEPTION = 2,
  TF2_DOTNET_EXTRAPOLATION_EXCEPTION = 3,
  TF2_DOTNET_INVALID_ARGUMENT_EXCEPTION = 4,
  TF2_DOTNET_TIMEOUT_EXCEPTION = 5,
  TF2_DOTNET_TRANSFORM_EXCEPTION = 6,
  TF2_DOTNET_EXCEPTION = 1000,
  TF2_DOTNET_UNKNOWN_EXCEPTION = 1001, 
};

// Mirror of TF2ExceptionHelper.MessageBufferLength in C#, keep in Sync!
#define TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH 256

void
tf2_convert_exception(
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer);

TF2DOTNET_EXPORT
tf2::BufferCore *
TF2DOTNET_CDECL
tf2_dotnet_native_buffer_core_create(
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer);

TF2DOTNET_EXPORT
void
TF2DOTNET_CDECL
tf2_dotnet_native_buffer_core_destroy(
  tf2::BufferCore * buffer_core,
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer);

TF2DOTNET_EXPORT
int32_t
TF2DOTNET_CDECL
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
  char * exception_message_buffer);

TF2DOTNET_EXPORT
Tf2DotnetTransformStamped
TF2DOTNET_CDECL
tf2_dotnet_native_buffer_core_lookup_transform(
  tf2::BufferCore * buffer_core,
  const char * target_frame,
  const char * source_frame,
  int32_t sec,
  uint32_t nanosec,
  Tf2DotnetExceptionType * exception_type,
  char * exception_message_buffer);

}

#endif // TF2_DOTNET_H
