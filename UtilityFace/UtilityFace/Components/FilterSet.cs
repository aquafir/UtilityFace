using ACE.DatLoader.Entity;

namespace UtilityFace.Components;
public class FilterSet
{
    List<IFilter> Filters;  


    public bool Check()
    {
        bool Changed = false;
        foreach (var filter in Filters)

            Filters.Any(x => x.Check());

    }

    public override void DrawBody()
    {
        throw new NotImplementedException();
    }
}
