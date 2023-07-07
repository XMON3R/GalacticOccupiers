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
        const int sirka = 1280;                 //ur�en� sirky a vysky, zbytek const prom�nn�ch pe�liv� vybr�n, aby s t�mto rozli�en�m dob�e fungoval
        const int vyska = 720;

        //hrac 
        const int rozmerHrace = 50;
        const int rychlostHrace = 10;

        bool dotykaLevo;                           //2 bool prom�nn� na kontrolu, zda se hr�� dot�k� hranic hrac�ho pole, jednodu�e omezuje jeho pohyb v roz�ch
        bool dotykaPravo;                          //viz PoziceHrace()

        int pocetZivotu;                    //po 3 z�saz�ch konec


        //hrac BULLETS
        const int rozmerBullet = 5;             //lze libovoln� m�nit, pro n�jak� level obt�nosti jsem zvolil 5
        bool dopadla;                           //boolovsk� prom�nn� zaji��uj�c� to, aby hr�� nemohl "spamovat" st�ely
        const int rychlostBullet = 50;          //p�izp�sobeno na 65

        //enemy
        const int rozmerEnemy = 50;             //p�izp�sobeno na velikost 50

        const int posun = 30;           //posun �ady o kolik?
        const int mezera = 30;          //mezery mezi enemies

        int rychlostEnemy;                      //t�i int prom�nn� reprezentuj�c� rychlosti pohybu nep��tel, podobn� jako je v p�vodn�ch Space Invaders se zrychl� v ur�it�ch f�z�ch 
        int druhaRychlost;
        int finalRychlost;                      //nastaven� hodnot v inicializaci (kv�li restartu hry apod.)

        int rychlostEnemyBullet;
        const int limitEnBul = 3;           //za zv��en�m obt�nosti lze toto ��slo zv�t�it (reprezentuje, kolik kulek m��e v jednu chv�li b�t vyst�eleno maxim�ln� nep��teli) 

        const int enemyRady = 5;                //lze libovoln� m�nit pro zv��en� obt�nosti, realisticky jsem v�ak zvolil pro relativn� spravedlivou obt�nost 5 �ad
        const int enemySloupce = 11;            //maxim�ln� 11 pro velikost enemy 50, jinak bude neust�le spl�ovat podm�nky pro posun �ady dol� a hra skon��


        int pocetEnemies;                       //v inicializace()
        int pulka;                              //pomocn� prom�nn� p�lka mi pom��e s jednoduchou prac� pozd�ji


        //Enemy movement
        bool leftBorder;                //podobn� jako u hr��e pro kontrolu dot�k�n� se hranic
        bool rightBorder;

        bool lastRight;                 //ur�uje posledn� dotyk s hranic� hrac�ho pole, podle toho se pak pole nep��tel h�be doprava �i doleva
        bool lastLeft;

        //skore
        int skore;                      //skore a highscore, kter� se p�i nezav�en� formu propisuje p�i ka�d� v�h�e �i proh�e (pokud je skore>highscore)
        int highscore = 0;

        int[] bonusVal = { 100, 120, 140, 160, 180, 200, 220, 360 }; //seznam pro v�b�r n�hodn� odm�ny za sest�elen� bonusov� lod�

        bool bonusAlive; //zda bonusov� lo� byla ji� sest�elena

        bool isGameOver;                        //bool prom�nn�, kter� breakuje game loop, pokud je true



        public void inicializace()
        {
            this.BackColor = Color.Black;           //nastavit pozad� na �ernou
            //gameName.ForeColor = Color.Blue;                //n�zev hry modrou 
            //label1.ForeColor = Color.Blue;


            this.FormBorderStyle = FormBorderStyle.FixedSingle;         //nastaven� winformu tak, aby ho hr�� nemohl nijak upravovat, roztahovat atd. 
            this.MaximumSize = new Size(sirka, vyska);
            this.MinimumSize = this.MaximumSize;

            //this.Controls.Add(bullet);

            //nastaven� hr��e na za��tku hry:
            hrac.Size = new Size(rozmerHrace, rozmerHrace);             //hrac je ve sv� podstat� reprezentov�n jako �tverec 
            hrac.Location = new Point((sirka - rozmerHrace) / 2, vyska - 120);          //"(sirka - rozmerHrace) / 2" znamen� prost�edek na ���ku, 120 posun odspodu

            pocetZivotu = 2;            //jeden z�kladn� + 2 extra, tud� 3 �ivoty celkem

            dotykaLevo = false;         //nedot�k� se ani jedn� hranice, op�t o�et�en�, kdyby hr�� na konci hry st�l na jedn� z hranic
            dotykaPravo = false;

            bullet.Size = new Size(rozmerBullet, rozmerBullet);
            dopadla = true;     //p�i inicializaci chci, aby v�echny mo�n� kulky dopadly a hr�� mohl hned st��let

            //nep��tel�:
            enemyTimer.Start();     //za�n�te se h�bat 
            leftBorder = false;         //nedot�k� se zrovna lev� a ani prav� hranice
            rightBorder = false;

            lastRight = false;          //nastaven�, aby se �ada za�ala h�bat zleva doprava (�lo by klidn� prohodit, ale takto je to v "p�vodn�" h�e)
            lastLeft = true;

            rychlostEnemy = 2;                      //pro zt�en� obt�nosti sta�� zv�t�it hodnotu rychlostEnemy
            druhaRychlost = rychlostEnemy * 2;
            finalRychlost = druhaRychlost * 4;

            rychlostEnemyBullet = 10;

            pocetEnemies = enemyRady * enemySloupce;
            pulka = pocetEnemies / 2;

            //dal�� nastaven�
            skore = 0;              //vynuluj sk�re
            isGameOver = false;     //hra za�ala, tud� nen� game over


            explo.SizeMode = PictureBoxSizeMode.Zoom;
            explo.Size = new Size(rozmerEnemy, rozmerEnemy);
            this.Controls.Remove(explo);

            shipBonus.SizeMode = PictureBoxSizeMode.Zoom;
            shipBonus.Size = new Size(rozmerEnemy, rozmerEnemy);
            shipBonus.Location = new Point(1280, 10);
            bonusAlive = true;



        }


        private SoundPlayer efekt; //hraje efekt p�i trefen� nep��tele 
        private SoundPlayer efekt2; //hraje efekt p�i trefen� hr��e
        private SoundPlayer bonusEfekt; //hraje efekt p�i trefen� bonus lodi

        private void gameSetup()
        {
            inicializace();         //prove� inicializaci (p�i restartu d�ky tomu spr�vn� vynuluji a nastav�m hodnoty)
            VytvorEnemies();        //viz n�e VytvorEnemies();

            label1.SendToBack();    //aby tyto dva labely nep�ekr�valy hr��e
            en.SendToBack();

            bonusShip.Start();


            efekt = new SoundPlayer(GalacticOccupiers.Properties.Resources.bum);
            efekt2 = new SoundPlayer(GalacticOccupiers.Properties.Resources.bum2);
            bonusEfekt = new SoundPlayer(GalacticOccupiers.Properties.Resources.bonus_bum);

        }



        public Form1()
        {
            InitializeComponent();          //na�ti Form
            gameSetup();                    //prove� gameSetup

            this.DoubleBuffered = true;         //toto jsem nalezl jako �e�en� pro men�� sekanost pohyb� PictureBox� a vykreslov�n� nep��tel

            hrac.Image = GalacticOccupiers.Properties.Resources.hrac1;      //na�ti hrac.Image z Resources
            bullet.Image = GalacticOccupiers.Properties.Resources.ammo;     //to stejn� pro bullet
            explo.Image = GalacticOccupiers.Properties.Resources.explo;



        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }



        private void PoziceHrace()              //Funkce ur�uj�c� pohyby hr��e
        {
            if (hrac.Left <= 10)                                                  //OMEZEN� HRAC�HO POLE
            {
                dotykaLevo = true;                  //dot�k� se lev� hranice

            }
            else if (hrac.Left >= sirka - rozmerHrace - 20)
            {
                dotykaPravo = true;                 //dot�k� se prav� hranice
            }

            else
            {
                dotykaPravo = false;            //jinak se nedot�k� ani jedn�
                dotykaLevo = false;
            }

        }



        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            PoziceHrace();

            if (dotykaLevo)
            {
                if (e.KeyCode == Keys.Right)                                      //POHYB DOPRAVA, doleva nem��u = tam je hranice
                {
                    hrac.Left += rychlostHrace; //doprava
                }
            }

            else if (dotykaPravo)
            {
                if (e.KeyCode == Keys.Left)                                             //POHYB DOLEVA, doprava nem��u = tam je hranice
                {
                    hrac.Left -= rychlostHrace; //doleva o rychlostHrace
                }

            }

            else        //nedot�k�m se ani jedn� hranice
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
            if (e.KeyCode == Keys.Q)                //QUIT = CVI�N� GAMEOVER
            {
                gameOver();
            }
            */

            //ST�ELBA SPACEM
            if (dopadla)        //tzn., �e kulka hr��e u� nelet� (zas�hla, �i vybouchla mimo)
            {
                if (e.KeyCode == Keys.Space)                                      //ST�ELBA
                {
                    dopadla = false;        //st�ela let�

                    this.Controls.Add(bullet);                      //p�idej kulku
                    bullet.Left = hrac.Left + (rozmerHrace / 2);       //na pozici hr��e, aby to vypadalo,�e opravdu st��l� hr��
                    bullet.Top = hrac.Top;

                    //this.Controls.Add(bullet);
                    bullet.Image = GalacticOccupiers.Properties.Resources.ammo;

                    bulletTimer.Start();        //spus� bulletTimer, kter� posouv� kulku k vrchn� pozici, kde se kulka zni��

                    CollisionTimer.Start();     //kontrola a co se m� d�t p�i kolizi kulky s nep��telem
                }
            }
        }


        private void bulletTimer_Tick(object sender, EventArgs e)
        {
            bulletTimer.Interval = 50;

            bullet.Top = bullet.Top - rychlostBullet;


            if (bullet.Top <= 0)
            {
                bullet.Image = GalacticOccupiers.Properties.Resources.e1;        //ZM�NA NA JINOU V�C Z N�JAK� D�VODU NEPLAT�. 

                dopadla = true;         //SEM SE LOOP ZJEVN� DOSTANE

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
            public int x { get; set; }      //x sou�adnice, y sou�adnice
            public int y { get; set; }
            public int typ { get; set; }        //MO�N� SE NEPOU�IJE? ???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            public bool zije { get; set; }      //zda �ije, �i ne�ije, velmi d�le�it� funkce 
        }


        List<List<Enemy>> enemyList = new List<List<Enemy>>();      //nov� list list�, takto budu nep��tele reprezentovat (abych mohl dr�et dodate�n� informace o ka�d�m nep��teli)



        private void VytvorEnemies()          //napln�n� list� nep��tel s mezerami a v��m, co bude pot�eba
        {
            for (int row = 0; row < enemyRady; row++)       //pro ka�d� ��dek         
            {
                List<Enemy> enemyRow = new List<Enemy>();       //vytvo��m si list s enemies v �ad�
                for (int col = 0; col < enemySloupce; col++)        //pro ka�d� sloupec
                {
                    if (col == 0)
                    {
                        Enemy enemy = new Enemy();                      //vytvo��m enemy, ur��m x,y a �eknu, �e je �iv� a n�sledn� ho p�id�m do seznamu enemyRow

                        enemy.x = col * 2 * mezera;                         //ZDE P�JDOU DODAT R�ZN� TYPY NEP��TEL
                        enemy.y = row * 2 * mezera;

                        enemy.zije = true;

                        enemy.typ = row + 1;        //toto mi pom��e s ur�en�m typu nep��tel a pozd�ji s vykreslov�n�m, funkce Paint
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
                enemyList.Add(enemyRow);        //ka�dou �adu takto postupn� dod�m do seznamu nep��tel (enemyList)

            }

        }


        private void CollisionTimer_Tick(object sender, EventArgs e)
        {
            CollisionTimer.Interval = 1;
            foreach (List<Enemy> enemyRow in enemyList)         //viz class Enemy a  List<List<Enemy>> enemyList , mus�m proj�d� takto
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)                     //pokud dan� nep��tel �ije, tak kontroluj kolize, jinak logicky ne
                    {

                        Rectangle enemyRect = new Rectangle(enemy.x, enemy.y, rozmerEnemy, rozmerEnemy);                //funkce IntersectsWith pracuje s Rectangles, p�i�lo mi dobr� to ud�lat takto
                        Rectangle bulletRect = new Rectangle(bullet.Left, bullet.Top, bullet.Width, bullet.Height);


                        if (enemyRect.IntersectsWith(bulletRect))           //pokud dojde ke kolizi
                        {
                            bulletTimer.Stop();  //zastav pohyb kulky



                            this.Controls.Add(explo);
                            explo.Location = new Point(enemy.x, enemy.y);

                            efekt.Play();//p�ehraj zvuk z�sahu

                            Exploze.Start(); //start exploze


                            enemy.zije = false;     //enemy um�e (tud� nebude vykreslen)

                            this.Controls.Remove(bullet);       //dej pry� kulku, proto�e zas�hla
                            dopadla = true;             //dopadla, tud� m��e� zas st��let

                            //p�idej sk�re za zabit�ho nep��tele podle typu

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


                            pocetEnemies -= 1;      //ode�ti zabit�ho nep��tele z celkov�ho po�tu

                            //o�et�en�, aby n�kde kulka nez�stala viset
                            bullet.Left = hrac.Left + (rozmerHrace / 2);
                            bullet.Top = hrac.Top + 50;

                            CollisionTimer.Stop();
                            return;                 //p�eru� for loop, kdy� najde� kolizi

                        }
                    }
                }
            }
        }



        private void pohybEnemyRIGHT()
        {
            foreach (List<Enemy> enemyRow in enemyList)         //proj�d�n� enemylistu op�t stejn�m zp�sobem
            {
                foreach (Enemy enemy in enemyRow)
                {
                    enemy.x += rychlostEnemy;       //doprava dle rychlosti
                }
            }
        }



        private void pohybEnemyLEFT()               //analogicky jako p�edchoz� funkce, ale pro posun doleva ode��t�m
        {
            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    enemy.x -= rychlostEnemy;
                }
            }
        }


        private void pohybRady()                        //takt� analogicky, tentokr�t v�ak po y sou�adnic�ch (pro posko�en� p�i dotknut� se hranice)
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
            int rightmostX = rozmerEnemy * 2 * enemySloupce;        //rightmostX a leftmostX kontroluj� nejv�ce pravou a nejv�ce levou sou�adnici
            int leftmostX = rightmostX - rozmerEnemy;                  //to dovoluje kontrolu, zda se zrovna nedot�k�m hranice vlevo �i v pravo

            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)
                    {

                        if (enemy.x > rightmostX)
                        {
                            rightmostX = enemy.x;                   //p�i posunu se mi rightmostX i leftmostX m�n�, proto takto proj�d�m a updatuji
                        }

                        if (enemy.x < leftmostX)
                        {
                            leftmostX = enemy.x;
                        }
                    }
                }
            }


            if (rightmostX >= 1200)    //logicka zm�na bool prom�nn�ch, v tomto p��pad� jel zleva a dotkl se hranice
            {
                lastLeft = false;
                lastRight = true;

                rightBorder = true;
            }

            if (rightBorder)
            {
                pohybRady();            //posu� se tedy o �adu a pak za�ni zprava doleva
                rightBorder = false;
            }



            if (leftmostX <= 0)         //analogicky pro opa�n� sm�r
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


            if (lastRight)          //pokud byla posledn� zm�na po�ad� vpravo, jdi doleva
            {
                pohybEnemyLEFT();
            }

            if (lastLeft)           //analogicky
            {
                pohybEnemyRIGHT();
            }


            //kontrola, zda nep��tel� nejsou na �rovni hr��e

            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)
                    {
                        if (enemy.y >= 600)         //600 zde reprezentuje v��ku hr��e
                        {
                            gameOver();             //viz gameOver();
                            return;             //return, proto�e d�l nechci proj�d�t
                        }

                    }

                }

            }
        }


        int maxEnemyBullets = 3;
        List<PictureBox> enemyBullets = new List<PictureBox>();


        private void enemyTimer_Tick(object sender, EventArgs e)
        {
            // enemyTimer.Interval = 100;        v�c oldschool feel?
            enemyTimer.Interval = 50;

            checkBorders();
            Invalidate();             //s ka�d�m tickem p�eklesuje (bez tohoto by se grafiky neh�bali nep��tel�)


            if (pocetEnemies == 0)          //v�ichni nep��tel� byli zabiti hr��em, v�hra
            {
                isGameOver = true;
            }


            if (pocetEnemies <= pulka)              //zrychlen� 
            {            //P�LKA ENEMIES
                rychlostEnemy = druhaRychlost;
            }

            if (pocetEnemies == 1)          //zb�v� jeden nep��tel, je�t� v�ce zrychli
            {
                rychlostEnemy = finalRychlost;
            }

            if (isGameOver)         //pokud jakkoliv nastane, �e isGameOver = true, tak ukon�i hru (je to zde v enemyTimeru, proto�e ten b�� celou hru)
            {
                gameOver();         //viz gameOver();
            }

            score.Text = "Score: " + skore.ToString();
            en.Text = "Extra Lifes: " + pocetZivotu.ToString();

            enBull();       //vytv��ej kulky
            kontrolaKolize();   //kontroluj kolize



        }


        private void enBull()
        {
            if (enemyBullets.Count < limitEnBul)
            {
                Random random = new Random();                       //pou�it� random na vyhled�n� n�hodn�ho nep��tele

                int randomRowIndex = random.Next(enemyList.Count);      //randomRowIndex

                List<Enemy> randomRow = enemyList[randomRowIndex];          //vybr�n� n�hodn� Row
                int randomEnemyIndex = random.Next(randomRow.Count);

                Enemy randomEnemy = randomRow[randomEnemyIndex];        //z n�hodn� �ady n�hodn� enemy


                PictureBox enemyBullet = new PictureBox();              //vytvo��m novou kulku p��mo zde
                enemyBullet.Size = new Size(5, 5);                          //velikost, barva, tag, pozice
                enemyBullet.BackColor = Color.Yellow;
                enemyBullet.Tag = "enemyBullet";
                enemyBullet.Left = randomEnemy.x;
                enemyBullet.Top = randomEnemy.y;


                Controls.Add(enemyBullet);      //p�idej do Controls
                enemyBullets.Add(enemyBullet);  //p�idej do seznamu kulek k vyst�elen�

            }
        }



        private void kontrolaKolize()           //enemy bullets - hr�� kolize
        {

            for (int i = enemyBullets.Count - 1; i >= 0; i--)       //iteruji p�es v�echny vytvo�en� (let�c�) kulky
            {
                PictureBox enemyBullet = enemyBullets[i];       //pom��e mi s operov�n�m (dispose, pohyb, remove atd.)
                enemyBullet.Top += rychlostEnemyBullet;         //let kulky

                if (enemyBullet.Bounds.IntersectsWith(hrac.Bounds))             //IntersectsWith pro kulku nep��tele a hr��e
                {
                    efekt2.Play();

                    if (pocetZivotu == 0)
                    {
                        deleteBullets();        //pokud u� hr�� nem� �ivoty, konec hry
                        gameOver();

                        return;         //return proto, �e pak nepot�ebuju for loop dokon�ovat
                    }

                    else
                    {
                        stopEverything();       //jinak v�e stopni, hr��ovi napi�, kolik mu zb�v� �ivot� a odstra� let�c� kulky, aby hned nezem�el znovu, pak startni
                        deleteBullets();

                        Controls.Remove(enemyBullet);
                        enemyBullet.Dispose();
                        enemyBullets.Remove(enemyBullet);

                        pocetZivotu--;
                        MessageBox.Show("HIT! " + pocetZivotu.ToString() + " extra lifes left");

                        startEverything();
                    }
                }

                if (enemyBullet.Top > vyska)            //pokud kulka netrefila hr��e a u� let� mimo hrac� pole, odstra� ji (vytvo�� m�sto pro posl�n� dal�� kulky)
                {
                    Controls.Remove(enemyBullet);
                    enemyBullet.Dispose();
                    enemyBullets.Remove(enemyBullet);

                }
            }
        }


        private void deleteBullets()                //funkce, kter� zaru�uje, aby v�echny kulky zmizely a nijak se nepletly
        {
            foreach (PictureBox enemyBullet in enemyBullets)        //nutn� iterovat p�es list bullets
            {
                Controls.Remove(enemyBullet);
                enemyBullet.Dispose();
            }
        }


        protected override void OnPaint(PaintEventArgs e)                       //kresl�c� funkce, vzhledem k tomu, �e enemies nejsou PictureBoxy, tak jsem musel situaci �e�it jinak
        {
            base.OnPaint(e);

            foreach (List<Enemy> enemyRow in enemyList)
            {
                foreach (Enemy enemy in enemyRow)
                {
                    if (enemy.zije)         //pokud �ije, vykresli nep��tele na jeho pozici
                    {
                        if (enemy.typ == 1)
                        {
                            e.Graphics.DrawImage(GalacticOccupiers.Properties.Resources.e4, enemy.x, enemy.y, rozmerEnemy, rozmerEnemy - 20);           //podle typu obr�zek
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



        //UKON�OVAC� FUNKCE, GAMEOVER, PAUZA, RESET, ...

        private void stopEverything()
        {
            enemyTimer.Stop();          //umo�n� pozastaven� p�ed cel�m resetem hry �i pauzou
            bulletTimer.Stop();

            bonusShip.Stop();

        }

        private void startEverything()
        {
            enemyTimer.Start();         //pokud jen pauzuju, chci n�sledn� i pokra�ovat
            bulletTimer.Start();

            if (bonusAlive)
            {
                bonusShip.Start();
            }

        }



        private void pauza()
        {
            stopEverything();               //stopni
            MessageBox.Show(this, "Paused, press OK or ESC to continue..."); //vypi� zpr�vu a po�kej na jej� zav�en�
            startEverything();              //pokra�uj

        }


        private void gameOver()                                                             //STA�� ZAVOLAT GAMEOVER A JE KONEC.
        {
            deleteBullets();            //odstra� a pauzni v�e, co l�t�
            stopEverything();
            MessageBox.Show("Konec hry, chcete hr�t znovu?");   //vysko�� ok�nko

            if (skore > highscore)
            {
                highscore = skore;          //propis sou�asn�ho sk�re do highscore, pokud je v�, ne� highscore p�ed t�m
            }

            hs.Text = "Highscore: " + highscore.ToString();         //zobraz highscore

            reset();                        //viz n�e
        }



        private void reset()
        {
            enemyList.Clear();                  //odstra� v�echny enemies
            this.Controls.Remove(bullet);       //odstra� kulku hr��e

            this.Controls.Remove(shipBonus);

            gameSetup();                        //v�e znovu nastav

        }

        private void hrac_Click(object sender, EventArgs e)
        {

        }

        private void Exploze_Tick(object sender, EventArgs e)
        {
            Exploze.Interval = 500;
            this.Controls.Remove(explo);

        }

        private void timer2_Tick(object sender, EventArgs e)            //bonus ship, jde sest�elit jednou za hru
        {
            bonusShip.Interval = 50;
            this.Controls.Add(shipBonus);

            if (shipBonus.Bounds.IntersectsWith(bullet.Bounds))             //pokud ji hr�� ztrefil
            {
                Random random = new Random();                               //vyber random hodnotu z bonusVal[]

                bonusEfekt.Play();
                bonusAlive = false;


                int randomVal = bonusVal[random.Next(0, bonusVal.Length)];
                skore += randomVal;                 //p�i�ti danou hodnotu
                this.Controls.Remove(shipBonus);

                bonusShip.Stop();

            }

            else if (shipBonus.Left > 0)
            {
                shipBonus.Left -= 20;           //kdy� je v poli, posouvej
            }

            else if (shipBonus.Left <= 0)
            {
                shipBonus.Left = 1300;          //kdy� vyjede z pole
            }


        }

    }
}