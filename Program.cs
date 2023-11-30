// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.IO;
// using System.Drawing;
// using Emgu.CV;
// using Emgu.CV.CvEnum;
// using Emgu.CV.Structure;
//using Emgu.CV.Util;
using Microsoft.VisualBasic;
using OpenCvSharp;

namespace TestDigitalSolution324
{
    class Program
    {
        public static void  CaptureFrames(){
            string videoPath = "./video/videoTest.mp4";// Ruta del archivo de video

            using (var video = new VideoCapture(videoPath)) // Lectura del video
            {
                
                if (!video.IsOpened()) // validacion de apertura del video
                {
                    Console.WriteLine("No se pudo abrir el archivo de video.");
                    return;
                }

                string outputFolder = "frames/captures";// Crea una carpeta para guardar las imágenes
                Directory.CreateDirectory(outputFolder);

                Mat frame = new Mat(); // guarda cada frame como una imagen
                int frameCount = 0;
                while (true)
                {
                    
                    if (!video.Read(frame))
                    {
                        break;
                    }

                    // Guarda el frame como una imagen
                    string imagePath = Path.Combine(outputFolder, $"frame{frameCount:D5}.jpg");
                    Cv2.ImWrite(imagePath, frame);

                    frameCount++;
                }

                Console.WriteLine($"Se guardaron {frameCount} imágenes en la carpeta {outputFolder}.");
            }
        }

        public static void AnalyzeFace(){ //Solo detecta los rostros

            string videoPath = "video/analyzePeople.mp4";

            using (var video = new VideoCapture(videoPath))
            {
                
                if (!video.IsOpened()) // Validacion si el video se abrió correctamente
                {
                    Console.WriteLine("No se pudo abrir el archivo de video.");
                    return;
                }

                string outputFolder = "video/analyzedFaces"; // Crea una carpeta para guardar las imágenes
                Directory.CreateDirectory(outputFolder);

                //Para detectar caras
                CascadeClassifier faceDetector = new CascadeClassifier("data/haarcascade_frontalface_default.xml");

                //Analiza cada frame
                Mat frame = new Mat();
                int frameCount = 0;
                while (true)
                {
                   
                    if (!video.Read(frame))
                    {
                        break;
                    }

                    Rect[] faces = faceDetector.DetectMultiScale(frame); //Para detectar las caras en el frame

                    foreach (Rect face in faces) // Se encasilla cada cara que se detecta
                    {
                        Cv2.Rectangle(frame, face, Scalar.Red, 2);
                    }

                    //Guarda el frame con los rectángulos dibujados
                    string imagePath = Path.Combine(outputFolder, $"frame{frameCount:D5}.jpg");
                    Cv2.ImWrite(imagePath, frame);

                    frameCount++;
                }

                Console.WriteLine($"Se analizaron {frameCount} frames en el video.");    
            } 
        }
    
        public static void FacialDifferences(){
            
            VideoCapture capture = new VideoCapture("video/analyzePeople.mp4");

            //Para detectar caras
            CascadeClassifier faceCascade = new CascadeClassifier("data/haarcascade_frontalface_default.xml");

            //Objeto de comparacion de histograma
            HistCompMethods method = HistCompMethods.Correl;
            double Limit = 0.5; //umbral
            Mat[] histograms = new Mat[2];
            histograms[0] = new Mat();
            histograms[1] = new Mat();

            
            while (true) //Recorrido de los frames 
            {
               
                Mat frame = new Mat();
                capture.Read(frame);

                if (frame.Empty())
                {
                    break;
                }

                //Convierte el frame a escala de grises siguiendo el estandar de deteccion de caras
                Mat gray = new Mat();
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

                Rect[] faces = faceCascade.DetectMultiScale(gray, 1.3, 5);


                foreach (Rect face in faces)  // Recorre el arreglo donde estan todas las caras detectadas  
                {
                    // Extract the face from the frame
                    Mat faceMat = new Mat(frame, face);

                    //Calcula el histograma de la cara
                    Cv2.CalcHist(new Mat[] { faceMat }, new int[] { 0 }, new Mat(), histograms[0], 1, new int[] { 256 }, new Rangef[] { new Rangef(0, 256) });
                    Cv2.Normalize(histograms[0], histograms[0], 0, 1, NormTypes.MinMax);

                    // Loop through the histograms of the other faces
                    DirectoryInfo dir = new DirectoryInfo("faces");
                    FileInfo[] files = dir.GetFiles("*.jpg");
                    foreach (FileInfo file in files)
                    {
                        Mat otherHist = new Mat();
                        /* //Cv2.ImRead(file.FullName, ImreadModes.GrayScale).ConvertTo(otherHist, MatType.CV_32F);*/
                        Cv2.CalcHist(new Mat[] { otherHist }, new int[] { 0 }, new Mat(), histograms[1], 1, new int[] { 256 }, new Rangef[] { new Rangef(0, 256) });
                        Cv2.Normalize(histograms[1], histograms[1], 0, 1, NormTypes.MinMax);

                        //Compara los histogramas
                        double result = Cv2.CompareHist(histograms[0], histograms[1], method);

                        // Si está por encima del umbral, las caras coinciden
                        if (result > Limit)
                        {
                            // Para cada persona se guarda en su carpeta
                            string name = Path.GetFileNameWithoutExtension(file.Name);
                            string folder = Path.Combine("output", name);
                            Directory.CreateDirectory(folder);
                            string filename = Path.Combine(folder, Guid.NewGuid().ToString() + ".jpg");
                            Cv2.ImWrite(filename, faceMat);
                        }
                    }

                    //Guarda el histograma de la cara
                    string histFilename = Path.Combine("faces", Guid.NewGuid().ToString() + ".jpg");
                    Cv2.ImWrite(histFilename, faceMat);
                }
            }
            Console.WriteLine($"proceso terminado!");
        }

        public static void Main(string[]args){
            CaptureFrames();
            //AnalyzeFace();
            FacialDifferences();
        }
    }
        
}

