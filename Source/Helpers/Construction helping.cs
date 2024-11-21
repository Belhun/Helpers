using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal class Construction_helping
    {
        //TODO : Research how construction normally works and find where the best location to add helpers would be
        //TODO : Add construction helpers


        // Under "Frame frame = <>4__this.Frame;"
        // add helpers = actor.Helpers

        // Under "actor.skills.Learn(SkillDefOf.Construction, 0.25f);"
        // Give exsperence to helpers if they are being helped

        //Under
        //float num = actor.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f;
        // Add num += HelperMechanics.CalculateHelperContribution(actor, jobDriver, recipeDef, helpers, StatDefOf.ConstructionSpeed);

        //
        //
        //if (frame.Stuff != null)
        //{
        //    num *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor);
        //}
    }
}
