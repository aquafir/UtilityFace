namespace UtilityFace.Components;

public abstract class ICollectionPicker<T> : IPicker<T>
{
    //Debated about where to put this.  Some choices till be different, like string[] for Combos and mapped to the generic type
    /// <summary>
    /// Items being displayed and selected from
    /// </summary>
    public IEnumerable<T> Choices;
    //public T[] Choices;

    //How to handle layout?
    //Vector2 Size / float Width / etc.

    public override void DrawBody()
    {
        //Index used for line breaks?
        int index = 0;

        foreach (var choice in Choices)
            DrawItem(choice, index++);
    }

    //public abstract void DrawItem(T item, int index);
    public virtual void DrawItem(T item, int index) { }
}