using ACE.DatLoader.Entity;
using UtilityFace.Components;

namespace UtilityFace.Components;
/// <summary>
/// FilterSets are groups of filters sharing a type they are filtering
/// </summary>
public class FilterSet<T>(List<IFilter<T>> filters) : IFilter<T>
{
    public List<IFilter<T>> Filters = filters;

    /// <summary>
    /// Checks if any Filter has been interacted with
    /// </summary>
    //public override bool Check()
    //{
    //    retu
    //}

    /// <summary>
    /// Returns all items that have not been filtered
    /// </summary>
    public override IEnumerable<T> GetFiltered(IEnumerable<T> input) => input.Where(x => !IsFiltered(x));

    /// <summary>
    /// An item is filtered is any filter would filter it
    /// </summary>
    public override bool IsFiltered(T item) => Filters.Any(x => x.IsFiltered(item));


    public override void DrawBody() 
    {
        foreach (var filter in Filters)
        {
            if (filter.Check())
                Changed = true;
        }
    }
}
