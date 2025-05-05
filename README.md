# Funkcionális specifikáció

## Modul és kliens funkciók részletes leírása

### Tematikus Terepasztal Ajánló Modul

**Cél:**  
A webshop vásárlói számára egy interaktív felület biztosítása, amely lehetővé teszi számukra, hogy specifikus történelmi és környezeti kritériumok alapján személyre szabott termékcsomag-ajánlásokat kapjanak terepasztalok és diorámák építéséhez.  
A modul célja a vásárlási élmény javítása, a releváns termékek könnyebb megtalálása és potenciálisan az átlagos kosárérték növelése.

#### Fő Funkciók:

##### Kritériumok Kiválasztása (Felhasználói Interfész)

- A modul egy jól látható felületet biztosít a webshopban (pl. dedikált oldalon vagy kategóriaoldalakon elhelyezett blokkban).
- A felhasználó a következő paramétereket választhatja ki legördülő menükből vagy választólistákból:
  - **Korszak**: pl. Ókor, Középkor, I. Világháború, II. Világháború, Modern kor, Sci-fi, Fantasy stb.
  - **Hadműveleti Terület/Front**: pl. Keleti front, Nyugati front, Csendes-óceáni hadszíntér, Észak-Afrika stb.
  - **Tereptípus**: pl. Városi környezet, Sivatag, Tengerpart/Partraszállás, Erdős/Bokros terület, Mezőgazdasági terület, Téli/Havas táj, Dzsungel stb.
  - **Méretarány**: A termékek általában nem minden méretarányban elérhetőek, ezért ez kötelezően töltődik ki a többi választás alapján.

- Egy „**Ajánlat Kérése**” / „**Csomag Mutatása**” gomb a kiválasztás véglegesítésére.

##### Termékajánlás Logikája (Backend Működés)

- A kiválasztott kritériumok alapján a modul lekérdezést indít a Hotcakes termékadatbázisában vagy egy kapcsolódó adatbázisban.
- **Előfeltétel:** A webshop termékeit előzetesen megfelelően kell címkézni a kritériumok szerint (pl. "Tigris tank" → "II. Világháború", "Keleti front", "Páncélos").
- A modul összegyűjti a releváns termékeket, intelligens szűréssel:
  - Pl. „Tengerpart” terep esetén partraszálláshoz kapcsolódó termékek ajánlása.
- Az ajánlott csomag tartalmazhat:
  - 1-2 fő elemet (pl. jármű vagy épület makett)
  - Kiegészítő figurákat
  - Terepépítéshez szükséges anyagokat
  - Festékeket, ragasztókat, eszközöket (opcionálisan)

##### Ajánlat Megjelenítése (Felhasználói Interfész)

- Az ajánlott csomag világos megjelenítése:
  - Név, kép, ár, leírás/címkék
- Funkciók:
  - Teljes csomag kosárba helyezése egy kattintással
  - Egyedi elemek kiválasztása a csomagból
  - Kritériumok módosítása, új ajánlat kérés

##### Lehetséges Jövőbeli Bővítések

- Árkategória szerinti szűrés
- Teljesebb csomagok ajánlása
- Vizuálisabb interfész (pl. illusztrált tereptípusok)

---

### Webshop Termék Kapcsolatkezelő

**Cél:**  
Egy dedikált desktop alkalmazás biztosítása a webshop adminisztrátorai számára a tematikus metaadatok (korszak, front, terep) kezelésére.  
Fő funkció: gráf alapú vizualizáció a kapcsolatok áttekintéséhez.

#### Fő Funkciók:

##### Termékkezelő Felület

- **Terméklista megjelenítése**: Név, SKU, kép
- **Keresés és szűrés**: név, SKU, címkék, típus szerint
- **Termék kiválasztása** metaadat-szerkesztéshez

##### Metaadat Szerkesztő (Tag Manager)

- **Címkék megjelenítése**: Korszak, Front, Tereptípus
- **Címkék hozzáadása/eltávolítása**: legördülő listák, pipálható elemek, címkefelhő
- Több címke hozzárendelése egy kategórián belül is engedélyezett
- **Mentés** a központi adatbázisba

##### Kapcsolati Gráf Vizualizáció

- **Csúcsok**: termékek, kattinthatók
- **Élek**: kapcsolatok közös címkék alapján
- **Tengelyek**:
  - X-tengely: idő/korszak
  - Y-tengely: áttekinthetőség érdekében pozicionálás

##### Mesteradat Kezelés

- Elérhető címkék (korszak, front, tereptípus) karbantartása
- Új értékek hozzáadása, meglévők átnevezése, törlése

##### Adatbázis Kapcsolat

- Biztonságos, konfigurálható kapcsolat
- Olvasási és írási lehetőség

##### Példa Workflow

1. Új termék felvitele a webshopban (pl. T-34/85 Tank Makett)
2. WinForms alkalmazás elindítása
3. Terméklista frissítése, új termék megjelenik
4. Admin kiválasztja a terméket
5. Címkék hozzárendelése:
   - Korszak: II. Világháború
   - Front: Keleti front
   - Tereptípus: Városi, Mezőgazdasági, Téli
6. Mentés az adatbázisba
7. Opcionálisan: gráfnézetben kapcsolatok ellenőrzése

##### Szükséges További Feature-ök

- Visszavonás (Undo)
- Import/Export (pl. CSV)
- Mentés előtti validáció (pl. van-e legalább egy korszak)
- "Szett tesztelő"/Előnézet: felhasználói kiválasztások szimulációja

---

## Funkcionális követelmények összegyűjtése

### Adatműveletek és működési logikák felvázolása

**Cél:**  
Az adatbázis feladata, hogy strukturáltan tárolja a termékek és a tematikus kategóriák közötti kapcsolatokat.

#### Fő Entitások/Koncepciók

- **Termék Referenciák**: egyedi termékazonosító, a Hotcakes azonosítója alapján
- **Tematikus Kategóriák (Címkék)**:
  - Korszakok
  - Frontok
  - Tereptípusok
  - Az értékeket központi listákban kell kezelni
- **Hozzárendelések (Kapcsolatok)**:
  - Több-a-többhöz kapcsolatok
  - Egy termék több címkéhez, egy címke több termékhez tartozhat
- **Struktúra**: logikailag kapcsolja össze a termékek és címkék azonosítóit

#### Felhasználás

- **WinForms alkalmazás**: címkék és hozzárendelések menedzselése
- **Webshop ajánló modul**: ezen adatok alapján készít releváns termékajánlásokat

---

## Egyeztetés a megrendelővel, feedback-ek
