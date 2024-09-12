using KinematicCharacterController;
using RoR2;

namespace HIFURexTweaks.Misc
{
    internal class WallClimbing : MiscBase
    {
        public override string Name => ": Misc : Wall Climbing";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMotor.GenerateParameters += CharacterMotor_GenerateParameters;
        }

        private void CharacterMotor_GenerateParameters(On.RoR2.CharacterMotor.orig_GenerateParameters orig, CharacterMotor self)
        {
            orig(self);
            var body = self.body;
            if (body && body.bodyIndex == Main.rexBodyIndex)
            {
                // Main.HRTLogger.LogFatal("found rex body");
                var kinematicCharacterMotor = body.GetComponent<KinematicCharacterMotor>();
                kinematicCharacterMotor.LedgeAndDenivelationHandling = true;
                kinematicCharacterMotor.MaxStableSlopeAngle = 89f;
                kinematicCharacterMotor.MaxStepHeight = 2f;
                kinematicCharacterMotor.MinRequiredStepDepth = 0f;
                kinematicCharacterMotor.MaxStableDenivelationAngle = 180f;
            }
        }
    }
}