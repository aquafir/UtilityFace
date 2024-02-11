using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.Helpers;
public static class ImGuiExtensions
{
    public static void SetText(this ImGuiInputTextCallbackDataPtr ptr, string text, bool select = false)
    {
        //Todo: maybe directly set?
        //Marshal.Copy(Encoding.UTF8.GetBytes(chatMessage), 0, ptr.Buf, chatMessage.Length);

        ptr.DeleteChars(0, ptr.BufTextLen);
        ptr.InsertChars(0, text);

        if (select)
            ptr.SelectAll();
    }
}
