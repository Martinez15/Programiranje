using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormOcene : Form
    {
        private Label lblNaslov;
        private Label lblRangLista;
        private DataGridView dgvVozaci;

        private Label lblKomentariSection;
        private DataGridView dgvKomentari;

        // Ocenjivanje (vidljivo samo za Putnike ili Admine koji imaju završene vožnje)
        private GroupBox gbOceni;
        private Label lblIzborVoznje;
        private ComboBox cbVoznjeZaOcenjivanje;
        private Label lblZvezdice;
        private ComboBox cbOcena;
        private Label lblKomentar;
        private TextBox txtKomentar;
        private Button btnPosaljiOcenu;

        public FormOcene()
        {
            InitializeComponent();
            UcitajRangListu();
            UcitajVoznjeZaOcenjivanje();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Ocene i Rang Lista Vozaca";
            this.Size = new Size(950, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 9.5F);

            lblNaslov = new Label
            {
                Text = "Ocene i rang lista vozaca",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };
            this.Controls.Add(lblNaslov);

            // 1. Rang lista vozača
            lblRangLista = new Label
            {
                Text = "Rang lista vozaca (od najbolje ocenjenih):",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(20, 60),
                Size = new Size(400, 20)
            };
            this.Controls.Add(lblRangLista);

            dgvVozaci = new DataGridView
            {
                Location = new Point(20, 85),
                Size = new Size(450, 190),
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
            dgvVozaci.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvVozaci.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVozaci.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvVozaci.EnableHeadersVisualStyles = false;
            dgvVozaci.SelectionChanged += DgvVozaci_SelectionChanged;
            dgvVozaci.DataBindingComplete += DgvVozaci_DataBindingComplete;
            this.Controls.Add(dgvVozaci);

            // 2. Komentari i ocene putnika za izabranog vozača
            lblKomentariSection = new Label
            {
                Text = "Komentari i ocene putnika za izabranog vozaca:",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(20, 295),
                Size = new Size(450, 20)
            };
            this.Controls.Add(lblKomentariSection);

            dgvKomentari = new DataGridView
            {
                Location = new Point(20, 320),
                Size = new Size(450, 160),
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
            dgvKomentari.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvKomentari.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKomentari.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvKomentari.EnableHeadersVisualStyles = false;
            dgvKomentari.DataBindingComplete += DgvKomentari_DataBindingComplete;
            this.Controls.Add(dgvKomentari);

            // 3. Panel/GroupBox za ocenjivanje vožnje (desno)
            gbOceni = new GroupBox
            {
                Text = "Oceni svoju nedavnu voznju",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(500, 75),
                Size = new Size(410, 405),
                BackColor = Color.FromArgb(55, 65, 81)
            };

            lblIzborVoznje = new Label
            {
                Text = "Izaberite zavrsenu voznju:",
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(20, 35),
                Size = new Size(370, 20)
            };
            gbOceni.Controls.Add(lblIzborVoznje);

            cbVoznjeZaOcenjivanje = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 60),
                Size = new Size(370, 25)
            };
            gbOceni.Controls.Add(cbVoznjeZaOcenjivanje);

            lblZvezdice = new Label
            {
                Text = "Ocena (1 do 5 zvezda):",
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(20, 110),
                Size = new Size(370, 20)
            };
            gbOceni.Controls.Add(lblZvezdice);

            cbOcena = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 135),
                Size = new Size(150, 25)
            };
            cbOcena.Items.Add("5 - Odlican");
            cbOcena.Items.Add("4 - Vrlo dobar");
            cbOcena.Items.Add("3 - Dobar");
            cbOcena.Items.Add("2 - Dovoljan");
            cbOcena.Items.Add("1 - Los");
            cbOcena.SelectedIndex = 0;
            gbOceni.Controls.Add(cbOcena);

            lblKomentar = new Label
            {
                Text = "Komentar / utisci sa voznje:",
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(20, 185),
                Size = new Size(370, 20)
            };
            gbOceni.Controls.Add(lblKomentar);

            txtKomentar = new TextBox
            {
                Multiline = true,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(20, 210),
                Size = new Size(370, 100)
            };
            gbOceni.Controls.Add(txtKomentar);

            btnPosaljiOcenu = new Button
            {
                Text = "POSALJI OCENU",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 335),
                Size = new Size(370, 40),
                Cursor = Cursors.Hand
            };
            btnPosaljiOcenu.FlatAppearance.BorderSize = 0;
            btnPosaljiOcenu.Click += BtnPosaljiOcenu_Click;
            gbOceni.Controls.Add(btnPosaljiOcenu);

            this.Controls.Add(gbOceni);

            // Sakrivanje panela za ocenjivanje ako korisnik nema pravo
            if (AppState.TrenutniKorisnik == null || !AppState.TrenutniKorisnik.MozeOcenitiVozaca())
            {
                gbOceni.Visible = false;
                // Proširujemo gridove skroz na desnu stranu da lepše izgleda
                dgvVozaci.Width = 890;
                dgvKomentari.Width = 890;
                lblKomentariSection.Width = 890;
            }

            Button btnZatvori = new Button
            {
                Text = "Zatvori",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(770, 495),
                Size = new Size(140, 35),
                Cursor = Cursors.Hand
            };
            btnZatvori.FlatAppearance.BorderSize = 0;
            btnZatvori.Click += (s, e) => this.Close();
            this.Controls.Add(btnZatvori);
        }

        private void UcitajRangListu()
        {
            try
            {
                var vozaci = AppState.OcenaService.GetRangListaVozaca();

                // Privremeno otkacimo event da ne bi crashovao tokom DataSource assignmenta
                dgvVozaci.SelectionChanged -= DgvVozaci_SelectionChanged;

                dgvVozaci.DataSource = null;
                dgvVozaci.Columns.Clear();

                var displayList = vozaci.Select((v, index) => new
                {
                    Mesto = (index + 1).ToString() + ".",
                    ID = v.VozacID,
                    Ime = v.ImePrezime,
                    Prosek = v.ProsecnaOcena > 0 ? v.ProsecnaOcena.ToString("0.0#") + " ★" : "-",
                    BrojOcena = v.BrojOcena
                }).ToList();

                dgvVozaci.DataSource = displayList;

                // Sada ponovo kacimo event - sada je DataGridView spreman
                dgvVozaci.SelectionChanged += DgvVozaci_SelectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska pri ucitavanju rang liste: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UcitajVoznjeZaOcenjivanje()
        {
            if (AppState.TrenutniKorisnik == null || !AppState.TrenutniKorisnik.MozeOcenitiVozaca()) return;

            cbVoznjeZaOcenjivanje.Items.Clear();

            string putnikId = "";
            if (AppState.TrenutniKorisnik is Putnik p) putnikId = p.PutnikID;
            else return; // Admin ne ocenjuje svoje vožnje jer ih nema u tom smislu

            // Pronalazimo sve završene i neocenjene vožnje ovog putnika
            var voznje = AppState.VoznjaService.GetVoznjeZaKorisnika(AppState.TrenutniKorisnik);
            var neocenjene = voznje.Where(v => v.Status == StatusVoznje.Zavrsena && v.Ocena == null).ToList();

            var korisnici = AppState.KorisnikRepo.GetAll();

            foreach (var v in neocenjene)
            {
                var vozac = korisnici.OfType<Vozac>().FirstOrDefault(d => d.VozacID == v.VozacID);
                string text = $"{v.VoznjaID} - {v.Odrediste} (Vozac: {vozac?.ImePrezime ?? v.VozacID})";
                cbVoznjeZaOcenjivanje.Items.Add(new ComboBoxItem(text, v.VoznjaID));
            }

            if (cbVoznjeZaOcenjivanje.Items.Count > 0)
            {
                cbVoznjeZaOcenjivanje.SelectedIndex = 0;
                btnPosaljiOcenu.Enabled = true;
            }
            else
            {
                cbVoznjeZaOcenjivanje.Items.Add("Nema neocenjenih voznji.");
                cbVoznjeZaOcenjivanje.SelectedIndex = 0;
                btnPosaljiOcenu.Enabled = false;
            }
        }

        private void DgvVozaci_SelectionChanged(object sender, EventArgs e)
        {
            UcitajKomentare();
        }

        private void UcitajKomentare()
        {
            try
            {
                dgvKomentari.DataSource = null;
                dgvKomentari.Columns.Clear();

                if (dgvVozaci.SelectedRows.Count == 0) return;

                // Bezbedni pristup celiji - proveravamo da kolona "ID" postoji
                var idCell = dgvVozaci.SelectedRows[0].Cells["ID"];
                if (idCell == null || idCell.Value == null) return;

                string vozacId = idCell.Value.ToString();
                if (string.IsNullOrEmpty(vozacId)) return;

                var komentari = AppState.OcenaService.GetKomentariZaVozaca(vozacId);

                var displayKomentari = komentari.Select(k => new
                {
                    Ocena = k.Ocena.HasValue ? k.Ocena.Value.ToString() + " ★" : "-",
                    Komentar = k.Komentar ?? "",
                    Datum = k.VremeDolaska?.ToString("dd.MM.yyyy") ?? ""
                }).ToList();

                dgvKomentari.DataSource = displayKomentari;
            }
            catch { /* tihо ignorisemo greske u prikazu komentara */ }
        }

        private void BtnPosaljiOcenu_Click(object sender, EventArgs e)
        {
            if (cbVoznjeZaOcenjivanje.SelectedItem == null) return;
            var izabrano = cbVoznjeZaOcenjivanje.SelectedItem as ComboBoxItem;
            if (izabrano == null) return; // Nema izabrane validne vožnje

            string voznjaId = izabrano.Value;
            
            // Parsiramo ocenu
            int ocenaVal = 5;
            string sel = cbOcena.SelectedItem.ToString();
            if (sel.StartsWith("5")) ocenaVal = 5;
            else if (sel.StartsWith("4")) ocenaVal = 4;
            else if (sel.StartsWith("3")) ocenaVal = 3;
            else if (sel.StartsWith("2")) ocenaVal = 2;
            else if (sel.StartsWith("1")) ocenaVal = 1;

            string komentar = txtKomentar.Text.Trim();

            try
            {
                AppState.OcenaService.OceniVozaca(voznjaId, ocenaVal, komentar);
                MessageBox.Show("Hvala! Vasa ocena je uspesno zabelezena.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtKomentar.Text = "";
                UcitajRangListu();
                UcitajVoznjeZaOcenjivanje();
                UcitajKomentare();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska pri ocenjivanju: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PodesiSirineKolonaVozaci()
        {
            if (dgvVozaci.Columns.Count > 0)
            {
                if (dgvVozaci.Columns["Mesto"] != null) dgvVozaci.Columns["Mesto"].Width = 50;
                if (dgvVozaci.Columns["ID"] != null) dgvVozaci.Columns["ID"].Width = 85;
                if (dgvVozaci.Columns["Prosek"] != null) dgvVozaci.Columns["Prosek"].Width = 80;
            }
        }

        private void PodesiSirineKolonaKomentari()
        {
            if (dgvKomentari.Columns.Count > 0)
            {
                if (dgvKomentari.Columns["Ocena"] != null) dgvKomentari.Columns["Ocena"].Width = 60;
                if (dgvKomentari.Columns["Datum"] != null) dgvKomentari.Columns["Datum"].Width = 95;
            }
        }

        private void DgvVozaci_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            PodesiSirineKolonaVozaci();
        }

        private void DgvKomentari_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            PodesiSirineKolonaKomentari();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UcitajKomentare();
        }

        // Pomoćna klasa za elemente u ComboBox-u koji nose ID i Text
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public ComboBoxItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
