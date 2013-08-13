using System.Linq;
using NLog;
using run.Commands;

namespace run
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            if (args.Length == 0)
            {
                logger.Info("Missing parameters.");
                logger.Info("Example:");
                logger.Info("run.exe --artifacts --setPreviousId --processDiff --jiraFromGit");

                return;
            }

            logger.Info("Starting!....");
            if (args.Contains("--artifacts"))
            {
                logger.Info("Processing --artifacts.");
                if (!ArtifactsToDB.Run())
                {
                    logger.Info("Processing --artifacts Failed.");
                    logger.Info("Exiting.");
                }
            }

            if (args.Contains("--setPreviousId"))
            {
                logger.Info("Processing --setPreviousId.");
                SetPreviousId.Run();
            }

            if (args.Contains("--processDiff"))
            {
                logger.Info("Processing --processDiff.");
                ProcessDiff.Run();
            }

            if (args.Contains("--jiraFromGit"))
            {
                logger.Info("Processing --jiraFromGit.");
                JiraFromGit.Run();
            }

            logger.Info("Finished!....");
        }
    }
}
