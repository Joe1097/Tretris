using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tretris // v1.0.0
{
    // Tamaño del canvas(en matriz): 10x18 
    // Tamaño de un cudarito: 40x40 pixeles
    // Niveles: 7 (En el original hay 20, creo. Se pueden hacer mas niveles facilmente)
    // Bugs: 1 xD
    public partial class tetris : Form
    {
        Panel[,] Panels = new Panel[10, 18]; // Matriz donde se guardarán los paneles que se vayan creando 
        Panel[] O, l, S, Z, L, J, T;  // Conjunto de los 4 paneles(o cuadritos) que tiene cada figura
        Panel Pa, Pb, Pc, Pd;          // Paneles virtuales auxiliares para manipular los reales
        bool[,] tet = new bool[12, 20]; // Tamaño elejido para que una capa cubra el canvas
        byte[] vec = new byte[18];         // En este vector guardo la cantidad de paneles que hay en cada fila de la matriz de paneles (Panels)
        byte o_i, o_s, o_z, o_l, o_j, o_t;  // Orientacion de i, orientacion de s, orientacion de z...
        int r, r2;                         // Entero para asignarle un valor random(del 1 al 7) para elegir al alzar una figura 
        int score, scpls, lines;           // Entero con el puntaje, puntaje que se suma cuando se coloca una figura, numero de lineas
        bool paused = false;               // booleano para saber si esta en pausa o no

        public tetris()
        {
            InitializeComponent();
            O = new Panel[4];        
            l = new Panel[4];
            S = new Panel[4];
            Z = new Panel[4];
            L = new Panel[4];
            J = new Panel[4];
            T = new Panel[4];
            Pa = new Panel();
            Pb = new Panel();
            Pc = new Panel();
            Pd = new Panel();
            panel4.Visible = false;    // Esconde el boton de pausa antes de empezar el juego
        }

        private void new_game(object sender, EventArgs e)
        {
            panel6.BorderStyle = BorderStyle.Fixed3D;
            System.Threading.Thread.Sleep(100);
            panel6.BorderStyle = BorderStyle.FixedSingle;
            System.Threading.Thread.Sleep(100);            // Hasta aqui es para hacer la animacion de presionar un boton en el panel de "NEW GAME"
            canvas.Controls.Clear();
            canvas2.Controls.Clear();                      // Limpiar "canvas" y "canvas2" de los panels de colores del juego anterior 
            start_values();             
            paused = false;                                // Con esto, si el juego es reiniciado cuando estaba en pausa entonces dejara de estar pausado
            panel4.BorderStyle = BorderStyle.FixedSingle;  // Vuelve a la normalidad el aspecto del boton "PAUSE"
            canvas.Visible = true;                         // Cuando inicia el juego "canvas" se hara visible
            canvas2.Visible = true;
            panel4.Visible = true;                         // igual todo lo demas del boton de pausa
            label2.Visible = true;
            label5.Visible = false;
            Random rdn = new Random();                     // Genera por primera vez el numero random que corresponderá al numero de figura para crear
            r = rdn.Next(1, 8);
            create_n_down();                               // Llama la funcion para crear y bajar la figura
        }         // Funcion para empezar el juego

        private void start_values()
        {
            scpls = 18;                 
            score = 0;
            lscore.Text = "00";
            scpls = 18;
            llevel.Text = "01";
            lines = 0;
            llines.Text = "00";
            timer1.Interval = 500;

            for (int i = 0; i < 12; i++) // Asignandole el valor falso a la parte central de la matriz "tet"
            {
                for (int j = 0; j < 20; j++)
                {
                    tet[i, j] = false;
                }
            }
            for (int i = 0; i < 12; i++)  // Y estos cuatro for son los de la orilla de "tet" y se ocupara que sean true
            {
                tet[i, 0] = true;
            }
            for (int i = 0; i < 12; i++)
            {
                tet[i, 19] = true;
            }
            for (int i = 0; i < 20; i++)
            {
                tet[0, i] = true;
            }
            for (int i = 0; i < 20; i++)
            {
                tet[11, i] = true;
            }

            for (int i = 0; i < 10; i++)  // Cada posicion de la matriz "Panels" se hace nulo
            {
                for (int j = 0; j < 18; j++)
                {
                    Panels[i, j] = null;
                }
            }

            for (int i = 0; i < 18; i++)  // Cada posicion en el vector se cambia a cero
            {
                vec[i] = 0;
            }
        }      // Valores asignados al empezar un nuevo juego como los de: "tet", "Panels", "puntaje", etc 

        private void create_n_down()
        {
            switch (r)      // Crea la figura y la coloca en "canvas"
            {
                case 1: create_o(); break;
                case 2: create_i(); break;
                case 3: create_s(); break;
                case 4: create_z(); break;
                case 5: create_l(); break;
                case 6: create_j(); break;
                default: create_t(); break;
            }
            canvas2.Controls.Clear();  // Cada que empieza esta funcion, se limpia canvas2
            Random rdn = new Random(); // Crea elnumero random que correspondera al la figura que sera utilizada en la siguiente vez
            r2 = rdn.Next(1, 8);
            switch (r2)                 // Crea la figura y la coloca en "canvas2"
            {
                case 1: create_o2(); break;
                case 2: create_i2(); break;
                case 3: create_s2(); break;
                case 4: create_z2(); break;
                case 5: create_l2(); break;
                case 6: create_j2(); break;
                default: create_t2(); break;
            }
            if (Panels[4,1]==null)    // Mientras esa posicion de Panels no sea ocupada significara que no ha perdidio y llamará a "timer1"
            {
                timer1.Start();  // Nota: La ejecucion del programa es paralela al timer si hubiera utilizado System.Threading.Thread.Sleep(10000);   se pausaria todo, hasta el timer    
            }
            else                // Aquí ya habrá perdido
            {
                panel4.Visible = false;
                MessageBox.Show("Your score: " + score.ToString(), "Game Over");
            } 
        }     // Funcion para crear una figura y hacerla bajar con el timer

        private void create_o()
        {
            for (int i = 0; i < 4; i++)                            // for para crear cuatro paneles
            {
                var newPanel = new Panel();                        // Variable que llame "newPanel" es igual a "new Panel()"
                newPanel.Width = 40;                               // Ancho del panel
                newPanel.Height = 40;                              // Largo del panel
                newPanel.BorderStyle = BorderStyle.FixedSingle;    // Estilo de borde
                newPanel.Name = i.ToString();                      // Le asigno un nombre que es igual al contador (no se si sea necesario v:)
                newPanel.BackColor = System.Drawing.Color.Yellow;  // Color cuidadosamente elegido 
                O[i] = newPanel;                                   // El vector de paneles auxiliar en la posicion i va a contener el panel creado
            }
            int a = 160, b = 40;                                   // Ubicacion del primer panel creado
            for (int i = 0; i < 4; i++)
            {
                O[i].Left = a;
                O[i].Top = b;
                canvas.Controls.Add(O[i]);                         // Este es para que se vea el panel en "canavas"
                if (a == 160)
                    a += 40;
                else
                {
                    a = 160;
                    b += 40;
                }
            }                                                       // Este ciclo ubica el primer panel, en lado derecho el segundo, abajo a la izquierda el tercero y al lado derecho del tercero ubica el cuarto 
            Pa = O[0]; Pb = O[1]; Pc = O[2]; Pd = O[3];             // Paneles auxiliares que se les asigna uno del vector para que estos sean manipulados en todo el programa
        }          // Crear figura cuadrada

        private void create_i()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Aqua;
                l[i] = newPanel;
            }
            int x = 120;
            for (int i = 0; i < 4; i++)
            {
                l[i].Left = x;
                l[i].Top = 40;
                canvas.Controls.Add(l[i]);
                x += 40;
            }
            o_i = 1;                                     
            Pa = l[0]; Pb = l[1]; Pc = l[2]; Pd = l[3]; 
        }          // Crear figura larga

        private void create_s()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Red;
                S[i] = newPanel;
            }
            S[0].Left = 120; S[0].Top = 80; canvas.Controls.Add(S[0]);
            S[1].Left = 160; S[1].Top = 80; canvas.Controls.Add(S[1]);
            S[2].Left = 160; S[2].Top = 40; canvas.Controls.Add(S[2]);
            S[3].Left = 200; S[3].Top = 40; canvas.Controls.Add(S[3]);
            o_s = 1;
            Pa = S[0]; Pb = S[1]; Pc = S[2]; Pd = S[3]; 
        }          // "" "" ""

        private void create_z()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.LimeGreen;
                Z[i] = newPanel;
            }
            Z[0].Left = 120; Z[0].Top = 40; canvas.Controls.Add(Z[0]);
            Z[1].Left = 160; Z[1].Top = 40; canvas.Controls.Add(Z[1]);
            Z[2].Left = 160; Z[2].Top = 80; canvas.Controls.Add(Z[2]);
            Z[3].Left = 200; Z[3].Top = 80; canvas.Controls.Add(Z[3]);
            o_z = 1;
            Pa = Z[0]; Pb = Z[1]; Pc = Z[2]; Pd = Z[3]; 

        }          // "" "" ""

        private void create_j()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Fuchsia;
                J[i] = newPanel;
            }
            J[0].Left = 120; J[0].Top = 40; canvas.Controls.Add(J[0]);
            J[1].Left = 160; J[1].Top = 40; canvas.Controls.Add(J[1]);
            J[2].Left = 200; J[2].Top = 40; canvas.Controls.Add(J[2]);
            J[3].Left = 200; J[3].Top = 80; canvas.Controls.Add(J[3]);
            o_j = 1;
            Pa = J[0]; Pb = J[1]; Pc = J[2]; Pd = J[3];
        }          // ...

        private void create_l()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Orange;
                L[i] = newPanel;
            }
            L[0].Left = 120; L[0].Top = 80; canvas.Controls.Add(L[0]);
            L[1].Left = 120; L[1].Top = 40; canvas.Controls.Add(L[1]);
            L[2].Left = 160; L[2].Top = 40; canvas.Controls.Add(L[2]);
            L[3].Left = 200; L[3].Top = 40; canvas.Controls.Add(L[3]);
            o_l = 1;
            Pa = L[0]; Pb = L[1]; Pc = L[2]; Pd = L[3];           
        }      

        private void create_t()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.BlueViolet;
                T[i] = newPanel;
            }
            T[0].Left = 160; T[0].Top = 80; canvas.Controls.Add(T[0]);
            T[1].Left = 120; T[1].Top = 40; canvas.Controls.Add(T[1]);
            T[2].Left = 160; T[2].Top = 40; canvas.Controls.Add(T[2]);
            T[3].Left = 200; T[3].Top = 40; canvas.Controls.Add(T[3]);
            o_t = 1;
            Pa = T[0]; Pb = T[1]; Pc = T[2]; Pd = T[3]; 
        }

        private void create_o2()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Yellow;
                O[i] = newPanel;
            }
            int a = 40, b = 40;
            for (int i = 0; i < 4; i++)
            {
                O[i].Left = a;
                O[i].Top = b;
                canvas2.Controls.Add(O[i]);
                if (a == 40)
                    a += 40;
                else
                {
                    a = 40;
                    b += 40;
                }
            }
        }         // Estos eventos Crean las mismas figuras pero en "canvas2" (El panel que muestra la siguiente figura)

        private void create_i2()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Aqua;
                l[i] = newPanel;
            }
            int x = 0;
            for (int i = 0; i < 4; i++)
            {
                l[i].Left = x;
                l[i].Top = 40;
                canvas2.Controls.Add(l[i]);
                x += 40;
            }
        }

        private void create_s2()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Red;
                S[i] = newPanel;
            }
            S[0].Left = 0; S[0].Top = 80; canvas2.Controls.Add(S[0]);
            S[1].Left = 40; S[1].Top = 80; canvas2.Controls.Add(S[1]);
            S[2].Left = 40; S[2].Top = 40; canvas2.Controls.Add(S[2]);
            S[3].Left = 80; S[3].Top = 40; canvas2.Controls.Add(S[3]);
        }

        private void create_z2()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.LimeGreen;
                Z[i] = newPanel;
            }
            Z[0].Left = 0; Z[0].Top = 40; canvas2.Controls.Add(Z[0]);
            Z[1].Left = 40; Z[1].Top = 40; canvas2.Controls.Add(Z[1]);
            Z[2].Left = 40; Z[2].Top = 80; canvas2.Controls.Add(Z[2]);
            Z[3].Left = 80; Z[3].Top = 80; canvas2.Controls.Add(Z[3]);
        }

        private void create_j2()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Fuchsia;
                J[i] = newPanel;
            }
            J[0].Left = 0; J[0].Top = 40; canvas2.Controls.Add(J[0]);
            J[1].Left = 40; J[1].Top = 40; canvas2.Controls.Add(J[1]);
            J[2].Left = 80; J[2].Top = 40; canvas2.Controls.Add(J[2]);
            J[3].Left = 80; J[3].Top = 80; canvas2.Controls.Add(J[3]);
        }

        private void create_l2()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.Orange;
                L[i] = newPanel;
            }
            L[0].Left = 0; L[0].Top = 80; canvas2.Controls.Add(L[0]);
            L[1].Left = 0; L[1].Top = 40; canvas2.Controls.Add(L[1]);
            L[2].Left = 40; L[2].Top = 40; canvas2.Controls.Add(L[2]);
            L[3].Left = 80; L[3].Top = 40; canvas2.Controls.Add(L[3]);
        }

        private void create_t2()
        {
            for (int i = 0; i < 4; i++)
            {
                var newPanel = new Panel();
                newPanel.Width = 40;
                newPanel.Height = 40;
                newPanel.BorderStyle = BorderStyle.FixedSingle;
                newPanel.Name = i.ToString();
                newPanel.BackColor = System.Drawing.Color.BlueViolet;
                T[i] = newPanel;
            }
            T[0].Left = 40; T[0].Top = 80; canvas2.Controls.Add(T[0]);
            T[1].Left = 00; T[1].Top = 40; canvas2.Controls.Add(T[1]);
            T[2].Left = 40; T[2].Top = 40; canvas2.Controls.Add(T[2]);
            T[3].Left = 80; T[3].Top = 40; canvas2.Controls.Add(T[3]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((tet[Pa.Left / 40 + 1, (Pa.Top / 40) + 2] != true) && (tet[Pb.Left / 40 + 1, (Pb.Top / 40) + 2] != true) && (tet[Pc.Left / 40 + 1, (Pc.Top / 40) + 2] != true) && (tet[Pd.Left / 40 + 1, (Pd.Top / 40) + 2] != true))   // En pocas palabras, si abajo de cada panel auxiliar no hay hay nada entonces seguira bajando
            {
                Pa.Top += 40; Pb.Top += 40; Pc.Top += 40; Pd.Top += 40;        // A la cordenada "y" de cada panel auxiliar le aumenta 40 pixeles cada medio segundo 
            }
            else            // Cuando llega hasta abajo la figura...
            {       
                score += scpls;                                       // aumenta el puntaje,
                lscore.Text = score.ToString();                       // se muestra el puntaje en la pantalla,
                tet[Pa.Left / 40 + 1, Pa.Top / 40 + 1] = true;        // y en la matriz boolena "tet" se cabian los valores a verdaderos en las posiciones deonde se ubicaron los paneles de la figura en "canvas"
                tet[Pb.Left / 40 + 1, Pb.Top / 40 + 1] = true;        
                tet[Pc.Left / 40 + 1, Pc.Top / 40 + 1] = true;
                tet[Pd.Left / 40 + 1, Pd.Top / 40 + 1] = true;        // Cada coordenada "Left" y "Top" de cada panel se divide entre 40 para que los indices de tet coincidan, y ademas se agrega +1 por que tet abarca 1 fila/columna mas arededor de la cuadricula que corresponde al lo que se ve
                Panels[Pa.Left / 40, Pa.Top / 40] = Pa;
                Panels[Pb.Left / 40, Pb.Top / 40] = Pb;
                Panels[Pc.Left / 40, Pc.Top / 40] = Pc;
                Panels[Pd.Left / 40, Pd.Top / 40] = Pd;               // A la matriz de paneles se le asignan los panels que acaban de bajar en sus respectivas posiciones dentro de la matriz
                delete_n_down();                                      // Llama a esta funcion para checar si ya hay una fila llena
                level_up();                                           // El nivel aumenta con cada cierta cantidad de lineas 
                timer1.Stop();                                        // El timer se detiene por que la figura esta abajo, pero el código continua
                r = r2;                                               // El numero random r2(creado antes en la funcion create_n_down) pasa a ser r en el siguiente llmada a la funcion "create_n_down" 
                create_n_down();                                      // El timer1.Stop() va antes por que create_n_down crea una figura y activa el timer el cual seria detenido por el timer1.Stop(); antes de que pueda moverse            
            }
        }   // Función timer para desplazar la figura cada cierto tiempo 

        private void delete_n_down()
        {
            vec[Pa.Top / 40]++; vec[Pb.Top / 40]++; vec[Pc.Top / 40]++; vec[Pd.Top / 40]++;  // En cada uno de los vectores que determinan cuantos paneles hay en cada fila se suma 1 por cada panel que va llegando 
            for (int i = 0; i < 18; i++)   // Este for revisa cada uno de esos vectores
            {                 
                if(vec[i]==10)             // para ver si alguno llego a los 10 paneles
                {
                    for (int j = 0; j < 10; j++)             
                    {
                        canvas.Controls.Remove(Panels[j, i]);   // y si es asi los elimina de "canvas",
                        Panels[j, i] = null;                    // de "Panels",
                        tet[j + 1, i + 1] = false;              // sus posiciones en "tet" cambian a false,
                    }
                    score += 40;                                // cada linea(10 paneles) eliminada aumenta 40 puntos,
                    lscore.Text = score.ToString();                   
                    lines++;                                    // el numero de lineas aumenta,
                    llines.Text = lines.ToString();
                    vec[i] = 0;                                 // cuando se eliminan la linea tambien debe disminuir el numero de paneles que cuenta el vector en la posicion de la linea eliminada
         /*         for (int n = 0; n < i; n++) 
                    {
                        for (int m = 0; m < 10; m++) 
                        {
                            if(Panels[m,n]!=null)
                            {
                                tet[Panels[m, n].Left / 40 + 1, Panels[m, n].Top / 40 + 1] = false;
                                Panels[m, n].Top += 40;
                                tet[Panels[m, n].Left / 40 + 1, Panels[m, n].Top / 40 + 1] = true;
                            }
                        }
                    }     */                                   // Nota: No se por que eso no funcionaba ahi

                    for (int n = i - 1; n >= 0; n--)     // Despues se desplazan hacia abajo todos los demas paneles que estabam arriba de la linea eliminada  
                    {                                    // empezando desde la fila que se elimino menos 1
                        for (int m = 0; m < 10; m++)     // de izquierda a derecha
                        {
                            if (Panels[m, n] != null)    // revisando en "Panels" cuales son los que se van a desplazar
                            {
                                tet[Panels[m, n].Left / 40 + 1, Panels[m, n].Top / 40 + 1] = false;    // la posicion de cada panel en "tet" cambia a false,
                                Panels[m, n].Top += 40;                                                // se desplaza 40 pixeles,
                                tet[Panels[m, n].Left / 40 + 1, Panels[m, n].Top / 40 + 1] = true;     // la nueva posicion del panel en "tet" se hace true,
                                Panels[m, n + 1] = Panels[m, n];                                       // se hace lo mismo pero en la matriz "Panels" osea que el panel que va en la nueva posicion sera igual que el de la posicion inicial
                                Panels[m, n] = null;                                                   // se hace null la posicion inicial por que ya esta abajo el panel
                                vec[n + 1]++;                                                          // y por cada panel que se desplaza en se disminuye el numero de panles en la fila en la que estaba
                                vec[n]--;                                                              // pero aumenta el numero de paneles en lo de abajo
                            }
                        }
                    }
                }                
            }
        }              // Función que elimina una fila cuando está llena de paneles y desplaza los que estan arriba de ella

        private void level_up()
        {
            if((lines>=20)&&(lines<40))
            {
                timer1.Interval = 450; // cada que aumenta un nivel dimsinuye el intervalo del timer
                llevel.Text = "02";
            }
            else if((lines>=40)&&(lines<60))
            {
                timer1.Interval = 400;
                llevel.Text = "03";
            }
            else if ((lines >= 60) && (lines < 80))
            {
                timer1.Interval = 350;
                llevel.Text = "04";
            }
            else if ((lines >= 80) && (lines < 100))
            {
                scpls = 17;
                timer1.Interval = 300;
                llevel.Text = "05";
            }
            else if((lines >= 100) && (lines < 120))
            {
                timer1.Interval = 275;
                llevel.Text = "06";
            }
            else if((lines >= 120) && (lines < 150))
            {
                timer1.Interval = 250;
                llevel.Text = "07";
            }
            else if (lines>=150)
            {
                MessageBox.Show("You win!\n\rYour score: " + score.ToString(), "CONGRATULATIONS!!!");
                timer1.Stop();
            }
        }           //Función para aumentar de nivel

        private void keyboard(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Right:
                    rigth();
                    break;
                case Keys.Left:
                    left();
                    break;
                case Keys.Up:
                    rotate_fig();
                    break; 
                case Keys.Down:
                    down();
                    break;
            }
        }    // Función paramanipular la figura segun la teckla que se presione

        private void left()
        {
            if ((tet[Pa.Left / 40, Pa.Top / 40 + 1] != true) && (tet[Pb.Left / 40, (Pb.Top / 40) + 1] != true) && (tet[Pc.Left / 40, (Pc.Top / 40) + 1] != true) && (tet[Pd.Left / 40, (Pd.Top / 40) + 1] != true)) // Este if checa que no este ningun otro panel a la izquierda de cada panel
            {
                Pa.Left -= 40;
                Pb.Left -= 40;
                Pc.Left -= 40;
                Pd.Left -= 40;
            }
        }      // Desplaza hacia la izquierda la figura
         
        private void rigth()
        {
            if ((tet[Pa.Left / 40 + 2, (Pa.Top / 40) + 1] != true) && (tet[Pb.Left / 40 + 2, (Pb.Top / 40) + 1] != true) && (tet[Pc.Left / 40 + 2, (Pc.Top / 40) + 1] != true) && (tet[Pd.Left / 40 + 2, (Pd.Top / 40) + 1] != true))
            {
                Pa.Left += 40;
                Pb.Left += 40;
                Pc.Left += 40;
                Pd.Left += 40;
            }
        }     // Derecha

        private void down()
        {
            if ((tet[Pa.Left / 40 + 1, (Pa.Top / 40) + 2] != true) && (tet[Pb.Left / 40 + 1, (Pb.Top / 40) + 2] != true) && (tet[Pc.Left / 40 + 1, (Pc.Top / 40) + 2] != true) && (tet[Pd.Left / 40 + 1, (Pd.Top / 40) + 2] != true))
            {
                Pa.Top += 40; Pb.Top += 40; Pc.Top += 40; Pd.Top += 40;
            }
        }      // Abajo

        private void rotate_fig()
        {
            switch (r)
            {
                case 1:
                    /* Ps no hace nada, solo es un cuadrado, quedaria igual */
                    break;
                case 2:
                    if (o_i == 1) // Determinar la orientacion en la que se encuentra la figiura
                    {
                        if ((tet[Pb.Left / 40 + 1, (Pb.Top - 40) / 40 + 1] != true) && (tet[Pb.Left / 40 + 1, (Pb.Top + 40) / 40 + 1] != true) && (tet[Pb.Left / 40 + 1, (Pb.Top + 80) / 40 + 1] != true))  // Determinar si no hay interferencia de otros paneles al rotar la figura
                        {
                            if (Pb.Top <= canvas.Height - 80)  //  Determinar si la posicion en la que va a quedar la figura va a estar dentro del "tablero" en este caso el limite horizontal
                            {
                                Pa.Top = Pb.Top - 40; Pc.Top = Pb.Top + 40; Pd.Top = Pb.Top + 80;       
                                Pa.Left = Pb.Left; Pc.Left = Pb.Left; Pd.Left = Pb.Left;            // ubicacion de las figuras en relacion a su panel central 
                                o_i = 2;    // este entero va a cambiando según la orientacion que tenga la figura
                            }
                        }
                    }
                    else if (o_i == 2) 
                    {
                        if ((tet[(Pb.Left - 40) / 40 + 1, Pb.Top / 40 + 1] != true) && (tet[(Pb.Left + 40) / 40 + 1, Pb.Top / 40 + 1] != true) && (tet[(Pb.Left + 80) / 40 + 1, Pb.Top / 40 + 1] != true))  
                        {
                            if (Pb.Left <= canvas.Width - 120)
                            {
                                Pa.Top = Pb.Top; Pc.Top = Pb.Top; Pd.Top = Pb.Top;
                                Pa.Left = Pb.Left - 40; Pc.Left = Pb.Left + 40; Pd.Left = Pb.Left + 80;
                                o_i = 1;
                            }
                        }
                    } 
                    break;
                case 3:
                    if(o_s==1)
                    {
                        if ((tet[Pc.Left / 40 + 1, (Pc.Top - 40) / 40 + 1] != true) && (tet[(Pc.Left + 40) / 40 - 1, (Pc.Top + 40) / 40 - 1] != true))  
                        {
                            Pa.Left = Pc.Left + 40;
                            Pb.Top = Pc.Top - 40;
                            o_s = 2;
                        }
                    }
                    else if(o_s==2)
                    {
                        if ((tet[(Pc.Left - 40) / 40 + 1, (Pc.Top + 40) / 40 + 1] != true) && (tet[Pc.Left / 40 + 1, (Pc.Top - 40) / 40 + 1] != true)) 
                        {
                            Pa.Left = Pc.Left - 40;
                            Pb.Top = Pc.Top + 40;
                            o_s = 1;
                        }
                    }
                    break;
                case 4:
                    if(o_z==1)
                    {
                        if ((tet[(Pb.Left + 40) / 40 + 1,(Pb.Top - 40) / 40 + 1 ] != true) && (tet[(Pb.Left + 40) / 40 + 1, Pb.Top / 40 + 1] != true)) 
                        {
                            Pa.Left += 80;
                            Pd.Top -= 80;
                            o_z = 2;
                        }
                    }
                    else if(o_z==2)
                    {
                        if ((tet[(Pb.Left - 40) / 40 + 1, Pb.Top / 40 + 1] != true) && (tet[(Pb.Left + 40) / 40 + 1, (Pb.Top + 40) / 40 + 1] != true)) 
                        {
                            Pa.Left -= 80;
                            Pd.Top += 80;
                            o_z = 1;
                        }
                    }
                    break;
                case 5:
                    if(o_l==1)
                    {
                        if ((tet[(Pc.Left - 40) / 40 + 1, Pc.Top / 40 + 1] != true) && (tet[(Pc.Left - 40) / 40 + 1, (Pc.Top - 40) / 40 + 1] != true) && (tet[Pc.Left / 40 + 1, (Pc.Top + 40) / 40 + 1] != true))
                        {
                            Pa.Top -= 80; Pb.Left += 40; Pd.Left -= 40;
                            Pb.Top -= 40; Pd.Top += 40;
                            o_l = 2;
                        }
                    }
                    else if(o_l==2)
                    {
                        if ((tet[(Pc.Left - 40) / 40 + 1, Pc.Top / 40 + 1] != true) && (tet[(Pc.Left + 40) / 40 + 1, Pc.Top / 40 + 1] != true) && (tet[(Pc.Left + 40) / 40 + 1, (Pc.Top - 40) / 40 + 1] != true))
                        {
                            Pa.Left += 80; Pb.Left += 40; Pd.Left -= 40;
                            Pb.Top += 40; Pd.Top -= 40;
                            o_l = 3;
                        }
                    }
                    else if(o_l==3)
                    {
                        if ((tet[Pc.Left / 40 + 1, (Pc.Top - 40) / 40 + 1] != true) && (tet[Pc.Left / 40 + 1, (Pc.Top + 40) / 40 + 1] != true) && (tet[(Pc.Left + 40) / 40 + 1, (Pc.Top + 40) / 40 + 1] != true)) 
                        {
                            Pb.Left -= 40; Pd.Left += 40;
                            Pa.Top += 80; Pb.Top += 40; Pd.Top -= 40;
                            o_l = 4;
                        }
                    }
                    else if(o_l==4)
                    {
                        if ((tet[(Pc.Left - 40) / 40 + 1, (Pc.Top + 40) / 40 + 1] != true) && (tet[(Pc.Left - 40) / 40 + 1, Pc.Top / 40 + 1] != true) && (tet[(Pc.Left + 40) / 40 + 1, Pc.Top / 40 + 1] != true)) 
                        {
                            Pa.Left -= 80; Pb.Left -= 40; Pd.Left += 40;
                            Pb.Top -= 40; Pd.Top += 40;
                            o_l = 1;
                        }
                    }
                    break;
                case 6:
                    if(o_j==1)
                    {
                        if ((tet[Pb.Left / 40 + 1,(Pb.Top-40)  / 40 + 1] != true) && (tet[Pb.Left / 40 + 1,(Pb.Top+40)  / 40 + 1] != true) && (tet[(Pb.Left-40) / 40 + 1,(Pb.Top+40)  / 40 + 1] != true))
                        {
                            Pa.Left += 40; Pc.Left -=40; Pd.Left -= 80;
                            Pa.Top -= 40; Pc.Top += 40;
                            o_j = 2;
                        }
                    }
                    else if(o_j==2)
                    {
                        if ((tet[(Pb.Left + 40) / 40 + 1, Pb.Top / 40 + 1] != true) && (tet[(Pb.Left - 40) / 40 + 1, Pb.Top / 40 + 1] != true) && (tet[(Pb.Left - 40) / 40 + 1, (Pb.Top - 40) / 40 + 1] != true))
                        {
                            Pa.Left += 40;Pc.Left -= 40;Pd.Top -= 80;
                            Pa.Top += 40;Pc.Top -= 40;
                            o_j = 3;
                        }
                    }
                    else if(o_j==3)
                    {
                        if ((tet[Pb.Left / 40 + 1, (Pb.Top + 40) / 40 + 1] != true) && (tet[Pb.Left / 40 + 1, (Pb.Top - 40) / 40 + 1] != true) && (tet[(Pb.Left + 40) / 40 + 1, (Pb.Top - 40) / 40 + 1] != true)) 
                        {
                            Pa.Left -= 40;Pc.Left += 40;Pd.Left += 80;
                            Pa.Top += 40;Pc.Top -= 40;
                            o_j = 4;
                        }
                    }
                    else if(o_j==4)
                    {
                        if ((tet[(Pb.Left-40) / 40 + 1, Pb.Top / 40 + 1] != true) && (tet[(Pb.Left+40) / 40 + 1, Pb.Top  / 40 + 1] != true) && (tet[(Pb.Left + 40) / 40 + 1, (Pb.Top + 40) / 40 + 1] != true)) 
                        {
                            Pa.Left -= 40;Pc.Left += 40;Pd.Top += 80;
                            Pa.Top -= 40;Pc.Top += 40;
                            o_j = 1;
                        }
                    }
                    break;
                default:
                    if(o_t==1)
                    {
                        if (tet[Pc.Left / 40 + 1, (Pc.Top-40) / 40 + 1] != true)
                        {
                            Pb.Left += 40;Pa.Left -= 40;Pd.Left -= 40;
                            Pb.Top -= 40;Pa.Top -= 40;Pd.Top += 40;
                            o_t = 2;
                        }
                    }
                    else if(o_t==2)
                    {
                        if (tet[(Pc.Left+40) / 40 + 1, Pc.Top / 40 + 1] != true)
                        {
                            Pb.Left += 40;Pd.Left -= 40;Pa.Left += 40;
                            Pb.Top += 40;Pd.Top -= 40;Pa.Top -= 40;
                            o_t = 3;
                        }
                    }
                    else if(o_t==3)
                    {
                        if (tet[Pc.Left / 40 + 1, (Pc.Top+40) / 40 + 1] != true)
                        {
                            Pb.Left -= 40;Pa.Left += 40;Pd.Left += 40;
                            Pb.Top += 40;Pa.Top += 40;Pd.Top -= 40;
                            o_t = 4;
                        }
                    }
                    else if(o_t==4)
                    {
                        if (tet[(Pc.Left - 40) / 40 + 1, Pc.Top / 40 + 1] != true)
                        {
                            Pb.Left -= 40;Pa.Left -= 40;Pd.Left += 40;
                            Pb.Top -= 40;Pa.Top += 40;Pd.Top += 40;
                            o_t = 1;
                        }
                    }
                    break;                           // Si, es un buen haha 
            }
        }    // Funcion para rotar la figura

        private void Pause(object sender, EventArgs e)
        {
            if (paused == false)      // si no estaba pausado el juego...
            {
                System.Threading.Thread.Sleep(100);
                panel4.BorderStyle = BorderStyle.Fixed3D;     // Animacion de boton presionado
                timer1.Enabled = false;                       // El timer se detiene
                paused = true;                                // el booleano pause cambia a true
                canvas.Visible = false;
                canvas2.Visible = false;                      // esconde los canvas
                label5.Visible = true;
                label2.Visible = false;                       // y cambia el aspecto del label
            }
            else                     // si estaba pausado el juego...
            {
                System.Threading.Thread.Sleep(100);
                panel4.BorderStyle = BorderStyle.FixedSingle;    // Animacion de boton soltado
                timer1.Enabled = true;                           // El timer se activa de nuevo
                paused = false;                                  // el booleano pause cambia a false
                canvas.Visible = true;
                canvas2.Visible = true;                          // muestra de nuevo los canvas
                label2.Visible = true;
                label5.Visible = false;                          // y cambia el aspecto del label de nuevo
            }
        }    // Funcion para pausar el juego

        private void exit(object sender, EventArgs e)
        {
            panel5.BorderStyle = BorderStyle.Fixed3D;
            System.Threading.Thread.Sleep(100);
            panel5.BorderStyle = BorderStyle.FixedSingle;
            System.Threading.Thread.Sleep(100);
            Application.Exit();
        }     // Y funcion para salir :)
    }
}
