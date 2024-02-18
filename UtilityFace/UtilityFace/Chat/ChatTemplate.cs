namespace UtilityFace.Chat;

public class ChatTemplate
{
    //Match template params, leaving space for more complex templates
    static readonly Regex Pattern = new(@$"{Regex.Escape(TEMPLATE_STRING)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public const string TEMPLATE_STRING = "$$";

    public string Template;

    public List<ChatParameter> Parameters = new();

    public ChatTemplate(string template)
    {
        Template = template;
        Parse(Template);
    }

    private void Parse(string template)
    {
        Parameters.Clear();

        if (Template == null)
            return;

        int index = 0;
        foreach (Match match in Pattern.Matches(template))
        {
            //Text found between
            if (index != match.Index)
                Parameters.Add(new(ChatParamType.Constant, template.Substring(index, match.Index - index)));

            //Add the type of template
            if (match.Value == TEMPLATE_STRING)
                Parameters.Add(new(ChatParamType.String, ""));

            //Update the index to be after the match
            index = match.Index + match.Length;
        }

        //Add trailing text
        if (index != template.Length)
            Parameters.Add(new(ChatParamType.Constant, template.Substring(index)));

        Log.Chat($"Parsed {Parameters.Count} parameters:\n{string.Join("\n  ", Parameters.Select(x => $"{x.Type}: {x.Value}"))}");
    }

    public void DrawTemplate(ImGuiInputTextCallback callback)
    {
        int p = 0;
        for (var i = 0; i < Parameters.Count; i++)
        {
            var param = Parameters[i];

            switch (param.Type)
            {
                case ChatParamType.Constant:
                    ImGui.Text($"{param.Value}");
                    ImGui.SameLine();
                    break;
                case ChatParamType.String:
                    unsafe
                    {
                        //Log.Chat($"{param.Value} - {i} changed");
                        //ImGui.SetNextItemWidth(100);
                        var width = ImGui.CalcTextSize(param.Value).X + 5;
                        ImGui.SetNextItemWidth(width);
                        ImGui.InputText($"##P{p++}", ref param.Value, 40, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, callback);
                    }
                    ImGui.SameLine();
                    break;
            }
        }
    }

    public override string ToString() => String.Join("", Parameters.Select(x => x.Value));    
}
