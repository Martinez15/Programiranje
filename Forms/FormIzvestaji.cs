using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormIzvestaji : Form
    {
        private Label lblNaslov;
        private Label lblPeriod;
        private DateTimePicker dtpOd;
        private Label lblDo;
        private DateTimePicker dtpDo;
        private Button btnOsvezi;

        private Label lblUkupanPrihodText;
        private Label lblUkupanPrihodVrednost;

        private TabControl tcIzvestaji;
        private TabPage tpVozaci;
        private TabPage tpPutnici;
        private TabPage tpOpstaStats;

        private DataGridView dgvVozaciStats;
        private DataGridView dgvPutniciStats;

        // Opšta statistika kontrole
        private Label lblOpstaUkupnoVoznji;
        private Label lblOpstaZavrseno;
        private Label lblOpstaOtkazano;
        private Label lblOpstaAktivno;
        private Label lblOpstaProsecnaOcena;
        private Label lblOpstaBrojVozaca;
        private Label lblOpstaBrojPutnika;

        public FormIzvestaji()
        {
            InitializeComponent();
            PrikaziIzvestaje();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Finansijski Izvestaji";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 9.5F);

            lblNaslov = new Label
            {
                Text = "Finansijski izvestaji i statistika",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };
            this.Controls.Add(lblNaslov);

            // Filter perioda
            lblPeriod = new Label
            {
                Text = "Period od:",
                ForeColor = Color.White,
                Location = new Point(20, 62),
                Size = new Size(70, 20)
            };
            this.Controls.Add(lblPeriod);

            dtpOd = new DateTimePicker
            {
                Location = new Point(95, 59),
                Size = new Size(130, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1) // Početak mesec dana unazad
            };
            dtpOd.ValueChanged += Filter_Changed;
            this.Controls.Add(dtpOd);

            lblDo = new Label
            {
                Text = "do:",
                ForeColor = Color.White,
                Location = new Point(240, 62),
                Size = new Size(30, 20)
            };
            this.Controls.Add(lblDo);

            dtpDo = new DateTimePicker
            {
                Location = new Point(275, 59),
                Size = new Size(130, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(1)
            };
            dtpDo.ValueChanged += Filter_Changed;
            this.Controls.Add(dtpDo);

            btnOsvezi = new Button
            {
                Text = "Osvezi podatke",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(420, 56),
                Size = new Size(140, 30),
                Cursor = Cursors.Hand
            };
            btnOsvezi.FlatAppearance.BorderSize = 0;
            btnOsvezi.Click += (s, e) => PrikaziIzvestaje();
            this.Controls.Add(btnOsvezi);

            // Ukupan prihod kartica (gore desno)
            Panel pnlPrihodCard = new Panel
            {
                Location = new Point(620, 15),
                Size = new Size(290, 75),
                BackColor = Color.FromArgb(55, 65, 81)
            };

            lblUkupanPrihodText = new Label
            {
                Text = "UKUPAN PRIHOD ZA PERIOD",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 163, 175),
                Location = new Point(15, 10),
                Size = new Size(260, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlPrihodCard.Controls.Add(lblUkupanPrihodText);

            lblUkupanPrihodVrednost = new Label
            {
                Text = "0 RSD",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                Location = new Point(15, 30),
                Size = new Size(260, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlPrihodCard.Controls.Add(lblUkupanPrihodVrednost);

            this.Controls.Add(pnlPrihodCard);

            // TabControl za izveštaje
            tcIzvestaji = new TabControl
            {
                Location = new Point(20, 110),
                Size = new Size(890, 370),
                Font = new Font("Segoe UI", 10F)
            };

            // 1. Tab: Vozači
            tpVozaci = new TabPage("Statistika vozaca") { BackColor = Color.FromArgb(55, 65, 81) };
            dgvVozaciStats = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.Black,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvVozaciStats.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvVozaciStats.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVozaciStats.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvVozaciStats.EnableHeadersVisualStyles = false;
            tpVozaci.Controls.Add(dgvVozaciStats);
            tcIzvestaji.TabPages.Add(tpVozaci);

            // 2. Tab: Putnici
            tpPutnici = new TabPage("Top putnici (Top 10)") { BackColor = Color.FromArgb(55, 65, 81) };
            dgvPutniciStats = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.Black,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvPutniciStats.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvPutniciStats.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPutniciStats.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvPutniciStats.EnableHeadersVisualStyles = false;
            tpPutnici.Controls.Add(dgvPutniciStats);
            tcIzvestaji.TabPages.Add(tpPutnici);

            // 3. Tab: Opšta statistika sistema
            tpOpstaStats = new TabPage("Opsta statistika sistema") { BackColor = Color.FromArgb(55, 65, 81) };
            Panel pnlOpsta = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30) };

            lblOpstaUkupnoVoznji = KreirajOpstaLabel("Ukupno registrovanih voznji u sistemu: ...", 20, pnlOpsta);
            lblOpstaZavrseno = KreirajOpstaLabel("Uspesno zavrsenih voznji: ...", 55, pnlOpsta);
            lblOpstaOtkazano = KreirajOpstaLabel("Otkazanih voznji: ...", 90, pnlOpsta);
            lblOpstaAktivno = KreirajOpstaLabel("Trenutno aktivnih voznji u toku/na cekanju: ...", 125, pnlOpsta);
            lblOpstaProsecnaOcena = KreirajOpstaLabel("Prosecna ocena svih taxi vozaca: ...", 160, pnlOpsta);
            lblOpstaBrojVozaca = KreirajOpstaLabel("Ukupan broj vozila i vozaca na duznosti: ...", 195, pnlOpsta);
            lblOpstaBrojPutnika = KreirajOpstaLabel("Ukupan broj registrovanih korisnika (putnika): ...", 230, pnlOpsta);

            tpOpstaStats.Controls.Add(pnlOpsta);
            tcIzvestaji.TabPages.Add(tpOpstaStats);

            this.Controls.Add(tcIzvestaji);

            Button btnZatvori = new Button
            {
                Text = "Zatvori",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(770, 500),
                Size = new Size(140, 35),
                Cursor = Cursors.Hand
            };
            btnZatvori.FlatAppearance.BorderSize = 0;
            btnZatvori.Click += (s, e) => this.Close();
            this.Controls.Add(btnZatvori);
        }

        private Label KreirajOpstaLabel(string text, int top, Panel parent)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, top),
                Size = new Size(800, 25)
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            PrikaziIzvestaje();
        }

        private void PrikaziIzvestaje()
        {
            DateTime odDatuma = dtpOd.Value;
            DateTime doDatuma = dtpDo.Value;

            if (odDatuma > doDatuma)
            {
                MessageBox.Show("Datum 'od' mora biti pre datuma 'do'.", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Prihod za period
            decimal prihod = AppState.IzvestajService.GetUkupanPrihod(odDatuma, doDatuma);
            lblUkupanPrihodVrednost.Text = $"{prihod:N0} RSD";

            // 2. Statistika vozača
            var vozaciStats = AppState.IzvestajService.GetStatistikaVozaca(odDatuma, doDatuma);
            dgvVozaciStats.DataSource = null;
            if (vozaciStats.Count > 0)
            {
                dgvVozaciStats.DataSource = vozaciStats.Select(s => new
                {
                    ID = s.VozacID,
                    ImePrezime = s.ImePrezime,
                    BrojVoznji = s.BrojVoznji,
                    Zarada = s.UkupnaZarada.ToString("N0") + " RSD",
                    ProsekOcena = s.ProsecnaOcena > 0 ? s.ProsecnaOcena.ToString("0.0") + " ★" : "-"
                }).ToList();
            }

            // 3. Najaktivniji putnici
            var putniciStats = AppState.IzvestajService.GetTopPutnici(odDatuma, doDatuma);
            dgvPutniciStats.DataSource = null;
            if (putniciStats.Count > 0)
            {
                dgvPutniciStats.DataSource = putniciStats.Select((s, idx) => new
                {
                    Mesto = (idx + 1).ToString() + ".",
                    ID = s.PutnikID,
                    ImePrezime = s.ImePrezime,
                    BrojVoznji = s.BrojVoznji,
                    UkupnoPotroseno = s.UkupnoPotroseno.ToString("N0") + " RSD"
                }).ToList();
            }

            // 4. Opšta statistika
            var opsta = AppState.IzvestajService.GetOpstaStatistika();
            lblOpstaUkupnoVoznji.Text = $"Ukupno registrovanih voznji u sistemu: {opsta.UkupnoVoznji}";
            lblOpstaZavrseno.Text = $"Uspesno zavrsenih voznji: {opsta.ZavrsenoVoznji}";
            lblOpstaOtkazano.Text = $"Otkazanih voznji: {opsta.OtkazanoVoznji}";
            lblOpstaAktivno.Text = $"Trenutno aktivnih voznji u toku/na cekanju: {opsta.AktivnoVoznji}";
            lblOpstaProsecnaOcena.Text = $"Prosecna ocena svih taxi vozaca: {opsta.ProsecnaOcenaVozaca:0.00} ★";
            lblOpstaBrojVozaca.Text = $"Ukupan broj vozila i vozaca na duznosti: {opsta.BrojRegistrovanihVozaca}";
            lblOpstaBrojPutnika.Text = $"Ukupan broj registrovanih korisnika (putnika): {opsta.BrojRegistrovanihPutnika}";
        }
    }
}
