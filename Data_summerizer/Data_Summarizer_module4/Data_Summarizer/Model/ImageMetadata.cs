using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Data_Summarizer.Model
{
    public class ImageMetadata
    {
        static int idDescription = 0x010E;
        static int idTitle = 0x0320;
        static short PropertyItemType = 1;

        public static byte[] AddImageMetadata(byte[] imageByte, string text, int id, short type) 
        {
            //Bitmap imageBmp = ImageToBitmap(imageByte);

            //PropertyItem propertyItem = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            //propertyItem.Id = id;       // Image description. See https://msdn.microsoft.com/en-us/library/system.drawing.imaging.propertyitem.id(v=vs.110).aspx
            //propertyItem.Type = type;   // A byte array. See https://msdn.microsoft.com/en-us/library/system.drawing.imaging.propertyitem.type(v=vs.110).aspx
            //propertyItem.Value = Encoding.ASCII.GetBytes(text);
            //propertyItem.Len = propertyItem.Value.Length;
            //imageBmp.SetPropertyItem(propertyItem); 

            Image img = ConvertByteArrayToImage(imageByte);

            var newItem = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));

            //PropertyItem propItem = img.GetPropertyItem(id);
            newItem.Id = id;
            newItem.Type = 1;
            newItem.Value = Encoding.UTF8.GetBytes(text);
            newItem.Len = newItem.Value.Length;

            img.SetPropertyItem(newItem);

            //
            string text2 = Encoding.UTF8.GetString(img.GetPropertyItem(id).Value);
            //

            img.Save("test.png");
            byte[] binaryContent = File.ReadAllBytes("test.png");

            return ConvertImageToByteArray(img);
        }

        public static string ReadImageMetadata(byte[] imageByte, int id)
        {
            //Bitmap imageBmp = ImageToBitmap(imageByte);

            //var propertyItem = imageBmp.GetPropertyItem(id);
            //return Encoding.ASCII.GetString(propertyItem.Value);

            Image img = ConvertByteArrayToImage(imageByte);

            PropertyItem propItem = img.GetPropertyItem(id);

            string text = Encoding.UTF8.GetString(img.GetPropertyItem(id).Value);
            return text;
        }


        public static byte[] AddImageDescription(byte[] imageByte, string description)
        {
            return AddImageMetadata(imageByte, description, idDescription, PropertyItemType);
        }

        public static byte[] AddImageTitle(byte[] imageByte, string title)
        {
            return AddImageMetadata(imageByte, title, idTitle, PropertyItemType);
        }

        public static string ReadImageDescription(byte[] imageByte)
        {
            return ReadImageMetadata(imageByte, idDescription);
        }
        public static string ReadImageTitle(byte[] imageByte)
        {
            return ReadImageMetadata(imageByte, idTitle);
        }


        public static Bitmap ImageToBitmap(byte[] img)
        {
            using (var ms = new MemoryStream(img, true))
            {
                return new Bitmap(ms, true);
            }

            //TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            //return (Bitmap)tc.ConvertFrom(img);

            //Image image = Image.FromStream(new MemoryStream(img), true, true);
            //return (Bitmap)image;
        }

        public static byte[] ImageToByte(Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static Image ConvertByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms, true, true);
            }
        }
        public static byte[] ConvertImageToByteArray(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }
            //ImageConverter _imageConverter = new ImageConverter();
            //byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            //return xByte;
        }
    }
}
