using System.Reflection.Emit;
using System.Windows.Forms;

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using GalacticOccupiers.Properties;
using Microsoft.VisualBasic.ApplicationServices;
using System.Security.Policy;
using System.ComponentModel.Design;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;
using System.Numerics;
using System.Media;


namespace SpaceInvadersALPHA
{
    public partial class Form1 : Form
    {

        //hraci pole
        const int sirka = 1280;                 //urèení sirky a vysky, zbytek const promìnnıch peèlivì vybrán, aby s tímto rozlišením dobøe fungoval
        const int vyska = 720;

        //hrac 
        const int rozmerHrace = 50;
        const int rychlostHrace = 10;

        bool dotykaLevo;                           //2 bool promìnné na kontrolu, zda se hráè dotıká hranic hracího pole, jednoduše omezuje jeho pohyb v rozích
        bool dotykaPravo;                          //viz PoziceHrace()

        int pocetZivotu;                    //po 3 zásazích konec


        //hrac BULLETS
        const int rozmerBullet = 5;             //lze libovolnì mìnit, pro nìjakı level obtínosti jsem zvolil 5
        bool dopadla;                           //boolovská promìnná zajišující to, aby hráè nemohl "spamovat" støely
        const int rychlostBullet = 50;          //pøizpùsobeno na 65

        //enemy
        const int rozmerEnemy = 50;             //pøizpùsobeno na velikost 50

        const int posun = 30;           //posun øady o kolik?
        const int mezera = 30;          //mezery mezi enemies

        int rychlostEnemy;                      //tøi int promìnné reprezentující rychlosti pohybu nepøátel, podobnì jako je v pùvodních Space Invaders se zrychlí v urèitıch fázích 
        int druhaRychlost;
        int finalRychlost;                      //nastavení hodnot v inicializaci (kvùli restartu hry apod.)

        int rychlostEnemyBullet;
        const int limitEnBul = 3;           //za zvıšením obtínosti lze toto èíslo zvìtšit (reprezentuje, kolik kulek mùe v jednu chvíli bıt vystøeleno maximálnì nepøáteli) 

        const int enemyRady = 5;                //lze libovolnì mìnit pro zvıšení obtínosti, realisticky jsem však zvolil pro relativnì spravedlivou obtínost 5 øad
        const int enemySloupce = 11;            //maximálnì 11 pro velikost enemy 50, jinak bude neustále splòovat podmínky pro posun øady dolù a hra skonèí


        int pocetEnemies;                       //v inicializace()
        int pulka;                              //pomocná promìnná pùlka mi pomùe s jednoduchou prací pozdìji


        //Enemy movement
        bool leftBorder;                //podobnì jako u hráèe pro kontrolu dotıkání se hranic
        bool rightBorder;

        bool lastRight;                 //urèuje poslední dotyk s hranicí hracího pole, podle toho se pak pole nepøátel hıbe doprava èi doleva
        bool lastLeft;

        //skore
        int skore;                      //skore a highscore, kterı se pøi nezavøení formu propisuje pøi kadé vıhøe èi prohøe (pokud je skore>highscore)
        int highscore = 0;

        int[] bonusVal = { 100, 120, 140, 160, 180, 200, 220, 360 }; //seznam pro vıbìr náhodné odmìny za sestøelení bonusové lodì

        bool bonusAlive; //zda bonusová loï byla ji sestøelena

        bool isGameOver;                        //bool promìnná, která breakuje game loop, pokud je true



