using Decal.Adapter;
using ImGuiNET;
using System;
using System.Numerics;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;

namespace UtilityFace;
internal class ExampleUI : IDisposable
{
    /// <summary>
    /// The UBService Hud
    /// </summary>
    private readonly Hud hud;

    /// <summary>
    /// The default value for TestText.
    /// </summary>
    public const string DefaultTestText = "Some Test Text";

    /// <summary>
    /// Some test text. This value is used to the text input in our UI.
    /// </summary>
    public string TestText = DefaultTestText.ToString();

    public ExampleUI()
    {
        // Create a new UBService Hud
        hud = UBService.Huds.CreateHud("UtilityFace");

        // set to show our icon in the UBService HudBar
        hud.ShowInBar = true;

        // subscribe to the hud render event so we can draw some controls
        hud.OnRender += Hud_OnRender;
    }

    /// <summary>
    /// Called every time the ui is redrawing.
    /// </summary>
    private void Hud_OnRender(object sender, EventArgs e)
    {
        try
        {
            ImGui.InputTextMultiline("Test Text", ref TestText, 5000, new Vector2(400, 150));

            if (ImGui.Button("Print Test Text"))
            {
                OnPrintTestTextButtonPressed();
            }

            ImGui.SameLine();

            if (ImGui.Button("Reset Test Text"))
            {
                TestText = DefaultTestText;
            }
        }
        catch (Exception ex)
        {
            PluginCore.Log(ex);
        }
    }

    /// <summary>
    /// Called when our print test text button is pressed
    /// </summary>
    private void OnPrintTestTextButtonPressed()
    {
        var textToShow = $"Test Text:\n{TestText}";

        CoreManager.Current.Actions.AddChatText(textToShow, 1);
        UBService.Huds.Toaster.Add(textToShow, ToastType.Info);
    }

    public void Dispose()
    {
        hud.Dispose();
    }
}