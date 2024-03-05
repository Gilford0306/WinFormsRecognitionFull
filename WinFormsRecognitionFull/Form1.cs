using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsRecognitionFull
{
    public partial class Form1 : Form
    {
        BasicAWSCredentials credentials = new BasicAWSCredentials("AKIA6ODU52MBL3JUGBGE", "dnCW4j+YMoW2Xkfpkfvo0okal+aHRlSGdwbygyyY");
        string filePath = string.Empty;
        PictureBox pictureBox2 = new PictureBox();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var fileContent = string.Empty;
            filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "images files (*.jpg)|*.jpg";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    Upload(filePath);


                }
            }
            panel1.Controls.Remove(pictureBox2);
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.Image = System.Drawing.Image.FromFile(filePath);
            pictureBox2.SizeMode = PictureBoxSizeMode.Normal;
            panel1.Controls.Add(pictureBox2);
        }
        private async Task Upload(string str)
        {
            string bucketName = "pavelbucket0307";
            string keyName = "neon.jpg";
            string filePath = str;

            // Set up your AWS credentials


            // Create a new Amazon S3 client
            AmazonS3Client s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest1);

            try
            {
                // Upload the file to Amazon S3
                TransferUtility fileTransferUtility = new TransferUtility(s3Client);
                fileTransferUtility.Upload(filePath, bucketName, keyName);
                Console.WriteLine("Upload completed!");
                await Rekognition();
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }

        private async Task Rekognition()
        {

            String photo = "neon.jpg";
            String bucket = "pavelbucket0307";

            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(credentials, Amazon.RegionEndpoint.EUWest1);

            var detectfacerequest = new DetectFacesRequest
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new S3Object()
                    {
                        Name = photo,
                        Bucket = bucket,
                    },
                },

            };

            try
            {

                DetectFacesResponse detectLabelsResponse = await rekognitionClient.DetectFacesAsync(detectfacerequest);
                var lableDetections = detectLabelsResponse.FaceDetails;
                Bitmap bmp = new Bitmap(pictureBox2.Image);

                using (Graphics g = Graphics.FromImage(bmp))
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    foreach (FaceDetail face in lableDetections)

                    {

                        BoundingBox box = face.BoundingBox;

                        int left = (int)(box.Left * bmp.Width);
                        int top = (int)(box.Top * bmp.Height);
                        int width = (int)(box.Width * bmp.Width);
                        int height = (int)(box.Height * bmp.Height);

                        Rectangle rect = new Rectangle(left, top, width, height);
                        g.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height);


                    }
                }

                pictureBox2.Image = bmp;



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
    

}
