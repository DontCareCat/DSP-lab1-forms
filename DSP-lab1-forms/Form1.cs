using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;
using System.Runtime.InteropServices;

namespace DSP_lab1_forms
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = "";
            using(OpenFileDialog openFileDialog= new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "d:\\";
                openFileDialog.Filter = "wav files (*.wav)|*.wav|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if(openFileDialog.ShowDialog()==DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
            }
            textBox1.Text = path;
            Console.WriteLine("Loading: "+textBox1.Text);
            WAVFile source = new WAVFile(textBox1.Text);
            //source.buffer = File.ReadAllBytes(textBox1.Text);
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("#"));
            dt.Columns.Add(new DataColumn("Byte"));
            dt.Columns.Add(new DataColumn("ASCII"));
            dt.Columns.Add(new DataColumn("HEX"));

            using (FileStream fs = new FileStream("C:\\Users\\DontCareCat\\Desktop\\output.txt", FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs,Encoding.ASCII);
                foreach (DataColumn dc in dt.Columns)
                {
                    sw.Write($"{dc.ColumnName}\t");
                }

                int i = 0;
                foreach (byte b in source.buffer)
                {
                    Console.WriteLine(i);
                    var dr = dt.NewRow();
                    dr[0] = i;
                    dr[1] = b;
                    dr[2] = (char)b;
                    dr[3] = BitConverter.ToString(new byte[] { b });
                    dt.Rows.Add(dr);

                    /*sw.Write($"{i}\t");
                    sw.Write($"{b}\t");
                    sw.Write($"{(char)b}\t");
                    sw.Write($"{BitConverter.ToString(new byte[] { b })}");
                    sw.WriteLine();*/
                    i++;
                    Console.SetCursorPosition(0, 1);
                    if (i > 500)
                        break;
                }
            }
            foreach(DataColumn dc in dt.Columns)
            {
                Console.Write($"{dc.ColumnName}\t");
            }
            Console.WriteLine();
            
            foreach (DataRow dr in dt.Rows)
            {
                for (int j = 0; j < dr.ItemArray.Length; j++)
                    Console.Write($"{dr[j]}\t");
                Console.WriteLine();
            }
            source.GetMetadata();

            DrawSampleGraph(source);
        }

        private void DrawSampleGraph(WAVFile source)
        {
            Graphics graphics = pictureBox1.CreateGraphics();
            graphics.Clear(Color.White);
            Pen penL = new Pen(Color.Blue,1f);
            Pen penR = new Pen(Color.Green,1f);

            List<Point> tempPointsR = new List<Point>();
            List<Point> tempPointsL = new List<Point>();
            List<Point> pointsR = new List<Point>();
            List<Point> pointsL = new List<Point>();

            int ymax = int.MinValue;
            int ymin = int.MaxValue;

            for(int i = 0;i<source.sampleCount;i++)
            {
                if (source[i].Item3 > ymax)
                    ymax = source[i].Item3;
                if(source[i].Item3 < ymin)
                    ymin= source[i].Item3;
            }

            double xscale = ((double)source.sampleCount) / pictureBox1.Width;
            double yscale = ((double)(ymax - ymin)) / pictureBox1.Height;

            for(int i=0;i<source.sampleCount;i++)
            {
                (byte, int, int) sample = source[i];
                Point p = new Point((int)(i/xscale),(int)(sample.Item3/yscale));
                if(sample.Item1==0)
                {
                    if (tempPointsR.Count == 0 || tempPointsR[tempPointsR.Count - 1].X == p.X)
                        tempPointsR.Add(p);
                    if(tempPointsR.Count!=0&&tempPointsR[tempPointsR.Count-1].X!=p.X)
                    {

                    }
                }
                if(sample.Item1==1)
                {

                }
            }

            /*for(int i=0;i<source.subchunk2Size;i++)
            {
                Point p = new Point((int)(i / xscale), (int)(source.buffer[i + source.dataAddress + 8] / yscale));
                if(tempPointsR.Count==0||tempPointsR[tempPointsR.Count-1].X==p.X)
                {
                    tempPointsR.Add(p);
                }
                if(tempPointsR.Count != 0 && tempPointsR[tempPointsR.Count - 1].X != p.X)
                {
                    int sumY = 0;
                    for(int j=0;j<tempPointsR.Count;j++)
                    {
                        sumY += tempPointsR[j].Y;
                    }
                    pointsR.Add(new Point((int)(i / xscale)-1, sumY / tempPointsR.Count));
                    tempPointsR.Clear();
                }
            }*/
            graphics.DrawLines(penR, pointsR.ToArray());
            graphics.DrawLines(penL, pointsL.ToArray());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        class WAVFile
        {
            public byte[] buffer;
            public int dataAddress = -1;
            #region metadata
            public string chunkID
            {
                get
                {
                    return Encoding.ASCII.GetString(buffer.Skip(0).Take(4).ToArray());
                }
            }
            public int chunkSize
            {
                get
                {
                    return BitConverter.ToInt32(buffer, 4);
                }
            }
            public string format
            {
                get
                {
                    return Encoding.ASCII.GetString(buffer.Skip(8).Take(4).ToArray());
                }
            }
            public string subchunk1ID
            {
                get
                {
                    return Encoding.ASCII.GetString(buffer.Skip(12).Take(4).ToArray());
                }
            }
            public int subchunk1Size
            {
                get
                {
                    return BitConverter.ToInt32(buffer, 16);
                }
            }
            public Int16 audioFormat
            {
                get
                {
                    return BitConverter.ToInt16(buffer, 20);
                }
            }
            public Int16 numChannels
            {
                get
                {
                    return BitConverter.ToInt16(buffer, 22);
                }
            }
            public int sampleRate
            {
                get
                {
                    return BitConverter.ToInt32(buffer, 24);
                }
            }
            public int byteRate
            {
                get
                {
                    return BitConverter.ToInt32(buffer, 28);
                }
            }
            public Int16 blockAlign
            {
                get
                {
                    return BitConverter.ToInt16(buffer, 32);
                }
            }
            public Int16 bitsPerSample
            {
                get
                {
                    return BitConverter.ToInt16(buffer, 34);
                }
            }
            public string subchunk2ID
            {
                get
                {
                    return Encoding.ASCII.GetString(buffer.Skip(dataAddress).Take(4).ToArray());
                }
            }
            public int subchunk2Size
            {
                get
                {
                    return BitConverter.ToInt32(buffer, dataAddress+4);
                }
            }
            public int sampleCount
            {
                get
                {
                    return (buffer.Length - dataAddress - 8) / (bitsPerSample / 8);
                }
            }

            #endregion metadata

            public (byte,int,int) this[int i]
            {
                get
                {
                    byte channel = 0; // 0 or 1 for right and left channels
                    int sample = 0;
                    int block = i / blockAlign;
                    channel = (byte)(block % 2);
                    sample = Convert.ToInt32(buffer.Skip(i * bitsPerSample / 8 + dataAddress+8).Take(bitsPerSample / 8));
                    return (channel,block,sample);
                }
            }


            public WAVFile(string sourcePath)
            {
                buffer = File.ReadAllBytes(sourcePath);
                for(int i=36;i<buffer.Length-4&&dataAddress<0;i++)
                {
                    if (Encoding.ASCII.GetString(buffer.Skip(i).Take(4).ToArray()) == "data")
                        dataAddress = i;
                }
            }
            public void GetMetadata()
            {
                Console.WriteLine($"Chunk ID:\t\t{this.chunkID}");
                Console.WriteLine($"Chunk size:\t\t{this.chunkSize}");
                Console.WriteLine($"Format:\t\t\t{this.format}");
                Console.WriteLine($"Subchunk1ID:\t\t{this.subchunk1ID}");
                Console.WriteLine($"Subchunk1 Size:\t\t{this.subchunk1Size}");
                Console.WriteLine($"Audio format:\t\t{this.audioFormat}");
                Console.WriteLine($"Num Channels:\t\t{this.numChannels}");
                Console.WriteLine($"Sample Rate:\t\t{this.sampleRate}");
                Console.WriteLine($"Byte Rate:\t\t{this.byteRate}");
                Console.WriteLine($"Block Align:\t\t{this.blockAlign}");
                Console.WriteLine($"Bits per Sample:\t{this.bitsPerSample}");
                Console.WriteLine($"Subchunk2 ID:\t\t{this.subchunk2ID}");
                Console.WriteLine($"Subchunk2 Size:\t\t{this.subchunk2Size}");
            }
        }


    }
}
