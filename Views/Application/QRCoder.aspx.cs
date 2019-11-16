using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProjectCSA
{
    public partial class QRCoder_Page : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void qr_button_Click(object sender, EventArgs e)
        {
            //This variable is the input for the qr-code, which should be pulled from the database instead of being an on-click event
            string Code = qr_text_input.Text;
            QRCodeGenerator qrgenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrgenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            System.Web.UI.WebControls.Image imgQRcode = new System.Web.UI.WebControls.Image();
            imgQRcode.Width = 150;
            imgQRcode.Height = 150;

            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imgQRcode.ImageUrl = "data:image/png;base64" + Convert.ToBase64String(byteImage);
                }
                PlaceholderQR.Controls.Add(imgQRcode);
            }
        }

    }
}