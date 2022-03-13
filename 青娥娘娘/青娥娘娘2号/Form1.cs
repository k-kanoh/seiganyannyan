using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace 青娥娘娘2号
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DragEnter += (_, e) => e.Effect = DragDropEffects.Move;
            DragDrop += (_, e) => Form1_DragDrop(e);
        }

        private async void Form1_DragDrop(DragEventArgs e)
        {
            var dropFiles = ((string[])e.Data.GetData(DataFormats.FileDrop)).Select(x => new FileInfo(x));

            var decode = "";
            foreach (var fi in dropFiles.OrderBy(x => x.CreationTime))
            {
                var source = new Bitmap(fi.FullName);
                var 貼る先のRectangle = new Rectangle();

                for (int i = 0; i < 5; i++)
                {
                    貼る先のRectangle.Size = new Size(185 * 5, 185);

                    foreach (var bar in new[] { 0, 5 })
                    {
                        using (var newBmp = new Bitmap(貼る先のRectangle.Width, 貼る先のRectangle.Height, source.PixelFormat))
                        using (var g = Graphics.FromImage(newBmp))
                        {
                            try
                            {
                                var 切り取るRectangle = new Rectangle(new Point(bar * 185, 33 + i * 185), 貼る先のRectangle.Size);

                                g.DrawImage(source, 貼る先のRectangle, 切り取るRectangle, GraphicsUnit.Pixel);

                                pictureBox1.Image = newBmp;

                                var results = new BarcodeReader().DecodeMultiple(newBmp);
                                if (results == null) break;

                                decode += string.Concat(results.Select(x => x.Text));

                                Text = $"[{decode.Length}] {fi.Name} ({i + 1} x {results.Count()})";

                                await Task.Delay(10);
                            }
                            finally
                            {
                                pictureBox1.Image = null;
                            }
                        }
                    }
                }
            }

            try
            {
                File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "bin.bin"), Convert.FromBase64String(decode));
            }
            catch
            {
                MessageBox.Show("変換失敗", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
