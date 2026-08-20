using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace U5_18 {
    /// <summary>
    /// The main web form for handling server data uploads, sorting, and inactivity analysis.
    /// </summary>
    public partial class Form1 : System.Web.UI.Page {
        /// <summary>
        /// Gets the server-mapped physical path for the results text file.
        /// </summary>
        private string ResultFilePath => Server.MapPath("~/App_Data/Rezultatai.txt");

        /// <summary>
        /// Handles the page load event to restore dynamic tables and results from the Session.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Servers"] != null)
            {
                var servers = (List<Server>)Session["Servers"];
                var speeds = (List<ServerSpeed>)Session["Speeds"];

                GenerateSpeedsTable(speeds);
                GenerateServersTable(servers);

                if (!IsPostBack)
                {
                    UpdateDateList(servers);
                }

                PanelAnalysis.Visible = true;
            }

            if (Session["InactiveResults"] != null)
            {
                RenderInactiveTable((List<InactiveHourServer>)Session["InactiveResults"]);
            }

            if (Session["MinSendersHtml"] != null)
            {
                PlaceHolderMinSenders.Controls.Add(new LiteralControl((string)Session["MinSendersHtml"]));
            }
        }

        /// <summary>
        /// Handles the file upload process, parses server and speed data, and saves initial data to the results file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected void Button1_Click(object sender, EventArgs e) {
            LabelMessage.Visible = false;
            LabelMessage.Text = "";

            try {
                if (!FileUpload1.HasFiles) {
                    throw new Exception("Nepasirinkote jokių failų įkėlimui.");
                }

                string path = Server.MapPath("~/App_Data/TempFiles/");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                new DirectoryInfo(path).GetFiles().ToList().ForEach(file => file.Delete());

                FileUpload1.PostedFiles.ToList().ForEach(postedFile =>
                    postedFile.SaveAs(Path.Combine(path, Path.GetFileName(postedFile.FileName)))
                );

                var allFiles = new DirectoryInfo(path).GetFiles("*.txt").ToList();

                if (!allFiles.Any()) {
                    throw new Exception("Kataloge nerasta tinkamų tekstinių (.txt) failų.");
                }

                var speeds = allFiles
                    .Where(file => file.Name.ToLower().Contains("greit") || file.Name.ToLower().Contains("speed"))
                    .SelectMany(file => {
                        try {
                            using (var stream = file.OpenRead()) return InOutUtils.ReadSpeedsFromStream(stream);
                        }
                        catch {
                            throw new Exception($"Klaidingas greičių failo formatas: {file.Name}");
                        }
                    }).ToList();

                if (!speeds.Any()) {
                    throw new Exception("Tarp įkeltų failų nerastas nei vienas serverio greičių failas.");
                }

                var servers = allFiles
                    .Where(file => !(file.Name.ToLower().Contains("greit") || file.Name.ToLower().Contains("speed")))
                    .Select(file => {
                        try {
                            using (var stream = file.OpenRead()) return InOutUtils.ReadServerFromStream(stream);
                        }
                        catch {
                            throw new Exception($"Klaidingas serverio duomenų formatas faile: {file.Name}");
                        }
                    })
                    .Where(serverObj => serverObj != null).ToList();

                if (!servers.Any()) {
                    throw new Exception("Nerasta jokių tinkamų serverio duomenų failų.");
                }

                Session["Servers"] = servers;
                Session["Speeds"] = speeds;
                Session["InactiveResults"] = null;
                Session["MinSendersHtml"] = null;

                InOutUtils.PrintStartingData(ResultFilePath, servers, speeds);

                UpdateDateList(servers);
                PlaceHolderData.Controls.Clear();
                GenerateSpeedsTable(speeds);
                GenerateServersTable(servers);

                PanelAnalysis.Visible = true;
            }
            catch (Exception ex) {
                LabelMessage.Text = "Klaida: " + ex.Message;
                LabelMessage.Visible = true;
                PanelAnalysis.Visible = false;
            }
        }

        /// <summary>
        /// Performs sorting on the server list and executes the inactive hours analysis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <exception cref="Exception">Thrown if session data is missing.</exception>
        protected void Button2_Click(object sender, EventArgs e) {
            var servers = (List<Server>)Session["Servers"] ?? throw new Exception("Nėra duomenų");
            var speeds = (List<ServerSpeed>)Session["Speeds"] ?? throw new Exception("Nėra greičių duomenų");

            servers.Sort();

            var results = TaskUtils.FindInactiveHours(servers, speeds);
            Session["InactiveResults"] = results;

            List<string> fileLines = results.Select(res =>
                $"Serveris: {res.ServerName,-15} Data: {res.Date.ToShortDateString(),-10} Valandos: " +
                string.Join(", ", Enumerable.Range(0, res.GetHoursCount()).Select(i => res.GetHour(i) + ":00"))
            ).ToList();

            InOutUtils.PrintResults(ResultFilePath, "NEAKTYVUMO ANALIZĖ", fileLines);

            RenderInactiveTable(results);
        }

        /// <summary>
        /// Renders the results of the inactivity analysis into an HTML table.
        /// </summary>
        /// <param name="results">A list containing inactive hour data for each server.</param>
        private void RenderInactiveTable(List<InactiveHourServer> results) {
            PlaceHolderInactive.Controls.Clear();
            PlaceHolderInactive.Controls.Add(new LiteralControl("<h3>Neaktyvumo ataskaita</h3>"));

            if (results.Any()) {
                Table table = new Table { CssClass = "table-style" };

                TableHeaderRow header = new TableHeaderRow();
                header.Cells.Add(new TableHeaderCell { Text = "Serveris" });
                header.Cells.Add(new TableHeaderCell { Text = "Data" });
                header.Cells.Add(new TableHeaderCell { Text = "Neaktyvios valandos" });
                table.Rows.Add(header);

                foreach (var result in results) {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell { Text = result.ServerName });
                    row.Cells.Add(new TableCell { Text = result.Date.ToShortDateString() });

                    List<string> hourStrings = new List<string>();
                    for (int i = 0; i < result.GetHoursCount(); i++) {
                        hourStrings.Add(result.GetHour(i) + ":00");
                    }

                    row.Cells.Add(new TableCell { Text = string.Join(", ", hourStrings) });
                    table.Rows.Add(row);
                }
                PlaceHolderInactive.Controls.Add(table);
            }
            else {
                PlaceHolderInactive.Controls.Add(new LiteralControl("<p>Neaktyvių valandų nerasta.</p>"));
            }
        }

        /// <summary>
        /// Finds and displays the persons who sent the minimum number of letters on a selected date.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected void Button3_Click(object sender, EventArgs e) {
            var servers = (List<Server>)Session["Servers"];

            DateTime selectedDate = DateTime.Parse(DropDownListDates.SelectedValue);
            List<string> minSendersList;
            int minVal = TaskUtils.FindMinSendersByDate(servers, selectedDate, out minSendersList);

            List<string> fileLines = new List<string> {
                $"Data: {selectedDate:yyyy-MM-dd}",
                $"Mažiausias laiškų kiekis: {minVal}",
                $"Siuntėjai: {string.Join(", ", minSendersList)}"
            };
            InOutUtils.PrintResults(ResultFilePath, "MAŽIAUSIAI SIUNTĘ ASMENYS", fileLines);

            string html = $@"
                <div class='analysis-result-box'>
                    <h3>Mažiausiai siuntę ({selectedDate:yyyy-MM-dd})</h3>
                    <p>Laiškų kiekis: <b>{minVal}</b></p>
                    <p>Siuntėjai: <b>{string.Join(", ", minSendersList)}</b></p>
                </div>";

            Session["MinSendersHtml"] = html;
            PlaceHolderMinSenders.Controls.Clear();
            PlaceHolderMinSenders.Controls.Add(new LiteralControl(html));
        }

        /// <summary>
        /// Populates the dropdown list with distinct dates found in the uploaded server data.
        /// </summary>
        /// <param name="servers">The list of loaded servers.</param>
        private void UpdateDateList(List<Server> servers) {
            DropDownListDates.Items.Clear();
            var dates = servers.Select(server => server.Date.Date).Distinct().OrderBy(date => date);
            foreach (var date in dates)
                DropDownListDates.Items.Add(new ListItem(date.ToShortDateString(), date.ToString("yyyy-MM-dd")));
        }


        private void GenerateSpeedsTable(List<ServerSpeed> speeds)
        {
            PlaceHolderData.Controls.Add(new LiteralControl("<h3>Serverių greičiai</h3>"));
            PlaceHolderData.Controls.Add(CreateSpeedSummaryTable(speeds));
        }

        private void GenerateServersTable(List<Server> servers)
        {
            servers.ForEach(server => {
                PlaceHolderData.Controls.Add(new LiteralControl($"<h4>Serveris: {server.ServerName} ({server.Date:yyyy-MM-dd})</h4>"));
                PlaceHolderData.Controls.Add(CreateLettersTable(server));
            });
        }

        private Table CreateSpeedSummaryTable(List<ServerSpeed> speeds)
        {
            Table table = new Table { CssClass = "table-style" };

            TableHeaderRow header = new TableHeaderRow();
            header.Cells.Add(new TableHeaderCell { Text = "Serverio pavadinimas" });
            header.Cells.Add(new TableHeaderCell { Text = "Greitis (B/s)" });
            table.Rows.Add(header);

            speeds.ForEach(speed => {
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell { Text = speed.ServerName });
                row.Cells.Add(new TableCell { Text = speed.Speed.ToString(), CssClass = "text-right" });
                table.Rows.Add(row);
            });

            return table;
        }

        /// <summary>
        /// Creates a formatted HTML table containing all letter records for a specific server.
        /// </summary>
        /// <param name="server">The server object containing letters.</param>
        /// <returns>A Table control populated with letter data.</returns>
        private Table CreateLettersTable(Server server) {
            Table table = new Table { CssClass = "table-style" };
            TableHeaderRow header = new TableHeaderRow();
            header.Cells.Add(new TableHeaderCell { Text = "Laikas" });
            header.Cells.Add(new TableHeaderCell { Text = "Siuntėjas" });
            header.Cells.Add(new TableHeaderCell { Text = "Gavėjas" });
            header.Cells.Add(new TableHeaderCell { Text = "Dydis (B)" });
            table.Rows.Add(header);

            for (int i = 0; i < server.GetLettersCount(); i++) {
                var letter = server.GetLetter(i);
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell { Text = letter.Time.ToString(@"hh\:mm\:ss") });
                row.Cells.Add(new TableCell { Text = letter.SenderAddress });
                row.Cells.Add(new TableCell { Text = letter.RecieverAddress });
                row.Cells.Add(new TableCell { Text = letter.BiteSize.ToString(), CssClass = "text-right" });
                table.Rows.Add(row);
            }
            return table;
        }
    }
}