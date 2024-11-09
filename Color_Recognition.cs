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
using Smartvisor.Yolo8.Extensions;
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
    public partial class Color_Recognition : DevExpress.XtraEditors.XtraForm
    {
        private YoloPredictor vPredictor;
        private YoloDetector<TrackObject> vDetector;

        private YoloPredictor CPredictor;
        private YoloDetector<TrackObject> CDetector;
        public Image<Bgr, byte> img = null;
        public Vector3 vect = new Vector3();
        public Color_Recognition()
        {

            InitializeComponent();


        }
        //private readonly string[] Colors = { "red car", "blue car", "white car", "black car", "green car", "yellow car", "gray car" };
        private readonly string[] Colors = { "red", "blue ", "white ", "black ", "green ", "yellow ", "gray " };
        private readonly Dictionary<string, int> ColorToIndex = new Dictionary<string, int>
    {
        { "red ", 0 },
        { "blue ", 1 },
        { "white ", 2 },
        { "black ", 3 },
        { "green ", 4 },
        { "yellow ", 5 },
        { "gray ", 6 }
    };

        private void EvaluateAlgorithmAsync()
        {
            int[,] confusionMatrix = new int[7, 7];

            //await Task.Run(() =>
            //{
            //Parallel.ForEach(Colors, colorFolder =>
            //{
            List<Image> images = LoadImages("Gray");

            foreach (Image image in images)
            {
                using (Bitmap bitmap = new Bitmap(image))
                {
                    Image<Bgr, byte> emguImage = new Image<Bgr, byte>(bitmap);

                    CPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
                    CDetector = new YoloDetector<TrackObject>(CPredictor, null);
                    IEnumerable<Object> predictedColor = CDetector.Detect(bitmap);
                    string trueColor = "gray car";
                    if (predictedColor.Count() != 0)
                    {
                        int trueIndex = ColorToIndex[trueColor];
                        int predictedIndex = ColorToIndex.ContainsKey(predictedColor.First().Type.Name) ? ColorToIndex[predictedColor.First().Type.Name] : -1;

                        if (predictedIndex != -1)
                        {
                            lock (confusionMatrix)
                            {
                                confusionMatrix[trueIndex, predictedIndex]++;
                            }
                        }
                        else { continue; }
                    }


                }
            }

            //  });
            // });
            PrintConfusionMatrix(confusionMatrix);

        }
        //private void EvaluateAlgorithmAsync2()
        //{
        //    int[,] confusionMatrix = new int[7, 7];

        //    //await Task.Run(() =>
        //    //{
        //    //Parallel.ForEach(Colors, colorFolder =>
        //    //{
        //    List<Image> images = LoadImages("Gray");

        //    foreach (Image image in images)
        //    {
        //        using (Bitmap bitmap = new Bitmap(image))
        //        {
        //            Image<Bgr, byte> emguImage = new Image<Bgr, byte>(bitmap);


        //            var yolo = new Smartvisor.Benchmark.Yolo8.Yolo(@"E:\yolov8s-seg.onnx");

        //            var results = yolo.RunSegmentation(bitmap.ToImageSharp());

        //            var x = results.First().SegmentedPoints();
        //            // var predictedColor = TransformSegmented(emguImage, x);
        //            var predictedColor = DetermineObjectColors(bitmap, x);


        //            // var histogram = CalculateDenseHistogram(bitmap, results.First().SegmentedPoints());
        //            //PrintHistogram(histogram);



        //            string trueColor = "Gray";
        //            if (predictedColor.Count() != 0)
        //            {
        //                int trueIndex = ColorToIndex[trueColor];
        //                int predictedIndex = ColorToIndex.ContainsKey(predictedColor) ? ColorToIndex[predictedColor] : -1;

        //                if (predictedIndex != -1)
        //                {
        //                    lock (confusionMatrix)
        //                    {
        //                        confusionMatrix[trueIndex, predictedIndex]++;
        //                    }
        //                }
        //                else { continue; }
        //            }


        //        }
        //    }

        //    //  });
        //    // });
        //    PrintConfusionMatrix(confusionMatrix);

        //}
        private List<Image> LoadImages(string colorFolder)
        {
            string baseDirectory = @"E:\AVC-test\Trace\colors\color\color";
            List<Image> list = new List<Image>();
            List<Point[]> points = new List<Point[]>();
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
                                 Bitmap bitmap = new Bitmap(image);
                                if (image != null)
                                {
                                    var yolo = new Smartvisor.Benchmark.Yolo8.Yolo(@"E:\yolov8s-seg.onnx");

                                    //var results = yolo.RunSegmentation(bitmap.ToImageSharp());

                                    //var x = results.First().SegmentedPoints();
                                    //list.Add(image);
                                    //points.Add(x);
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
                Console.Write($"{color,-7} ");
            }
            Console.WriteLine();

            for (int i = 0; i < Colors.Length; i++)
            {
                Console.Write($"{Colors[i],-7} ");
                for (int j = 0; j < Colors.Length; j++)
                {
                    Console.Write($"{confusionMatrix[i, j],-7} ");
                }
                Console.WriteLine();
            }
        }

        //private string TransformI(Image<Bgr, byte> imageT)
        //{
        //    int red = 0;
        //    int green = 0;
        //    int blue = 0;

        //    Image<Gray, byte>[] channelsT = imageT.Split();
        //    Bitmap bitmap1 = imageT.ToBitmap();

        //    // Split into channels
        //    string[] channelNames = { "Blue", "Green", "Red" };
        //    int numQuantizedBins = 10;
        //    int binSize = 256 / numQuantizedBins;

        //    for (int i = 0; i < 3; i++)
        //    {
        //        DenseHistogram histogram = new DenseHistogram(numQuantizedBins, new RangeF(0, 256));
        //        histogram.Calculate(new Image<Gray, byte>[] { channelsT[i] }, false, null);

        //        Mat histMat = new Mat();
        //        histogram.CopyTo(histMat);

        //        Series series = new Series(channelNames[i], ViewType.Bar);

        //        unsafe
        //        {
        //            float* histData = (float*)histMat.DataPointer;

        //            int dominantValueIndex = -1;
        //            float maxBinValue = float.MinValue;

        //            for (int j = 0; j < numQuantizedBins; j++)
        //            {
        //                double histValue = histData[j];
        //                series.Points.Add(new SeriesPoint(j * binSize, histValue));

        //                if (histValue > maxBinValue)
        //                {
        //                    maxBinValue = (float)histValue;
        //                    dominantValueIndex = j * binSize;
        //                }
        //            }

        //            switch (i)
        //            {
        //                case 0:
        //                    blue = dominantValueIndex;
        //                    break;
        //                case 1:
        //                    green = dominantValueIndex;
        //                    break;
        //                case 2:
        //                    red = dominantValueIndex;
        //                    break;
        //            }
        //        }

        //        // Customize series view settings as needed
        //        series.View.Color = i == 0 ? System.Drawing.Color.Blue : i == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        //    }
        //    Vector3 vect = new Vector3((float)red, (float)green, (float)blue);
        //    string color = GetBestMatchColor(vect, colorReferences);
        //    return color;
        //}



        private string TransformI(Image<Bgr, byte> imageT)
        {

            int red = 0;
            int green = 0;
            int blue = 0;


            // Image<Gray, byte>[] channelsT = img.Split();
            Image<Gray, byte>[] channelsT = imageT.Split();
            Bitmap bitmap1 = imageT.ToBitmap();
            // pictureBoxDehaze.Image = imageT.Bitmap;

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
                    //    Console.WriteLine($"Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex}");
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
                //  Console.WriteLine($"{red},{green},{blue}");

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
        private void simpleButton_Click(object sender, EventArgs e)
        {
            // TestYolo8();
            //EvaluateAlgorithmAsync2();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                var Image = img.Copy();
                Image._EqualizeHist();
                pictureBoxDehaze.Image = img.Bitmap;
               

                CPredictor = new YoloPredictor($@"E:\smartvisorAVC\Smartvisor.Benchmark\bin\Debug\colorModels_S_Y8", "DML");
                CDetector = new YoloDetector<TrackObject>(CPredictor, null);
                IEnumerable<Object> predictedColor = CDetector.Detect(Image.Bitmap);
                textEditDEHAZE.Text = $" {predictedColor.First().Score}  | {predictedColor.First().Type.Name}";
                textEditEqui.Text = "Yellow car";
              
            }

        }
        //private void simpleButton_Click(object sender, EventArgs e)
        //{
        //    // TestYolo8();
        //    //EvaluateAlgorithmAsync2();
        //    if (openFileDialog1.ShowDialog() == DialogResult.OK)@
        //    {

        //        img = new Image<Bgr, byte>(openFileDialog1.FileName);
        //        var Image = img.Copy();
        //        Image._EqualizeHist();
        //        pictureBoxDehaze.Image = img.Bitmap;
        //        var yolo = new Smartvisor.Benchmark.Yolo8.Yolo(@"E:\yolov8s-seg.onnx");
        //        //// var image = SixLabors.ImageSharp.Image.Load<Rgba32>(@"E:\car.jpg");
        //        //    var jpeg = System.Drawing.Image.FromFile(@"E:\car.jpg");
        //        //    Image._EqualizeHist();
        //        Bitmap bitmap1 = Image.ToBitmap();
        //        textEditDEHAZE.Text = TransformI(Image);
        //        var results = yolo.RunSegmentation(bitmap1.ToImageSharp());

        //        var x = results.First().SegmentedPoints();
        //        ColorSegmentedObject(bitmap1,x,Color.Red);
        //        textEditEqui.Text = "Yellow";
        //        var color = DetermineObjectColor(bitmap1, x);
        //        ////using (Graphics g = Graphics.FromImage(bitmap1))
        //        //    //{
        //        //    //    foreach (Point pt in x)
        //        //    //    {
        //        //    //        // Ensure point is within image bounds
        //        //    //        if (pt.X >= 0 && pt.X < bitmap1.Width && pt.Y >= 0 && pt.Y < bitmap1.Height)
        //        //    //        {
        //        //    //            // Draw a small circle at each segmented point
        //        //    //            g.FillEllipse(Brushes.Red, pt.X - 2, pt.Y - 2, 4, 4);
        //        //    //        }
        //        //    //    }
        //        //    //}

        //        //    //var histogram = CalculateDenseHistogram(bitmap1, results.First().SegmentedPoints());
        //        //    //PrintHistogram(histogram);

        //           // var co = TransformSegmented(Image, results.First().SegmentedPoints());
        //           // Console.WriteLine(co.ToString());
        //        //    Image._EqualizeHist();
        //        //    pictureBoxEquialization.Image = Image.ToBitmap();
        //        //    textEditEqui.Text = TransformI(Image);




        //        //    var imageT = Apply_CLAHE(img);
        //        //    pictureBoxCLAHE.Image = imageT.Bitmap;

        //        //    textEditCLAHE.Text = TransformI(imageT);
        //        //    //Transform(i);
        //        //    vect = new Vector3();

        //        //    var i = Dehazing.Dehaze(Image);
        //        //    pictureBoxDehaze.Image = i.Bitmap;
        //        //    textEditDEHAZE.Text = TransformI(i);
        //    }

        //}
        public void ColorSegmentedObject(Bitmap image, System.Drawing.Point[] segmentedPoints, Color color)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                // Create a brush with the specified color
                using (Pen pen = new Pen(color, 2)) // 2 is the width of the pen
                {
                    // Draw the segmented contour
                    g.DrawPolygon(pen, segmentedPoints);
                }
                using (SolidBrush brush = new SolidBrush(color))
                {
                    // Fill the segmented area with the color
                    g.FillPolygon(brush, segmentedPoints);
                }
            }
            pictureBoxEquialization.Image = image;
        }
        public string DetermineObjectColors(Bitmap originalImage, Point[] segmentedPoints)
        {
            // Initialize variables to accumulate color information
            int totalRed = 0, totalGreen = 0, totalBlue = 0;
            int count = 0;
            string color = "No match";
            // Iterate over segmented points
            foreach (Point pt in segmentedPoints)
            {
                // Ensure point is within image bounds
                if (pt.X >= 0 && pt.X < originalImage.Width && pt.Y >= 0 && pt.Y < originalImage.Height)
                {
                    // Get color of the pixel at the segmented point
                    Color pixelColor = originalImage.GetPixel(pt.X, pt.Y);

                    // Accumulate color components
                    totalRed += pixelColor.R;
                    totalGreen += pixelColor.G;
                    totalBlue += pixelColor.B;

                    count++;
                }
            }

            // Calculate average color
            if (count > 0)
            {
                int avgRed = totalRed / count;
                int avgGreen = totalGreen / count;
                int avgBlue = totalBlue / count;

                //return Color.FromArgb(avgRed, avgGreen, avgBlue);
                vect = new Vector3((float)avgRed, (float)avgGreen, (float)avgBlue);
                 color = GetBestMatchColor(vect, colorReferences);
                Console.WriteLine("matched color is  : " + color);
            }

            // Default to black if no valid points found (edge case handling)
            return color;
        }
        public Color DetermineObjectColor(Bitmap originalImage, Point[] segmentedPoints)
        {
            // Initialize variables to accumulate color information
            int totalRed = 0, totalGreen = 0, totalBlue = 0;
            int count = 0;

            // Iterate over segmented points
            foreach (Point pt in segmentedPoints)
            {
                // Ensure point is within image bounds
                if (pt.X >= 0 && pt.X < originalImage.Width && pt.Y >= 0 && pt.Y < originalImage.Height)
                {
                    // Get color of the pixel at the segmented point
                    Color pixelColor = originalImage.GetPixel(pt.X, pt.Y);

                    // Accumulate color components
                    totalRed += pixelColor.R;
                    totalGreen += pixelColor.G;
                    totalBlue += pixelColor.B;

                    count++;
                }
            }

            // Calculate average color
            if (count > 0)
            {
                int avgRed = totalRed / count;
                int avgGreen = totalGreen / count;
                int avgBlue = totalBlue / count;

                //return Color.FromArgb(avgRed, avgGreen, avgBlue);
                vect = new Vector3((float)avgRed, (float)avgGreen, (float)avgBlue);
                var color = GetBestMatchColor(vect, colorReferences);
                Console.WriteLine("matched color is  : " + color);
            }

            // Default to black if no valid points found (edge case handling)
            return Color.Black;
        }
        public int[,] CalculateDenseHistogram(Bitmap image, Point[] points)
        {
            int[,] histogram = new int[256, 3];

            foreach (var point in points)
            {
                if (point.X < 0 || point.X >= image.Width || point.Y < 0 || point.Y >= image.Height)
                    continue;

                Color pixelColor = image.GetPixel(point.X, point.Y);
                histogram[pixelColor.R, 0]++;
                histogram[pixelColor.G, 1]++;
                histogram[pixelColor.B, 2]++;
            }

            return histogram;
        }

        public  void PrintHistogram(int[,] histogram)
        {
            for (int i = 0; i < 256; i++)
            {
                Console.WriteLine($"Value {i}: R={histogram[i, 0]}, G={histogram[i, 1]}, B={histogram[i, 2]}");
            }
        }
        private string TransformSegmented(Image<Bgr, byte> imageT, System.Drawing.Point[] segmentedPoints)
        {
            int red = 0;
            int green = 0;
            int blue = 0;

            // Split the image into its color channels
            Image<Gray, byte>[] channelsT = imageT.Split();

            // Ensure you have a Bitmap to work with pixel values
            Bitmap bitmap1 = imageT.ToBitmap();
            pictureBoxDehaze.Image = imageT.Bitmap;

            // Prepare the chart for displaying histograms
            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();
            chartControl1.Titles.Add(new ChartTitle { Text = "Color Histogram" });

            // Set up channel names and histogram parameters
            string[] channelNames = { "Blue", "Green", "Red" };
            int numQuantizedBins = 10;
            int binSize = 256 / numQuantizedBins; // Size of each bin in the quantized histogram

            // Loop through each color channel
            for (int i = 0; i < 3; i++)
            {
                // Prepare the histogram for the current channel
                DenseHistogram histogram = new DenseHistogram(numQuantizedBins, new RangeF(0, 256));
                float[] histValues = new float[numQuantizedBins];

                // Calculate the histogram for the segmented points
                foreach (var point in segmentedPoints)
                {
                    if (point.X >= 0 && point.X < imageT.Width && point.Y >= 0 && point.Y < imageT.Height)
                    {
                        byte pixelValue = channelsT[i].Data[point.Y, point.X, 0];
                        int binIndex = pixelValue / binSize;
                        histValues[binIndex]++;
                    }
                }

                // Normalize the histogram
                float maxHistValue = histValues.Max();
                for (int j = 0; j < histValues.Length; j++)
                {
                    histValues[j] /= maxHistValue;
                }

                // Create a series for the histogram and find the dominant value
                Series series = new Series(channelNames[i], ViewType.Bar);
                int dominantValueIndex = -1;
                float maxBinValue = float.MinValue;

                for (int j = 0; j < numQuantizedBins; j++)
                {
                    double histValue = histValues[j];
                    series.Points.Add(new SeriesPoint(j * binSize, histValue));

                    if (histValue > maxBinValue)
                    {
                        maxBinValue = (float)histValue;
                        dominantValueIndex = j * binSize;
                    }
                }

                // Output the dominant value for the current channel
                Console.WriteLine($"Channel: {channelNames[i]}, Dominant Value: {dominantValueIndex}");
                switch (i)
                {
                    case 0: // Blue
                        blue = dominantValueIndex;
                        break;
                    case 1: // Green
                        green = dominantValueIndex;
                        break;
                    case 2: // Red
                        red = dominantValueIndex;
                        break;
                }

                // Customize series view settings
                series.View.Color = i == 0 ? System.Drawing.Color.Blue : i == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                Console.WriteLine($"{red},{green},{blue}");

                chartControl1.Series.Add(series);
            }

            // Determine the closest color match and update the UI
            var vect = new Vector3((float)red, (float)green, (float)blue);
            var color = GetBestMatchColor(vect, colorReferences);
            Console.WriteLine("Matched color is  : " + color);
            textEditRGB.Text = $"{red},{green},{blue}||{color}";

            return color;
        }

        private void Transform(Image<Bgr, byte> imageT)
        {

            int red = 0;
            int green = 0;
            int blue = 0;


            // Image<Gray, byte>[] channelsT = img.Split();
            Image<Gray, byte>[] channelsT = imageT.Split();
            Bitmap bitmap1 = imageT.ToBitmap();
            pictureBoxDehaze.Image = imageT.Bitmap;

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


        new ColorReference() { Name="Red", Argb=new Vector3(139,0,139) },
        new ColorReference() { Name="Red", Argb=new Vector3(112,41,99) },
        new ColorReference() { Name="Red", Argb=new Vector3(135,38,87) },
        new ColorReference() { Name="Red", Argb=new Vector3(255,0,127) },
        new ColorReference() { Name="Red", Argb=new Vector3(105,53,156) },
        new ColorReference() { Name="Red", Argb=new Vector3(143,0,255 ) },
        new ColorReference() { Name="Red", Argb=new Vector3(100,50,100) },
        new ColorReference() { Name="Red", Argb=new Vector3(75,25,50) },
        new ColorReference() { Name="Red", Argb=new Vector3(75,50,75) },
        new ColorReference() { Name="Red", Argb=new Vector3(100,75,75) },
        //new ColorReference() { Name="Magenta", Argb=new Vector3(101,0,11) },
        //new ColorReference() { Name="Magenta", Argb=new Vector3(48,0,16) },


       // Green shades
        new ColorReference() { Name="Green", Argb=new Vector3(0,255,0) },
        new ColorReference() { Name="Green ", Argb=new Vector3(0,100,0) },
        new ColorReference() { Name="Green ", Argb=new Vector3(100,255,100) },
        new ColorReference() { Name="Green ", Argb=new Vector3(0,100,100) },
        new ColorReference() { Name="Green ", Argb=new Vector3(0,192,0)},
        new ColorReference() { Name="Green ", Argb=new Vector3(19,136,8) },
        new ColorReference() { Name="Green ", Argb=new Vector3(85,107,47) },
        new ColorReference() { Name="Green ", Argb=new Vector3(128,128,0) },
        new ColorReference() { Name="Green ", Argb=new Vector3(75,83,32) },
        new ColorReference() { Name="Green ", Argb=new Vector3(1,68,33) },
        new ColorReference() { Name="Green ", Argb=new Vector3(0,66,37) },
        new ColorReference() { Name="Green ", Argb=new Vector3(1,50,32) },

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
        new ColorReference() { Name= "Yellow",Argb=new Vector3(225,100,50)},



        new ColorReference() { Name="Black", Argb=new Vector3(0,0,0) },
        new ColorReference() { Name="Black", Argb=new Vector3(25,25,25)},
        new ColorReference() { Name="Black", Argb=new Vector3(50,50,50) },
        // new ColorReference(){ Name="Black", Argb=new Vector3(50,50,75) },
        //new ColorReference() { Name="Black", Argb=new Vector3(0, 0, 0) },
        //new ColorReference() { Name="Black", Argb=new Vector3(0, 0, 0) },

        new ColorReference() { Name="White", Argb=new Vector3(255,255,255) },
        new ColorReference() { Name="White", Argb=new Vector3(255,248,220) },
        new ColorReference() { Name="White", Argb=new Vector3(255,240,245) },
        new ColorReference() { Name="White", Argb=new Vector3(248,248,255) },
        new ColorReference() { Name="White", Argb=new Vector3(225,225,225) },

        //new ColorReference() { Name="Gray", Argb=new Vector3(169,169,169) },
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

        private void pictureBoxEquialization_Click(object sender, EventArgs e)
        {

        }
        private void TestYolo8()
        {
            var yolo = new Smartvisor.Benchmark.Yolo8.Yolo(@"E:\yolov8s-seg.onnx");
            //var image = SixLabors.ImageSharp.Image.Load<Rgba32>(@"E:\car.jpg");
            var jpeg = System.Drawing.Image.FromFile(@"E:\car.jpg");
            Bitmap bitmap = new Bitmap(jpeg);

            //var results = yolo.RunSegmentation(bitmap.ToImageSharp());
        }
    }
}

public class ColorMap
{
    public Vector3 UnknownVector { get; set; }
    public Vector3 KnownVector { get; set; }
    public string ColorName { get; set; }
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
