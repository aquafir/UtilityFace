namespace UtilityFace.Table;

public class PropertyTable
{
    #region Constants
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;

    #endregion

    #region Members
    PropertyData Target;
    public PropertyFilter Filter;

    public string Name { get; set; }
    public PropType Type { get; set; }

    public bool UseFilter { get; set; } = true;

    //List<PropertyFilter> filters = new();

    //Data for the table
    //public List<TableRow> tableData = new ();
    public TableRow[] tableData = new TableRow[0];

    #endregion

    public PropertyTable(PropType type)
    {
        Type = type;
        Name = type.ToString().Replace("Property", "");

        //Setup filter
        Filter = new(type);
        Filter.IncludeMissing = false;
        Filter.ShowIncludeMissing = false;
        Filter.Width = 120;

        //Build table data
        Filter?.UpdateFilter();
        UpdateTable();
    }

    public void UpdateTable()
    {
        //Todo: how to preserve existing edit data
        try
        {
            if (Filter is null)
                return;

            //Get keys from filter
            //tableData.Clear();
            tableData = new TableRow[Filter.Props.Length];

            //ImGui.Text($"{Filter.Props.Length}");

            //For each filtered property...
            for (var i = 0; i < Filter.Props.Length; i++)
            {
                //Try to find the value corresponding to the Type if it exists in the Target
                var name = Filter.Props[i];
                var key = Filter.PropKeys[i];
                if (!Type.TryGetString(key, Target, out var val))
                    continue;

                //Add it to the table
                tableData[i] = new()
                {
                    Key = key,
                    Property = name,
                    OriginalValue = val,
                    CurrentValue = val,
                };

                //tableData.Add(new()
                //{
                //    Key = key,
                //    Property = name,
                //    OriginalValue = val,
                //    CurrentValue = val,
                //});
            }
            //Log.Chat($"{tableData.Length}");
        }catch(Exception ex) { Log.Error(ex); }
        
    }

    public void SetTarget(PropertyData target)
    {
        //Todo: clone?
        Target = target;
        Filter?.SetTarget(target);

        Log.Chat($"Table: {target.IntValues.Count} -- Filter: {Filter.Props.Length}");

        UpdateTable();
    }

    //Sort table based on column/direction
    private uint sortColumn = 0; // Currently sorted column index
    private ImGuiSortDirection sortDirection = ImGuiSortDirection.Ascending;
    private int CompareTableRows(TableRow a, TableRow b) => sortColumn switch
    {
        0 => a.Key.CompareTo(b.Key) * (sortDirection == ImGuiSortDirection.Descending ? -1 : 1),
        1 => a.Property.CompareTo(b.Property) * (sortDirection == ImGuiSortDirection.Descending ? -1 : 1),
        2 => a.OriginalValue.CompareTo(b.OriginalValue) * (sortDirection == ImGuiSortDirection.Descending ? -1 : 1),
        3 => a.CurrentValue.CompareTo(b.CurrentValue) * (sortDirection == ImGuiSortDirection.Descending ? -1 : 1),
    };
    //Sort if needed
    unsafe private void Sort()
    {
        var tableSortSpecs = ImGui.TableGetSortSpecs();

        //Check if a sort is needed
        if (tableSortSpecs.SpecsDirty)
        {
            //Set column/direction
            sortDirection = tableSortSpecs.Specs.SortDirection;
            sortColumn = tableSortSpecs.Specs.ColumnUserID;
            //C.Chat($"Dirty: {sortDirection} - {tableSortSpecs.Specs.ColumnUserID}");

            //Todo: do this more better
            var specs = (&tableSortSpecs)->NativePtr;
            specs->SpecsDirty = 0;

            Array.Sort(tableData, CompareTableRows);
        }
    }

    public void Render()
    {
        if (Target is null || tableData.Length == 0)
        {
            ImGui.Text($"{Type} - {Filter.Props.Length} - {tableData.Length}");
            return;
        }
        Filter.Render();


        if (Filter.Changed)
            UpdateTable();


        ImGui.Text($"Data: {tableData.Length}");

        if (ImGui.BeginTable($"{Type}", 4, TABLE_FLAGS))
        {
            // Set up columns
            uint columnIndex = 0;
            //ImGui.TableSetupColumn($"Key", ImGuiTableColumnFlags.DefaultHide, 50, columnIndex++);
            ImGui.TableSetupColumn($"Key", 0, 0, columnIndex++);
            ImGui.TableSetupColumn($"Prop", ImGuiTableColumnFlags.DefaultSort, 0, columnIndex++);
            ImGui.TableSetupColumn($"Value", 0, 0, columnIndex++);
            ImGui.TableSetupColumn($"New Value", 0, 0, columnIndex++);


            //, ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags, 50, (uint)columnIndex++);

            //ImGui::PushItemWidth(-ImGui::GetContentRegionAvail().x * 0.5f);
            // Headers row
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            //Sort if needed
            //Sort();

            //ImGui.EndTable();
            //return;

            for (int i = 0; i < tableData.Length; i++)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{tableData[i].Key}");

                ImGui.TableNextColumn();
                ImGui.Text($"{tableData[i].Property}");

                //if (ImGui.BeginPopupContextItem())
                //{
                //    if (ImGui.MenuItem("Test123"))
                //        Console.WriteLine("Clicked");
                //    ImGui.EndPopup();
                //}

                ImGui.TableNextColumn();
                ImGui.Text($"{tableData[i].OriginalValue}");

                ImGui.TableNextColumn();
                ImGui.InputText($"###{Type}{i}", ref tableData[i].CurrentValue, 300);

                //ImGui.Text($"{tableData[i].CurrentValue}");
                //break;
            }

            ImGui.EndTable();
        }

    }
}