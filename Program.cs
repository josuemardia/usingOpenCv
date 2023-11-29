// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.IO;
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

                string outputFolder = "video/salida";// Crea una carpeta para guardar las imágenes
                Directory.CreateDirectory(outputFolder);

                // Lee el video y guarda cada frame como una imagen
                Mat frame = new Mat();
                int frameCount = 0;
                while (true)
                {
                    
                    if (!video.Read(frame))// Lee el siguiente frame
                    {
                        break;
                    }

                    // Guarda el frame como una imagen
                    string imagePath = Path.Combine(outputFolder, $"frame{frameCount:D5}.jpg");
                    Cv2.ImWrite(imagePath, frame);

                    frameCount++; // Incrementador
                }

                Console.WriteLine($"Se guardaron {frameCount} imágenes en la carpeta {outputFolder}.");
            }
        }

        public static void AnalyzeFace(){
            // Ruta del archivo de video
            string videoPath = "video/analyzePeople.mp4";

            // Crea un objeto VideoCapture para leer el video
            using (var video = new VideoCapture(videoPath))
            {
                // Verifica si el video se abrió correctamente
                if (!video.IsOpened())
                {
                    Console.WriteLine("No se pudo abrir el archivo de video.");
                    return;
                }

                // Crea una carpeta para guardar las imágenes
                string outputFolder = "video/AnalyzePeopleFaces";
                Directory.CreateDirectory(outputFolder);

                // Crea un objeto CascadeClassifier para detectar caras
                CascadeClassifier faceDetector = new CascadeClassifier("haarcascade_frontalface_default.xml");

                // Lee el video y analiza cada frame
                Mat frame = new Mat();
                int frameCount = 0;
                while (true)
                {
                    // Lee el siguiente frame
                    if (!video.Read(frame))
                    {
                        break;
                    }

                    // Detecta las caras en el frame
                    Rect[] faces = faceDetector.DetectMultiScale(frame);

                    // Dibuja un rectángulo alrededor de cada cara detectada
                    foreach (Rect face in faces)
                    {
                        Cv2.Rectangle(frame, face, Scalar.Red, 2);
                    }

                    // Guarda el frame con los rectángulos dibujados
                    string imagePath = Path.Combine(outputFolder, $"frame{frameCount:D5}.jpg");
                    Cv2.ImWrite(imagePath, frame);

                    // Incrementa el contador de frames
                    frameCount++;
                }

                Console.WriteLine($"Se analizaron {frameCount} frames en el video.");

                // Analiza las diferencias faciales en cada imagen
                // y genera una imagen con las diferencias faciales de cada persona
                // (Este paso requiere más código y no se puede proporcionar aquí).
            }   
        }

        public static void Main(string[]args){
            CaptureFrames();
            AnalyzeFace();
        }
    }
}
