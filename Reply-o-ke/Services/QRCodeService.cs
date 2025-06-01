using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace Reply_o_ke.Services
{
    public class QRCodeService
    {
        public byte[] GenerateQRCode(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            return qrCode.GetGraphic(20);
        }

        public string GenerateQRCodeBase64(string text)
        {
            var qrCodeBytes = GenerateQRCode(text);
            return Convert.ToBase64String(qrCodeBytes);
        }
    }
}
