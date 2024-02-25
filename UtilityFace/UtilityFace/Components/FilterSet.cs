using ACE.DatLoader.Entity;
using UtilityFace.Components;

//namespace UtilityFace.Components;
public class FilterSet(List<IComp> filters)
{
    public List<IComp> Filters = filters;

    public bool Check()
    {
        //bool Changed = false;
        //foreach (var filter in Filters)

        return Filters.Any(x => x.Check());
    }
}
