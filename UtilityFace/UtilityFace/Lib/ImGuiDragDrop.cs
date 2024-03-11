// https://github.com/NightmareXIV/ECommons/blob/aa5305f34b24a7c6955c864675244b8ff5fddfcb/ECommons/ImGuiMethods/ImGuiDragDrop.cs
/*
 MIT License

Copyright (c) 2023 NightmareXIV

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

using ImGuiNET;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ECommons.ImGuiMethods;
#nullable disable

// ImGui extra functionality related with Drag and Drop
public static class ImGuiDragDrop {
    // TODO: review
    // can now pass refs with Unsafe.AsRef

    public static unsafe void SetDragDropPayload<T>(string type, T data, ImGuiCond cond = 0)
    where T : unmanaged {
        void* ptr = Unsafe.AsPointer(ref data);
        ImGui.SetDragDropPayload(type, new IntPtr(ptr), (uint)Unsafe.SizeOf<T>(), cond);
    }

    public static unsafe bool AcceptDragDropPayload<T>(string type, out T payload, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None)
    where T : unmanaged {
        ImGuiPayload* pload = ImGui.AcceptDragDropPayload(type, flags);
        payload = (pload != null) ? Unsafe.Read<T>(pload->Data) : default;
        return pload != null;
    }

    public static unsafe void SetDragDropPayload(string type, string data, ImGuiCond cond = 0) {
        fixed (char* chars = data) {
            int byteCount = Encoding.Default.GetByteCount(data);
            byte* bytes = stackalloc byte[byteCount];
            Encoding.Default.GetBytes(chars, data.Length, bytes, byteCount);

            ImGui.SetDragDropPayload(type, new IntPtr(bytes), (uint)byteCount, cond);
        }
    }

    public static unsafe bool AcceptDragDropPayload(string type, out string payload, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None) {
        ImGuiPayload* pload = ImGui.AcceptDragDropPayload(type, flags);
        payload = (pload != null) ? Encoding.Default.GetString((byte*)pload->Data, pload->DataSize) : null;
        return pload != null;
    }
}