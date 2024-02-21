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

    public static bool IsFloat(this ImGuiStyleVar style) => style switch
    {
        //ImGuiStyleVar.ButtonTextAlign => true,
        //ImGuiStyleVar.CellPadding => true,
        //ImGuiStyleVar.COUNT => true,
        //ImGuiStyleVar.FramePadding => true,
        //ImGuiStyleVar.IndentSpacing => true,
        //ImGuiStyleVar.ItemInnerSpacing => true,
        //ImGuiStyleVar.ItemSpacing => true,
        //ImGuiStyleVar.SelectableTextAlign => true,
        //ImGuiStyleVar.SeparatorTextPadding => true,
        //ImGuiStyleVar.WindowMinSize => true,
        //ImGuiStyleVar.WindowPadding => true,
        //ImGuiStyleVar.WindowTitleAlign => true,
        ImGuiStyleVar.Alpha => true,
        ImGuiStyleVar.ChildBorderSize => true,
        ImGuiStyleVar.ChildRounding => true,
        ImGuiStyleVar.DisabledAlpha => true,
        ImGuiStyleVar.FrameBorderSize => true,
        ImGuiStyleVar.FrameRounding => true,
        ImGuiStyleVar.GrabMinSize => true,
        ImGuiStyleVar.GrabRounding => true,
        ImGuiStyleVar.PopupBorderSize => true,
        ImGuiStyleVar.PopupRounding => true,
        ImGuiStyleVar.ScrollbarRounding => true,
        ImGuiStyleVar.ScrollbarSize => true,
        ImGuiStyleVar.SeparatorTextAlign => true,
        ImGuiStyleVar.SeparatorTextBorderSize => true,
        ImGuiStyleVar.TabRounding => true,
        ImGuiStyleVar.WindowBorderSize => true,
        ImGuiStyleVar.WindowRounding => true,
        _ => false,
    };
    public static Style Style(this ImGuiStyleVar var, object value) => var.IsFloat() ?
        new StyleFloat(var, Convert.ToSingle(value)) : new StyleVector2(var, (Vector2)value);
    public static Style Style(this ImGuiCol var, Vector4 value) => var.Style(value.ToUInt());
    public static Style Style(this ImGuiCol var, uint value) => new StyleColor(var, value);

    public static Styling GetStyling(this ImGuiStylePtr style)
    {
        Styling styling = new();

        //Loop through styles?
        unsafe
        {
            foreach (ImGuiCol colStyle in Enum.GetValues(typeof(ImGuiCol)))
            {
                var color = *ImGui.GetStyleColorVec4(colStyle);
                styling.Styles.Add(colStyle.Style(color));
            }
        }

        styling.Styles.AddRange(new List<Style>()
        {
            ImGuiStyleVar.Alpha.Style(style.Alpha),
            ImGuiStyleVar.ButtonTextAlign.Style(style.ButtonTextAlign),
            ImGuiStyleVar.CellPadding.Style(style.CellPadding),
            ImGuiStyleVar.ChildBorderSize.Style(style.ChildBorderSize),
            ImGuiStyleVar.ChildRounding.Style(style.ChildRounding),
            ImGuiStyleVar.DisabledAlpha.Style(style.DisabledAlpha),
            ImGuiStyleVar.FrameBorderSize.Style(style.FrameBorderSize),
            ImGuiStyleVar.FramePadding.Style(style.FramePadding),
            ImGuiStyleVar.FrameRounding.Style(style.FrameRounding),
            ImGuiStyleVar.GrabMinSize.Style(style.GrabMinSize),
            ImGuiStyleVar.GrabRounding.Style(style.GrabRounding),
            ImGuiStyleVar.IndentSpacing.Style(style.IndentSpacing),
            ImGuiStyleVar.ItemInnerSpacing.Style(style.ItemInnerSpacing),
            ImGuiStyleVar.ItemSpacing.Style(style.ItemSpacing),
            ImGuiStyleVar.PopupBorderSize.Style(style.PopupBorderSize),
            ImGuiStyleVar.PopupRounding.Style(style.PopupRounding),
            ImGuiStyleVar.ScrollbarRounding.Style(style.ScrollbarRounding),
            ImGuiStyleVar.ScrollbarSize.Style(style.ScrollbarSize),
            ImGuiStyleVar.SelectableTextAlign.Style(style.SelectableTextAlign),
            ImGuiStyleVar.TabRounding.Style(style.TabRounding),
            ImGuiStyleVar.WindowBorderSize.Style(style.WindowBorderSize),
            ImGuiStyleVar.WindowMinSize.Style(style.WindowMinSize),
            ImGuiStyleVar.WindowPadding.Style(style.WindowPadding),
            ImGuiStyleVar.WindowRounding.Style(style.WindowRounding),
            ImGuiStyleVar.WindowTitleAlign.Style(style.WindowTitleAlign),
        });
        //public byte AntiAliasedFill;
        //public byte AntiAliasedLines;
        //public byte AntiAliasedLinesUseTex;
        //public float CircleTessellationMaxError;
        //public float ColumnsMinSpacing;
        //public float CurveTessellationTol;
        //public float HoverDelayNormal;
        //public float HoverDelayShort;
        //public float HoverStationaryDelay;
        //public float LogSliderDeadzone;
        //public float MouseCursorScale;
        //public float SeparatorTextBorderSize;
        //public float TabBorderSize;
        //public float TabMinWidthForCloseButton;
        //public ImGuiDir ColorButtonPosition;
        //public ImGuiDir WindowMenuButtonPosition;
        //public ImGuiHoveredFlags HoverFlagsForTooltipMouse;
        //public ImGuiHoveredFlags HoverFlagsForTooltipNav;
        //public Vector2 DisplaySafeAreaPadding;
        //public Vector2 DisplayWindowPadding;
        //public Vector2 SeparatorTextAlign;
        //public Vector2 SeparatorTextPadding;
        //public Vector2 TouchExtraPadding;

        return styling;
    }
}

