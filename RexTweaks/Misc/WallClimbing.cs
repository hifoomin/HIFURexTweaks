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
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (body.name == "TreebotBody(Clone)")
            {
                var kinematicCharacterMotor = body.GetComponent<KinematicCharacterMotor>();
                kinematicCharacterMotor.MaxStableSlopeAngle = 180f;
                kinematicCharacterMotor.MaxStepHeight = 2f;
                kinematicCharacterMotor.MinRequiredStepDepth = 0f;
                kinematicCharacterMotor.PreventSnappingOnLedges = true;
                kinematicCharacterMotor.MaxStableDenivelationAngle = 180f;
            }
        }
    }
}