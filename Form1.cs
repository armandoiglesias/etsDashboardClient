using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Net.Sockets;

//importing InTheHand library
using InTheHand.Net.Bluetooth;
using InTheHand.Windows.Forms;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth.AttributeIds;
using Libreria;
using System.Runtime.InteropServices;
using Ets2SdkClient;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        public Ets2SdkTelemetry Telemetry;
        Ets2Telemetry _data;


        //Thread AcceptAndListeningThread;
        // helper variable
        Boolean isConnected = false;
        //bluetooth stuff
        //BluetoothClient btClient;  //represent the bluetooth client
        //connection
        //BluetoothListener btListener; //represent this server bluetooth

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                //Hide();
                notifyIcon1.Visible = true;
            }
           
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1_DoubleClick(null, null);
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

            timer1.Interval = 200;
            timer1.Enabled = true;

            Telemetry = new Ets2SdkTelemetry();
            Telemetry.Data += Telemetry_Data;

            
        }

        private void Telemetry_Data(Ets2Telemetry data, bool updated)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new TelemetryData(Telemetry_Data), new object[2] { data, updated });
                    _data = data;
                    return;
                }
                _data = data;
            }
            catch (Exception exception) {
                notifyIcon1.ShowBalloonTip(450, "Error", exception.Message , ToolTipIcon.Error);
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (serialPort1.IsOpen && _data != null)
            {
                if (!_data.Paused )
            	{

                    int luces = 0;
                    if (_data.Lights.BlinkerRightOn)
                    {
                        luces += 32;
                    }

                    if (_data.Lights.BlinkerLeftOn)
                    {
                        luces += 16;
                    }

                    if (_data.Lights.LowBeams)
                    {
                        luces += 8;
                    }

                    if (_data.Lights.HighBeams)
                    {
                        luces += 4;
                    }


                    int lucesAdvertencia = 0;
                    if (_data.Drivetrain.ParkingBrake)
                    {
                        lucesAdvertencia += 127;
                    }

                    if (_data.Drivetrain.FuelWarningLight     )
                    {
                        lucesAdvertencia += 8;
                    }

                    string _mensaje = _data.Physics.SpeedKmh +   _data.Drivetrain.EngineRpm + "00000000" + luces + lucesAdvertencia + "00" ; 
                    logsText.Text += _mensaje + Environment.NewLine;

		            serialPort1.Write(0xFF + "2" +  _mensaje );
	            }
            }
            else {
                notifyIcon1.ShowBalloonTip(450, "Error", "Error al intentar enviar por puerto serial", ToolTipIcon.Error);
            }
        }

    }

}