using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;


namespace QCM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //------------------Установка порта---------------------
        //------------------------------------------------------

        public string SetPortName(string defaultPortName)
        {
            string portName = "";
            foreach (string portname in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(portname); //добавить порт в список 
            }
            // comboBox1.SelectedIndex = 0;
            if (portName == "")
            {
                portName = defaultPortName;
            }
            return portName; //возвращает порт по умолчанию 
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.PortName = SetPortName(serialPort1.PortName);
            chart1.Series[0].Points.Add();
            chart1.Series[0].Points[0].IsEmpty = true;



        }


        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[0].Points.Add();
            chart1.Series[0].Points[0].IsEmpty = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        // ----------------------открытие порта------------------

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = ((string)comboBox1.SelectedItem);

                    button1.Text = "Закрыть";
                    serialPort1.Open();
                    panel1.BackColor = Color.FromName("Lime");
                    serialPort1.DataReceived += serialPort1_DataReceived;
                    timer1.Enabled = true;

                }
                else
                {

                    serialPort1.Close();
                    button1.Text = "Открыть";
                    panel1.BackColor = Color.FromName("Red");
                    timer1.Enabled = false;

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }


        //-----------------Чтение данных из порта-------------------
        //----------------------------------------------------------


        private int byteRecieved;
        byte[] messByte = new byte[3];

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            byteRecieved = serialPort1.BytesToRead;
       
            try
            {
               

                serialPort1.Read(messByte, 0, 3);

                this.Invoke(new EventHandler(dispData));


            }
            catch (TimeoutException)
            {

            }
        }

        int timer = 0,max = 0, min = 0, counterToFIle=0;

        private void dispData(object o, EventArgs e)
        {
            int data, data1, data2,counter=0;
            int dspData;
            DateTime thisTime = DateTime.Now;
            

            
            data = messByte[0];     // старший байт
            data1 = messByte[1];    // средний байт
            data2 = messByte[2];    // младший байт

            dspData =   (65536*messByte[0] + messByte[1]*256 + messByte[2])*10;

            if (timer == 1) min = dspData;


           // chart1.ChartAreas["ChartArea1"].Axes[1].Minimum = 900000;
           // chart1.ChartAreas["ChartArea1"].Axes[1].Maximum = 1000000;

            if (dspData > max) max = dspData;
            if (min > dspData) min = dspData;

            label3.Text = max.ToString() + " Гц";
            label4.Text = min.ToString() + " Гц";

            label2.Text = dspData.ToString()+" Гц";
            label5.Text = String.Format("{0} c", timer) ;
            chart1.Series["Series1"].Points.AddXY(counter++, dspData);


            File.AppendAllText(day + "/" + time + ".txt", (++counterToFIle) + " " + dspData.ToString() + "  " + thisTime.Hour.ToString() + ":" + thisTime.Minute.ToString() + ":" + thisTime.Second.ToString() + Environment.NewLine);


        }




        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
        }

        private void обновлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 uplFrm = new Form2();

            uplFrm.ShowInTaskbar = false;
            uplFrm.StartPosition = FormStartPosition.CenterScreen;
            uplFrm.ShowDialog(this);

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("    QCM v 1.0.1 \n    pobiyaha@gmail.com \n    Alexander Pobiyaha, 2017");
        }




        //---------------------Отправка данных в порт----------------
        //-------------------------Старт частотомера--------------------

        byte flag = 0;
        string time,day;

        private void button4_Click(object sender, EventArgs e)
        {
            char[] data = new char[1];
            DateTime thisDay = DateTime.Today;
            DateTime thisTime = DateTime.Now;

            if (serialPort1.IsOpen)
               {
                   if (flag==1) {
                       flag = 0;   
                       data[0] = 't';
                       serialPort1.Write(data, 0, 1);
                       button4.Text = "Начать";
                   }
                   else
                   {
                       day = thisDay.ToString("d");
                       // создаём папку с файом текущей сессии
                       Directory.CreateDirectory(day);

                       // создаём файл в текущей папке

                       time = thisTime.Hour.ToString()+ "h" + thisTime.Minute.ToString()+ "m" + thisTime.Second.ToString()+"s";

                     //  File.Create(thisDay.ToString("d") + "/" + time + ".txt");
                       

                       flag = 1;
                       data[0] = 's';
                       serialPort1.Write(data, 0, 1);
                       button4.Text = "Закончить";

                   }
  

               }

        }



    }
}
