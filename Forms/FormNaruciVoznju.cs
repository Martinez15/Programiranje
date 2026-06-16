using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormNaruciVoznju : Form
    {
        private Label lblNaslov;
        private Label lblPolaziste;
        private TextBox txtPolaziste;
        private Label lblOdrediste;
        private TextBox txtOdrediste;
        private Label lblKilometraza;
        private TextBox txtKilometraza;
        private Label lblProcenaCene;
        private Button btnNaruci;
        private Button btnZatvori;

        // Panel za praćenje aktivne vožnje nakon naručivanja
        private Panel pnlPracenje;
        private Label lblPracenjeNaslov;
        private Label lblStatusRada;
        private Label lblVozacInfo;
        private Label lblVoziloInfo;
        private Button btnRefresh;
        private Button btnOtkaziPracenu;

        private Voznja aktivnaVoznja;

        public FormNaruciVoznju()
        {
            InitializeComponent();
            ProveriAktivneVoznjeKorisnika();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Naruci Voznju";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblNaslov = new Label
            {
                Text = "Naruci novu voznju",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                Size = new Size(440, 35)
            };
            this.Controls.Add(lblNaslov);

            lblPolaziste = new Label
            {
                Text = "Adresa polazista:",
                ForeColor = Color.White,
                Location = new Point(30, 75),
                Size = new Size(440, 20)
            };
            this.Controls.Add(lblPolaziste);

            txtPolaziste = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(30, 100),
                Size = new Size(420, 28)
            };
            this.Controls.Add(txtPolaziste);

            lblOdrediste = new Label
            {
                Text = "Adresa odredista:",
                ForeColor = Color.White,
                Location = new Point(30, 145),
                Size = new Size(440, 20)
            };
            this.Controls.Add(lblOdrediste);

            txtOdrediste = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(30, 170),
                Size = new Size(420, 28)
            };
            this.Controls.Add(txtOdrediste);

            lblKilometraza = new Label
            {
                Text = "Procenjena kilometraza (km):",
                ForeColor = Color.White,
                Location = new Point(30, 215),
                Size = new Size(440, 20)
            };
            this.Controls.Add(lblKilometraza);

            txtKilometraza = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "5.0",
                Location = new Point(30, 240),
                Size = new Size(150, 28)
            };
            txtKilometraza.TextChanged += TxtKilometraza_TextChanged;
            this.Controls.Add(txtKilometraza);

            lblProcenaCene = new Label
            {
                Text = "Procenjena cena: 0 RSD",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129), // Zelena
                Location = new Point(30, 290),
                Size = new Size(440, 30)
            };
            this.Controls.Add(lblProcenaCene);

            btnNaruci = new Button
            {
                Text = "POTVRDI NARUCIVANJE",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(30, 340),
                Size = new Size(420, 45),
                Cursor = Cursors.Hand
            };
            btnNaruci.FlatAppearance.BorderSize = 0;
            btnNaruci.Click += BtnNaruci_Click;
            this.Controls.Add(btnNaruci);

            btnZatvori = new Button
            {
                Text = "Zatvori",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(30, 400),
                Size = new Size(420, 35),
                Cursor = Cursors.Hand
            };
            btnZatvori.FlatAppearance.BorderSize = 0;
            btnZatvori.Click += (s, e) => this.Close();
            this.Controls.Add(btnZatvori);

            // Inicijalizacija Panela za pracenje (skriven na pocetku)
            pnlPracenje = new Panel
            {
                Size = new Size(460, 410),
                Location = new Point(20, 60),
                BackColor = Color.FromArgb(55, 65, 81),
                Visible = false
            };

            lblPracenjeNaslov = new Label
            {
                Text = "Pracenje narucene voznje",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                Size = new Size(430, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlPracenje.Controls.Add(lblPracenjeNaslov);

            lblStatusRada = new Label
            {
                Text = "Status: Ucitavanje...",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(15, 60),
                Size = new Size(430, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlPracenje.Controls.Add(lblStatusRada);

            lblVozacInfo = new Label
            {
                Text = "Vozac: ...",
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = Color.White,
                Location = new Point(20, 110),
                Size = new Size(420, 40)
            };
            pnlPracenje.Controls.Add(lblVozacInfo);

            lblVoziloInfo = new Label
            {
                Text = "Vozilo: ...",
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = Color.White,
                Location = new Point(20, 160),
                Size = new Size(420, 60)
            };
            pnlPracenje.Controls.Add(lblVoziloInfo);

            btnRefresh = new Button
            {
                Text = "🔄 Osvezi status",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 240),
                Size = new Size(420, 40),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;
            pnlPracenje.Controls.Add(btnRefresh);

            btnOtkaziPracenu = new Button
            {
                Text = "❌ Otkazi voznju",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 295),
                Size = new Size(420, 40),
                Cursor = Cursors.Hand
            };
            btnOtkaziPracenu.FlatAppearance.BorderSize = 0;
            btnOtkaziPracenu.Click += BtnOtkaziPracenu_Click;
            pnlPracenje.Controls.Add(btnOtkaziPracenu);

            Button btnNazad = new Button
            {
                Text = "Zatvori pracenje",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 350),
                Size = new Size(420, 35),
                Cursor = Cursors.Hand
            };
            btnNazad.FlatAppearance.BorderSize = 0;
            btnNazad.Click += (s, e) => this.Close();
            pnlPracenje.Controls.Add(btnNazad);

            this.Controls.Add(pnlPracenje);

            ObnoviProcenuCene();
        }

        private void ProveriAktivneVoznjeKorisnika()
        {
            if (AppState.TrenutniKorisnik is Putnik putnik)
            {
                // Ako putnik već ima vožnju koja nije završena niti otkazana, odmah mu otvaramo ekran za praćenje!
                var voznje = AppState.VoznjaService.GetVoznjeZaKorisnika(putnik);
                var aktivna = voznje.FirstOrDefault(v => v.Status != StatusVoznje.Zavrsena && v.Status != StatusVoznje.Otkazana);
                if (aktivna != null)
                {
                    PrikaziEkranZaPracenje(aktivna);
                }
            }
        }

        private void TxtKilometraza_TextChanged(object sender, EventArgs e)
        {
            ObnoviProcenuCene();
        }

        private void ObnoviProcenuCene()
        {
            try
            {
                double km = double.Parse(txtKilometraza.Text, System.Globalization.CultureInfo.InvariantCulture);
                var cenovnik = AppState.CenovnikService.GetCenovnik() ?? new Cenovnik { StartnaCena = 200, CenaPoKm = 65, NocnaDoplataCena = 50 };
                bool nocna = DateTime.Now.Hour >= 22 || DateTime.Now.Hour < 6;
                decimal procena = cenovnik.ObracunajCenu(km, nocna);
                lblProcenaCene.Text = $"Procenjena cena: {procena:0} RSD" + (nocna ? " (Nocna tarifa)" : "");
            }
            catch
            {
                lblProcenaCene.Text = "Procenjena cena: - RSD";
            }
        }

        private void BtnNaruci_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPolaziste.Text) || string.IsNullOrWhiteSpace(txtOdrediste.Text))
            {
                MessageBox.Show("Molimo unesite polaziste i odrediste.", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                double km = double.Parse(txtKilometraza.Text, System.Globalization.CultureInfo.InvariantCulture);
                if (km <= 0) throw new Exception("Kilometraza mora biti veca od 0.");

                string putnikId = "";
                if (AppState.TrenutniKorisnik is Putnik p) putnikId = p.PutnikID;
                else if (AppState.TrenutniKorisnik is Admin) putnikId = "ADMIN-NARUDZBA"; // Ako admin naručuje

                // Naručivanje poziva servis
                var voznja = AppState.VoznjaService.NaruciVoznju(putnikId, txtPolaziste.Text, txtOdrediste.Text, km);
                MessageBox.Show($"Voznja je uspesno narucena!\nDodeljen je slobodan vozac.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);

                PrikaziEkranZaPracenje(voznja);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska pri narucivanju: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrikaziEkranZaPracenje(Voznja voznja)
        {
            this.aktivnaVoznja = voznja;

            // Sakrij sve kontrole za unos nove voznje da ne bi doslo do UI preklapanja
            lblNaslov.Visible = false;
            lblPolaziste.Visible = false;
            txtPolaziste.Visible = false;
            lblOdrediste.Visible = false;
            txtOdrediste.Visible = false;
            lblKilometraza.Visible = false;
            txtKilometraza.Visible = false;
            lblProcenaCene.Visible = false;
            btnNaruci.Visible = false;
            btnZatvori.Visible = false;

            pnlPracenje.Visible = true;
            OsveziPracenjePodatke();
        }

        private void OsveziPracenjePodatke()
        {
            if (aktivnaVoznja == null) return;

            // Učitavanje najnovijih podataka iz baze
            var sve = AppState.VoznjaService.GetSveVoznje();
            var azurirana = sve.FirstOrDefault(v => v.VoznjaID == aktivnaVoznja.VoznjaID);
            if (azurirana == null) return;

            aktivnaVoznja = azurirana;

            lblStatusRada.Text = $"Status: {aktivnaVoznja.Status}";

            // Boje za status
            if (aktivnaVoznja.Status == StatusVoznje.Narucena)
            {
                lblStatusRada.ForeColor = Color.FromArgb(139, 92, 246); // Ljubicasta
                btnOtkaziPracenu.Enabled = true;
            }
            else if (aktivnaVoznja.Status == StatusVoznje.Prihvacena)
            {
                lblStatusRada.ForeColor = Color.FromArgb(59, 130, 246); // Plava
                btnOtkaziPracenu.Enabled = true;
            }
            else if (aktivnaVoznja.Status == StatusVoznje.UToku)
            {
                lblStatusRada.ForeColor = Color.FromArgb(245, 158, 11); // Zuta
                btnOtkaziPracenu.Enabled = false; // Vozač je krenuo, nema otkazivanja!
            }
            else if (aktivnaVoznja.Status == StatusVoznje.Zavrsena)
            {
                lblStatusRada.ForeColor = Color.FromArgb(16, 185, 129); // Zelena
                btnOtkaziPracenu.Enabled = false;
                MessageBox.Show($"Vozac je stigao na odrediste!\nUkupna cena voznje je: {aktivnaVoznja.Cena:0} RSD", "Voznja Zavrsena", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.BeginInvoke(new Action(this.Close));
                return;
            }
            else if (aktivnaVoznja.Status == StatusVoznje.Otkazana)
            {
                lblStatusRada.ForeColor = Color.FromArgb(239, 68, 68); // Crvena
                btnOtkaziPracenu.Enabled = false;
                MessageBox.Show("Voznja je otkazana.", "Otkazano", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.BeginInvoke(new Action(this.Close));
                return;
            }

            // Učitavanje vozača i vozila
            var vozac = AppState.VozacService.GetVozacPoID(aktivnaVoznja.VozacID);
            if (vozac != null)
            {
                lblVozacInfo.Text = $"Vozac: {vozac.ImePrezime}\nBroj licence: {vozac.BrojLicence}\nProsecna ocena: {vozac.ProsecnaOcena:0.0} ★";

                var vozila = AppState.VoziloRepo.GetAll();
                var vozilo = vozila.FirstOrDefault(v => v.VoziloID == vozac.VoziloID);
                if (vozilo != null)
                {
                    lblVoziloInfo.Text = $"Vozilo: {vozilo.Marka} {vozilo.Model} ({vozilo.Godiste}. god)\nBoja: {vozilo.Boja}\nRegistracija: {vozilo.Registracija}";
                }
                else
                {
                    lblVoziloInfo.Text = "Vozilo: Podaci nisu nadjeni.";
                }
            }
            else
            {
                lblVozacInfo.Text = "Vozac: Trazenje slobodnog vozaca...";
                lblVoziloInfo.Text = "Vozilo: -";
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            OsveziPracenjePodatke();
        }

        private void BtnOtkaziPracenu_Click(object sender, EventArgs e)
        {
            if (aktivnaVoznja == null) return;
            var rez = MessageBox.Show("Da li zelite da otkazete ovu voznju?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                try
                {
                    AppState.VoznjaService.OtkaziVoznju(aktivnaVoznja.VoznjaID);
                    MessageBox.Show("Voznja je otkazana.", "Otkazano", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.BeginInvoke(new Action(this.Close));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
