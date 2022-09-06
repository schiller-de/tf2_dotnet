// Copyright 2016-2018 Esteve Fernandez <esteve@apache.org>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#ifndef TF2DOTNET_MACROS_H
#define TF2DOTNET_MACROS_H

#if defined(_MSC_VER)
    #define TF2DOTNET_EXPORT __declspec(dllexport)
    #define TF2DOTNET_IMPORT __declspec(dllimport)
    #if defined(_M_IX86)
        #define TF2DOTNET_CDECL __cdecl
    #else
        #define TF2DOTNET_CDECL
    #endif
#elif defined(__GNUC__)
    #define TF2DOTNET_EXPORT __attribute__((visibility("default")))
    #define TF2DOTNET_IMPORT
    #if defined(__i386__)
        #define TF2DOTNET_CDECL __attribute__((__cdecl__))
    #else
        #define TF2DOTNET_CDECL
    #endif
#else
    #define TF2DOTNET_EXPORT
    #define TF2DOTNET_IMPORT
    #define TF2DOTNET_CDECL
    #pragma warning Unknown dynamic link import/export semantics.
#endif

#endif // TF2DOTNET_MACROS_H
