using Mochizuki.VariationPackager;

namespace Mochizuki.ConstraintByHumanoid.Packaging
{
    public static class AutomatedRelease
    {
        public static void Build()
        {
            CLI.BuildWithScene("Assets/Scenes/Release.unity", true);
        }
    }
}