using Mochizuki.VariationPackager;

namespace Mochizuki.ConstraintByHumanoid.Packaging
{
    public static class AutomatedRelease
    {
        public static void Build()
        {
            CLI.BuildWithScene("Scenes/Release.unity", true);
        }
    }
}