        public void inicializace()
        {
            this.BackColor = Color.Black;           //nastavit pozadí na èernou
            //gameName.ForeColor = Color.Blue;                //název hry modrou 
            //label1.ForeColor = Color.Blue;


            this.FormBorderStyle = FormBorderStyle.FixedSingle;         //nastavení winformu tak, aby ho hráè nemohl nijak upravovat, roztahovat atd. 
            this.MaximumSize = new Size(sirka, vyska);
            this.MinimumSize = this.MaximumSize;

            //this.Controls.Add(bullet);

            //nastavení hráèe na zaèátku hry:
            hrac.Size = new Size(rozmerHrace, rozmerHrace);             //hrac je ve své podstatì reprezentován jako ètverec 
            hrac.Location = new Point((sirka - rozmerHrace) / 2, vyska - 120);          //"(sirka - rozmerHrace) / 2" znamená prostøedek na šíøku, 120 posun odspodu

            pocetZivotu = 2;            //jeden základní + 2 extra, tudí 3 ivoty celkem

            dotykaLevo = false;         //nedotıká se ani jedné hranice, opìt ošetøení, kdyby hráè na konci hry stál na jedné z hranic
            dotykaPravo = false;

            bullet.Size = new Size(rozmerBullet, rozmerBullet);
            dopadla = true;     //pøi inicializaci chci, aby všechny moné kulky dopadly a hráè mohl hned støílet

            //nepøátelé:
            enemyTimer.Start();     //zaènìte se hıbat 
            leftBorder = false;         //nedotıká se zrovna levé a ani pravé hranice
            rightBorder = false;

            lastRight = false;          //nastavení, aby se øada zaèala hıbat zleva doprava (šlo by klidnì prohodit, ale takto je to v "pùvodní" høe)
            lastLeft = true;

            rychlostEnemy = 2;                      //pro ztíení obtínosti staèí zvìtšit hodnotu rychlostEnemy
            druhaRychlost = rychlostEnemy * 2;
            finalRychlost = druhaRychlost * 4;

            rychlostEnemyBullet = 10;

            pocetEnemies = enemyRady * enemySloupce;
            pulka = pocetEnemies / 2;

            //další nastavení
            skore = 0;              //vynuluj skóre
            isGameOver = false;     //hra zaèala, tudí není game over


            explo.SizeMode = PictureBoxSizeMode.Zoom;
            explo.Size = new Size(rozmerEnemy, rozmerEnemy);
            this.Controls.Remove(explo);

            shipBonus.SizeMode = PictureBoxSizeMode.Zoom;
            shipBonus.Size = new Size(rozmerEnemy, rozmerEnemy);
            shipBonus.Location = new Point(1280, 10);
            bonusAlive = true;



        }


        private SoundPlayer efekt; //hraje efekt pøi trefení nepøítele 
        private SoundPlayer efekt2; //hraje efekt pøi trefení hráèe
        private SoundPlayer bonusEfekt; //hraje efekt pøi trefení bonus lodi

        private void gameSetup()
        {
            inicializace();         //proveï inicializaci (pøi restartu díky tomu správnì vynuluji a nastavím hodnoty)
            VytvorEnemies();        //viz níe VytvorEnemies();

            label1.SendToBack();    //aby tyto dva labely nepøekrıvaly hráèe
            en.SendToBack();

            bonusShip.Start();


            efekt = new SoundPlayer(GalacticOccupiers.Properties.Resources.bum);
            efekt2 = new SoundPlayer(GalacticOccupiers.Properties.Resources.bum2);
            bonusEfekt = new SoundPlayer(GalacticOccupiers.Properties.Resources.bonus_bum);

        }



        public Form1()
        {
            InitializeComponent();          //naèti Form
            gameSetup();                    //proveï gameSetup

            this.DoubleBuffered = true;         //toto jsem nalezl jako øešení pro menší sekanost pohybù PictureBoxù a vykreslování nepøátel

            hrac.Image = GalacticOccupiers.Properties.Resources.hrac1;      //naèti hrac.Image z Resources
            bullet.Image = GalacticOccupiers.Properties.Resources.ammo;     //to stejné pro bullet
            explo.Image = GalacticOccupiers.Properties.Resources.explo;



        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }



        private void PoziceHrace()              //Funkce urèující pohyby hráèe
        {
            if (hrac.Left <= 10)                                                  //OMEZENÍ HRACÍHO POLE
            {
                dotykaLevo = true;                  //dotıká se levé hranice

            }
            else if (hrac.Left >= sirka - rozmerHrace - 20)
            {
                dotykaPravo = true;                 //dotıká se pravé hranice
            }

            else
            {
                dotykaPravo = false;            //jinak se nedotıká ani jedné
                dotykaLevo = false;
            }

        }



        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            PoziceHrace();

            if (dotykaLevo)
            {
                if (e.KeyCode == Keys.Right)                                      //POHYB DOPRAVA, doleva nemùu = tam je hranice
                {
                    hrac.Left += rychlostHrace; //doprava
                }
            }

            else if (dotykaPravo)
            {
                if (e.KeyCode == Keys.Left)                                             //POHYB DOLEVA, doprava nemùu = tam je hranice
                {
                    hrac.Left -= rychlostHrace; //doleva o rychlostHrace
                }

            }

            else        //nedotıkám se ani jedné hranice
            {
                if (e.KeyCode == Keys.Left)                                             //POHYB DOLEVA
                {
                    hrac.Left -= rychlostHrace; //doleva o rychlostHrace

                }
                else if (e.KeyCode == Keys.Right)                                      //POHYB DOPRAVA
                {
                    hrac.Left += rychlostHrace; //doprava
                }
            }