//Put on hold since there's a probably replacement
////byte AntiAliasedFill,
////byte AntiAliasedLines,
////byte AntiAliasedLinesUseTex,
//public enum ImStyleVec
//{
//    ButtonTextAlign,
//    CellPadding,
//    DisplaySafeAreaPadding,
//    DisplayWindowPadding,
//    FramePadding,
//    ItemInnerSpacing,
//    ItemSpacing,
//    SelectableTextAlign,
//    SeparatorTextAlign,
//    SeparatorTextPadding,
//    TouchExtraPadding,
//    WindowMinSize,
//    WindowPadding,
//    WindowTitleAlign,
//}
//public enum ImStyleFloat
//{
//    Alpha,
//    ChildBorderSize,
//    ChildRounding,
//    CircleTessellationMaxError,
//    ColumnsMinSpacing,
//    CurveTessellationTol,
//    DisabledAlpha,
//    FrameBorderSize,
//    FrameRounding,
//    GrabMinSize,
//    GrabRounding,
//    HoverDelayNormal,
//    HoverDelayShort,
//    HoverStationaryDelay,
//    IndentSpacing,
//    LogSliderDeadzone,
//    MouseCursorScale,
//    PopupBorderSize,
//    PopupRounding,
//    ScrollbarRounding,
//    ScrollbarSize,
//    SeparatorTextBorderSize,
//    TabBorderSize,
//    TabMinWidthForCloseButton,
//    TabRounding,
//    WindowBorderSize,
//    WindowRounding,
//}
//public enum ImStyleAlign
//{
//    //ImGuiDir 
//    ColorButtonPosition,
//    WindowMenuButtonPosition,

//}
//public enum ImStyleHover
//{
//    //ImGuiHoveredFlags
//    HoverFlagsForTooltipMouse,
//    HoverFlagsForTooltipNav,
//}


public abstract class Style
{
    public abstract void PushStyle();
    public abstract void PopStyle();
    public abstract bool TryParse(string s, out Style style);
}

public abstract class StyleVar(ImGuiStyleVar type) : Style
{
    public ImGuiStyleVar Type;
    public override void PopStyle() => ImGui.PopStyleVar();
}
public class StyleFloat(ImGuiStyleVar type, float value) : StyleVar(type)
{
    public float Value = value;

    public override void PushStyle() => ImGui.PushStyleVar(Type, Value);

    //alpha = 1.0
    public override string ToString() => $"{Type} = {Value}";

    public override bool TryParse(string s, out Style style)
    {
        throw new NotImplementedException();
    }
}
public class StyleVector2(ImGuiStyleVar type, Vector2 value) : StyleVar(type)
{
    public Vector2 Value = value;

    public override void PushStyle() => ImGui.PushStyleVar(Type, Value);

    //windowPadding = [8.0, 8.0]
    public override string ToString() => $"{Type} = [{Value.X}, {Value.Y}]";
    public override bool TryParse(string s, out Style style)
    {
        throw new NotImplementedException();
    }
}

public class StyleColor(ImGuiCol type, uint value) : Style
{
    public ImGuiCol Type = type;
    public uint Value = value;

    public override void PushStyle() => ImGui.PushStyleColor(Type, Value);
    public override void PopStyle() => ImGui.PopStyleColor();

    //Text = "rgba(255, 255, 255, 1.0)"
    public override string ToString()
    {
        var bytes = BitConverter.GetBytes(Value);
        return $"{Type} = rgba({bytes[0]}, {bytes[1]}, {bytes[2]}, {bytes[3]})";
    }

    public override bool TryParse(string s, out Style style)
    {
        throw new NotImplementedException();
    }

    //public override bool TryParse(string s, out Style style)
    //{
    //    style = null;
    //    var split = s.Split('=');
    //    if (split.Length < 2)
    //        return false;
    //}
}

//TODO: Deal with GetStyle() -> only stuff
//style.ColorButtonPosition = ImGuiDir.Left;
//style.WindowMenuButtonPosition = ImGuiDir.Right;
//public class StyleDirection(ImGuiCol type, uint value) : Style
//{
//    public ImGuiCol Type = type;
//    public uint Value = value;

//    public override void PushStyle() => ImGui.PushStyleColor(Type, Value);
//    public override void PopStyle() => ImGui.PopStyleColor();

//    //Text = "rgba(255, 255, 255, 1.0)"
//    public override string ToString()
//    {
//        var bytes = BitConverter.GetBytes(Value);
//        return $"{Type} = rgba({bytes[0]}, {bytes[1]}, {bytes[2]}, {bytes[3]})";
//    }

//    public override bool TryParse(string s, out Style style)
//    {
//        throw new NotImplementedException();
//    }
//}

public class Styling
{
    public List<Style> Styles = new();

    //public void SetStyle(ImGuiStyleVar style, object value) => Styles.AddOrUpdate(style, value);
    public void PushStyles()
    {
        foreach (var style in Styles)
            style.PushStyle();
    }
    public void PopStyles()
    {
        foreach (var style in Styles)
            style.PopStyle();
    }
}