using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormMojeVoznje : Form
    {
        private Label lblNaslov;
        private Label lblPendingSection;
        private DataGridView dgvPending;
        private Button btnPrihvati;
        private Button btnOdbij;

        // Aktivna vožnja sekcija
        private Label lblAktivnaSection;
        private Panel pnlAktivna;
        private Label lblAktivnaDetails;
        private Button btnKrenuo;
        private TextBox txtStvarniKm;
        private Label lblStvarniKm;
        private Button btnStigao;

        // Istorija i zarada
        private Label lblIstorijaSection;
        private DataGridView dgvIstorija;
        private Label lblUkupnaZarada;

        private Vozac trenutniVozac;

        public FormMojeVoznje()
        {
            if (AppState.TrenutniKorisnik is Vozac v)
            {
                trenutniVozac = v;
            }
            InitializeComponent();
            UcitajPodatke();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Moje Voznje (Vozac)";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 9.5F);

            lblNaslov = new Label
            {
                Text = "Upravljanje voznjama",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 15),
                Size = new Size(420, 30)
            };
            this.Controls.Add(lblNaslov);

            // 1. Sekcija: Cekajuce narudzbenice
            lblPendingSection = new Label
            {
                Text = "Dodeljene narudzbine na cekanju:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(40, 60),
                Size = new Size(430, 20)
            };
            this.Controls.Add(lblPendingSection);

            dgvPending = new DataGridView
            {
                Location = new Point(40, 85),
                Size = new Size(440, 130),
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
            dgvPending.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvPending.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPending.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPending.EnableHeadersVisualStyles = false;
            dgvPending.SelectionChanged += DgvPending_SelectionChanged;
            this.Controls.Add(dgvPending);

            btnPrihvati = new Button
            {
                Text = "Prihvati",
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(40, 225),
                Size = new Size(210, 32),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnPrihvati.FlatAppearance.BorderSize = 0;
            btnPrihvati.Click += BtnPrihvati_Click;
            this.Controls.Add(btnPrihvati);

            btnOdbij = new Button
            {
                Text = "Odbij (Prosledi drugom)",
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(270, 225),
                Size = new Size(210, 32),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnOdbij.FlatAppearance.BorderSize = 0;
            btnOdbij.Click += BtnOdbij_Click;
            this.Controls.Add(btnOdbij);

            // 2. Sekcija: Aktivna voznja
            lblAktivnaSection = new Label
            {
                Text = "Aktivna voznja:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(510, 60),
                Size = new Size(400, 20)
            };
            this.Controls.Add(lblAktivnaSection);

            pnlAktivna = new Panel
            {
                Location = new Point(510, 85),
                Size = new Size(400, 172),
                BackColor = Color.FromArgb(55, 65, 81)
            };

            lblAktivnaDetails = new Label
            {
                Text = "Trenutno nemate aktivnu voznju.",
                ForeColor = Color.White,
                Location = new Point(15, 15),
                Size = new Size(400, 50),
                Font = new Font("Segoe UI", 10F, FontStyle.Italic)
            };
            pnlAktivna.Controls.Add(lblAktivnaDetails);

            btnKrenuo = new Button
            {
                Text = "KRENUO (Zapocni voznju)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, 75),
                Size = new Size(400, 35),
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnKrenuo.FlatAppearance.BorderSize = 0;
            btnKrenuo.Click += BtnKrenuo_Click;
            pnlAktivna.Controls.Add(btnKrenuo);

            lblStvarniKm = new Label
            {
                Text = "Stvarni km:",
                ForeColor = Color.White,
                Location = new Point(15, 78),
                Size = new Size(80, 20),
                Visible = false
            };
            pnlAktivna.Controls.Add(lblStvarniKm);

            txtStvarniKm = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(100, 75),
                Size = new Size(80, 25),
                Visible = false
            };
            pnlAktivna.Controls.Add(txtStvarniKm);

            btnStigao = new Button
            {
                Text = "STIGAO (Zavrsi voznju)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(190, 74),
                Size = new Size(225, 30),
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnStigao.FlatAppearance.BorderSize = 0;
            btnStigao.Click += BtnStigao_Click;
            pnlAktivna.Controls.Add(btnStigao);

            this.Controls.Add(pnlAktivna);

            // 3. Sekcija: Istorija i zarada
            lblIstorijaSection = new Label
            {
                Text = "Istorija mojih zavrsenih voznji:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 280),
                Size = new Size(450, 20)
            };
            this.Controls.Add(lblIstorijaSection);

            dgvIstorija = new DataGridView
            {
                Location = new Point(40, 310),
                Size = new Size(870, 175),
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
            dgvIstorija.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvIstorija.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvIstorija.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvIstorija.EnableHeadersVisualStyles = false;
            this.Controls.Add(dgvIstorija);

            lblUkupnaZarada = new Label
            {
                Text = "Ukupna zarada: 0 RSD",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                Location = new Point(40, 500),
                Size = new Size(450, 30)
            };
            this.Controls.Add(lblUkupnaZarada);

            Button btnZatvori = new Button
            {
                Text = "Zatvori",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(790, 500),
                Size = new Size(120, 35),
                Cursor = Cursors.Hand
            };
            btnZatvori.FlatAppearance.BorderSize = 0;
            btnZatvori.Click += (s, e) => this.Close();
            this.Controls.Add(btnZatvori);
        }

        private void UcitajPodatke()
        {
            if (trenutniVozac == null) return;

            // Uvek ucitavamo sveze iz fajla
            var sveVoznje = AppState.VoznjaService.GetVoznjeZaKorisnika(trenutniVozac);
            var korisnici = AppState.KorisnikRepo.GetAll();

            // ── 1. Cekajuce narudzbine (Status = Narucena) ──────────────────────
            var pending = sveVoznje.Where(v => v.Status == StatusVoznje.Narucena).ToList();

            // Uvek cistimo, da ne bi ostali stari podaci
            dgvPending.SelectionChanged -= DgvPending_SelectionChanged;
            dgvPending.DataSource = null;
            dgvPending.Columns.Clear();

            if (pending.Count > 0)
            {
                var displayPending = pending.Select(v =>
                {
                    var putnik = korisnici.OfType<Putnik>().FirstOrDefault(p => p.PutnikID == v.PutnikID);
                    return new
                    {
                        ID      = v.VoznjaID,
                        Putnik  = putnik?.ImePrezime ?? v.PutnikID,
                        Od      = v.Polaziste,
                        Do      = v.Odrediste,
                        KmProc  = v.Kilometraza.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " km"
                    };
                }).ToList();
                dgvPending.DataSource = displayPending;
                lblPendingSection.Text = $"Dodeljene narudzbine na cekanju: ({pending.Count})";
            }
            else
            {
                lblPendingSection.Text = "Dodeljene narudzbine na cekanju: (nema)";
            }

            dgvPending.SelectionChanged += DgvPending_SelectionChanged;
            btnPrihvati.Enabled = false;
            btnOdbij.Enabled = false;

            // ── 2. Aktivna voznja (Status = Prihvacena ili UToku) ───────────────
            var aktivna = sveVoznje.FirstOrDefault(v => v.Status == StatusVoznje.Prihvacena || v.Status == StatusVoznje.UToku);
            if (aktivna != null)
            {
                var putnik = korisnici.OfType<Putnik>().FirstOrDefault(p => p.PutnikID == aktivna.PutnikID);
                string statusTekst = aktivna.Status == StatusVoznje.Prihvacena ? "✓ Prihvacena" : "⟳ U toku";
                lblAktivnaDetails.Text =
                    $"[{statusTekst}] ID: {aktivna.VoznjaID}\n" +
                    $"Putnik: {putnik?.ImePrezime ?? aktivna.PutnikID}\n" +
                    $"Od: {aktivna.Polaziste}\n" +
                    $"Do: {aktivna.Odrediste}\n" +
                    $"Procenjeno km: {aktivna.Kilometraza:0.0} km";
                lblAktivnaDetails.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

                if (aktivna.Status == StatusVoznje.Prihvacena)
                {
                    btnKrenuo.Visible = true;
                    lblStvarniKm.Visible = false;
                    txtStvarniKm.Visible = false;
                    btnStigao.Visible = false;
                }
                else // UToku
                {
                    btnKrenuo.Visible = false;
                    lblStvarniKm.Visible = true;
                    txtStvarniKm.Visible = true;
                    txtStvarniKm.Text = aktivna.Kilometraza.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
                    btnStigao.Visible = true;
                }
            }
            else
            {
                lblAktivnaDetails.Text = "Trenutno nemate aktivnu voznju.";
                lblAktivnaDetails.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
                btnKrenuo.Visible = false;
                lblStvarniKm.Visible = false;
                txtStvarniKm.Visible = false;
                btnStigao.Visible = false;
            }

            // ── 3. Istorija i zarada (Status = Zavrsena) ───────────────────────
            var zavrsene = sveVoznje.Where(v => v.Status == StatusVoznje.Zavrsena).ToList();

            dgvIstorija.DataSource = null;
            dgvIstorija.Columns.Clear();

            if (zavrsene.Count > 0)
            {
                var displayIstorija = zavrsene.OrderByDescending(v => v.VremeDolaska).Select(v =>
                {
                    var putnik = korisnici.OfType<Putnik>().FirstOrDefault(p => p.PutnikID == v.PutnikID);
                    return new
                    {
                        ID       = v.VoznjaID,
                        Putnik   = putnik?.ImePrezime ?? v.PutnikID,
                        Od       = v.Polaziste,
                        Do       = v.Odrediste,
                        Datum    = v.VremeDolaska?.ToString("dd.MM.yyyy HH:mm") ?? "",
                        KmStvarni = v.Kilometraza.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " km",
                        Zarada   = v.Cena.ToString("0") + " RSD",
                        Ocena    = v.Ocena.HasValue ? v.Ocena.Value.ToString() + " ★" : "-",
                        Komentar = v.Komentar ?? ""
                    };
                }).ToList();
                dgvIstorija.DataSource = displayIstorija;
            }

            decimal ukupnaZarada = zavrsene.Sum(v => v.Cena);
            lblUkupnaZarada.Text = $"Ukupna zarada: {ukupnaZarada:0} RSD";
        }

        private void DgvPending_SelectionChanged(object sender, EventArgs e)
        {
            if (trenutniVozac == null) return;
            bool hasSelection = dgvPending.SelectedRows.Count > 0;

            // Vozac ne moze prihvatiti voznju ako vec ima aktivnu (pravilo 5)
            bool imaAktivnu = false;
            try
            {
                var sve = AppState.VoznjaService.GetVoznjeZaKorisnika(trenutniVozac);
                imaAktivnu = sve.Any(v => v.Status == StatusVoznje.Prihvacena || v.Status == StatusVoznje.UToku);
            }
            catch { }

            btnPrihvati.Enabled = hasSelection && !imaAktivnu;
            btnOdbij.Enabled = hasSelection;
        }

        private string GetSelectedPendingVoznjaId()
        {
            if (dgvPending.SelectedRows.Count == 0) return null;
            var cell = dgvPending.SelectedRows[0].Cells["ID"];
            return cell?.Value?.ToString();
        }

        private void BtnPrihvati_Click(object sender, EventArgs e)
        {
            string voznjaId = GetSelectedPendingVoznjaId();
            if (string.IsNullOrEmpty(voznjaId)) return;

            try
            {
                AppState.VoznjaService.PrihvatiVoznju(voznjaId);
                MessageBox.Show(
                    "Voznja je prihvacena!\nMolimo krenite na adresu polazista.",
                    "Prihvaceno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UcitajPodatke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska pri prihvatanju: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOdbij_Click(object sender, EventArgs e)
        {
            string voznjaId = GetSelectedPendingVoznjaId();
            if (string.IsNullOrEmpty(voznjaId)) return;

            var rez = MessageBox.Show(
                "Da li zelite da odbijete ovu voznju?\nBice automatski prosledjena sledecem slobodnom vozacu.",
                "Potvrda odbijanja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rez == DialogResult.Yes)
            {
                try
                {
                    AppState.VoznjaService.OdbijVoznju(voznjaId);
                    MessageBox.Show("Voznja je odbijena i prosledjena.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UcitajPodatke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greska pri odbijanju: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnKrenuo_Click(object sender, EventArgs e)
        {
            var sve = AppState.VoznjaService.GetVoznjeZaKorisnika(trenutniVozac);
            var aktivna = sve.FirstOrDefault(v => v.Status == StatusVoznje.Prihvacena);
            if (aktivna != null)
            {
                try
                {
                    AppState.VoznjaService.KreniVoznju(aktivna.VoznjaID);
                    UcitajPodatke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greska: " + ex.Message);
                }
            }
        }

        private void BtnStigao_Click(object sender, EventArgs e)
        {
            var sve = AppState.VoznjaService.GetVoznjeZaKorisnika(trenutniVozac);
            var aktivna = sve.FirstOrDefault(v => v.Status == StatusVoznje.UToku);
            if (aktivna != null)
            {
                try
                {
                    double stvarniKm = double.Parse(txtStvarniKm.Text, System.Globalization.CultureInfo.InvariantCulture);
                    if (stvarniKm <= 0) throw new Exception("Kilometraza mora biti veca od 0.");

                    AppState.VoznjaService.ZavrsiVoznju(aktivna.VoznjaID, stvarniKm);
                    
                    // Učitamo je ponovo iz baze da bismo videli konačnu obračunatu cenu
                    var azurirana = AppState.VoznjaService.GetSveVoznje().FirstOrDefault(v => v.VoznjaID == aktivna.VoznjaID);
                    decimal finalnaCena = azurirana?.Cena ?? 0;

                    MessageBox.Show($"Voznja je uspesno zavrsena!\nKonacna cena: {finalnaCena:0} RSD\nKlijent ce dobiti racun.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UcitajPodatke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greska: " + ex.Message);
                }
            }
        }
    }
}
