using System.Collections.Generic;
using System.Linq;
using Model;

namespace run.Commands
{
    internal class SetPreviousId
    {
        public static void Run()
        {
            // Get all existing builds
            using (var context = new whatsgoingonEntities())
            {
                List<Build> existingBuilds = context.Builds.Where(b=>!b.Incomplete)
                                                    .ToList()
                                                    .OrderBy(b => b.VMajor)
                                                    .ThenBy(b => b.VMinor)
                                                    .ThenBy(b => b.VPatch)
                                                    .ThenBy(b => b.VBuild)
                                                    .ToList();

                for (int i = 1; i < existingBuilds.Count; i++)
                {
                    if (!existingBuilds[i].PreviousId.HasValue)
                    {
                        existingBuilds[i].PreviousId = existingBuilds[i - 1].Id;
                    }
                }

                context.SaveChanges();
            }
        }
    }
}