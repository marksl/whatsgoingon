using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Model;

namespace view
{
    public class BasePage :Page
    {
        private List<string> _categories;
        public List<string> Categories
        {
            get { return _categories ?? (_categories = GetCategories()); }
        }

        private static List<string> GetCategories()
        {
            var list = new List<string>() {"JIRA"};
            using (var context = new whatsgoingonEntities())
            {
                list.AddRange(context.Categories.Select(c => c.Name).ToList());
                return list;
            }
        }

        protected static IOrderedQueryable<Build> GetAll(whatsgoingonEntities context)
        {
            return from b in context.Builds.Where(b => !b.Incomplete)
                   orderby b.VMajor descending,
                       b.VMinor descending,
                       b.VPatch descending,
                       b.VBuild descending
                   select b;
        }
    }
}