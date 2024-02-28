namespace UtilityFace.Components.Pickers;

public abstract class ICollectionPicker<T> : IPicker<T>
{
    //Debated about where to put this.  Some choices till be different, like string[] for Combos and mapped to the generic type
    /// <summary>
    /// Items being displayed and selected from
    /// </summary>
    public IEnumerable<T> Choices;
    //public T[] Choices;

    /// <summary>
    /// Currently selected set of items
    /// </summary>
    public System.Collections.Generic.HashSet<T> Selected;

    //How to handle layout?
    //Vector2 Size / float Width / etc.


    public virtual void ToggleRange(int i)
    {
        //Get index of selected
        var list = Choices.ToList();
        var currentIndex = Selection.Equals(default(T)) ? -1 : list.IndexOf(Selection);

        //Missing goes from start?
        if (currentIndex < 0)
            currentIndex = 0;

        var start = Math.Min(currentIndex, i);
        var end = Math.Max(currentIndex, i);
        var num = end - start + 1;

        Log.Chat($"i={i} - {currentIndex} - {start} to {end}");

        var range = list.Skip(start).Take(num);

        if (range.All(x => Selected.Contains(x)))
        {
            Log.Chat($"Remove");
            foreach (var e in range)
                Remove(e);
        }
        else
        {
            Log.Chat($"Add");
            foreach (var e in range)
                Add(e);
        }
    }
    /// <summary>
    /// Toggles the presence of a Choice in Selected, setting Selection to the item if it is being added.
    /// Returns true if the item was added
    /// </summary>
    /// <param name="item"></param>
    public virtual bool Toggle(T item)
    {
        if (Selected.Add(item))
        {
            Selection = item;
            return true;
        }
        else
        {
            Selected.Remove(item);
            if (item.Equals(Selection))
                Selection = default(T);

            return false;
        }
    }
    /// <summary>
    /// Select an individual item
    /// </summary>
    public virtual void Select(T item, bool deselect = false)
    {
        if (deselect)
        {
            item = default(T);
            Selected.Remove(item);
        }
        else
        {
            Selection = item;
            Selected.Add(item);
        }
    }
    /// <summary>
    /// Adds item to the Selection
    /// </summary>
    public virtual bool Add(T item)
    {
        if (Selected.Add(item))
        {
            Selection = item;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Removes item from the Selection
    /// </summary>
    public virtual bool Remove(T item)
    {
        if (Selected.Contains(item))
        {
            Selected.Remove(item);
            if (item.Equals(Selection))
                Selection = default(T);

            return true;
        }
        return false;
    }

    public override void Init()
    {
        Selected = new();
        base.Init();
    }

    public override void DrawBody()
    {
        //Index used for line breaks?
        int index = 0;

        foreach (var choice in Choices)
            DrawItem(choice, index++);
    }

    //public abstract void DrawItem(T item, int index);
    public virtual void DrawItem(T item, int index) { }

    /// <summary>
    /// Called when an item is interacted with
    /// </summary>
    public virtual void SelectItem(T item, int index)
    {
        //TODO ADD MULTI SELECTION?
        if (ImGui.IsKeyDown(ImGuiKey.ModShift))
        {
            ToggleRange(index);
            Changed = false;
            Log.Chat($"Range {index} - {Selected.Count} - {Selection}");
        }
        else if (ImGui.IsKeyDown(ImGuiKey.ModCtrl))
        {
            Toggle(item);
            Changed = false;
            Log.Chat($"Toggle {index} - {Selected.Count} - {Selection}");
        }
        else
        {
            Select(item); //Selection = item;
            Changed = true;
            Log.Chat($"Select {index} - {Selected.Count} - {Selection}");
        }

    }
}