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
  int valid;
};

tf2_ros::TransformListener *tf2_transform_listener;
tf2::BufferCore *tf2_buffer_core;

Tf2DotnetTransformStamped
tf2_lookup_transform(
    const char * frame_from, const char * frame_to,
    const tf2::TimePoint& tp);

TF2DOTNET_EXPORT
void TF2DOTNET_CDECL native_tf2_init ();

TF2DOTNET_EXPORT
void TF2DOTNET_CDECL native_tf2_add_transform (int32_t sec, uint32_t nanosec,
  const char * frame_id, const char * child_frame_id,
  double trans_x, double trans_y, double trans_z,
  double rot_x, double rot_y, double rot_z, double rot_w, int32_t is_static);

TF2DOTNET_EXPORT
Tf2DotnetTransformStamped TF2DOTNET_CDECL native_tf2_lookup_transform (
  const char * frame_from, const char * frame_to,
  int32_t sec, uint32_t nanosec);

TF2DOTNET_EXPORT
Tf2DotnetTransformStamped TF2DOTNET_CDECL native_tf2_lookup_last_transform (
  const char * frame_from, const char * frame_to);
}
#endif // TF2_DOTNET_H
