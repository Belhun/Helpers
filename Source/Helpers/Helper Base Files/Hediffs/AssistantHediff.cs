using Verse;

namespace Helpers
{
    public class AssistantHediffComp : HediffComp
    {
        public override void CompExposeData()
        {
            base.CompExposeData();
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                return "AssistantHediff Active";
            }
        }
    }

    public class AssistantHediffCompProperties : HediffCompProperties
    {
        public AssistantHediffCompProperties()
        {
            this.compClass = typeof(AssistantHediffComp);
        }
    }
}
