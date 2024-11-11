using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    [DefOf]
    public static class SkillDefOf
    {
        public static SkillDef Helping;

        static SkillDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SkillDefOf));
        }
    }

}
