using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;


namespace webApidemo4.Controllers
{
    public class ImageController : ApiController
    {
        // post
        [Authorize]
        [HttpPost]
        [Route("api/UploadFile")]
        public async Task<HttpResponseMessage> UploadFile()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            var ctx = System.Web.HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_data");
            var provider = new MultipartFormDataStreamProvider(root);
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);
            if (!Directory.Exists(root + "\\compressed")) Directory.CreateDirectory(root + "\\compressed");

            try
            {
                await Request.Content
                     .ReadAsMultipartAsync(provider);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
                    name = name.Trim('"');

                    string localfile = file.LocalFileName;
                    // var filepath = Path.Combine(root, name);
                    // File.Move(localfile, filepath);
                    CompressImageFile(localfile, name, root + "\\compressed");

                    // delete files 
                    string[] filePaths = Directory.GetFiles(root);
                    foreach (string filePath in filePaths)
                        File.Delete(filePath);
                    response = Downloadfile(name);
                    filePaths = Directory.GetFiles(root + "\\compressed");
                    foreach (string filePath in filePaths)
                        File.Delete(filePath);
                }


            }
            catch (Exception e)
            {
                //  return $"Error: {e.Message}";

                response.Content = new StringContent("File size to big. " + e.Message);

            }
            // return "File uploaded";
            return response;
        }


        public HttpResponseMessage Downloadfile(string name)
        {
            var ctx = System.Web.HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_data/compressed");
            string filePath = Path.Combine(root, name);
            byte[] file = File.ReadAllBytes(filePath);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            MemoryStream ms = new MemoryStream(file);
            response.Content = new ByteArrayContent(file);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            response.Content.Headers.ContentDisposition.FileName = name;
            return response;
        }




        public void CompressImageFile(String mfile, String fileName, String path)
        {
            MemoryStream imgStream = new MemoryStream();
            MemoryStream toStream = new MemoryStream();
            FileStream fs = new FileStream(mfile, FileMode.Open, FileAccess.Read);
            fs.CopyTo(imgStream);
            var image = Image.FromStream(imgStream);
            // double scaleFactor = .10;
            decimal scaleFactor = 1;
            if (image.Width > 600)
            {
                int wd = image.Width;
                decimal factor = wd / 600;
                scaleFactor = (decimal)(1 / factor);
            }
            var newWidth = (int)(image.Width * scaleFactor);
            var newHeight = (int)(image.Height * scaleFactor);

            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(toStream, image.RawFormat);
            String outPath = Path.Combine(path, fileName);
            FileStream file = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            toStream.WriteTo(file);

            fs.Close();
            file.Close();
            toStream.Close();
            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}