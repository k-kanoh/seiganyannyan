using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace 青娥娘娘1号
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            AllowDrop = true;
            DragEnter += (_, e) => e.Effect = DragDropEffects.Move;
            DragDrop += (_, e) => Form1_DragDrop(e);
        }

        private void Form1_DragDrop(DragEventArgs e)
        {
            var drop = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

            var writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions()
                {
                    ErrorCorrection = ErrorCorrectionLevel.L,
                    QrVersion = 40,
                }
            };

            const int maxlen = 2953;
            const int page = 0;

            var bytes = File.ReadAllBytes(drop);
            var encode = Convert.ToBase64String(bytes).Substring(maxlen * 50 * page);

            for (int i = 0; i * maxlen < encode.Length && i < 50; i++)
            {
                var moji = new string(encode.Skip(i * maxlen).Take(maxlen).ToArray());
                var qrImage = writer.Write(moji);

                Controls.Add(new PictureBox()
                {
                    Image = qrImage,
                    Size = qrImage.Size,
                    Location = new Point((i % 10) * qrImage.Size.Width, 10 + (i / 10) * qrImage.Size.Height)
                });
            }

            WindowState = FormWindowState.Maximized;
        }
    }
}
