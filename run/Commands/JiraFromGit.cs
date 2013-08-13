using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using LibGit2Sharp;
using Model;
using NLog;
using run.Configuration;
using CompareOptions = System.Globalization.CompareOptions;

namespace run.Commands
{
    internal class GitCommit
    {
        public string Sha { get; set; }
        public string ShortMessage { get; set; }
    }

    /// <summary>
    /// Figures out what Jira tickets are associated with a given build based on the short
    /// SHA and using the bit bucket pull request naming standard.
    /// </summary>
    internal class JiraFromGit
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Run()
        {
            logger.Info("Getting config section [jiraFromGit].");
            var config = (JiraGitSection) ConfigurationManager.GetSection("jiraGit");
            if (config == null)
            {
                logger.Info("Failed to Find config section [jiraGit].");
                return;
            }

            var allHashes = new HashSet<string>();

            HashSet<string> jirasPassed;

            List<Build> unprocessedBuilds;
            using (var context = new whatsgoingonEntities())
            {
                unprocessedBuilds = context.Builds.Where(b => b.GitSha != null).ToList();

                foreach (var p in unprocessedBuilds)
                {
                    if (allHashes.Contains(p.GitSha))
                    {

                    }
                    else
                    {
                        allHashes.Add(p.GitSha);
                    }
                }

                jirasPassed = new HashSet<string>(
                        context.BuildJiras.Select(b => b.Jira).ToList());

                unprocessedBuilds = unprocessedBuilds.Where(b => b.PreviousId.HasValue && !b.JiraProcessed)
                                                     .ToList();
            }

            string prefix = "from " + config.Prefix;

            using (var repo = new Repository(config.Directory))
            {
                var all = repo.Commits.QueryBy(new Filter { SortBy = GitSortOptions.Time | GitSortOptions.Reverse }).ToList();

                var passed = new HashSet<string>();
                

                foreach (Build b in unprocessedBuilds)
                {
                    var jiras = new List<string>();


                    // Failed attempt. I'm not sure how to behave when there are multiple parents.
                    foreach (var commit in all)
                    {
                        var curr = commit;
                        if (commit.Id.ToString(7) == b.GitSha)
                        {

                            var parents = new Queue<Commit>();
                            parents.Enqueue(curr);
                            while (parents.Count > 0)
                            {
                                Commit current = parents.Dequeue();

                                // We're at the root of the repo or the commit was deleted
                                if (current == null)
                                    continue;

                                // We've hit another release. Terminate.
                                string value = current.Id.ToString(7);
                                if (current != curr && allHashes.Contains(value))
                                    continue;

                                // We've already passed this node.
                                if (passed.Contains(value))
                                    continue;
                                passed.Add(value);

                                

                                string shortMessage = current.MessageShort;
                                if (shortMessage.StartsWith("Merge pull request"))
                                {

                                    int index = CultureInfo.InvariantCulture.
                                                            CompareInfo.IndexOf(shortMessage, prefix,
                                                                                CompareOptions.IgnoreCase);
                                    if (index != -1)
                                    {
                                        int start = index + 5;
                                        int end = shortMessage.IndexOf(' ', start);

                                        string jira = shortMessage.Substring(start, end - start);

                                        if (jirasPassed.Contains(jira))
                                            continue;

                                        jiras.Add(jira);
                                        jirasPassed.Add(jira);
                                    }
                                }

                                current.Parents.ToList().ForEach(parents.Enqueue);
                            }


                            break;
                        }
                    }

                    using (var c = new whatsgoingonEntities())
                    {
                        var updateMe = c.Builds.Single(build => build.Id == b.Id);
                        updateMe.JiraProcessed = true;

                        foreach (string jira in jiras)
                        {
                            c.BuildJiras.Add(new BuildJira
                                                 {
                                                     BuildId = b.Id,
                                                     Jira = jira
                                                 });
                        }

                        c.SaveChanges();
                    }
                }
            }
        }
    }
}