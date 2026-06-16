using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormVozaci : Form
    {
        private Label lblNaslov;
        private DataGridView dgvVozaci;

        private Label lblKorisnickoIme;
        private TextBox txtKorisnickoIme;
        private Label lblLozinka;
        private TextBox txtLozinka;
        private Label lblImePrezime;
        private TextBox txtImePrezime;
        private Label lblBrojLicence;
        private TextBox txtBrojLicence;
        private Label lblVozilo;
        private ComboBox cbVozila;
        private CheckBox chkDostupan;

        private Button btnDodaj;
        private Button btnIzmeni;
        private Button btnDeaktiviraj;
        private Button btnOcisti;

        public FormVozaci()
        {
            InitializeComponent();
            UcitajVozace();
            UcitajVozila();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Upravljanje Vozacima";
            this.Size = new Size(950, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 9.5F);

            lblNaslov = new Label
            {
                Text = "Upravljanje vozacima",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };
            this.Controls.Add(lblNaslov);

            // DataGridView (levo)
            dgvVozaci = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(540, 420),
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
            this.Controls.Add(dgvVozaci);

            // Panel za unos (desno)
            Panel pnlInputs = new Panel
            {
                Location = new Point(580, 60),
                Size = new Size(330, 420),
                BackColor = Color.FromArgb(55, 65, 81),
                Padding = new Padding(15)
            };

            lblKorisnickoIme = KreirajLabel("Korisnicko ime (za nalog):", 15, pnlInputs);
            txtKorisnickoIme = KreirajTextBox(35, pnlInputs);

            lblLozinka = KreirajLabel("Lozinka (samo za nove):", 80, pnlInputs);
            txtLozinka = KreirajTextBox(100, pnlInputs);
            txtLozinka.UseSystemPasswordChar = true;

            lblImePrezime = KreirajLabel("Ime i Prezime:", 145, pnlInputs);
            txtImePrezime = KreirajTextBox(165, pnlInputs);

            lblBrojLicence = KreirajLabel("Broj licence:", 210, pnlInputs);
            txtBrojLicence = KreirajTextBox(230, pnlInputs);

            lblVozilo = KreirajLabel("Vozilo iz voznog parka:", 275, pnlInputs);
            cbVozila = new ComboBox
            {
                Location = new Point(15, 295),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            pnlInputs.Controls.Add(cbVozila);

            chkDostupan = new CheckBox
            {
                Text = "Dostupan na duznosti",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 340),
                Size = new Size(300, 25),
                Checked = true
            };
            pnlInputs.Controls.Add(chkDostupan);

            this.Controls.Add(pnlInputs);

            // Akciona dugmad na dnu (desno)
            btnDodaj = new Button { Text = "DODAJ NOVOG", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(580, 490), Size = new Size(105, 32), Cursor = Cursors.Hand };
            btnDodaj.FlatAppearance.BorderSize = 0;
            btnDodaj.Click += BtnDodaj_Click;
            this.Controls.Add(btnDodaj);

            btnIzmeni = new Button { Text = "IZMENI", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(695, 490), Size = new Size(100, 32), Enabled = false, Cursor = Cursors.Hand };
            btnIzmeni.FlatAppearance.BorderSize = 0;
            btnIzmeni.Click += BtnIzmeni_Click;
            this.Controls.Add(btnIzmeni);

            btnDeaktiviraj = new Button { Text = "OBRISI", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(810, 490), Size = new Size(100, 32), Enabled = false, Cursor = Cursors.Hand };
            btnDeaktiviraj.FlatAppearance.BorderSize = 0;
            btnDeaktiviraj.Click += BtnDeaktiviraj_Click;
            this.Controls.Add(btnDeaktiviraj);

            // Dugme zatvori levo
            Button btnZatvori = new Button
            {
                Text = "Zatvori",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 490),
                Size = new Size(140, 32),
                Cursor = Cursors.Hand
            };
            btnZatvori.FlatAppearance.BorderSize = 0;
            btnZatvori.Click += (s, e) => this.Close();
            this.Controls.Add(btnZatvori);

            btnOcisti = new Button
            {
                Text = "Ocisti unos",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(460, 490),
                Size = new Size(100, 32),
                Cursor = Cursors.Hand
            };
            btnOcisti.FlatAppearance.BorderSize = 0;
            btnOcisti.Click += (s, e) => ResetujFormu();
            this.Controls.Add(btnOcisti);
        }

        private Label KreirajLabel(string text, int top, Panel parent)
        {
            var lbl = new Label
            {
                Text = text,
                ForeColor = Color.White,
                Location = new Point(15, top),
                Size = new Size(300, 20)
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private TextBox KreirajTextBox(int top, Panel parent)
        {
            var txt = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(15, top),
                Size = new Size(300, 25)
            };
            parent.Controls.Add(txt);
            return txt;
        }

        private void UcitajVozace()
        {
            var vozaci = AppState.VozacService.GetSviVozaci();
            dgvVozaci.DataSource = null;
            dgvVozaci.Columns.Clear();

            var displayList = vozaci.Select(v => new
            {
                ID = v.VozacID,
                Korisnik = v.KorisnickoIme,
                ImePrezime = v.ImePrezime,
                Licenca = v.BrojLicence,
                Vozilo = "Vozilo#" + v.VoziloID,
                Status = v.Dostupan ? "Dostupan" : "Zauzet",
                Ocena = v.ProsecnaOcena > 0 ? v.ProsecnaOcena.ToString("0.0") + " ★" : "-"
            }).ToList();

            dgvVozaci.DataSource = displayList;

            if (this.IsHandleCreated)
            {
                PodesiSirineKolona();
            }
        }

        private void PodesiSirineKolona()
        {
            if (dgvVozaci.Columns.Count > 0)
            {
                if (dgvVozaci.Columns["ID"] != null) dgvVozaci.Columns["ID"].Width = 80;
                if (dgvVozaci.Columns["Status"] != null) dgvVozaci.Columns["Status"].Width = 85;
                if (dgvVozaci.Columns["Ocena"] != null) dgvVozaci.Columns["Ocena"].Width = 65;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PodesiSirineKolona();
        }

        private void UcitajVozila()
        {
            cbVozila.Items.Clear();
            var vozila = AppState.VoziloRepo.GetAll().Where(v => v.Aktivno).ToList();
            foreach (var v in vozila)
            {
                cbVozila.Items.Add(new ComboBoxItem($"{v.Marka} {v.Model} ({v.Registracija})", v.VoziloID.ToString()));
            }

            if (cbVozila.Items.Count > 0)
                cbVozila.SelectedIndex = 0;
        }

        private void DgvVozaci_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVozaci.SelectedRows.Count == 0)
            {
                btnIzmeni.Enabled = false;
                btnDeaktiviraj.Enabled = false;
                return;
            }

            string id = dgvVozaci.SelectedRows[0].Cells["ID"].Value.ToString();
            var vozac = AppState.VozacService.GetVozacPoID(id);

            if (vozac != null)
            {
                txtKorisnickoIme.Text = vozac.KorisnickoIme;
                txtKorisnickoIme.ReadOnly = true; // Korisničko ime je ključ, ne menja se
                txtLozinka.Text = ""; // Lozinka se ne prikazuje
                txtLozinka.Enabled = false; // Ne dozvoljavamo promenu lozinke ovde (može u Korisnicima)
                txtImePrezime.Text = vozac.ImePrezime;
                txtBrojLicence.Text = vozac.BrojLicence;
                chkDostupan.Checked = vozac.Dostupan;

                // Selektujemo vozilo u combobox-u
                for (int i = 0; i < cbVozila.Items.Count; i++)
                {
                    var item = cbVozila.Items[i] as ComboBoxItem;
                    if (item != null && item.Value == vozac.VoziloID.ToString())
                    {
                        cbVozila.SelectedIndex = i;
                        break;
                    }
                }

                btnDodaj.Enabled = false;
                btnIzmeni.Enabled = true;
                btnDeaktiviraj.Enabled = true;
            }
        }

        private void ResetujFormu()
        {
            txtKorisnickoIme.Text = "";
            txtKorisnickoIme.ReadOnly = false;
            txtLozinka.Text = "";
            txtLozinka.Enabled = true;
            txtImePrezime.Text = "";
            txtBrojLicence.Text = "";
            chkDostupan.Checked = true;
            if (cbVozila.Items.Count > 0) cbVozila.SelectedIndex = 0;

            btnDodaj.Enabled = true;
            btnIzmeni.Enabled = false;
            btnDeaktiviraj.Enabled = false;
            dgvVozaci.ClearSelection();
        }

        private void BtnDodaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKorisnickoIme.Text) || 
                string.IsNullOrWhiteSpace(txtLozinka.Text) || 
                string.IsNullOrWhiteSpace(txtImePrezime.Text) || 
                string.IsNullOrWhiteSpace(txtBrojLicence.Text) || 
                cbVozila.SelectedItem == null)
            {
                MessageBox.Show("Sva polja su obavezna za dodavanje novog vozaca.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selVozilo = cbVozila.SelectedItem as ComboBoxItem;
                int voziloId = int.Parse(selVozilo.Value);

                AppState.VozacService.DodajVozaca(txtKorisnickoIme.Text.Trim(), txtLozinka.Text, txtImePrezime.Text.Trim(), txtBrojLicence.Text.Trim(), voziloId);
                MessageBox.Show("Novi vozac je uspesno dodan u sistem.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ResetujFormu();
                UcitajVozace();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnIzmeni_Click(object sender, EventArgs e)
        {
            if (dgvVozaci.SelectedRows.Count == 0) return;
            string id = dgvVozaci.SelectedRows[0].Cells["ID"].Value.ToString();

            if (string.IsNullOrWhiteSpace(txtImePrezime.Text) || 
                string.IsNullOrWhiteSpace(txtBrojLicence.Text) || 
                cbVozila.SelectedItem == null)
            {
                MessageBox.Show("Molimo popunite sva polja.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selVozilo = cbVozila.SelectedItem as ComboBoxItem;
                int voziloId = int.Parse(selVozilo.Value);

                AppState.VozacService.IzmeniVozaca(id, txtImePrezime.Text.Trim(), txtBrojLicence.Text.Trim(), voziloId, chkDostupan.Checked);
                MessageBox.Show("Podaci o vozacu su uspesno azurirani.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ResetujFormu();
                UcitajVozace();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeaktiviraj_Click(object sender, EventArgs e)
        {
            if (dgvVozaci.SelectedRows.Count == 0) return;
            string id = dgvVozaci.SelectedRows[0].Cells["ID"].Value.ToString();

            var rez = MessageBox.Show($"Da li ste sigurni da zelite da obrisete/deaktivirate vozaca {id}?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                try
                {
                    AppState.VozacService.DeaktivirajVozaca(id);
                    MessageBox.Show("Vozac je obrisan iz baze.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ResetujFormu();
                    UcitajVozace();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

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
