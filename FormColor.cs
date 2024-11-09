using DevExpress.XtraEditors;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraCharts;

using Smartvisor.Benchmark.CV.DETECTION;
using Smartvisor.Benchmark.CV.TRACKING;
using Smartvisor.Common.Persistance;
using System.Numerics;
using System.IO;
using Smartvisor.Benchmark.CV.COMMON;
using Object = Smartvisor.Benchmark.CV.DETECTION.Object;
using Smartvisor.VISION.CV.TRACKING;
using Smartvisor.VISION.CV.AFR;

namespace Smartvisor.Benchmark
{
    public partial class FormColor : DevExpress.XtraEditors.XtraForm
    {
        private YoloPredictor vPredictor;
        private YoloDetector<TrackObject> vDetector;

        private YoloPredictor CPredictor;
        private YoloDetector<TrackObject> CDetector;
        private Server server;
        public Image<Bgr, byte> img = null;
        public Vector3 vect = new Vector3();
        public FormColor()
        {

            InitializeComponent();
            server = new Server();
            server = JSON.Load<Server>("ColorConfig");
            carTraceBindingSource.DataSource = server.CarTraceList;
        }
        private static readonly string[] Colors = { "Red", "Blue", "Green", "Yellow", "White", "Black", "Cyan", "Gray" };
        private static readonly Dictionary<string, int> ColorToIndex = new Dictionary<string, int>
    {
        { "Black", 0 },
        { "White", 1 },
        { "Cyan", 2 },
        { "Gray", 3 },
        { "Green", 4 },
        { "Red", 5 },
        { "White", 6 },
        { "Yellow", 7}
    };

        private void EvaluateAlgorithm()
        {
            int[,] confusionMatrix = new int[6, 6];

            Parallel.ForEach(Colors, colorFolder =>
            {
                List<Image> images = LoadImages(colorFolder);

                foreach (Image image in images)
                {
                    using (Bitmap bitmap = new Bitmap(image))
                    {
                        Image<Bgr, byte> emguImage = new Image<Bgr, byte>(bitmap);

                        string predictedColor = TransformI(emguImage);
                        string trueColor = colorFolder;

                        int trueIndex = ColorToIndex[trueColor];
                        int predictedIndex = ColorToIndex[predictedColor];

                        // Use thread-safe operation for updating confusion matrix
                        lock (confusionMatrix)
                        {
                            confusionMatrix[trueIndex, predictedIndex]++;
                        }
                    }
                }
            });

            PrintConfusionMatrix(confusionMatrix);
        }

