namespace UtilityFace.Helpers;
public static class Formulas
{
    public static float ArmorReduction(float armor) => armor > 0 ?
        armor / (armor + 200 / 3) :
        1 + (-armor / (200 / 3));
    public static float ArmorReduction(int armor) => ArmorReduction((float)armor);
}
