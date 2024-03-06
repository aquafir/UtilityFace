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

    static Vector2 ButtonSize = new(25);

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

            //Clear modal
            textureModal = null;
            modalRow = -1;

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
        }
        catch (Exception ex) { Log.Error(ex); }

    }

    public void SetTarget(PropertyData target)
    {
        //Todo: clone?
        Target = target;
        Filter?.SetTarget(target);

        //Log.Chat($"Table: {target.IntValues.Count} -- Filter: {Filter.Props.Length}");

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

            try
            {
                switch (sortColumn)
                {
                    case 0:
                        tableData = sortDirection == ImGuiSortDirection.Descending ?
                            tableData.OrderByDescending(x => x.Key).ToArray() :
                            tableData.OrderBy(x => x.Key).ToArray();
                        return;
                    case 1:
                        tableData = sortDirection == ImGuiSortDirection.Descending ?
                            tableData.OrderByDescending(x => x.Property).ToArray() :
                            tableData.OrderBy(x => x.Property).ToArray();
                        return;
                    case 2:
                        //Base on string value
                        if (Type == PropType.String || Type == PropType.Bool)
                        {
                            tableData = sortDirection == ImGuiSortDirection.Descending ?
                                tableData.OrderByDescending(x => x.OriginalValue).ToArray() :
                                tableData.OrderBy(x => x.OriginalValue).ToArray();
                        }
                        //Base on number value
                        else
                        {
                            tableData = sortDirection == ImGuiSortDirection.Descending ?
                                tableData.OrderByDescending(x => double.Parse(x.OriginalValue)).ToArray() :
                                tableData.OrderBy(x => double.Parse(x.OriginalValue)).ToArray();
                        }
                        return;
                    case 3:
                        if (Type == PropType.String || Type == PropType.Bool)
                        {
                            tableData = sortDirection == ImGuiSortDirection.Descending ?
                                tableData.OrderByDescending(x => x.CurrentValue).ToArray() :
                                tableData.OrderBy(x => x.CurrentValue).ToArray();
                        }
                        else
                        {
                            tableData = sortDirection == ImGuiSortDirection.Descending ?
                                tableData.OrderByDescending(x => double.Parse(x.CurrentValue)).ToArray() :
                                tableData.OrderBy(x => double.Parse(x.CurrentValue)).ToArray();
                        }
                        return;
                }
            }
            catch (Exception ex) { Log.Error(ex); }
        }
    }


    /// <summary>
    /// Render a table for a PropType
    /// </summary>
    public void Render(PropertyTable table)
    {
        if (Target is null)
        {
            ImGui.Text($"{Type} - {Filter.Props.Length} - {tableData.Length}");
            return;
        }

        //Show the filter
        Filter.Render();

        if (Filter.Changed)
            UpdateTable();


        //Show item count?
        //ImGui.Text($"Data: {tableData.Length}");

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
            Sort();

            //ImGui.EndTable();
            //return;

            for (int i = 0; i < tableData.Length; i++)
            {
                var row = tableData[i];
                RenderRow(table, row, i);
            }
        }

        ImGui.EndTable();
    }

    public void RenderRow(PropertyTable table, TableRow row, int i)
    {
        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.Text($"{row.Key}");

        ImGui.TableNextColumn();
        ImGui.Text($"{row.Property}");

        ImGui.TableNextColumn();
        ImGui.Text($"{row.OriginalValue}");

        ImGui.TableNextColumn();
        RenderEdit(table, row, i);
    }

    public void RenderEdit(PropertyTable table, TableRow row, int i)
    {
        //Special handling of enum type
        bool changed = false;

        if (Type == PropType.Int && ((IntId)row.Key).TryGetEnumNames(out var names))
            changed = RenderPickEnum(row, i, names);
        else if (Type == PropType.DataId && row.Property.Contains("Icon"))
            //else if (Type == PropType.DataId && ((DataId)row.Key) == DataId.Icon)
            changed = RenderPickIcon(row, i);
        else if (Type == PropType.Bool)
            changed = RenderPickBool(row, i);
        else if (ImGui.InputText($"###{Type}{i}", ref row.CurrentValue, 300, ImGuiInputTextFlags.EnterReturnsTrue))
            changed = true;

        if (changed)
        {
            var g = new Game();
            //var s = g.World.Selected;
            if (!g.World.TryGet(Target.Id, out var wo))
            {
                Log.Chat("Unable to find target");
                return;
            }
            //Todo: rethink?  Id item
            unsafe
            {
                var clientUI = ((AcClient.ClientUISystem*)AcClient.ClientUISystem.s_pUISystem);
                clientUI->ExamineObject(wo.Id);
            }
            wo.Select();

            var cmd = table.Type switch
            {
                //PropType.Unknown => $"/setproperty PropertyBool.{row.Property} {row.CurrentValue}",
                PropType.Bool => $"/setproperty PropertyBool.{row.Property} {row.CurrentValue}",
                PropType.DataId => $"/setproperty PropertyDataId.{row.Property} {row.CurrentValue}",
                PropType.Float => $"/setproperty PropertyFloat.{row.Property} {row.CurrentValue}",
                PropType.InstanceId => $"/setproperty PropertyInstanceId.{row.Property} {row.CurrentValue}",
                PropType.Int => $"/setproperty PropertyInt.{row.Property} {row.CurrentValue}",
                PropType.Int64 => $"/setproperty PropertyInt64.{row.Property} {row.CurrentValue}",
                PropType.String => $"/setproperty PropertyString.{row.Property} \"{row.CurrentValue}\"",
            };

            g.Actions.InvokeChat(cmd);
            Log.Chat($"Ran command: {cmd}");
        }

    }

    /// <summary>
    /// Render a combobox for an enum
    /// </summary>
    private bool RenderPickEnum(TableRow row, int i, string[] names)
    {
        if (!int.TryParse(row.CurrentValue, out var value))
        {
            Log.Chat($"Can't parse?");
        }

        if (ImGui.Combo($"###{Type}{i}", ref value, names, names.Length))
        {
            row.CurrentValue = value.ToString();
            Log.Chat($"Set to {names[value]} ({value})");
            return true;
        }

        return false;
    }


    int modalRow = -1;
    PickerModal<uint> textureModal;
    private bool RenderPickIcon(TableRow row, int i)
    {
        if (!uint.TryParse(row.CurrentValue, out var id))
            return false;

        //Use current ID as button
        var tex = TextureManager.GetOrCreateTexture(id);
        Vector2 size = tex.Bitmap.ToVector2();
        if (ImGui.TextureButton($"{i}{row.Key}", tex, ButtonSize)) // size.ScaleToMax(new Vector2(80))))
        {
            //Find or create
            if (!TextureManager.TryGetModal(size, out textureModal, true))
            {
                modalRow = -1;
                return false;
            }
            modalRow = i;

            //Set up the modal?
            textureModal.MinSize = new(525);

            //Page to current
            if (textureModal.Picker is TexturedPicker<uint> picker
                && uint.TryParse(row.CurrentValue, out var current)
                && TextureManager.TextureGroups.TryGetValue(size, out var ids))
            {
                //Get the group
                var index = Math.Max(ids.IndexOf(current), ids.IndexOf(current + TextureManager.TEXTURE_OFFSET));
                var page = index < 0 ? 0 : index / picker.PerPage;
                Log.Chat($"ID {current}->Index {index}/{ids.Count} for page {page}/{picker.Pages}");
                picker.CurrentPage = page;
            }
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text($"{id} - {tex.Bitmap.ToVector2()}");
            ImGui.EndTooltip();
        }

        //Only check modal for the opening row
        if (modalRow != i || textureModal is null)
            return false;

        if (textureModal.Check())
        {
            var iconId = textureModal.Selection;
            Log.Chat($"{i} - {row.Property} - ID {iconId}");

            if (iconId == default(uint))
                return false;

            //Not sure this matters?
            //if (iconId >= TextureManager.TEXTURE_OFFSET)
            //    iconId -= TextureManager.TEXTURE_OFFSET;

            row.CurrentValue = $"{iconId}";

            //Reusing the modal so null it to indicate not in use
            //textureModal = null;
            modalRow = -1;
            return true;
        }

        return false;
    }

    private bool RenderPickBool(TableRow row, int i)
    {
        if (!bool.TryParse(row.CurrentValue, out var value))
            return false;

        if(ImGui.Checkbox($"##{i}", ref value))
        {
            row.CurrentValue = value.ToString();
            return true;
        }

        return false;
    }    
}

