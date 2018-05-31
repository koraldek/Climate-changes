using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace dotNETLab6 {

    class Program {
        static void Main(string[] args)
        {
            Scenariusz scenariusz = new Scenariusz();
            scenariusz.rozpocznij();

            Console.ReadKey();
        }

        interface Document {
            void podniesStopienWaznosci();
        }

        class Email : Document {
            int stopienWaznosci;
            String tresc;
            public Email(int stopienWaznosci, String tresc)
            {
                this.stopienWaznosci = stopienWaznosci;
                this.tresc = tresc;
            }
            public void podniesStopienWaznosci()
            {
                stopienWaznosci++;
            }
        }

        class Pismo : Document {
            int stopienWaznosci;
            String tresc;
            public Pismo(int stopienWaznosci, String tresc)
            {
                this.stopienWaznosci = stopienWaznosci;
                this.tresc = tresc;
            }
            public void podniesStopienWaznosci()
            {
                stopienWaznosci++;
            }
        }

        class Scenariusz {
            int IlOpadowDeszczu, ilOpadowSniegu, wiatr, temperatura;
            int sezon = 1;
            private static Random r = new Random();
            ZbiornikRetencyjny zbiornikRetencyjny;
            Elektrownia elektrownia;
            Autostrada autostrada;
            Szpital szpital;
            Lotnisko lotnisko;
            WarunkiAtmosferyczne wAtm;
            public Scenariusz()
            {
                Console.WriteLine("Rozpoczynam scenariusz");
                zbiornikRetencyjny = new ZbiornikRetencyjny(50);
                autostrada = new Autostrada(0, 100);
                szpital = new Szpital();
                lotnisko = new Lotnisko();
                elektrownia = new Elektrownia(szpital, lotnisko);
                wAtm = new WarunkiAtmosferyczne(5, 50, 10, 20, zbiornikRetencyjny, elektrownia, autostrada, szpital, lotnisko);
            }

            public void rozpocznij()
            {
                int sleepTime = 2000;
                Console.WriteLine("Rozpocznij!");
                while (true)
                {
                    sezon++;
                    this.poryRoku(sezon);
                    Console.WriteLine("=======================================================");
                    Console.ReadKey();
                    if (sezon >= 4)
                        sezon = 0;
                }
            }
            private void poryRoku(int sezon)
            {
                StringBuilder sb = new StringBuilder();
                switch (sezon)
                {
                    case 1:
                        {
                            Console.WriteLine("Nadciaga zima.");
                            IlOpadowDeszczu = r.Next(20);
                            ilOpadowSniegu = r.Next(50) + 20;
                            wiatr = r.Next(100);
                            temperatura = r.Next(45) - 35;
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Nadciaga wiosna.");
                            IlOpadowDeszczu = r.Next(30) + 30;
                            ilOpadowSniegu = r.Next(10);
                            wiatr = r.Next(40) + 25;
                            temperatura = r.Next(15) + 10;
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Nadciaga lato.");
                            IlOpadowDeszczu = r.Next(20);
                            ilOpadowSniegu = 0;
                            wiatr = r.Next(35) + 25;
                            temperatura = r.Next(25) + 20;
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("Nadciaga jesien.");
                            IlOpadowDeszczu = r.Next(35) + 40;
                            ilOpadowSniegu = r.Next(50) + 20;
                            wiatr = r.Next(55) + 35;
                            temperatura = r.Next(15) + 10;
                            break;
                        }
                }
                sb.Append("Warunki atmosferyczne:");
                sb.Append("\n\t Opady deszczu: " + IlOpadowDeszczu + "%");
                sb.Append("\n\t Opady sniegu : " + ilOpadowSniegu + "%");
                sb.Append("\n\t Wiatr        : " + wiatr + "%");
                sb.Append("\n\t Temperatura  : " + temperatura + " stopni");
                Console.WriteLine(sb.ToString());

                wAtm.IlOpadowDeszczu = IlOpadowDeszczu;
                wAtm.IlOpadowSniegu = ilOpadowSniegu;
                wAtm.Wiatr = wiatr;
                wAtm.Temperatura = temperatura;
                Console.WriteLine("Pora roku dobiega konca. Opanowano sytuacje w kraju.");
                endSeason();
            }
            private void endSeason()
            {
                zbiornikRetencyjny.setPoziomWody(50);
                elektrownia.WylaczZasilanieAwaryjne();
                szpital.turnOffEmergencyPower();
                lotnisko.otworz();
                Console.WriteLine("\n");
            }
        }
        class WarunkiAtmosferyczne {
            private int ilOpadowDeszczu, ilOpadowSniegu, wiatr, temperatura;
            private int xOpadyDeszczu = 50;
            private int xOpadySniegu = 50;
            private int xWiatr = 50;
            private int xTemperatura = 30;
            public delegate void Action();
            Action delWA;
            public event Action intensywneOpadyDeszczu;
            public event Action intensywneOpadySniegu;
            public event Action silnePodmuchyWiatru;
            public event Action upal;

            public WarunkiAtmosferyczne(int wiatr, int ilOpadowDeszczu, int ilOpadowSniegu, int temperatura, ZbiornikRetencyjny zr, Elektrownia ele, Autostrada aut, Szpital szp, Lotnisko lot)
            {
                this.wiatr = wiatr;
                this.ilOpadowDeszczu = ilOpadowDeszczu;
                this.ilOpadowSniegu = ilOpadowSniegu;
                this.temperatura = temperatura;
                delWA = new Action(zr.zmniejszPoziomWody);
                intensywneOpadyDeszczu += delWA;
                delWA = new Action(aut.wyslijPlugi);
                intensywneOpadySniegu += delWA;
                delWA = new Action(lot.zamknij);
                silnePodmuchyWiatru += delWA;
                delWA = new Action(ele.WlaczZasilanieAwaryjne);
                upal += delWA;
            }

            public int IlOpadowDeszczu
            {
                get { return ilOpadowDeszczu; }
                set
                {
                    if (value >= xOpadyDeszczu)
                        intensywneOpadyDeszczu();
                    ilOpadowDeszczu = value;
                }
            }
            public int IlOpadowSniegu
            {
                get { return ilOpadowSniegu; }
                set
                {
                    if (value >= xOpadySniegu)
                        intensywneOpadySniegu();
                    ilOpadowSniegu = value;
                }
            }
            public int Wiatr
            {
                get { return wiatr; }
                set
                {
                    if (value >= xWiatr)
                        silnePodmuchyWiatru();
                    wiatr = value;
                }
            }
            public int Temperatura
            {
                get { return temperatura; }
                set
                {
                    if (value >= xTemperatura)
                        upal();
                    temperatura = value;
                }
            }

            //public override String ToString()
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append("Warunki atmosferyczne:");
            //    sb.Append("\n\t Opady deszczu: " + IlOpadowDeszczu + "%");
            //    sb.Append("\n\t Opady sniegu : " + ilOpadowSniegu + "%");
            //    sb.Append("\n\t Wiatr        : " + wiatr + "%");
            //    sb.Append("\n\t Temperatura  : " + temperatura + " stopni");
            //    return sb.ToString();
            //}
        }

        class ZbiornikRetencyjny {
            public int poziomWody;

            public ZbiornikRetencyjny(int poziomWody)
            {
                this.poziomWody = poziomWody;
            }
            public void zmniejszPoziomWody()
            {
                Console.WriteLine("Podaj o ile procent obnizyc poziom wody:");
                int lvl = Int32.Parse(Console.ReadLine());
                poziomWody = -lvl;
            }

            public void setPoziomWody(int lvl)
            {
                if (poziomWody != lvl)
                {
                    this.poziomWody = lvl;
                    Console.WriteLine("Poziom wody wyrownany do:" + poziomWody);
                }
            }
        }
        class Elektrownia {
            private bool trybAwaryjny;
            public delegate void Action();
            Action delegata;
            public event Action reakcjaNaElektrownie;
            public Elektrownia(Szpital szp, Lotnisko lot)
            {
                this.trybAwaryjny = false;
                delegata = new Action(szp.turnOnEmergencyPower);
                reakcjaNaElektrownie += delegata;
                delegata = new Action(lot.zamknij);
                reakcjaNaElektrownie += delegata;
            }
            public void WlaczZasilanieAwaryjne()
            {
                if (trybAwaryjny == false)
                {
                    Console.WriteLine("Wlaczono tryb awaryjny elektrowni.");
                    this.trybAwaryjny = true;
                }
                reakcjaNaElektrownie();
            }
            public void WylaczZasilanieAwaryjne()
            {
                if (trybAwaryjny == true)
                {
                    Console.WriteLine("Wylaczono tryb awaryjny elektrowni.");
                    this.trybAwaryjny = false;
                }
            }
        }
        class Autostrada {
            int lPlugow, lDostepnychplugow;
            public delegate String Action();
            Action delegata;
            public event Action brakujePlugow;
            public Autostrada(int lPlugow, int lDostepnychplugow)
            {
                this.lPlugow = lPlugow;
                this.lDostepnychplugow = lDostepnychplugow;
                delegata = new Action(this.ostrzezenie);
                brakujePlugow += delegata;
            }

            public void wyslijPlugi()
            {
                Console.WriteLine("Podaj ilosc plugow do wyslania na autostrade i liczbe dostepnych plugow po \";\":");
                String[] input = Console.ReadLine().Split(';');
                lPlugow = Int32.Parse(input[0]);
                lDostepnychplugow = Int32.Parse(input[1]);
                if (lPlugow > lDostepnychplugow)
                    Console.WriteLine(brakujePlugow());

            }
            public String ostrzezenie()
            {
                lPlugow = lDostepnychplugow;
                return "Dostepnych jest " + lDostepnychplugow + ". Wszystkie wyslano na autostrady.";
            }

        }
        class Szpital {
            bool generatoryPradu;

            public Szpital()
            {
                this.generatoryPradu = false;
            }
            public void turnOnEmergencyPower()
            {
                Console.WriteLine("Wlaczam awaryjne zasilanie w szpitalu.");
                this.generatoryPradu = true;
            }
            public void turnOffEmergencyPower()
            {
                if (generatoryPradu == true)
                {
                    Console.WriteLine("Wylaczam awaryjne zasilanie w szpitalu.");
                    this.generatoryPradu = false;
                }
            }
        }
        class Lotnisko {
            bool otwarte;
            public Lotnisko()
            {
                this.otwarte = true;
            }
            public void otworz()
            {
                if (otwarte == false)
                {
                    Console.WriteLine("Otwarto lotnisko.");
                    this.otwarte = true;
                }
            }
            public void zamknij()
            {
                if (otwarte != false)
                {
                    Console.WriteLine("Zamknieto lotnisko.");
                    this.otwarte = false;
                }
            }
        }
    }
}