        private List<Image> LoadImages(string colorFolder)
        {
            string baseDirectory = @"C:\Users\SmartDev\Desktop\color recognition\color\color";
            List<Image> list = new List<Image>();

            try
            {
                string[] directories = Directory.GetDirectories(baseDirectory);

                foreach (string subdir in directories)
                {
                    string folderName = Path.GetFileName(subdir);

                    if (folderName.Equals(colorFolder, StringComparison.OrdinalIgnoreCase))
                    {
                        string[] imageFiles = Directory.GetFiles(subdir, "*.jpg");

                        foreach (string imagePath in imageFiles)
                        {
                            using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                var image = Image.FromStream(stream);

                                if (image != null)
                                {
                                    list.Add(image);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return list;
        }

        private void PrintConfusionMatrix(int[,] confusionMatrix)
        {
            Console.WriteLine("Confusion Matrix:");
            Console.Write("        ");
            foreach (var color in Colors)
            {
                Console.Write($"{color,-6} ");
            }
            Console.WriteLine();

            for (int i = 0; i < Colors.Length; i++)
            {
                Console.Write($"{Colors[i],-6} ");
                for (int j = 0; j < Colors.Length; j++)
                {
                    Console.Write($"{confusionMatrix[i, j],-6} ");
                }
                Console.WriteLine();
            }
        }

        public Rectangle GetFaceRect( Rectangle face, Size frameSize, int margen = 0)
        {
            if (margen == 0)
                margen = face.Width / 10;
            if (face.Width > face.Height)
            {
                int diff = face.Width - face.Height;
                face.Height = face.Width;
                face.Y -= diff / 2;
            }
            else
            {
                int diff = face.Height - face.Width;
                face.Width = face.Height;
                face.X -= diff / 2;
            }

            int x = face.X - margen;
            if (x < 0)
                x = 0;
            int y = face.Y - margen;
            if (y < 0)
                y = 0;
            int w = face.Width + 2 * margen;
            if (w + x > frameSize.Width)
                w = frameSize.Width - x;
            int h = face.Height + 2 * margen;
            if (h + y > frameSize.Height)
                h = frameSize.Height - y;
            Rectangle rect = new Rectangle(x, y, w, h);

            return rect;
        }
        public  void Resize( IEnumerable<Object> faces, Size frameSize, int margen = 0)
        {
            foreach (var face in faces)
            {
                face.Rectangle = GetFaceRect(face.Rectangle,frameSize, margen);
            }
        }
        private Image<Bgr, byte> Apply_CLAHE(Image<Bgr, byte> imgIn, int clipLimit = 2, int tileSize = 25)
        {

            var imglabcol = new Image<Lab, byte>(imgIn.Size);
            var imgoutL = new Image<Gray, byte>(imgIn.Size);
            var imgoutBGR = new Image<Bgr, byte>(imgIn.Size);

            //clahe filter must be applied on luminance channel or grayscale image
            CvInvoke.CvtColor(imgIn, imglabcol, ColorConversion.Bgr2Lab, 0);

            CvInvoke.CLAHE(imglabcol[0], clipLimit, new Size(tileSize, tileSize), imgoutL);
            imglabcol[0] = imgoutL; // write clahe results on Lum channel into image

            CvInvoke.CvtColor(imglabcol, imgoutBGR, ColorConversion.Lab2Bgr, 0);

            return imgoutBGR;
        }
        private void hsvHisto()
        {

            int hue = 0; int saturation = 0; int value = 0;
            // var imageT = Apply_CLAHE(img);
            var imageT = Dehazing.Dehaze(img);
            // img._GammaCorrect(0.8d);

            Image<Hsv, byte> hsvImage = imageT.Convert<Hsv, byte>();
            //  hsvImage._EqualizeHist();
            Image<Gray, byte>[] channels = hsvImage.Split();

            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();
            chartControl1.Titles.Add(new ChartTitle { Text = "HSV Histogram" });

            // Histogram calculation and display for each channel
            string[] channelNames = { "Hue", "Saturation", "Value" };
            //string[] channelNames = { "Hue" };
            for (int i = 0; i < 3; i++)
            {
                // Calculate histogram
                DenseHistogram histogram = new DenseHistogram(10, new RangeF(0, 256));
                histogram.Calculate(new Image<Gray, byte>[] { channels[i] }, false, null);

                // Convert histogram to Mat for data access
                Mat histMat = new Mat();
                histogram.CopyTo(histMat);

                // Create a series for each channel
                Series series = new Series(channelNames[i], ViewType.Bar);


                unsafe
                {
                    float* histData = (float*)histMat.DataPointer;

                    // Find the bin with the highest value (dominant value)
                    int dominantValueIndex = -1;
                    float maxBinValue = float.MinValue;

                    for (int j = 0; j < 10; j++)
                    {
                        double histValue = histData[j];
                        series.Points.Add(new SeriesPoint(j * (256 / 10), histValue));

                        if (histValue > maxBinValue)
                        {
                            maxBinValue = (float)histValue;
                            dominantValueIndex = j * (256 / 10);
                        }
                    }

                    // Output the dominant value for the current channel
                    Console.WriteLine($"Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex}");
                    // textEdit1.Text = $" || Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex} ||";
                    if (i == 0) { hue = dominantValueIndex; }
                    else if (i == 1) { saturation = dominantValueIndex; }
                    else { value = dominantValueIndex; }

                }
                Console.WriteLine($"{hue},{saturation},{value}");
                Vector3 vect = new Vector3((float)hue, (float)saturation, (float)value);
                var co = GetColorNameFromHSV2((float)hue, (float)saturation, (float)value);
                //ColorReference nearestColor = GetClosestColorWithHSV(vect);
                //Console.WriteLine(nearestColor.Name);
                Console.WriteLine(co);
                textEdithsv.Text = $"{hue},{saturation},{value}||{co}";
                chartControl1.Series.Add(series);
            }
        }
        private void Transform(Image<Bgr, byte> imageT)
        {

            int red = 0;
            int green = 0;
            int blue = 0;


            // Image<Gray, byte>[] channelsT = img.Split();
            Image<Gray, byte>[] channelsT = imageT.Split();
            Bitmap bitmap1 = imageT.ToBitmap();
            pictureBoxHistogramme.Image = imageT.Bitmap;

            // Split into channels


            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();
            chartControl1.Titles.Add(new ChartTitle { Text = "Color Histogram" });

            // Histogram calculation and display for each channel
            string[] channelNames = { "Blue", "Green", "Red", };
            int numQuantizedBins = 10;
            int binSize = 256 / numQuantizedBins; // Calculate the size of each bin in the quantized histogram
                                                  // Modify the loop for histogram calculation to use quantized bins
            for (int i = 0; i < 3; i++)
            {
                //Calculate histogram with reduced bins
                DenseHistogram histogram = new DenseHistogram(numQuantizedBins, new RangeF(0, 256));
                histogram.Calculate(new Image<Gray, byte>[] { channelsT[i] }, false, null);

                Mat histMat = new Mat();
                histogram.CopyTo(histMat);

                Series series = new Series(channelNames[i], ViewType.Bar);

                unsafe
                {
                    float* histData = (float*)histMat.DataPointer;

                    int dominantValueIndex = -1;
                    float maxBinValue = float.MinValue;

                    // Adjust loop for the reduced number of bins
                    for (int j = 0; j < numQuantizedBins; j++)
                    {
                        double histValue = histData[j];
                        // Adjust the display to show the quantized bins
                        series.Points.Add(new SeriesPoint(j * binSize, histValue)); // Use j*binSize to approximate original histogram position

                        if (histValue > maxBinValue)
                        {
                            maxBinValue = (float)histValue;
                            dominantValueIndex = j * binSize; // Approximate the dominant value's original position
                        }
                    }

                    // Output the dominant value for the current channel
                    Console.WriteLine($"Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex}");
                    switch (i)
                    {
                        case 0: // Blue
                            blue = dominantValueIndex;
                            //redPercentage = percentage;
                            break;
                        case 1: // Green
                            green = dominantValueIndex;
                            //greenPercentage = percentage;
                            break;
                        case 2: // red
                            red = dominantValueIndex;
                            //bluePercentage = percentage;
                            break;
                    }
                    //textEdit1.Text = $" || Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex} ||";

                }

                // Customize series view settings as needed
                series.View.Color = i == 0 ? System.Drawing.Color.Blue : i == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                Console.WriteLine($"{red},{green},{blue}");
                vect = new Vector3((float)red, (float)green, (float)blue);
                //ColorReference nearestColor = GetClosestColor(vect);
                //Console.WriteLine(nearestColor.Name);
                //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor.Name}";
                chartControl1.Series.Add(series);
            }
            var color = GetBestMatchColor(vect, colorReferences);
            Console.WriteLine("matched color is  : " + color);
            textEditRGB.Text = $"{red},{green},{blue}||{color}";
            // nearestColor1 = GetClosestColor(vect);
            //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor1.Name}";


        }
       
        private string TransformI(Image<Bgr, byte> imageT)
        {

            int red = 0;
            int green = 0;
            int blue = 0;


            // Image<Gray, byte>[] channelsT = img.Split();
            Image<Gray, byte>[] channelsT = imageT.Split();
            Bitmap bitmap1 = imageT.ToBitmap();
            pictureBoxHistogramme.Image = imageT.Bitmap;

            // Split into channels


            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();
            chartControl1.Titles.Add(new ChartTitle { Text = "Color Histogram" });

            // Histogram calculation and display for each channel
            string[] channelNames = { "Blue", "Green", "Red", };
            int numQuantizedBins = 10;
            int binSize = 256 / numQuantizedBins; // Calculate the size of each bin in the quantized histogram
                                                  // Modify the loop for histogram calculation to use quantized bins
            for (int i = 0; i < 3; i++)
            {
                //Calculate histogram with reduced bins
                DenseHistogram histogram = new DenseHistogram(numQuantizedBins, new RangeF(0, 256));
                histogram.Calculate(new Image<Gray, byte>[] { channelsT[i] }, false, null);

                Mat histMat = new Mat();
                histogram.CopyTo(histMat);

                Series series = new Series(channelNames[i], ViewType.Bar);

                unsafe
                {
                    float* histData = (float*)histMat.DataPointer;

                    int dominantValueIndex = -1;
                    float maxBinValue = float.MinValue;

                    // Adjust loop for the reduced number of bins
                    for (int j = 0; j < numQuantizedBins; j++)
                    {
                        double histValue = histData[j];
                        // Adjust the display to show the quantized bins
                        series.Points.Add(new SeriesPoint(j * binSize, histValue)); // Use j*binSize to approximate original histogram position

                        if (histValue > maxBinValue)
                        {
                            maxBinValue = (float)histValue;
                            dominantValueIndex = j * binSize; // Approximate the dominant value's original position
                        }
                    }

                    // Output the dominant value for the current channel
                    Console.WriteLine($"Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex}");
                    switch (i)
                    {
                        case 0: // Blue
                            blue = dominantValueIndex;
                            //redPercentage = percentage;
                            break;
                        case 1: // Green
                            green = dominantValueIndex;
                            //greenPercentage = percentage;
                            break;
                        case 2: // red
                            red = dominantValueIndex;
                            //bluePercentage = percentage;
                            break;
                    }
                    //textEdit1.Text = $" || Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex} ||";

                }

                // Customize series view settings as needed
                series.View.Color = i == 0 ? System.Drawing.Color.Blue : i == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                Console.WriteLine($"{red},{green},{blue}");
                vect = new Vector3((float)red, (float)green, (float)blue);
                //ColorReference nearestColor = GetClosestColor(vect);
                //Console.WriteLine(nearestColor.Name);
                //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor.Name}";
                chartControl1.Series.Add(series);
            }
            var color = GetBestMatchColor(vect, colorReferences);
            Console.WriteLine("matched color is  : " + color);
            textEditRGB.Text = $"{red},{green},{blue}||{color}";
            // nearestColor1 = GetClosestColor(vect);
            //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor1.Name}";

            return color;
        }
        private string TransformIBeforeYoloWorld(Image<Bgr, byte> imageT)
        {

            int red = 0;
            int green = 0;
            int blue = 0;


            // Image<Gray, byte>[] channelsT = img.Split();
            Image<Gray, byte>[] channelsT = imageT.Split();
            Bitmap bitmap1 = imageT.ToBitmap();
            pictureBoxHistogramme.Image = imageT.Bitmap;

            // Split into channels


            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();
            chartControl1.Titles.Add(new ChartTitle { Text = "Color Histogram" });

            // Histogram calculation and display for each channel
            string[] channelNames = { "Blue", "Green", "Red", };
            int numQuantizedBins = 10;
            int binSize = 256 / numQuantizedBins; // Calculate the size of each bin in the quantized histogram
                                                  // Modify the loop for histogram calculation to use quantized bins
            for (int i = 0; i < 3; i++)
            {
                //Calculate histogram with reduced bins
                DenseHistogram histogram = new DenseHistogram(numQuantizedBins, new RangeF(0, 256));
                histogram.Calculate(new Image<Gray, byte>[] { channelsT[i] }, false, null);

                Mat histMat = new Mat();
                histogram.CopyTo(histMat);

                Series series = new Series(channelNames[i], ViewType.Bar);

                unsafe
                {
                    float* histData = (float*)histMat.DataPointer;

                    int dominantValueIndex = -1;
                    float maxBinValue = float.MinValue;

                    // Adjust loop for the reduced number of bins
                    for (int j = 0; j < numQuantizedBins; j++)
                    {
                        double histValue = histData[j];
                        // Adjust the display to show the quantized bins
                        series.Points.Add(new SeriesPoint(j * binSize, histValue)); // Use j*binSize to approximate original histogram position

                        if (histValue > maxBinValue)
                        {
                            maxBinValue = (float)histValue;
                            dominantValueIndex = j * binSize; // Approximate the dominant value's original position
                        }
                    }

                    // Output the dominant value for the current channel
                    Console.WriteLine($"Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex}");
                    switch (i)
                    {
                        case 0: // Blue
                            blue = dominantValueIndex;
                            //redPercentage = percentage;
                            break;
                        case 1: // Green
                            green = dominantValueIndex;
                            //greenPercentage = percentage;
                            break;
                        case 2: // red
                            red = dominantValueIndex;
                            //bluePercentage = percentage;
                            break;
                    }
                    //textEdit1.Text = $" || Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex} ||";

                }

                // Customize series view settings as needed
                series.View.Color = i == 0 ? System.Drawing.Color.Blue : i == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                Console.WriteLine($"{red},{green},{blue}");
                vect = new Vector3((float)red, (float)green, (float)blue);
                //ColorReference nearestColor = GetClosestColor(vect);
                //Console.WriteLine(nearestColor.Name);
                //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor.Name}";
                chartControl1.Series.Add(series);
            }
            string yolo1 = "";
            var color = GetBestMatchColor(vect, colorReferences);
            Console.WriteLine("matched color is  : " + color);
            textEditRGB.Text = $"{red},{green},{blue}||{color}";
            vPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
            vDetector = new YoloDetector<TrackObject>(vPredictor, null, "red car, blue car, white car, black car, green car, yellow car");
            var vehicles = vDetector.Detect(bitmap1, 0.35f);

            if (vehicles.Count() != 0)
            {
                yolo1 = vehicles.First().Type.Name;
                Console.WriteLine(vehicles.First().Type.Name);
            }
            else { yolo1 = "no detection"; }
            // nearestColor1 = GetClosestColor(vect);
            //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor1.Name}";

            return yolo1;
        }
        private ColorReference GetClosestColor(Vector3 currentColor)
        {
            ColorReference closestColor = null;
            float minDist = float.PositiveInfinity;

            foreach (ColorReference referenceColor in colorReferences)
            {
                float dist = Vector3.Distance(referenceColor.Argb, currentColor);
                if (dist < minDist)
                {
                    closestColor = referenceColor;
                    minDist = dist;
                }
            }

            return closestColor;
        }
        private void SetColorVector(Vector3 colorVector)
        {
            string colorName = textEditColor.Text;
            Palette p = server.Colors.FirstOrDefault(_ => _.ColorName.ToLower() == colorName.ToLower());
            if (p == null)
            {
                p = new Palette(colorVector, colorName);
                server.Colors.Add(p);
            }
            else
            {
                server.Colors.Add(p);
            }

            JSON.Save(server, "ColorConfig");

            vect = new Vector3();
        }
        public String GetBestMatchColor(Vector3 unkownColor, ColorReference[] colors)
        {
            if (colors == null)
                return null;
            var mapList = new MapList();

            foreach (var color in colors.Where(_ => _.Argb != null))
            {

                double dist = Vector3.Distance(unkownColor, color.Argb);
                //double dist = CosineSimilarity(unkownColor, color.Argb);
                mapList.Maps.Add(new ColorMap(unkownColor, color.Argb, color, dist));


            }
            mapList.ShowList();
            var first = mapList.Maps.OrderBy(_ => _.Distance).First();

            //var first = mapList.Maps.Where(_ => _.Distance > 0.6).OrderBy(_ => _.Distance).Last();

            return first.Reference.Name;



        }
        public static string GetColorNameFromHSV2(double hue, double saturation, double value)
        {
            // Achromatic colors (no hue)
            if (value == 0) return "Black";
            if (saturation <= 10) // Adjust the threshold as necessary
            {
                if (value <= 10) return "Black";
                else if (value > 80) return "White"; // Consider removing hue check for white, as achromatic colors technically have no hue
                else return "Gray";
            }

            // Chromatic colors (colors with hue)
            // Adjusting the hue ranges according to the provided description
            // Red
            if ((hue >= 0 && hue <= 60))
            {
                if (value <= 10) return "Dark Red";
                else if (value > 90) return "Light Red";
                else return "Red";
            }

            // Yellow
            if (hue >= 61 && hue <= 120)
            {
                if (value <= 10) return "Dark Yellow";
                else if (value > 90) return "Light Yellow";
                else return "Yellow";
            }

            // Green
            if (hue >= 121 && hue <= 180)
            {
                if (value <= 10) return "Dark Green";
                else if (value > 90) return "Light Green";
                else return "Green";
            }

            // Cyan
            if (hue >= 181 && hue <= 240)
            {
                if (value <= 10) return "Dark Cyan";
                else if (value > 90) return "Light Cyan";
                else return "Cyan";
            }

            // Blue
            if (hue >= 241 && hue <= 300)
            {
                if (value <= 10) return "Dark Blue";
                else if (value > 90) return "Light Blue";
                else return "Blue";
            }

            // Magenta
            if (hue >= 301 && hue <= 360)
            {
                if (value <= 10) return "Dark Magenta";
                else if (value > 90) return "Light Magenta";
                else return "Magenta";
            }

            return "Unknown";
        }
        private static readonly ColorReference[] colorReferences = {

        new ColorReference() { Name="Red", Argb=new Vector3(255,0,0) },
        new ColorReference() { Name="Red", Argb=new Vector3(100,0,0) },
        new ColorReference() { Name="Red", Argb=new Vector3(255,100,100) },
        new ColorReference() { Name="Red", Argb=new Vector3(255,192,192) },
        new ColorReference() { Name="Red", Argb=new Vector3(204,0,0)},
        new ColorReference() { Name="Red", Argb=new Vector3(179,27,27)},
        new ColorReference() { Name="Red", Argb=new Vector3(164,0,0)},
        new ColorReference() { Name="Red", Argb=new Vector3(128,0,0)},
        new ColorReference() { Name="Red", Argb=new Vector3(178,34,34) },
        new ColorReference() { Name="Red", Argb=new Vector3(150,0,24) },
        new ColorReference() { Name="Red", Argb=new Vector3(101,0,11) },
        new ColorReference() { Name="Red", Argb=new Vector3(48,0,16) },
        new ColorReference() { Name="Red", Argb=new Vector3(100,50,50) },
        new ColorReference() { Name="Red", Argb=new Vector3(225,50,75) },


        new ColorReference() { Name="Magenta", Argb=new Vector3(139,0,139) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(112,41,99) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(135,38,87) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(255,0,127) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(105,53,156) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(143,0,255 ) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(100,50,100) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(75,25,50) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(75,50,75) },
        new ColorReference() { Name="Magenta", Argb=new Vector3(100,75,75) },
        //new ColorReference() { Name="Magenta", Argb=new Vector3(101,0,11) },
        //new ColorReference() { Name="Magenta", Argb=new Vector3(48,0,16) },


       // Green shades
        new ColorReference() { Name="Green shades", Argb=new Vector3(0,255,0) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(0,100,0) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(100,255,100) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(0,100,100) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(0,192,0)},
        new ColorReference() { Name="Green shades", Argb=new Vector3(19,136,8) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(85,107,47) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(128,128,0) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(75,83,32) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(1,68,33) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(0,66,37) },
        new ColorReference() { Name="Green shades", Argb=new Vector3(1,50,32) },

        new ColorReference() { Name="Blue", Argb=new Vector3(0,0,255)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,0,100)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,0,192)},
        new ColorReference() { Name="Blue", Argb=new Vector3(100,100,255)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,47,167)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,51,153)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,35,102)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,71,171)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,127,255)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,0,128)},
        new ColorReference() { Name="Blue", Argb=new Vector3(0,123,167)},
        new ColorReference() { Name="Blue", Argb=new Vector3(25,25,50)},
        new ColorReference() { Name="Blue", Argb=new Vector3(50,50,75)},
        new ColorReference() { Name="Blue", Argb=new Vector3(75,125,150)},

        new ColorReference() { Name="Yellow", Argb=new Vector3(233,228,98)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(192,192,0)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(255,255,192)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(146,146,0)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(228,208,10)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(255,223,0)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(255,191,0)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(236,213,64)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(128,128,0)},
        new ColorReference() { Name="Yellow", Argb=new Vector3(181,166,66)},
        new ColorReference() { Name= "Yellow", Argb=new Vector3(225,100,50)},



        new ColorReference() { Name="Black", Argb=new Vector3(0,0,0) },
        new ColorReference() { Name="Black", Argb=new Vector3(25,25,25)},
        new ColorReference() { Name="DarkGray", Argb=new Vector3(50,50,50) },
        // new ColorReference(){ Name="Black", Argb=new Vector3(50,50,75) },
        //new ColorReference() { Name="Black", Argb=new Vector3(0, 0, 0) },
        //new ColorReference() { Name="Black", Argb=new Vector3(0, 0, 0) },

        new ColorReference() { Name="White", Argb=new Vector3(255,255,255) },
        new ColorReference() { Name="White", Argb=new Vector3(255,248,220) },
        new ColorReference() { Name="White", Argb=new Vector3(255,240,245) },
        new ColorReference() { Name="White", Argb=new Vector3(248,248,255) },
        new ColorReference() { Name="White", Argb=new Vector3(225,225,225) },

        new ColorReference() { Name="Gray", Argb=new Vector3(169,169,169) },
        new ColorReference() { Name="Gray", Argb=new Vector3(229,229,229) },
        new ColorReference() { Name="Gray", Argb=new Vector3(192,192,192) },
        new ColorReference() { Name="Gray", Argb=new Vector3(128,128,128) },
        new ColorReference() { Name="Gray", Argb=new Vector3(100,100,100) },
        new ColorReference() { Name="Gray", Argb=new Vector3(178,190,181) },

        new ColorReference() { Name="Gray", Argb=new Vector3(75,75,75) },
        //new ColorReference() { Name="DarkGray", Argb=new Vector3(25, 50, 50)},
        //new ColorReference() { Name="Gray", Argb=new Vector3(54,69,79)},
        //new ColorReference() { Name="Gray", Argb=new Vector3(59,68,75)},
        //new ColorReference() { Name="Gray", Argb=new Vector3(132,132,130)},
        //new ColorReference() { Name="Gray", Argb=new Vector3(72,60,50)},
        };


        private void simpleButton2_Click(object sender, EventArgs e)
        {
            vect = new Vector3();
            var Image = img.Copy();
            var imageT = Apply_CLAHE(Image);
            pictureBoxCLAHE.Image = imageT.Bitmap;
            vPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
            vDetector = new YoloDetector<TrackObject>(vPredictor, null, "red car, blue car, white car, black car, green car, yellow car");
            var vehicles = vDetector.Detect(imageT.Bitmap, 0.35f);
            if (vehicles.Count() != 0)
                Console.WriteLine(vehicles.First().Type.Name);
            Transform(imageT);
        }

        private void simpleButton_Click(object sender, EventArgs e)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                var Image = img.Copy();


                Image._EqualizeHist();
                Bitmap bitmap1 = Image.ToBitmap();
                CPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\carModels_S_Y8", "DML");
                CDetector = new YoloDetector<TrackObject>(CPredictor, null, "car");
                IEnumerable<Object> car = CDetector.Detect(bitmap1);
                // Create the new bitmap and the graphics object
                if (car != null)
                {
                    Bitmap croppedBitmap = new Bitmap(car.First().Rectangle.Width, car.First().Rectangle.Height);
                    using (Graphics g = Graphics.FromImage(croppedBitmap))
                    {
                        // Draw the specified section of the source bitmap to the new one
                        g.DrawImage(bitmap1, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height),
                                    car.First().Rectangle, GraphicsUnit.Pixel);
                    }

                    // Save the cropped image to a file
                    croppedBitmap.Save(@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\path_to_save_cropped_image.jpg");
                }
                else { XtraMessageBox.Show("car is null");}
                //Rectangle cropRect = new Rectangle(10,10, 250,250);

                //// Create the new bitmap and the graphics object
                //Bitmap croppedBitmap = new Bitmap(cropRect.Width, cropRect.Height);
                //using (Graphics g = Graphics.FromImage(croppedBitmap))
                //{
                //    // Draw the specified section of the source bitmap to the new one
                //    g.DrawImage(bitmap1, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height),
                //                cropRect, GraphicsUnit.Pixel);
                //}

                //// Save the cropped image to a file
                //croppedBitmap.Save(@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\path_to_save_cropped_image.jpg");
                //vPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
                //vDetector = new YoloDetector<TrackObject>(vPredictor, null, "red color, blue color, white color, black color, green color, yellow color");
                //var vehicles = vDetector.Detect(Image.ToBitmap(), 0.1f);

                pictureBoxHistogramme.Image = Image.Bitmap;
                //if (vehicles.Count() != 0)
                //    Console.WriteLine(vehicles.First().Type.Name);

                // Split into channels
                Image<Gray, byte>[] channels = Image.Split();

                chartControl1.Series.Clear();
                chartControl1.Titles.Clear();
                chartControl1.Titles.Add(new ChartTitle { Text = "Color Histogram" });

                // Histogram calculation and display for each channel
                string[] channelNames = { "Blue", "Green", "Red", };
                int numQuantizedBins = 10;
                int binSize = 256 / numQuantizedBins; // Calculate the size of each bin in the quantized histogram
                // Modify the loop for histogram calculation to use quantized bins
                for (int i = 0; i < 3; i++)
                {
                    //Calculate histogram with reduced bins
                    DenseHistogram histogram = new DenseHistogram(numQuantizedBins, new RangeF(0, 256));
                    histogram.Calculate(new Image<Gray, byte>[] { channels[i] }, false, null);

                    Mat histMat = new Mat();
                    histogram.CopyTo(histMat);

                    Series series = new Series(channelNames[i], ViewType.Bar);

                    unsafe
                    {
                        float* histData = (float*)histMat.DataPointer;

                        int dominantValueIndex = -1;
                        float maxBinValue = float.MinValue;

                        // Adjust loop for the reduced number of bins
                        for (int j = 0; j < numQuantizedBins; j++)
                        {
                            double histValue = histData[j];
                            // Adjust the display to show the quantized bins
                            series.Points.Add(new SeriesPoint(j * binSize, histValue)); // Use j*binSize to approximate original histogram position

                            if (histValue > maxBinValue)
                            {
                                maxBinValue = (float)histValue;
                                dominantValueIndex = j * binSize; // Approximate the dominant value's original position
                            }
                        }

                        // Output the dominant value for the current channel
                        Console.WriteLine($"Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex}");
                        switch (i)
                        {
                            case 0: // Blue
                                blue = dominantValueIndex;
                                //redPercentage = percentage;
                                break;
                            case 1: // Green
                                green = dominantValueIndex;
                                //greenPercentage = percentage;
                                break;
                            case 2: // red
                                red = dominantValueIndex;
                                //bluePercentage = percentage;
                                break;
                        }
                        //textEdit1.Text = $" || Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex} ||";

                    }

                    // Customize series view settings as needed
                    series.View.Color = i == 0 ? System.Drawing.Color.Blue : i == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                    Console.WriteLine($"{red},{green},{blue}");
                    vect = new Vector3((float)red, (float)green, (float)blue);
                    //ColorReference nearestColor = GetClosestColor(vect);
                    //Console.WriteLine(nearestColor.Name);
                    //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor.Name}";
                    chartControl1.Series.Add(series);
                }
                //this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormVisualize_Paint);
            }
            var color = GetBestMatchColor(vect, colorReferences);
            Console.WriteLine("matched color is  : " + color);

            textEditRGB.Text = $"{red},{green},{blue}||{color}";
            //string colorName = ColorRecognizer.RecognizeColor();
            //Console.WriteLine($"The color is: {colorName}");
            //ColorReference nearestColor = GetClosestColor(vect);
            //Console.WriteLine(nearestColor.Name);
            //textEditRGB.Text = $"{red},{green},{blue}||{nearestColor.Name}";





            vect = new Vector3();

            //var output = GetBestMatchColor(vect, server.Colors);
            //Console.WriteLine(output);
            //foreach (var colorRange in colorRanges)
            //{
            //    if (colorRange.Contains(vect))
            //    {
            //        Console.WriteLine($"The color {vect} is {colorRange.ColorName}.");
            //        break;
            //    }
            //    else
            //    {
            //        Console.WriteLine($"The color {vect} isn't found.");

            //    }
            //}
            // hsvHisto();

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            vect = new Vector3();
            var Image = img.Copy();
            var i = Dehazing.Dehaze(Image);
            pictureBoxDEHAZE.Image = i.Bitmap;
            vPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
            vDetector = new YoloDetector<TrackObject>(vPredictor, null, "red car, blue car, white car, black car, green car, yellow car");
            var vehicles = vDetector.Detect(i.Bitmap, 0.35f);
            if (vehicles.Count() != 0)
                Console.WriteLine(vehicles.First().Type.Name);
            Transform(i);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SetColorVector(vect);
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            CreateTestSet();
        }

        private void LoadModel()
        {
            vPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
            vDetector = new YoloDetector<TrackObject>(vPredictor, null, "red car, blue car, white car, black car, green car, yellow car");
            var vehicles = vDetector.Detect(img.Bitmap, 0.35f);
        }

        private void LoadColorModel()
        {
            vPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
            vDetector = new YoloDetector<TrackObject>(vPredictor, null, "red, blue, white, black, green, yellow");
            var vehicles = vDetector.Detect(img.Bitmap, 0.35f);
        }
        public  IObject ToIObject(Object obj)
        {
            IObject iobj = obj as IObject;
            return iobj;
        }
        public IList<IObject> ToIObjects( IEnumerable<Object> objects)
        {
            IList<IObject> objs = new List<IObject>();
            foreach (Object obj in objects)
            {
                objs.Add(ToIObject(obj));
            }
            return objs;
        }
      
        private void CreateTestSet()
            {
            //LoadModel();
            // LoadColorModel();


            string[] directories = Directory.GetDirectories($@"E:\AVC-test\Trace\Kaggle dataset\YELLOWColor");
            int id = 0;
            int ACC = 0;
            int notACC = 0;
            foreach (string subdir in directories)

            {
                string[] imageFiles = null;
                id++;
                
                // Console.WriteLine(subdir);
                int dernierIndexBackslash = subdir.LastIndexOf("\\");

                // Extraire la partie du chemin après le dernier backslash
                string name = subdir.Substring(dernierIndexBackslash + 1);
                Console.WriteLine("Nom du dossier extrait : " + name);


                imageFiles = Directory.GetFiles(subdir, "*.jpg");

                for (var i = 0; i < imageFiles.Count(); i++)
                {
                    var ImagePath = imageFiles[i];
                    Console.WriteLine(ImagePath);
                    using (FileStream stream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read))
                    {
                        var Image = (Bitmap)System.Drawing.Image.FromStream(stream);
                        if (Image != null)
                        {
                            CPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\carModels_S_Y8", "DML");
                            CDetector = new YoloDetector<TrackObject>(CPredictor, null, "car");
                            IEnumerable<Object> car = CDetector.Detect(Image);
                            // Create the new bitmap and the graphics object
                            if (car.Count()!=0)
                            {
                                Bitmap croppedBitmap = new Bitmap(car.First().Rectangle.Width, car.First().Rectangle.Height);
                                using (Graphics g = Graphics.FromImage(croppedBitmap))
                                {
                                    // Draw the specified section of the source bitmap to the new one
                                    g.DrawImage(Image, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height),
                                                car.First().Rectangle, GraphicsUnit.Pixel);
                                }

                                // Save the cropped image to a file
                                croppedBitmap.Save($@"E:\AVC-test\Trace\colors\{i}.jpg");
                            }
                            else 
                            {
                                //XtraMessageBox.Show("car is null");
                                continue;
                            }
                            //////

                            // vect = new Vector3();
                            // var img = new Image<Bgr, byte>(Image) ;
                            //// var Image1 = img.Copy();
                            // var ii = Dehazing.Dehaze(img);
                            // string yolo1 = "";
                            // //pictureBoxDEHAZE.Image = ii.Bitmap;
                            // vPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
                            // vDetector = new YoloDetector<TrackObject>(vPredictor, null, "red car, blue car, white car, black car, green car, yellow car");
                            // //var vehicles = vDetector.Detect(ii.Bitmap, 0.35f);
                            // var vehicles = vDetector.Detect(img.Bitmap, 0.35f);
                            // //var vehicles = vDetector.Detect(car.First().Image, 0.35f);
                            // if (vehicles.Count() != 0)
                            // {
                            //     yolo1 = vehicles.First().Type.Name;
                            //     Console.WriteLine(vehicles.First().Type.Name); }
                            // else { yolo1 = "no detection"; } 

                            // var color=TransformI(img);
                           
                            //CarTrace cars = new CarTrace(id,name, i + 1, TransformI(img), TransformIBeforeYoloWorld(img), yolo1);

                            ////
                            //UserFace userface = new UserFace(i + 1, user.Id, Unknownfaces.First().DoubleEncoding);
                            //string nomFichier = Path.GetFileName(ImagePath);
                            //userface.ImageTitle = nomFichier;
                            //user.Faces.Add(userface);



                            //server.CarTraceList.Add(cars);
                        }
                        else { continue; }

                    }
                    //Console.WriteLine($@"{name} |+ predection {ACC}|- predection {notACC}");
                }

                imageFiles = null;
                JSON.Save(server, "ColorConfig");
                
            }
            carTraceBindingSource.DataSource = server.CarTraceList;
            XtraMessageBox.Show("Dataset explored");


        }

    }








    public class CarColorRecognitionEvaluator
    {
        private static readonly string[] Colors = { "Red", "Blue", "Green", "Yellow", "White", "Black" };
        private static readonly Dictionary<string, int> ColorToIndex = new Dictionary<string, int>
    {
        { "Red", 0 },
        { "Blue", 1 },
        { "Green", 2 },
        { "Yellow", 3 },
        { "White", 4 },
        { "Black", 5 }
    };

        private void EvaluateAlgorithm()
        {
            int[,] confusionMatrix = new int[6, 6];

            Parallel.ForEach(Colors, colorFolder =>
            {
                List<Image> images = LoadImages(colorFolder);

                foreach (Image image in images)
                {
                    using (Bitmap bitmap = new Bitmap(image))
                    {
                        Image<Bgr, byte> emguImage = new Image<Bgr, byte>(bitmap);

                        string predictedColor = TransformI(emguImage);
                        string trueColor = colorFolder;

                        int trueIndex = ColorToIndex[trueColor];
                        int predictedIndex = ColorToIndex[predictedColor];

                        // Use thread-safe operation for updating confusion matrix
                        lock (confusionMatrix)
                        {
                            confusionMatrix[trueIndex, predictedIndex]++;
                        }
                    }
                }
            });

            PrintConfusionMatrix(confusionMatrix);
        }

        private List<Image> LoadImages(string colorFolder)
        {
            string baseDirectory = @"E:\AVC-test\Trace\Kaggle dataset\YELLOWColor";
            List<Image> list = new List<Image>();

            try
            {
                string[] directories = Directory.GetDirectories(baseDirectory);

                foreach (string subdir in directories)
                {
                    string folderName = Path.GetFileName(subdir);

                    if (folderName.Equals(colorFolder, StringComparison.OrdinalIgnoreCase))
                    {
                        string[] imageFiles = Directory.GetFiles(subdir, "*.jpg");

                        foreach (string imagePath in imageFiles)
                        {
                            using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                var image = Image.FromStream(stream);

                                if (image != null)
                                {
                                    list.Add(image);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return list;
        }

        private void PrintConfusionMatrix(int[,] confusionMatrix)
        {
            Console.WriteLine("Confusion Matrix:");
            Console.Write("        ");
            foreach (var color in Colors)
            {
                Console.Write($"{color,-6} ");
            }
            Console.WriteLine();

            for (int i = 0; i < Colors.Length; i++)
            {
                Console.Write($"{Colors[i],-6} ");
                for (int j = 0; j < Colors.Length; j++)
                {
                    Console.Write($"{confusionMatrix[i, j],-6} ");
                }
                Console.WriteLine();
            }
        }

        // Placeholder for your prediction method
        private string TransformI(Image<Bgr, byte> image)
        {
            // Your color prediction logic here
            // Return the predicted color as a string
            return "Red"; // Example placeholder return value
        }
    }
        public class ColorMap
    {
        public Vector3 UnknownVector { get; set; }
        public Vector3 KnownVector { get; set; }
      
        public double Distance { get; set; }  
        public Palette ColorP { get; set; }
        public ColorReference Reference { get; set; }

        public ColorMap(Vector3 unknownVector, Vector3 knownVector, ColorReference reference, double distance)
        {
            UnknownVector = unknownVector;
            KnownVector = knownVector;
            Reference = reference;
            Distance = distance;

        }
        public ColorMap(Vector3 unknownVector, Palette colorP, double distance)
        {
            UnknownVector = unknownVector;
            ColorP = colorP;
            Distance = distance;

        }
    }
    public class MapList
    {
        public List<ColorMap> Maps;
        public MapList()
        {
            Maps = new List<ColorMap>();
        }
        public void ShowList()
        {
            Console.WriteLine("-----------------------------------------");
            foreach (ColorMap color in Maps.OrderBy(_ => _.Distance))
            {
                Console.WriteLine($" {color.UnknownVector}  | {color?.Reference.Name} | {color?.Reference.Argb}|{color.Distance}");
            }
            Console.WriteLine("-----------------------------------------");
        }

    }
    public class ColorReference
    {
        public string Name { get; set; }
        public Vector3 Argb { get; set; }
        public Vector3 Hsv { get; set; }
    }
    public class ColorRange
    {
        public string ColorName { get; set; }
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        public ColorRange(string colorName, Vector3 min, Vector3 max)
        {
            ColorName = colorName;
            Min = min;
            Max = max;
        }

        public bool Contains(Vector3 color)
        {
            return color.X >= Min.X && color.X <= Max.X &&
                   color.Y >= Min.Y && color.Y <= Max.Y &&
                   color.Z >= Min.Z && color.Z <= Max.Z;
        }
    }

    public class ColorRangeRGB
    {
        public string Name { get; set; }
        public (int min, int max) R { get; set; }
        public (int min, int max) G { get; set; }
        public (int min, int max) B { get; set; }

        public bool IsWithinRange(Color color)
        {
            return color.R >= R.min && color.R <= R.max &&
                   color.G >= G.min && color.G <= G.max &&
                   color.B >= B.min && color.B <= B.max;
        }
    }
    public class ColorRecognizer
    {
        private static readonly List<ColorRangeRGB> colorRanges = new List<ColorRangeRGB>
    {
        // Primary Colors
        new ColorRangeRGB { Name = "Red", R = (150, 255), G = (0, 100), B = (0, 100) },
        new ColorRangeRGB { Name = "Green", R = (0, 100), G = (150, 255), B = (0, 100) },
        new ColorRangeRGB { Name = "Blue", R = (0, 100), G = (0, 100), B = (150, 255) },
        
        // Secondary Colors
        new ColorRangeRGB { Name = "Yellow", R = (200, 255), G = (200, 255), B = (0, 150) },
        new ColorRangeRGB { Name = "Cyan", R = (0, 150), G = (200, 255), B = (200, 255) },
        new ColorRangeRGB { Name = "Magenta", R = (200, 255), G = (0, 150), B = (200, 255) },

        // Tertiary Colors
        new ColorRangeRGB { Name = "Orange", R = (200, 255), G = (100, 180), B = (0, 100) },
        new ColorRangeRGB { Name = "Purple", R = (150, 200), G = (0, 100), B = (150, 200) },

        // Shades
        new ColorRangeRGB { Name = "White", R = (200, 255), G = (200, 255), B = (200, 255) },
        new ColorRangeRGB { Name = "Gray", R = (100, 200), G = (100, 200), B = (100, 200) },
        new ColorRangeRGB { Name = "Black", R = (0, 50), G = (0, 50), B = (0, 50) },

        // Add more colors as needed
    };

        public static string RecognizeColor(Color color)
        {
            foreach (var range in colorRanges)
            {
                if (range.IsWithinRange(color))
                {
                    return range.Name;
                }
            }
            return "Unknown";
        }
    }
    public static class Dehazing
    {
        public static Image<Bgr, byte> Dehaze(Image<Bgr, byte> inputImage, int windowSize = 20, double omega = 0.5, double t0 = 0.1)
        {
            // Step 1: Convert the input image to grayscale
            Image<Gray, byte> grayImage = inputImage.Convert<Gray, byte>();

            // Step 2: Compute the dark channel of the input image
            Image<Gray, byte> darkChannel = ComputeDarkChannel(grayImage, windowSize);

            // Step 3: Estimate the atmospheric light
            double atmosphericLight = EstimateAtmosphericLight(inputImage, darkChannel);

            // Step 4: Estimate the transmission map
            Image<Gray, float> transmissionMap = EstimateTransmissionMap(grayImage, atmosphericLight, omega, t0);

            // Step 5: Recover the haze-free image
            Image<Bgr, byte> dehazedImage = RecoverHazeFreeImage(inputImage, transmissionMap, atmosphericLight);

            return dehazedImage;
        }

        private static Image<Gray, byte> ComputeDarkChannel(Image<Gray, byte> grayImage, int windowSize)
        {
            Image<Gray, byte> darkChannel = new Image<Gray, byte>(grayImage.Size);

            // Iterate through each pixel in the image
            for (int y = 0; y < grayImage.Height; y++)
            {
                for (int x = 0; x < grayImage.Width; x++)
                {
                    // Define the window
                    int startX = Math.Max(0, x - windowSize / 2);
                    int startY = Math.Max(0, y - windowSize / 2);
                    int endX = Math.Min(grayImage.Width - 1, x + windowSize / 2);
                    int endY = Math.Min(grayImage.Height - 1, y + windowSize / 2);

                    //Compute the minimum intensity in the window
                    byte minIntensity = byte.MaxValue;
                    for (int i = startY; i <= endY; i++)
                    {
                        for (int j = startX; j <= endX; j++)
                        {
                            byte intensity = (byte)grayImage[i, j].Intensity;
                            if (intensity < minIntensity)
                                minIntensity = intensity;
                        }
                    }
                    darkChannel[y, x] = new Gray(minIntensity);
                }
            }

            return darkChannel;
        }

        private static double EstimateAtmosphericLight(Image<Bgr, byte> inputImage, Image<Gray, byte> darkChannel)
        {
            // Flatten the dark channel to a 1D array
            byte[] darkChannelValues = darkChannel.Bytes;

            // Compute the average of the dark channel values
            double sum = 0;
            foreach (byte value in darkChannelValues)
            {
                sum += value;
            }
            double atmosphericLight = sum / darkChannelValues.Length;

            return atmosphericLight;
        }

        private static Image<Gray, float> EstimateTransmissionMap(Image<Gray, byte> grayImage, double atmosphericLight, double omega, double t0)
        {
            Image<Gray, float> transmissionMap = new Image<Gray, float>(grayImage.Size);

            // Iterate through each pixel in that image 
            for (int y = 0; y < grayImage.Height; y++)
            {
                for (int x = 0; x < grayImage.Width; x++)
                {
                    // Compute the transmission value for each pixel
                    double transmission = 1 - omega * Math.Min(grayImage[y, x].Intensity / atmosphericLight, 1 - t0);
                    transmissionMap[y, x] = new Gray((float)transmission);
                }
            }

            return transmissionMap;
        }

        private static Image<Bgr, byte> RecoverHazeFreeImage(Image<Bgr, byte> inputImage, Image<Gray, float> transmissionMap, double atmosphericLight)
        {
            Image<Bgr, byte> dehazedImage = new Image<Bgr, byte>(inputImage.Size);

            // Iterate through each pixel in the image
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    //Compute the haze-free intensity for each channel
                    double red = (inputImage[y, x].Red - atmosphericLight) / transmissionMap[y, x].Intensity + atmosphericLight;
                    double green = (inputImage[y, x].Green - atmosphericLight) / transmissionMap[y, x].Intensity + atmosphericLight;
                    double blue = (inputImage[y, x].Blue - atmosphericLight) / transmissionMap[y, x].Intensity + atmosphericLight;

                    //Clip the values to the valid range [0,255]
                    red = Math.Max(0, Math.Min(255, red));
                    green = Math.Max(0, Math.Min(255, green));
                    blue = Math.Max(0, Math.Min(255, blue));

                    // Set the dehazed pixel values
                    dehazedImage[y, x] = new Bgr(blue, green, red);
                }
            }

            return dehazedImage;
        }
    }
    public class Palette
    {
        public Vector3 RGBvalue { get; set; }
        public string ColorName { get; set; }
        public Palette(Vector3 v, string colorName)
        {
            RGBvalue = v;
            ColorName = colorName;
        }
    }
    public class PaletteList
    {
        public List<Palette> Colors;
        public PaletteList()
        {
            Colors = new List<Palette>();
        }
    }
    public class Server
    {


        public List<Palette> Colors { get; set; }
        public List<CarTrace> CarTraceList { get; set; }

        public Server()
        {


            Colors = new List<Palette>();
            CarTraceList = new List<CarTrace>();
        }
        public void Save()
        {
            JSON.Save(this, "Config");
        }
        public static Server Load(string path = "Config")
        {
            var server = JSON.Load<Server>(path);
            return server;
        }


    }

    public class CarTrace
    {
        private Bitmap image;
      //  public string ImageTitle { get; set; }
        public string MatchedColor { get; set; }
        public string YoloWorld1ColorDetection { get; set; }
        public string YoloWorld2ColorDetection { get; set; }
        public string CurrentColor { get; set; }
        public int Id { get; set; }
        public int ImageId { get; set; }


        public CarTrace(int id,string current, int imageId, string matchedColor, string yoloworld1, string yoloworld2)
        {   this.CurrentColor = current;
            this.Id = id;
            this.ImageId = imageId;
            this.MatchedColor = matchedColor;
            this.YoloWorld1ColorDetection = yoloworld1;
            this.YoloWorld2ColorDetection = yoloworld2;
        }



    }
}