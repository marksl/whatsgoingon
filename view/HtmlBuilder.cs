using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Model;
using run.Misc;

namespace view
{
    public class HtmlBuilder
    {
        public static StringBuilder GetHtml(string diff, string type, string prev, string curr)
        {
            Diff.Item[] items = DeserializeObject(diff);

            StringBuilder html = type == "Method"
                                     ? GetSingleLineHtml(prev, curr, items)
                                     : GetMultilineHtml(prev, curr, items);
            return html;
        }

        public static Diff.Item[] DeserializeObject(string deSerialize)
        {
            var serializer = new XmlSerializer(typeof(Diff.Item[]));
            var rdr = new StringReader(deSerialize);
            var resultingMessage = (Diff.Item[])serializer.Deserialize(rdr);
            return resultingMessage;
        }



        private static StringBuilder GetSingleLineHtml(string a_line, string b_line, Diff.Item[] diffs)
        {
            var builder = new StringBuilder();

            int pos = 0;
            for (int n = 0; n < diffs.Length; n++)
            {
                Diff.Item it = diffs[n];

                // write unchanged chars
                while ((pos < it.StartB) && (pos < b_line.Length))
                {
                    builder.Append(HttpUtility.HtmlEncode(b_line[pos]));
                    pos++;
                } // while

                // write deleted chars
                if (it.deletedA > 0)
                {
                    builder.Append("<span class='d'>");
                    for (int m = 0; m < it.deletedA; m++)
                    {

                        builder.Append(HttpUtility.HtmlEncode(a_line[it.StartA + m]));
                    } // for
                    builder.Append("</span>");
                }

                // write inserted chars
                if (pos < it.StartB + it.insertedB)
                {
                    builder.Append("<span class='i'>");
                    while (pos < it.StartB + it.insertedB)
                    {
                        builder.Append(HttpUtility.HtmlEncode(b_line[pos]));
                        pos++;
                    } // while
                    builder.Append("</span>");
                } // if
            } // while

            // write rest of unchanged chars
            while (pos < b_line.Length)
            {
                builder.Append(HttpUtility.HtmlEncode(b_line[pos]));
                pos++;
            } // while

            builder.Append("<br/><br/>");

            return builder;
        }

        private static StringBuilder GetMultilineHtml(string a, string b, Diff.Item[] f)
        {
            var builder = new StringBuilder(200);

            string[] aLines = a.Split('\n');
            string[] bLines = b.Split('\n');

            int n = 0;
            for (int fdx = 0; fdx < f.Length; fdx++)
            {
                Diff.Item aItem = f[fdx];

                // write unchanged lines
                while ((n < aItem.StartB) && (n < bLines.Length))
                {
                    WriteLine(builder, null, bLines[n]);
                    n++;
                } // while

                // write deleted lines
                for (int m = 0; m < aItem.deletedA; m++)
                {
                    WriteLine(builder, "d", aLines[aItem.StartA + m]);
                } // for

                // write inserted lines
                while (n < aItem.StartB + aItem.insertedB)
                {
                    WriteLine(builder, "i", bLines[n]);
                    n++;
                } // while
            } // while

            // write rest of unchanged lines
            while (n < bLines.Length)
            {
                WriteLine(builder, null, bLines[n]);
                n++;
            } // while

            builder.Append("<br/>");

            return builder;
        }

        private static void WriteLine(StringBuilder builder, string typ, string aText)
        {
            builder.Append("<span ");
            if (typ != null)
                builder.Append(" class=\"" + typ + "\"");
            aText = HttpContext.Current.Server.HtmlEncode(aText).Replace("\r", "").Replace(" ", "&nbsp;");

            builder.Append(">" + aText + "</span>\n");

            builder.Append("<br/>");

        } 
    }
}