            /*
            if (e.KeyCode == Keys.Q)                //QUIT = CVIÈNİ GAMEOVER
            {
                gameOver();
            }
            */

            //STØELBA SPACEM
            if (dopadla)        //tzn., e kulka hráèe u neletí (zasáhla, èi vybouchla mimo)
            {
                if (e.KeyCode == Keys.Space)                                      //STØELBA
                {
                    dopadla = false;        //støela letí

                    this.Controls.Add(bullet);                      //pøidej kulku
                    bullet.Left = hrac.Left + (rozmerHrace / 2);       //na pozici hráèe, aby to vypadalo,e opravdu støílí hráè
                    bullet.Top = hrac.Top;

                    //this.Controls.Add(bullet);
                    bullet.Image = GalacticOccupiers.Properties.Resources.ammo;

                    bulletTimer.Start();        //spus bulletTimer, kterı posouvá kulku k vrchní pozici, kde se kulka znièí

                    CollisionTimer.Start();     //kontrola a co se má dít pøi kolizi kulky s nepøítelem
                }
            }
        }


        private void bulletTimer_Tick(object sender, EventArgs e)
        {
            bulletTimer.Interval = 50;

            bullet.Top = bullet.Top - rychlostBullet;


            if (bullet.Top <= 0)
            {
                bullet.Image = GalacticOccupiers.Properties.Resources.e1;        //ZMÌNA NA JINOU VÌC Z NÌJAKÉ DÙVODU NEPLATÍ. 

                dopadla = true;         //SEM SE LOOP ZJEVNÌ DOSTANE

                /*
                bullet.Left = hrac.Left + (rozmerHrace / 2);
                bullet.Top = hrac.Top + 50;
                */

                this.Controls.Remove(bullet);

                bulletTimer.Stop();

            }
        }


        public class Enemy
        {
            public int x { get; set; }      //x souøadnice, y souøadnice
            public int y { get; set; }
            public int typ { get; set; }        //MONÁ SE NEPOUIJE? ???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            public bool zije { get; set; }      //zda ije, èi neije, velmi dùleitá funkce 
        }


        List<List<Enemy>> enemyList = new List<List<Enemy>>();      //novı list listù, takto budu nepøátele reprezentovat (abych mohl dret dodateèné informace o kadém nepøíteli)



        private void VytvorEnemies()          //naplnìní listù nepøátel s mezerami a vším, co bude potøeba
        {
            for (int row = 0; row < enemyRady; row++)       //pro kadı øádek         
            {
                List<Enemy> enemyRow = new List<Enemy>();       //vytvoøím si list s enemies v øadì
                for (int col = 0; col < enemySloupce; col++)        //pro kadı sloupec
                {
                    if (col == 0)
                    {
                        Enemy enemy = new Enemy();                      //vytvoøím enemy, urèím x,y a øeknu, e je ivı a následnì ho pøidám do seznamu enemyRow

                        enemy.x = col * 2 * mezera;                         //ZDE PÙJDOU DODAT RÙZNÉ TYPY NEPØÁTEL
                        enemy.y = row * 2 * mezera;

                        enemy.zije = true;

                        enemy.typ = row + 1;        //toto mi pomùe s urèením typu nepøátel a pozdìji s vykreslováním, funkce Paint
                        enemyRow.Add(enemy);
                    }

                    else if (col == enemySloupce - 1)
                    {
                        Enemy enemy = new Enemy();

                        enemy.x = col * 3 * mezera;
                        enemy.y = row * 2 * mezera;

                        enemy.zije = true;

                        enemy.typ = row + 1;
                        enemyRow.Add(enemy);
                    }

                    else
                    {
                        Enemy enemy = new Enemy();

                        enemy.x = col * 3 * mezera;
                        enemy.y = row * 2 * mezera;

                        enemy.zije = true;

                        enemy.typ = row + 1;
                        enemyRow.Add(enemy);
                    }

                }
                enemyList.Add(enemyRow);        //kadou øadu takto postupnì dodám do seznamu nepøátel (enemyList)

            }

        }


