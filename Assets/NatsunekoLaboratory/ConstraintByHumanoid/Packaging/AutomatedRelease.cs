// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the License Zero Parity 7.0.0 (see LICENSE-PARITY file) and MIT (contributions, see LICENSE-MIT file) with exception License Zero Patron 1.0.0 (see LICENSE-PATRON file)
// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using Mochizuki.VariationPackager;

namespace NatsunekoLaboratory.ConstraintByHumanoid.Packaging
{
    public static class AutomatedRelease
    {
        public static void Build()
        {
            CLI.BuildWithScene("Assets/Scenes/Release.unity", true);
        }
    }
}