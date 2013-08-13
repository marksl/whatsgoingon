using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Model;

namespace view
{
    public partial class Default : BasePage
    {
        protected override void OnLoad(System.EventArgs e)
        {
            if (!IsPostBack)
            {
                PreviousDropDownList.DataSource = BuildNames;
                PreviousDropDownList.DataBind();

                CurrentDropDownList.DataSource = BuildNames;
                CurrentDropDownList.DataBind();
            }

            base.OnLoad(e);
        }

        
        private List<Data> _buildNames;
        public List<Data> BuildNames
        {
            get
            {
                if (_buildNames == null)
                {
                    using (var context = new whatsgoingonEntities())
                    {
                        _buildNames = GetAll(context).ToList().Select(BuildData.GetNameData).ToList();
                    }
                }
                return _buildNames;
            }
        }

        private List<BuildData> _builds;
        public List<BuildData> Builds
        {
            get
            {
                if (_builds == null)
                {
                    var builds = new List<BuildData>();
                    using (var context = new whatsgoingonEntities())
                    {
                        context.Configuration.LazyLoadingEnabled = true;

                        var all = (GetAll(context));

                        int num;
                        if (int.TryParse(ConfigurationManager.AppSettings["NumBuilds"], out num))
                        {
                            var filtered = all.Take(num).ToList();
                            builds.AddRange(filtered.Select(existingBuild => new BuildData(existingBuild)));    
                        }
                        else
                        {
                            builds.AddRange(all.ToList().Select(existingBuild => new BuildData(existingBuild)));    
                        }
                    }

                    if (Request.QueryString["showempties"] == null)
                    {
                        BuildData first = builds.First();
                        builds.RemoveAll(b => b != first && b.IsEmpty);
                    }

                    _builds = builds;
                }

                return _builds;
            }
        }

        

        protected string NumBuilds
        {
            get { return Builds.Count.ToString(); }
        }

        protected void ViewReport_Click(object sender, System.EventArgs e)
        {
            // Redirect to page
            Response.Redirect(string.Format("Filtered.aspx?prev={0}&curr={1}",
                PreviousDropDownList.SelectedValue,
                CurrentDropDownList.SelectedValue));


        }

        protected void ShowEmpties_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?showempties=1");
        }
    }
}