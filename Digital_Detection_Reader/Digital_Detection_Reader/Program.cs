using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization;
using System.Drawing;
using System.Web;

namespace Digital_Detection_Reader
{
    public partial class Program : Form
    {
        protected SerialPort port;
        private Button stop_button;
        private Button connect_button;
        private TextBox inlet_value_display;
        private TextBox TV_value_display;
        private TextBox NG_value_display;
        private TextBox CT_value_display;
        private TextBox Control_value_display;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private bool stop_program = false;
        public UInt16[] values = new UInt16[5];

        public bool using_AMR_board = false;

        BackgroundWorker data_collector;
        BackgroundWorker text_updater;
        BackgroundWorker data_plotter;
        public bool update_plot = false;
        public string path = @"C:\Digital Files";
        public FileStream log;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        StreamWriter log_writer;

        // add time of call to test
        System.Windows.Forms.DataVisualization.Charting.StripLine TV_call, NG_call, CT_call, Control_call;
        private TextBox TV_displayed_result;
        private TextBox NG_displayed_result;
        private TextBox CT_displayed_result;
        private CheckBox checkBox1;
        private TextBox Control_displayed_result;

        public Program()
        {
            InitializeComponent();

            // initialize BackgroudnWorker for reading data
            data_collector = new BackgroundWorker();
            data_collector.DoWork += data_collector_main;
            data_collector.WorkerSupportsCancellation = true;

            // initialize BackgroundWorker for updating values to screen
            text_updater = new BackgroundWorker();
            text_updater.DoWork += text_updater_main;
            text_updater.WorkerSupportsCancellation = true;

            // initialize BackgroundWorker for updating plot
            data_plotter = new BackgroundWorker();
            data_plotter.DoWork += data_plotter_main;
            data_plotter.WorkerSupportsCancellation = true;

            chart_load();
        }

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.connect_button = new System.Windows.Forms.Button();
            this.stop_button = new System.Windows.Forms.Button();
            this.inlet_value_display = new System.Windows.Forms.TextBox();
            this.TV_value_display = new System.Windows.Forms.TextBox();
            this.NG_value_display = new System.Windows.Forms.TextBox();
            this.CT_value_display = new System.Windows.Forms.TextBox();
            this.Control_value_display = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.TV_displayed_result = new System.Windows.Forms.TextBox();
            this.NG_displayed_result = new System.Windows.Forms.TextBox();
            this.CT_displayed_result = new System.Windows.Forms.TextBox();
            this.Control_displayed_result = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // connect_button
            // 
            this.connect_button.Location = new System.Drawing.Point(509, 594);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(108, 31);
            this.connect_button.TabIndex = 0;
            this.connect_button.Text = "Connect";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // stop_button
            // 
            this.stop_button.Location = new System.Drawing.Point(639, 594);
            this.stop_button.Name = "stop_button";
            this.stop_button.Size = new System.Drawing.Size(108, 31);
            this.stop_button.TabIndex = 1;
            this.stop_button.Text = "Stop and Log";
            this.stop_button.UseVisualStyleBackColor = true;
            this.stop_button.Click += new System.EventHandler(this.stop_button_Click);
            // 
            // inlet_value_display
            // 
            this.inlet_value_display.Location = new System.Drawing.Point(327, 550);
            this.inlet_value_display.Name = "inlet_value_display";
            this.inlet_value_display.Size = new System.Drawing.Size(100, 20);
            this.inlet_value_display.TabIndex = 2;
            // 
            // TV_value_display
            // 
            this.TV_value_display.Location = new System.Drawing.Point(441, 550);
            this.TV_value_display.Name = "TV_value_display";
            this.TV_value_display.Size = new System.Drawing.Size(100, 20);
            this.TV_value_display.TabIndex = 3;
            // 
            // NG_value_display
            // 
            this.NG_value_display.Location = new System.Drawing.Point(556, 550);
            this.NG_value_display.Name = "NG_value_display";
            this.NG_value_display.Size = new System.Drawing.Size(100, 20);
            this.NG_value_display.TabIndex = 4;
            // 
            // CT_value_display
            // 
            this.CT_value_display.Location = new System.Drawing.Point(671, 550);
            this.CT_value_display.Name = "CT_value_display";
            this.CT_value_display.Size = new System.Drawing.Size(100, 20);
            this.CT_value_display.TabIndex = 5;
            // 
            // Control_value_display
            // 
            this.Control_value_display.Location = new System.Drawing.Point(786, 550);
            this.Control_value_display.Name = "Control_value_display";
            this.Control_value_display.Size = new System.Drawing.Size(100, 20);
            this.Control_value_display.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(344, 534);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Inlet";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(481, 534);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "TV";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(594, 534);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "NG";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(710, 534);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "CT";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(831, 534);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "4+";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(76, 31);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1260, 482);
            this.chart1.TabIndex = 12;
            this.chart1.Text = "chart1";
            // 
            // TV_displayed_result
            // 
            this.TV_displayed_result.Location = new System.Drawing.Point(1369, 115);
            this.TV_displayed_result.Name = "TV_displayed_result";
            this.TV_displayed_result.Size = new System.Drawing.Size(100, 20);
            this.TV_displayed_result.TabIndex = 13;
            // 
            // NG_displayed_result
            // 
            this.NG_displayed_result.Location = new System.Drawing.Point(1369, 141);
            this.NG_displayed_result.Name = "NG_displayed_result";
            this.NG_displayed_result.Size = new System.Drawing.Size(100, 20);
            this.NG_displayed_result.TabIndex = 14;
            // 
            // CT_displayed_result
            // 
            this.CT_displayed_result.Location = new System.Drawing.Point(1369, 167);
            this.CT_displayed_result.Name = "CT_displayed_result";
            this.CT_displayed_result.Size = new System.Drawing.Size(100, 20);
            this.CT_displayed_result.TabIndex = 15;
            // 
            // Control_displayed_result
            // 
            this.Control_displayed_result.Location = new System.Drawing.Point(1369, 193);
            this.Control_displayed_result.Name = "Control_displayed_result";
            this.Control_displayed_result.Size = new System.Drawing.Size(100, 20);
            this.Control_displayed_result.TabIndex = 16;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(770, 602);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(74, 17);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "AMR PCB";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(1561, 734);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.Control_displayed_result);
            this.Controls.Add(this.CT_displayed_result);
            this.Controls.Add(this.NG_displayed_result);
            this.Controls.Add(this.TV_displayed_result);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Control_value_display);
            this.Controls.Add(this.CT_value_display);
            this.Controls.Add(this.NG_value_display);
            this.Controls.Add(this.TV_value_display);
            this.Controls.Add(this.inlet_value_display);
            this.Controls.Add(this.stop_button);
            this.Controls.Add(this.connect_button);
            this.Name = "Program";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #region Digital Detection Board Communication methods

        /// <summary>
        /// Connects SerialPort instance port to Digital Detection board
        /// </summary>
        /// <returns> true if connection is successfully made. Else returns false</returns>
        public bool connect_to_device()
        {
            // create new SerialPort object 
            port = new SerialPort();
            string[] available_com_ports = SerialPort.GetPortNames(); // get names of all serial port objects
            string s;
            bool digital_board_successfully_connected = false;

            // set parameters
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.Handshake = Handshake.None;
            port.ReadTimeout = 2500;
            port.WriteTimeout = 1000;

            if (available_com_ports.Length == 0)
            {
                Console.WriteLine("No COM ports detected");
                return false;
            }
            else
            {
                for (int i = 0; i < available_com_ports.Length; i++)
                {
                    s = available_com_ports[i];
                    port.PortName = s;
                    Console.WriteLine("attempting connection to: " + s);

                    // open port
                    try
                    {
                        port.Open();

                        if (verify_detection_board(port) == true) // COM port for Digital Detection Board found
                        {
                            digital_board_successfully_connected = true;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Cannot open: " + s);
                    }

                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                }

                if (digital_board_successfully_connected == true)
                {
                    Console.WriteLine("device connected on " + port.PortName);
                }
                else
                {
                    Console.WriteLine("Failed to detect Digital Detection Board");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Commands Digital Detection Board to start logging data 
        /// </summary>
        /// <returns> true if no error occured. Else false</returns>
        public bool command_board_to_begin_logging()
        {
            try
            {
                port.WriteLine("startLogging");
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Commands Digital Detection Board to stop logging data 
        /// </summary>
        /// <returns> true if no error occured. Else false</returns>
        public bool command_board_to_stop_logging()
        {
            try
            {
                port.WriteLine("endLogging");
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Returns true
        /// </summary> verifies that COM port port is attached to is Digital Detection Board
        /// <param name="local_port"></param>
        /// <returns>true if port is connected to Digital Detection Board. ELse false</returns>
        private bool verify_detection_board(SerialPort local_port)
        {
            string board_response;

            local_port.WriteLine("board"); // first response after Digital Detection Board is powered may not be valid
            Thread.Sleep(10);
            local_port.WriteLine("board");
            board_response = local_port.ReadLine();

            if ((board_response.Substring(0, 7).Equals("Digital")) && (using_AMR_board == false))
            {
                return true; // Digital Detection PCB
            }
            else if ((board_response.Substring(0, 3).Equals("AMR")) && (using_AMR_board == true))
            {
                return true; // AMR NG PCB
            }

            return false;
        }
        #endregion // Digital Detection Board Communication methods

        #region Button methods
        private void stop_button_Click(object sender, EventArgs e)
        {
            // set flag to stop program
            stop_program = true;
            data_plotter.CancelAsync();
            data_collector.CancelAsync();
            text_updater.CancelAsync();
            log_writer.Close();

            if(using_AMR_board == false)
            {
                command_board_to_stop_logging(); // doesnt work right now
                get_results();
            }
        }

        private void get_results()
        {
            string board_response;
            string[] temp;
            int[] spot_formation_detected = new int[5];
            string[] targets = new string[4];
            TV_call = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            NG_call = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            CT_call = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            Control_call = new System.Windows.Forms.DataVisualization.Charting.StripLine();

            targets[0] = "TV";
            targets[1] = "NG";
            targets[2] = "CT";
            targets[3] = "Control";

            port.DiscardInBuffer();
            port.DiscardOutBuffer();

            port.WriteLine("sendResults");
            board_response = port.ReadLine();

            // parse data from board and store in string array
            temp = board_response.Split(',');

            // convert to integers
            for (int b = 0; b < 5; b++)
            {
                spot_formation_detected[b] = Convert.ToInt16(temp[b]);
            }

            // display values to Console
            for (int i = 0; i < 5; i++)
            {
                // target was detected. ignore first response because its inlet
                if((spot_formation_detected[i] != -1) && (i > 0))
                {
                    Console.WriteLine(targets[i - 1]);
                }
            }

            if (spot_formation_detected[1] != -1)
            {
                TV_call.StripWidth = 2;
                TV_call.BackColor = Color.FromArgb(255, Color.Orange);
                TV_call.Interval = 0;
                TV_call.IntervalOffset = spot_formation_detected[1];
                chart1.ChartAreas[0].AxisX.StripLines.Add(TV_call);
                TV_displayed_result.Text = "TV: POSITIVE";
            }
            if (spot_formation_detected[2] != -1)
            {
                NG_call.StripWidth = 2;
                NG_call.BackColor = Color.FromArgb(255, Color.Green);
                NG_call.Interval = 0;
                NG_call.IntervalOffset = spot_formation_detected[2];
                chart1.ChartAreas[0].AxisX.StripLines.Add(NG_call);
                NG_displayed_result.Text = "NG: POSITIVE";
            }
            if (spot_formation_detected[3] != -1)
            {
                CT_call.StripWidth = 2;
                CT_call.BackColor = Color.FromArgb(255, Color.Blue);
                CT_call.Interval = 0;
                CT_call.IntervalOffset = spot_formation_detected[3];
                chart1.ChartAreas[0].AxisX.StripLines.Add(CT_call);
                CT_displayed_result.Text = "CT: POSITIVE";
            }
            if (spot_formation_detected[4] != -1)
            {
                Control_call.StripWidth = 2;
                Control_call.BackColor = Color.FromArgb(255, Color.Violet);
                Control_call.Interval = 0;
                Control_call.IntervalOffset = spot_formation_detected[4];
                chart1.ChartAreas[0].AxisX.StripLines.Add(Control_call);
                Control_displayed_result.Text = "4+: POSITIVE";
            }
        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            // connected port to Digital Detection Board
            connect_to_device();

            // create file for logging data
            CreateFile();

            // tell board to begin sending data
            if(using_AMR_board == false) // AMR NG board is constantly streaming data during detection so no need to command it to start logging
            {
                command_board_to_begin_logging(); // comment out for AMR NG board
            }


            // start recording the data
            data_collector.RunWorkerAsync();
            data_plotter.RunWorkerAsync();
            text_updater.RunWorkerAsync();
        }
        #endregion // Button methods

        #region data logging
        public void CreateFile()
        {
            // create the filename from current date and time
            DateTime currentTime = DateTime.Now;
            string fileName = currentTime.ToString("yyMMdd");
            fileName = fileName + "_" + currentTime.ToString("hh_mm_ss");
            fileName = path + @"\" + fileName + ".csv";

            // make sure the folder exists
            if(Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            // create the file
            log = File.Create(fileName);
            log_writer = new StreamWriter(log);
            Console.WriteLine(fileName);
        }
        #endregion

        #region BackgroundWorker data_collector
        private void data_collector_main(object sender, DoWorkEventArgs e)
        {
            string board_response;
            string[] temp;

            //while(stop_program == false)
            while(stop_program == false)
            {
                try
                {
                    // clear the bugger
                    port.DiscardInBuffer();

                    // get photodiode data from Digital Detection Board
                    board_response = port.ReadLine();

                    // parse data from board and store in string array
                    temp = board_response.Split(',');

                    // ',' is added after last digit so every other iteration of this loop yields no useful data
                    Console.WriteLine(temp.Length);
                    if(temp.Length >= 5)
                    {
                        // convert to integers
                        for (int b = 0; b < 5; b++)
                        {
                            values[b] = Convert.ToUInt16(temp[b]);
                        }

                        // display values to Console
                        for (int i = 0; i < 5; i++)
                        {
                            Console.Out.WriteLine(values[i]);
                            log_writer.Write(Convert.ToString(values[i]) + ',');
                        }

                        // add new line between each set of data
                        log_writer.WriteLine();
                        Console.WriteLine("newline written");
                        update_plot = true; // flag to update plot on UI
                    }
                }
                catch
                {
                    //return false;
                }
            }
            //return true;
        }

        private void background_worker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        #endregion // BackgroundWorker data_collector

        #region BackgroundWorker text_updater
        private void text_updater_main(object sender, DoWorkEventArgs e)
        {
            while(stop_program == false)
            {
                //inlet_value_display.Text = Convert.ToString(values[0]);
                inlet_value_display.Invoke((MethodInvoker)delegate { inlet_value_display.Text = Convert.ToString(values[0]); });
                TV_value_display.Invoke((MethodInvoker)delegate { TV_value_display.Text = Convert.ToString(values[1]); });
                NG_value_display.Invoke((MethodInvoker)delegate { NG_value_display.Text = Convert.ToString(values[2]); });
                CT_value_display.Invoke((MethodInvoker)delegate { CT_value_display.Text = Convert.ToString(values[3]); });
                Control_value_display.Invoke((MethodInvoker)delegate { Control_value_display.Text = Convert.ToString(values[4]); });
                System.Threading.Thread.Sleep(500);
            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            using_AMR_board = checkBox1.Checked;
        }

        private void text_updater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs a)
        {

        }
        #endregion

        #region BackgroundWorker data_plotter
        private void data_plotter_main(object sender, DoWorkEventArgs e)
        {
            UInt16 i = 0;
            while (stop_program == false)
            {
                if((update_plot == true))
                {
                    update_plot = false;
                    chart1.Invoke((MethodInvoker)delegate { chart1.Series["Inlet"].Points.AddXY(++i, values[0]); });
                    chart1.Invoke((MethodInvoker)delegate { chart1.Series["TV"].Points.AddXY(i, values[1]); });
                    chart1.Invoke((MethodInvoker)delegate { chart1.Series["NG"].Points.AddXY(i, values[2]); });
                    chart1.Invoke((MethodInvoker)delegate { chart1.Series["CT"].Points.AddXY(i, values[3]); });
                    chart1.Invoke((MethodInvoker)delegate { chart1.Series["Control"].Points.AddXY(i, values[4]); });
                    chart1.Invoke((MethodInvoker)delegate { chart1.ChartAreas[0].RecalculateAxesScale(); });
                }
                Thread.Sleep(100);
            }
        }
        #endregion

        void chart_load()
        {
            var chart = chart1.ChartAreas[0];

            chart.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;

            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            chart.AxisX.LabelStyle.IsEndLabelVisible = true;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
            //chart.AxisX.Minimum = 0;
            //chart.AxisY.Maximum = 4096;

            chart.AxisX.Interval = 1;
            chart.AxisY.Interval = 100; 

            chart1.Series[0].IsVisibleInLegend = false;

            chart1.Series.Add("Inlet");
            chart1.Series["Inlet"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Inlet"].Color = Color.Red;

            chart1.Series.Add("TV");
            chart1.Series["TV"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["TV"].Color = Color.Orange;

            chart1.Series.Add("NG");
            chart1.Series["NG"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["NG"].Color = Color.Green;

            chart1.Series.Add("CT");
            chart1.Series["CT"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["CT"].Color = Color.Blue;

            chart1.Series.Add("Control");
            chart1.Series["Control"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Control"].Color = Color.Violet;

            chart1.Series.RemoveAt(0);
        }
    }
}
