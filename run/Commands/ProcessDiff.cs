using System.Collections.Generic;
using System.Linq;
using Model;
using NLog;

namespace run.Commands
{
    internal class ProcessDiff
    {
        public static void Run()
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            // Get all build data that has not been processed.
            int i = 0;
            using (var context = new whatsgoingonEntities())
            {
                context.Configuration.LazyLoadingEnabled = true;

                var existingBuilds2 = context.Builds.Where(b => b.PreviousId.HasValue).ToList();
                foreach (var build in existingBuilds2)
                {
                    List<int> vals;
                    var diffs = Helper.GetDiffs(build.PreviousBuild, build, out vals);

                    foreach (var diff in diffs)
                    {
                        context.Database.ExecuteSqlCommand(
                            "INSERT INTO BuildDiff(BuildId, PreviousBuildDataId, CurrentBuildDataId, Diff) VALUES({0},{1},{2},{3})",
                            diff.BuildId,
                            diff.PreviousBuildDataId,
                            diff.CurrentBuildDataId,
                            diff.Diff);

                        logger.Info("INSERTING {0}", i++);
                    }

                    foreach (var val in vals)
                    {
                        // Set build data to completed.
                        //currData.Processed = true;
                        context.Database.ExecuteSqlCommand(
                            "UPDATE BuildData SET Processed = 1 WHERE Id = {0}",
                            val);

                        logger.Info("UPDATING {0}", val);
                    }
                }
            }
        }
    }
}