# PROJEKTNA DOKUMENTACIJA: TAXI PREVOZ
**Predmet:** Programiranje / Razvoj softvera
**Tehnologija:** C# Windows Forms (.NET Framework 4.8)
**Baza podataka:** Tekstualni fajlovi (.txt)

---

## 1. UVOD I OPIS PROJEKTA
Aplikacija **Taxi Prevoz** je osmišljena kao sistem za upravljanje radom taxi službe. Sistem omogućava korisnicima različitih uloga (Putnik, Vozač, Administrator) interakciju sa sistemom u realnom vremenu:
- **Putnici** mogu naručiti vožnju, pratiti njen status i oceniti vozača.
- **Vozači** dobijaju zahteve, prihvataju ih i upravljaju stanjima vožnje.
- **Administratori** imaju potpunu kontrolu nad svim vozilima, vozačima, korisničkim nalozima, cenovnikom i finansijskim izveštajima.

---

## 2. KORISNIČKE ULOGE I PRAVA PRISTUPA (TABELA PRAVA)
Sistem implementira bezbednosni interfejs `IDozvola` koji strogo definiše šta koja uloga može da izvršava.

| Dozvola / Akcija | Administrator | Vozač | Putnik |
| :--- | :---: | :---: | :---: |
| Upravljanje vozačima i vozilima | DA | NE | NE |
| Naručivanje vožnje | DA | NE | DA |
| Otkazivanje vožnje | DA | NE | DA |
| Prihvatanje / odbijanje vožnje | DA | DA | NE |
| Menjanje statusa vožnje | DA | DA | NE |
| Pregled svih vožnji | DA | NE | NE |
| Pregled sopstvenih vožnji | DA | DA | DA |
| Ocenjivanje vozača | DA | NE | DA |
| Pregled ocena i rang liste | DA | DA | DA |
| Upravljanje cenovnikom | DA | NE | NE |
| Finansijski izveštaji | DA | NE | NE |

---

## 3. ARHITEKTURA SISTEMA
Aplikacija je realizovana kroz višeslojnu arhitekturu (N-tier architecture) kako bi se postigla modularnost i lakše održavanje koda:

1. **UI Sloj (Forms):** 
   - `FormLogin` - Prijava na sistem sa proverom lozinke.
   - `FormMain` - Glavni meni sa dinamičkim prikazom dugmadi u zavisnosti od uloge.
   - `FormVoznje` / `FormMojeVoznje` - Prikaz i filtriranje vožnji.
   - `FormNaruciVoznju` - Naručivanje i praćenje statusa vožnje u realnom vremenu.
   - `FormVozaci` / `FormVozila` - Upravljanje resursima taxi službe.
   - `FormIzvestaji` - Statistika i prihodi.
2. **Sloj Poslovne Logike (Services):**
   - `AuthService` - Logika za prijavu i proveru SHA-256 heša lozinke.
   - `VoznjaService` - Upravljanje životnim ciklusom vožnje (Naručena -> Prihvaćena -> U toku -> Završena/Otkazana) i automatska dodela slobodnih vozača.
   - `CenovnikService` - Obračun cene vožnji na osnovu startne cene, pređenih kilometara i noćne doplate.
   - `LogService` - Logovanje svake promene statusa vožnje u `log.txt`.
3. **Sloj Podataka (Repositories):**
   - Čitanje i upisivanje u `.txt` fajlove. Podaci se čuvaju u strukturiranom formatu (deljeni sa `;` ili `|`).
   - Automatsko generisanje jedinstvenih identifikatora (npr. `VOZ-2026-0001` za vožnje) preko brojača u `brojaci.txt`.

---

## 4. DETALJAN OPIS BAZIČNIH KLASA I MODELA

### 4.1 Korisnici (`Korisnik.cs`)
Apstraktna klasa `Korisnik` implementira interfejs `IDozvola` i predstavlja osnovu za sve tri uloge:
- **Admin:** Nasleđuje `Korisnik`.
- **Vozac:** Sadrži dodatna polja: `VozacID`, `ImePrezime`, `BrojLicence`, `VoziloID`, `Dostupan`, `ProsecnaOcena` i `BrojOcena`.
- **Putnik:** Sadrži dodatna polja: `PutnikID`, `ImePrezime`, `Telefon`, `Email`.

### 4.2 Vozilo (`Vozilo.cs`)
Predstavlja vozilo iz voznog parka. Sadrži polja: `VoziloID`, `Registracija`, `Marka`, `Model`, `Godiste`, `Boja` i status `Aktivno`.

### 4.3 Voznja (`Voznja.cs`)
Modeluje pojedinačnu vožnju sa poljima:
- `VoznjaID` (npr. `VOZ-2026-0001`)
- `PutnikID` i `VozacID`
- `Polaziste` i `Odrediste`
- `VremeNarucivanja`, `VremePolaska` i `VremeDolaska`
- `Kilometraza`, `Cena` i `Status` (enum `StatusVoznje`)
- `Ocena` (1-5) i `Komentar`

---

## 5. BEZBEDNOST: HESIRANJE LOZINKI (SHA-256)
U skladu sa bezbednosnim standardima, lozinke korisnika se **nikada** ne čuvaju u običnom tekstualnom formatu (plain text).
- Kada korisnik kreira nalog ili menja lozinku, aplikacija koristi klasu `HashHelper` da generiše jednosmerni SHA-256 heš.
- Prilikom prijave, uneta lozinka se hešira i poredi sa hešom koji je sačuvan u fajlu `korisnici.txt`.

Primer zapisa u `korisnici.txt`:
`putnik1|2d50109ce50b5e7d84a4d9dc75c1e8a699ff0be5879c5e27b25f9a12bb83f3a1|Putnik|PUT-0001`

---

## 6. UPUTSTVO ZA TESTIRANJE APLIKACIJE

### 6.1 Testni podaci za prijavu:
1. **Administrator:**
   - Korisničko ime: `admin`
   - Lozinka: `admin123`
2. **Putnik 1:**
   - Korisničko ime: `putnik1`
   - Lozinka: `put1`
3. **Vozač 1:**
   - Korisničko ime: `vozac1`
   - Lozinka: `voz1`

### 6.2 Scenario za simulaciju vožnje:
1. Prijavite se kao **putnik1** i naručite vožnju unošenjem polazišta i odredišta.
2. Odjavite se i prijavite se kao **vozac1**. Videćete naručenu vožnju u listi. Kliknite na **Prihvati**, a zatim promenite status u **U toku** i na kraju **Završena**.
3. Pri završetku vožnje, sistem će automatski obračunati cenu na osnovu pređene kilometraže i ažurirati dostupnost vozača.
4. Prijavite se ponovo kao **putnik1**. Idite na istoriju vožnji gde možete oceniti vozača ocenom 1-5 i ostaviti komentar.
