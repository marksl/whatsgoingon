using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Model;

namespace view
{
    public partial class Filtered : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string prevString = Request.QueryString["prev"];
                string currString = Request.QueryString["curr"];

                if (string.IsNullOrWhiteSpace(prevString) || string.IsNullOrWhiteSpace(currString))
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }

        public string PrevText
        {
            get { return Context.Server.HtmlEncode(Request.QueryString["prev"].Split('-')[0]); }
        }

        public string CurrText
        {
            get { return Context.Server.HtmlEncode(Request.QueryString["curr"].Split('-')[0]); }
        }

        private Build Prev
        {
            get
            {
                string prevString = Request.QueryString["prev"];
                return Helper.GetNewBuild(prevString);
            }
        }

        private Build Curr
        {
            get
            {
                string prevString = Request.QueryString["curr"];
                return Helper.GetNewBuild(prevString);
            }
        }


        Dictionary<string, StringBuilder> _categories;

        public string GetHtml(string category)
        {
            if (_categories == null)
            {
                _categories = new Dictionary<string, StringBuilder>();

                BuildCategories(Prev, Curr);
            }

            StringBuilder val;
            _categories.TryGetValue(category, out val);

            return val == null ? "No changes." : val.ToString();
        }

        private void BuildCategories(Build prev, Build curr)
        {
            using (var context = new whatsgoingonEntities())
            {
                context.Configuration.LazyLoadingEnabled = true;

                var previous =
                    context.Builds.Single(
                        b =>
                        (b.VMajor == prev.VMajor && b.VMinor == prev.VMinor && b.VPatch == prev.VPatch &&
                         b.VBuild == prev.VBuild && b.GitSha == prev.GitSha));
                var current =
                    context.Builds.Single(
                        b =>
                        (b.VMajor == curr.VMajor && b.VMinor == curr.VMinor && b.VPatch == curr.VPatch &&
                         b.VBuild == curr.VBuild && b.GitSha == curr.GitSha));

                var builds = context.Builds.ToList().Where(b =>
                                              string.Compare(b.Version, curr.Version) <= 0
                                              &&
                                              string.Compare(b.Version, prev.Version) >= 0).ToList();


                string jiraUrl = ConfigurationManager.AppSettings["JiraUrl"];

                var jiras = new StringBuilder();

                foreach(var build in builds)
                    BuildData.GetJira(build, jiraUrl, jiras);

                if (jiras.Length != 0)
                    _categories.Add("JIRA", jiras);

                List<int> ignored;
                List<BuildDiff> diffs = Helper.GetDiffs(previous, current, out ignored, false);

                foreach (var diff in diffs)
                {
                    // Prev data...
                    StringBuilder html = HtmlBuilder.GetHtml(diff.Diff, diff.Type, diff.PreviousData, diff.CurrentData);

                    if (!_categories.ContainsKey(diff.Category))
                    {
                        _categories.Add(diff.Category, new StringBuilder());
                    }

                    _categories[diff.Category].Append(html);
                }
            }
        }
    }
}