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

    //public static Styling GetStyling(this ImGuiStylePtr style)
    //{
    //    style.Alpha = 1.0f;
    //    style.DisabledAlpha = 0.1000000014901161f;
    //    style.WindowPadding = new Vector2(8.0f, 8.0f);
    //    style.WindowRounding = 10.0f;
    //    style.WindowBorderSize = 0.0f;
    //    style.WindowMinSize = new Vector2(30.0f, 30.0f);
    //    style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
    //    style.WindowMenuButtonPosition = ImGuiDir.Right;
    //    style.ChildRounding = 5.0f;
    //    style.ChildBorderSize = 1.0f;
    //    style.PopupRounding = 10.0f;
    //    style.PopupBorderSize = 0.0f;
    //    style.FramePadding = new Vector2(5.0f, 3.5f);
    //    style.FrameRounding = 5.0f;
    //    style.FrameBorderSize = 0.0f;
    //    style.ItemSpacing = new Vector2(5.0f, 4.0f);
    //    style.ItemInnerSpacing = new Vector2(5.0f, 5.0f);
    //    style.CellPadding = new Vector2(4.0f, 2.0f);
    //    style.IndentSpacing = 5.0f;
    //    style.ColumnsMinSpacing = 5.0f;
    //    style.ScrollbarSize = 15.0f;
    //    style.ScrollbarRounding = 9.0f;
    //    style.GrabMinSize = 15.0f;
    //    style.GrabRounding = 5.0f;
    //    style.TabRounding = 5.0f;
    //    style.TabBorderSize = 0.0f;
    //    style.TabMinWidthForCloseButton = 0.0f;
    //    style.ColorButtonPosition = ImGuiDir.Right;
    //    style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
    //    style.SelectableTextAlign = new Vector2(0.0f, 0.0f);
    //}
}

public abstract class Style
{
    public ImGuiStyleVar Type;
    public abstract void PushStyle();
}
public class StyleFloat : Style
{
    public float Value;

    public StyleFloat(ImGuiStyleVar type, float value)
    {
        Type = type;
        Value = value;
    }

    public override void PushStyle() => ImGui.PushStyleVar(Type, Value);
}
public class StyleVector2 : Style
{
    public Vector2 Value;

    public StyleVector2(ImGuiStyleVar type, Vector2 value)
    {
        Type = type;
        Value = value;
    }

    public override void PushStyle() => ImGui.PushStyleVar(Type, Value);
}
public class Styling
{
    public List<Style> Styles = new();

    //public void SetStyle(ImGuiStyleVar style, object value) => Styles.AddOrUpdate(style, value);
    public void PushStyles()
    {
        foreach (var style in Styles)
            style.PushStyle();
    }
    public void PopStyles() => ImGui.PopStyleVar(Styles.Count);
}