        private void CollisionTimer_Tick(object sender, EventArgs e)
        {
            CollisionTimer.Interval = 1;
            foreach (List<Enemy> enemyRow in enemyList)         //viz class Enemy a  List<List<Enemy>> enemyList , musím projídì takto
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)                     //pokud danı nepøítel ije, tak kontroluj kolize, jinak logicky ne
                    {

                        Rectangle enemyRect = new Rectangle(enemy.x, enemy.y, rozmerEnemy, rozmerEnemy);                //funkce IntersectsWith pracuje s Rectangles, pøišlo mi dobré to udìlat takto
                        Rectangle bulletRect = new Rectangle(bullet.Left, bullet.Top, bullet.Width, bullet.Height);


                        if (enemyRect.IntersectsWith(bulletRect))           //pokud dojde ke kolizi
                        {
                            bulletTimer.Stop();  //zastav pohyb kulky



                            this.Controls.Add(explo);
                            explo.Location = new Point(enemy.x, enemy.y);

                            efekt.Play();//pøehraj zvuk zásahu

                            Exploze.Start(); //start exploze


                            enemy.zije = false;     //enemy umøe (tudí nebude vykreslen)

                            this.Controls.Remove(bullet);       //dej pryè kulku, protoe zasáhla
                            dopadla = true;             //dopadla, tudí mùeš zas støílet

                            //pøidej skóre za zabitého nepøítele podle typu

                            if (enemy.typ == 1)
                            {
                                skore += 50;
                            }

                            else if (enemy.typ == 2)
                            {
                                skore += 40;
                            }

                            else if (enemy.typ == 3)
                            {
                                skore += 30;
                            }

                            else if (enemy.typ == 4)
                            {
                                skore += 20;
                            }

                            else if (enemy.typ == 5)
                            {
                                skore += 10;
                            }


                            pocetEnemies -= 1;      //odeèti zabitého nepøítele z celkového poètu

                            //ošetøení, aby nìkde kulka nezùstala viset
                            bullet.Left = hrac.Left + (rozmerHrace / 2);
                            bullet.Top = hrac.Top + 50;

                            CollisionTimer.Stop();
                            return;                 //pøeruš for loop, kdy najdeš kolizi

                        }
                    }
                }
            }
        }



        private void pohybEnemyRIGHT()
        {
            foreach (List<Enemy> enemyRow in enemyList)         //projídìní enemylistu opìt stejnım zpùsobem
            {
                foreach (Enemy enemy in enemyRow)
                {
                    enemy.x += rychlostEnemy;       //doprava dle rychlosti
                }
            }
        }



        private void pohybEnemyLEFT()               //analogicky jako pøedchozí funkce, ale pro posun doleva odeèítám
        {
            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    enemy.x -= rychlostEnemy;
                }
            }
        }


        private void pohybRady()                        //takté analogicky, tentokrát však po y souøadnicích (pro poskoèení pøi dotknutí se hranice)
        {
            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    enemy.y += posun;
                }
            }
        }


        private void checkBorders()
        {
            int rightmostX = rozmerEnemy * 2 * enemySloupce;        //rightmostX a leftmostX kontrolují nejvíce pravou a nejvíce levou souøadnici
            int leftmostX = rightmostX - rozmerEnemy;                  //to dovoluje kontrolu, zda se zrovna nedotıkám hranice vlevo èi v pravo

            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)
                    {

                        if (enemy.x > rightmostX)
                        {
                            rightmostX = enemy.x;                   //pøi posunu se mi rightmostX i leftmostX mìní, proto takto projídím a updatuji
                        }

                        if (enemy.x < leftmostX)
                        {
                            leftmostX = enemy.x;
                        }
                    }
                }
            }


            if (rightmostX >= 1200)    //logicka zmìna bool promìnnıch, v tomto pøípadì jel zleva a dotkl se hranice
            {
                lastLeft = false;
                lastRight = true;

                rightBorder = true;
            }

            if (rightBorder)
            {
                pohybRady();            //posuò se tedy o øadu a pak zaèni zprava doleva
                rightBorder = false;
            }



            if (leftmostX <= 0)         //analogicky pro opaènı smìr
            {
                lastRight = false;
                lastLeft = true;

                leftBorder = true;
            }

            if (leftBorder)
            {
                pohybRady();
                leftBorder = false;
            }


            if (lastRight)          //pokud byla poslední zmìna poøadí vpravo, jdi doleva
            {
                pohybEnemyLEFT();
            }

            if (lastLeft)           //analogicky
            {
                pohybEnemyRIGHT();
            }


            //kontrola, zda nepøátelé nejsou na úrovni hráèe

            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)
                    {
                        if (enemy.y >= 600)         //600 zde reprezentuje vıšku hráèe
                        {
                            gameOver();             //viz gameOver();
                            return;             //return, protoe dál nechci projídìt
                        }

                    }

                }

            }
        }


        int maxEnemyBullets = 3;
        List<PictureBox> enemyBullets = new List<PictureBox>();


        private void enemyTimer_Tick(object sender, EventArgs e)
        {
            // enemyTimer.Interval = 100;        víc oldschool feel?
            enemyTimer.Interval = 50;

            checkBorders();
            Invalidate();             //s kadım tickem pøeklesuje (bez tohoto by se grafiky nehıbali nepøátelé)


            if (pocetEnemies == 0)          //všichni nepøátelé byli zabiti hráèem, vıhra
            {
                isGameOver = true;
            }


            if (pocetEnemies <= pulka)              //zrychlení 
            {            //PÙLKA ENEMIES
                rychlostEnemy = druhaRychlost;
            }

            if (pocetEnemies == 1)          //zbıvá jeden nepøítel, ještì více zrychli
            {
                rychlostEnemy = finalRychlost;
            }

            if (isGameOver)         //pokud jakkoliv nastane, e isGameOver = true, tak ukonèi hru (je to zde v enemyTimeru, protoe ten bìí celou hru)
            {
                gameOver();         //viz gameOver();
            }

            score.Text = "Score: " + skore.ToString();
            en.Text = "Extra Lifes: " + pocetZivotu.ToString();

            enBull();       //vytváøej kulky
            kontrolaKolize();   //kontroluj kolize



        }


        private void enBull()
        {
            if (enemyBullets.Count < limitEnBul)
            {
                Random random = new Random();                       //pouití random na vyhledání náhodného nepøítele

                int randomRowIndex = random.Next(enemyList.Count);      //randomRowIndex

                List<Enemy> randomRow = enemyList[randomRowIndex];          //vybrání náhodné Row
                int randomEnemyIndex = random.Next(randomRow.Count);

                Enemy randomEnemy = randomRow[randomEnemyIndex];        //z náhodné øady náhodnı enemy


                PictureBox enemyBullet = new PictureBox();              //vytvoøím novou kulku pøímo zde
                enemyBullet.Size = new Size(5, 5);                          //velikost, barva, tag, pozice
                enemyBullet.BackColor = Color.Yellow;
                enemyBullet.Tag = "enemyBullet";
                enemyBullet.Left = randomEnemy.x;
                enemyBullet.Top = randomEnemy.y;


                Controls.Add(enemyBullet);      //pøidej do Controls
                enemyBullets.Add(enemyBullet);  //pøidej do seznamu kulek k vystøelení

            }
        }



        private void kontrolaKolize()           //enemy bullets - hráè kolize
        {

            for (int i = enemyBullets.Count - 1; i >= 0; i--)       //iteruji pøes všechny vytvoøené (letící) kulky
            {
                PictureBox enemyBullet = enemyBullets[i];       //pomùe mi s operováním (dispose, pohyb, remove atd.)
                enemyBullet.Top += rychlostEnemyBullet;         //let kulky

                if (enemyBullet.Bounds.IntersectsWith(hrac.Bounds))             //IntersectsWith pro kulku nepøítele a hráèe
                {
                    efekt2.Play();

                    if (pocetZivotu == 0)
                    {
                        deleteBullets();        //pokud u hráè nemá ivoty, konec hry
                        gameOver();

                        return;         //return proto, e pak nepotøebuju for loop dokonèovat
                    }

                    else
                    {
                        stopEverything();       //jinak vše stopni, hráèovi napiš, kolik mu zbıvá ivotù a odstraò letící kulky, aby hned nezemøel znovu, pak startni
                        deleteBullets();

                        Controls.Remove(enemyBullet);
                        enemyBullet.Dispose();
                        enemyBullets.Remove(enemyBullet);

                        pocetZivotu--;
                        MessageBox.Show("HIT! " + pocetZivotu.ToString() + " extra lifes left");

                        startEverything();
                    }
                }

                if (enemyBullet.Top > vyska)            //pokud kulka netrefila hráèe a u letí mimo hrací pole, odstraò ji (vytvoøí místo pro poslání další kulky)
                {
                    Controls.Remove(enemyBullet);
                    enemyBullet.Dispose();
                    enemyBullets.Remove(enemyBullet);

                }
            }
        }


        private void deleteBullets()                //funkce, která zaruèuje, aby všechny kulky zmizely a nijak se nepletly
        {
            foreach (PictureBox enemyBullet in enemyBullets)        //nutné iterovat pøes list bullets
            {
                Controls.Remove(enemyBullet);
                enemyBullet.Dispose();
            }
        }


        protected override void OnPaint(PaintEventArgs e)                       //kreslící funkce, vzhledem k tomu, e enemies nejsou PictureBoxy, tak jsem musel situaci øešit jinak
        {
            base.OnPaint(e);

            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)         //pokud ije, vykresli nepøítele na jeho pozici
                    {
                        if (enemy.typ == 1)
                        {
                            e.Graphics.DrawImage(GalacticOccupiers.Properties.Resources.e4, enemy.x, enemy.y, rozmerEnemy, rozmerEnemy - 20);           //podle typu obrázek
                        }

                        if (enemy.typ == 2)
                        {
                            e.Graphics.DrawImage(GalacticOccupiers.Properties.Resources.e3, enemy.x, enemy.y, rozmerEnemy, rozmerEnemy - 20);
                        }

                        if (enemy.typ == 3)
                        {
                            e.Graphics.DrawImage(GalacticOccupiers.Properties.Resources.e2, enemy.x, enemy.y, rozmerEnemy, rozmerEnemy - 20);
                        }

                        if (enemy.typ == 4)
                        {
                            e.Graphics.DrawImage(GalacticOccupiers.Properties.Resources.e5, enemy.x, enemy.y, rozmerEnemy, rozmerEnemy - 20);
                        }

                        if (enemy.typ == 5)
                        {
                            e.Graphics.DrawImage(GalacticOccupiers.Properties.Resources.e1, enemy.x, enemy.y, rozmerEnemy, rozmerEnemy - 20);
                        }

                    }
                }
            }
        }



        //UKONÈOVACÍ FUNKCE, GAMEOVER, PAUZA, RESET, ...

        private void stopEverything()
        {
            enemyTimer.Stop();          //umoní pozastavení pøed celım resetem hry èi pauzou
            bulletTimer.Stop();

            bonusShip.Stop();

        }

        private void startEverything()
        {
            enemyTimer.Start();         //pokud jen pauzuju, chci následnì i pokraèovat
            bulletTimer.Start();

            if (bonusAlive)
            {
                bonusShip.Start();
            }

        }



        private void pauza()
        {
            stopEverything();               //stopni
            MessageBox.Show(this, "Paused, press OK or ESC to continue..."); //vypiš zprávu a poèkej na její zavøení
            startEverything();              //pokraèuj

        }


        private void gameOver()                                                             //STAÈÍ ZAVOLAT GAMEOVER A JE KONEC.
        {
            deleteBullets();            //odstraò a pauzni vše, co lítá
            stopEverything();
            MessageBox.Show("Konec hry, chcete hrát znovu?");   //vyskoèí okénko

            if (skore > highscore)
            {
                highscore = skore;          //propis souèasného skóre do highscore, pokud je víš, ne highscore pøed tím
            }

            hs.Text = "Highscore: " + highscore.ToString();         //zobraz highscore

            reset();                        //viz níe
        }



        private void reset()
        {
            enemyList.Clear();                  //odstraò všechny enemies
            this.Controls.Remove(bullet);       //odstraò kulku hráèe

            this.Controls.Remove(shipBonus);

            gameSetup();                        //vše znovu nastav

        }

        private void hrac_Click(object sender, EventArgs e)
        {

        }

        private void Exploze_Tick(object sender, EventArgs e)
        {
            Exploze.Interval = 500;
            this.Controls.Remove(explo);

        }

        private void timer2_Tick(object sender, EventArgs e)            //bonus ship, jde sestøelit jednou za hru
        {
            bonusShip.Interval = 50;
            this.Controls.Add(shipBonus);

            if (shipBonus.Bounds.IntersectsWith(bullet.Bounds))             //pokud ji hráè ztrefil
            {
                Random random = new Random();                               //vyber random hodnotu z bonusVal[]

                bonusEfekt.Play();
                bonusAlive = false;


                int randomVal = bonusVal[random.Next(0, bonusVal.Length)];
                skore += randomVal;                 //pøièti danou hodnotu
                this.Controls.Remove(shipBonus);

                bonusShip.Stop();

            }

            else if (shipBonus.Left > 0)
            {
                shipBonus.Left -= 20;           //kdy je v poli, posouvej
            }

            else if (shipBonus.Left <= 0)
            {
                shipBonus.Left = 1300;          //kdy vyjede z pole
            }


        }

    }
}