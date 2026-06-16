using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormMain : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblUserTitle;
        private Label lblUserRole;
        private Label lblLogo;
        private Button btnSveVoznje;
        private Button btnMojeVoznje;
        private Button btnNaruci;
        private Button btnVozaci;
        private Button btnVozila;

        private Button btnCenovnik;
        private Button btnIzvestaji;
        private Button btnKorisnici;
        private Button btnOdjava;

        // Dashboard Stats
        private Label lblDashTitle;
        private Panel pnlStatCard1;
        private Panel pnlStatCard2;
        private Panel pnlStatCard3;
        private Panel pnlStatCard4;

        private Label lblStatValue1;
        private Label lblStatValue2;
        private Label lblStatValue3;
        private Label lblStatValue4;

        private Label lblStatTitle1;
        private Label lblStatTitle2;
        private Label lblStatTitle3;
        private Label lblStatTitle4;

        private Button btnToggleDostupnost; // Za vozača

        public FormMain()
        {
            InitializeComponent();
            UcitajDashboardPodatke();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Glavni Meni";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 10F);

            // 1. Sidebar Panel
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(17, 24, 39) // Skoro crna pozadina #111827
            };

            lblLogo = new Label
            {
                Text = "🚕 TAXI PREVOZ",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(10, 20),
                Size = new Size(230, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlSidebar.Controls.Add(lblLogo);

            // Kreiranje dugmadi za navigaciju
            btnSveVoznje = KreirajSidebarDugme("📋 Sve voznje", 80);
            btnMojeVoznje = KreirajSidebarDugme("🚗 Moje voznje", 130);
            btnNaruci = KreirajSidebarDugme("📍 Naruci voznju", 180);
            btnVozaci = KreirajSidebarDugme("👥 Vozaci", 230);
            btnVozila = KreirajSidebarDugme("🚘 Vozni park", 280);

            btnCenovnik = KreirajSidebarDugme("💰 Cenovnik", 380);
            btnIzvestaji = KreirajSidebarDugme("📊 Fin. izvestaji", 430);
            btnKorisnici = KreirajSidebarDugme("👤 Korisnicki nalozi", 480);

            // User Info Card na dnu
            Panel pnlUserCard = new Panel
            {
                Location = new Point(10, 530),
                Size = new Size(230, 70),
                BackColor = Color.FromArgb(55, 65, 81) // #374151
            };

            string prikaziIme = AppState.TrenutniKorisnik.KorisnickoIme;
            if (AppState.TrenutniKorisnik is Vozac v) prikaziIme = v.ImePrezime;
            else if (AppState.TrenutniKorisnik is Putnik p) prikaziIme = p.ImePrezime;

            lblUserTitle = new Label
            {
                Text = prikaziIme,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(5, 5),
                Size = new Size(220, 20)
            };

            lblUserRole = new Label
            {
                Text = AppState.TrenutniKorisnik.Uloga,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(156, 163, 175),
                Location = new Point(5, 25),
                Size = new Size(220, 15)
            };

            btnOdjava = new Button
            {
                Text = "Odjava",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(5, 45),
                Size = new Size(220, 22),
                Cursor = Cursors.Hand
            };
            btnOdjava.FlatAppearance.BorderSize = 0;
            btnOdjava.Click += BtnOdjava_Click;

            pnlUserCard.Controls.Add(lblUserTitle);
            pnlUserCard.Controls.Add(lblUserRole);
            pnlUserCard.Controls.Add(btnOdjava);
            pnlSidebar.Controls.Add(pnlUserCard);

            // Sidebar se dodaje POSLE content panela - WinForms Dock logika zahteva ovaj redosled

            // 2. Content Panel (Dashboard)
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(31, 41, 55),
                Padding = new Padding(30)
            };

            lblDashTitle = new Label
            {
                Text = $"Dobrodosli nazad, {prikaziIme}!",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 20),
                Size = new Size(650, 45)
            };
            pnlContent.Controls.Add(lblDashTitle);

            // Kreiranje statistickih kartica - bolje rasporedene
            pnlStatCard1 = KreirajStatKarticu(50,  80, out lblStatTitle1, out lblStatValue1);
            pnlStatCard2 = KreirajStatKarticu(430, 80, out lblStatTitle2, out lblStatValue2);
            pnlStatCard3 = KreirajStatKarticu(50, 250, out lblStatTitle3, out lblStatValue3);
            pnlStatCard4 = KreirajStatKarticu(430, 250, out lblStatTitle4, out lblStatValue4);

            pnlContent.Controls.Add(pnlStatCard1);
            pnlContent.Controls.Add(pnlStatCard2);
            pnlContent.Controls.Add(pnlStatCard3);
            pnlContent.Controls.Add(pnlStatCard4);

            // Dugme za dostupnost vozaca
            btnToggleDostupnost = new Button
            {
                Text = "Promeni dostupnost",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(50, 430),
                Size = new Size(320, 48),
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnToggleDostupnost.FlatAppearance.BorderSize = 0;
            btnToggleDostupnost.Click += BtnToggleDostupnost_Click;
            pnlContent.Controls.Add(btnToggleDostupnost);

            this.Controls.Add(pnlContent);  // index 0 - dokuje se POSLEDNJI (Fill uzima ostatak)
            this.Controls.Add(pnlSidebar);  // index 1 - dokuje se PRVI (Left uzima 250px)

            // 3. Konfigurisanje vidljivosti po ulogama
            PodesiVidljivostPoUlogama();
        }

        private Button KreirajSidebarDugme(string text, int top)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 163, 175),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, top),
                Size = new Size(230, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(31, 41, 55);
            btn.Click += NavigationButton_Click;
            pnlSidebar.Controls.Add(btn);
            return btn;
        }

        private Panel KreirajStatKarticu(int left, int top, out Label lblTitle, out Label lblValue)
        {
            var pnl = new Panel
            {
                Size = new Size(300, 130),
                Location = new Point(left, top),
                BackColor = Color.FromArgb(55, 65, 81)
            };

            lblTitle = new Label
            {
                Text = "Statistika",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 163, 175),
                Location = new Point(15, 15),
                Size = new Size(270, 25)
            };

            lblValue = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 45),
                Size = new Size(270, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnl.Controls.Add(lblTitle);
            pnl.Controls.Add(lblValue);
            return pnl;
        }

        private void PodesiVidljivostPoUlogama()
        {
            Korisnik k = AppState.TrenutniKorisnik;

            // Sakrij sve po default-u
            btnSveVoznje.Visible = false;
            btnMojeVoznje.Visible = false;
            btnNaruci.Visible = false;
            btnVozaci.Visible = false;
            btnVozila.Visible = false;
            btnCenovnik.Visible = false;
            btnIzvestaji.Visible = false;
            btnKorisnici.Visible = false;

            if (k is Admin)
            {
                btnSveVoznje.Visible = true;
                btnNaruci.Visible = true;
                btnVozaci.Visible = true;
                btnVozila.Visible = true;
                btnCenovnik.Visible = true;
                btnIzvestaji.Visible = true;
                btnKorisnici.Visible = true;
            }
            else if (k is Vozac)
            {
                btnMojeVoznje.Visible = true;
                btnToggleDostupnost.Visible = true;
            }
            else if (k is Putnik)
            {
                btnMojeVoznje.Visible = true;
                btnNaruci.Visible = true;
            }
        }

        private void UcitajDashboardPodatke()
        {
            Korisnik k = AppState.TrenutniKorisnik;

            if (k is Admin)
            {
                var stats = AppState.IzvestajService.GetOpstaStatistika();
                lblStatTitle1.Text = "Ukupno voznji";
                lblStatValue1.Text = stats.UkupnoVoznji.ToString();

                lblStatTitle2.Text = "Prosecna ocena vozaca";
                lblStatValue2.Text = stats.ProsecnaOcenaVozaca > 0 
                    ? $"{stats.ProsecnaOcenaVozaca:0.0} ★" 
                    : "Nema ocena";

                lblStatTitle3.Text = "Registrovani vozaci";
                lblStatValue3.Text = stats.BrojRegistrovanihVozaca.ToString();

                lblStatTitle4.Text = "Registrovani putnici";
                lblStatValue4.Text = stats.BrojRegistrovanihPutnika.ToString();
            }
            else if (k is Vozac vozac)
            {
                var voznje = AppState.VoznjaService.GetVoznjeZaKorisnika(vozac);
                var zavrsene = voznje.Where(v => v.Status == StatusVoznje.Zavrsena).ToList();

                lblStatTitle1.Text = "Ukupno voznji";
                lblStatValue1.Text = voznje.Count.ToString();

                lblStatTitle2.Text = "Moja prosecna ocena";
                // Ažuriramo iz baze radi tačnosti
                var vBaza = AppState.VozacService.GetVozacPoID(vozac.VozacID);
                lblStatValue2.Text = vBaza.BrojOcena > 0 
                    ? $"{vBaza.ProsecnaOcena:0.0} ★" 
                    : "Nema ocena";

                lblStatTitle3.Text = "Ukupna zarada";
                lblStatValue3.Text = $"{zavrsene.Sum(v => v.Cena):0} RSD";

                lblStatTitle4.Text = "Moj status";
                lblStatValue4.Text = vBaza.Dostupan ? "SLOBODAN" : "ZAUZET";
                lblStatValue4.ForeColor = vBaza.Dostupan ? Color.FromArgb(16, 185, 129) : Color.FromArgb(245, 158, 11);

                btnToggleDostupnost.BackColor = vBaza.Dostupan ? Color.FromArgb(239, 68, 68) : Color.FromArgb(16, 185, 129);
                btnToggleDostupnost.Text = vBaza.Dostupan ? "Postavi: nedostupan" : "Postavi: dostupan";
            }
            else if (k is Putnik putnik)
            {
                var voznje = AppState.VoznjaService.GetVoznjeZaKorisnika(putnik);
                var zavrsene = voznje.Where(v => v.Status == StatusVoznje.Zavrsena).ToList();

                lblStatTitle1.Text = "Moje narucene voznje";
                lblStatValue1.Text = voznje.Count.ToString();

                lblStatTitle2.Text = "Ukupno potroseno";
                lblStatValue2.Text = $"{zavrsene.Sum(v => v.Cena):0} RSD";

                // Sakrivamo donje stat kartice za putnika da izgleda čistije
                pnlStatCard3.Visible = false;
                pnlStatCard4.Visible = false;
            }
        }

        private void BtnToggleDostupnost_Click(object sender, EventArgs e)
        {
            if (AppState.TrenutniKorisnik is Vozac vozac)
            {
                try
                {
                    var vBaza = AppState.VozacService.GetVozacPoID(vozac.VozacID);
                    // Provera da li ima aktivnu vožnju
                    var voznje = AppState.VoznjaService.GetVoznjeZaKorisnika(vozac);
                    if (vBaza.Dostupan == false && voznje.Any(v => v.Status == StatusVoznje.UToku || v.Status == StatusVoznje.Prihvacena || v.Status == StatusVoznje.Narucena))
                    {
                        // Ako je već zauzet zbog vožnje u toku, ne može se postaviti kao dostupan dok ne završi!
                        MessageBox.Show("Ne mozete promeniti status jer imate aktivnu voznju.", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    AppState.VozacService.IzmeniVozaca(vozac.VozacID, vBaza.ImePrezime, vBaza.BrojLicence, vBaza.VoziloID, !vBaza.Dostupan);
                    UcitajDashboardPodatke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void NavigationButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            Form modalForm = null;

            if (clickedButton == btnSveVoznje)
            {
                modalForm = new FormVoznje();
            }
            else if (clickedButton == btnMojeVoznje)
            {
                if (AppState.TrenutniKorisnik is Vozac)
                    modalForm = new FormMojeVoznje();
                else
                    modalForm = new FormVoznje();
            }
            else if (clickedButton == btnNaruci)
            {
                modalForm = new FormNaruciVoznju();
            }
            else if (clickedButton == btnVozaci)
            {
                modalForm = new FormVozaci();
            }
            else if (clickedButton == btnVozila)
            {
                modalForm = new FormVozila();
            }
            else if (clickedButton == btnCenovnik)
            {
                // Otvaranje cenovnika
                var cenovnik = AppState.CenovnikService.GetCenovnik();
                using (var prompt = new Form())
                {
                    prompt.Width = 350;
                    prompt.Height = 300;
                    prompt.Text = "Cenovnik";
                    prompt.StartPosition = FormStartPosition.CenterParent;
                    prompt.BackColor = Color.FromArgb(31, 41, 55);
                    prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                    prompt.MaximizeBox = false;

                    Label lblStart = new Label { Text = "Startna cena (RSD):", Left = 20, Top = 20, Width = 150, ForeColor = Color.White };
                    TextBox txtStart = new TextBox { Left = 180, Top = 18, Width = 120, Text = cenovnik.StartnaCena.ToString(), BackColor = Color.FromArgb(55, 65, 81), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

                    Label lblPoKm = new Label { Text = "Cena po kilometru:", Left = 20, Top = 60, Width = 150, ForeColor = Color.White };
                    TextBox txtPoKm = new TextBox { Left = 180, Top = 58, Width = 120, Text = cenovnik.CenaPoKm.ToString(), BackColor = Color.FromArgb(55, 65, 81), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

                    Label lblNocna = new Label { Text = "Nocna doplata:", Left = 20, Top = 100, Width = 150, ForeColor = Color.White };
                    TextBox txtNocna = new TextBox { Left = 180, Top = 98, Width = 120, Text = cenovnik.NocnaDoplataCena.ToString(), BackColor = Color.FromArgb(55, 65, 81), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

                    Button btnSave = new Button { Text = "Sacuvaj", Left = 100, Top = 160, Width = 150, Height = 35, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                    btnSave.FlatAppearance.BorderSize = 0;
                    btnSave.Click += (senderSave, eSave) =>
                    {
                        try
                        {
                            decimal s = decimal.Parse(txtStart.Text);
                            decimal p = decimal.Parse(txtPoKm.Text);
                            decimal n = decimal.Parse(txtNocna.Text);
                            AppState.CenovnikService.AzurirajCenovnik(s, p, n);
                            prompt.DialogResult = DialogResult.OK;
                            prompt.Close();
                        }
                        catch (Exception ex) { MessageBox.Show("Neispravan unos brojeva. " + ex.Message); }
                    };

                    prompt.Controls.Add(lblStart); prompt.Controls.Add(txtStart);
                    prompt.Controls.Add(lblPoKm); prompt.Controls.Add(txtPoKm);
                    prompt.Controls.Add(lblNocna); prompt.Controls.Add(txtNocna);
                    prompt.Controls.Add(btnSave);

                    prompt.ShowDialog();
                }
            }
            else if (clickedButton == btnIzvestaji)
            {
                modalForm = new FormIzvestaji();
            }
            else if (clickedButton == btnKorisnici)
            {
                modalForm = new FormKorisnici();
            }

            if (modalForm != null)
            {
                modalForm.ShowDialog();
                UcitajDashboardPodatke(); // Osveži dashboard po zatvaranju subforme
            }
        }

        private void BtnOdjava_Click(object sender, EventArgs e)
        {
            AppState.TrenutniKorisnik = null;
            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
}
