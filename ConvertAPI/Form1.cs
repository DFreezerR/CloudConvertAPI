using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CloudConvert.API;
using CloudConvert.API.Models.ExportOperations;
using CloudConvert.API.Models.ImportOperations;
using CloudConvert.API.Models.JobModels;
using CloudConvert.API.Models.TaskOperations;

namespace ConvertAPI
{
    public partial class Form1 : Form
    {
        public Logger logger;
        public string filePath = String.Empty;
        public string fileDownloadPath = String.Empty;
        public bool isHeaderPressed = false;
        public Point prevMousePos;
        public async void  CreateTask(string pathFrom, string pathTo, string extFrom, string extTo)
        {
            try
            {
                var _cloudConvert = new CloudConvertAPI("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiM2U5MDk5MWQ3OGEyNjhiZGM5NWQzNjZmNTI3NTJlYjRhZTEzNGVlMGI2YWJlN2RhZmQ3ZDMzYTRkNTkyZDg5MjQyY2ZlNDI1M2RhYzVmN2UiLCJpYXQiOjE2MzE0NTQyMDYuNzM4MjE1LCJuYmYiOjE2MzE0NTQyMDYuNzM4MjE4LCJleHAiOjQ3ODcxMjc4MDYuNzA1ODQzLCJzdWIiOiI1MzM2ODA2OCIsInNjb3BlcyI6WyJ0YXNrLnJlYWQiLCJ0YXNrLndyaXRlIl19.UQLWtBbElpOT1VnoIdjvikXNRxyDtaIBaRcTJnZ2G-GPvUa2SR4W6b7Xztzb_V2zur2xk5QzKvFKNzYIj1etbLdQQijlqGigTsw0TAuU1-KyjAqlCB2pE-w0K3xPZHqpTv-FNvVzhmBu0mczeBNnLW0bEGsQ_6q5hBMwzC3Xh4WW5gTntS6p3IVAdv_3uIkRoLAaD9UIfn4-YajHfpg6PiTqceT1G4FSVZGEEjRI5UzbwiIiNNx5GN-Uu6lkIzftP6RAch9mzDta4lSoY1IX0AYo8_KCglfro3qbpCZhUnMHIDvHygv2seQlsE8KU9zeoYQxemr6ReHjjxFG-5E8rianBMz1cPmXMNGqDGtUN8yRsT_R3-d-yXMVytkM82-5wolrAKhPgyct2uzRfH5tZMvS7Bg_PMb65FaZV-RhWj2FvTUn4F52hFdpo2QR7Grk63bBtUD_pD8exKMBZhQc0iXlKA8fmtwz1S5mbuPuYIDQ6SYTbSOIYTiUo3Xc2iXs8h4vTZ6lvQDYHIifYI8FrthEa9F1HcPDWmH63Q_UzxpCJXmrRL8TTP_XnzPmv6KrQSKZltXG66Q4X5UYgsql3yS4mvCPEP8pCPs9TI13rUU_3MJVWlr6Wsw7hfWl6U6QFB-0ydunsfvWv54ZbJYbXZPaMUlN43QEXl6SLHYb8_8", true);

                var job = await _cloudConvert.CreateJobAsync(new JobCreateRequest
                {
                    Tasks = new
                    {
                        upload = new ImportUploadCreateRequest(),
                        convert = new ConvertCreateRequest
                        {
                            Input = "upload",
                            Input_Format = extFrom,
                            Output_Format = extTo
                        
                        },
                        export = new ExportUrlCreateRequest
                        {
                            Input = "convert",
                            Archive_Multiple_Files = true
                        }
                    },
                    Tag = "TEST"
                
                });
                logger.Log("Tasks created!");
                var uploadTask = job.Data.Tasks.FirstOrDefault(t => t.Name == "upload");

                byte[] file = await File.ReadAllBytesAsync(pathFrom);
            
                string fileName = Path.GetFileName(pathFrom);
                logger.Log("Files are uploading...", fileName);
                await _cloudConvert.UploadAsync(uploadTask.Result.Form.Url.ToString(), file, fileName, uploadTask.Result.Form.Parameters);
                logger.Log("Files uploaded!", fileName);
                logger.Log("Files are converting...", fileName);
                var exportJob = await _cloudConvert.WaitJobAsync(job.Data.Id);
                if (exportJob.Data.Status == "error")
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("\nSomething is wrong!");
                    for (int i = 0; i < exportJob.Data.Tasks.Count; i++)
                    {
                        builder.AppendLine($"Operation: {exportJob.Data.Tasks[i].Operation} Code: {exportJob.Data.Tasks[i].Code} Message: {exportJob.Data.Tasks[i].Message}");
                    }

                    throw new Exception(builder.ToString());
                }
                logger.Log("Files converted!", fileName);
                var exportTask = exportJob.Data.Tasks.FirstOrDefault(t => t.Name == "export");
        
                var fileExport = exportTask.Result.Files.FirstOrDefault();
                logger.Log("Files are downloading...", fileExport.Filename);
                using (var client = new WebClient())
                {
                    client.DownloadFile(fileExport.Url, Path.Combine(pathTo, fileExport.Filename));
                }
                logger.Log("Files downloaded!", fileExport.Filename);
                if (MessageBox.Show("Open download directory?", "Open", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Process.Start("explorer.exe", fileDownloadPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error! " + e.Message);
            }
        }
        public Form1()
        {
            InitializeComponent();
            logger = new Logger(txtLogs);
            cmbFrom.SelectedIndex = 0;
            cmbTo.SelectedIndex = 0;
            txtLogs.TextChanged += TxtLogsOnTextChanged;
        }

        private void TxtLogsOnTextChanged(object? sender, EventArgs e)
        {
            txtLogs.SelectionStart = txtLogs.Text.Length;
            txtLogs.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Select files";
            dialog.Filter = $"{cmbFrom.SelectedItem.ToString().ToUpper()} files (*.{cmbFrom.SelectedItem.ToString()})|*.{cmbFrom.SelectedItem.ToString()}|All files (*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filePath = dialog.FileName;
                txtFilePath.Text = "File path: " + filePath;
                fileDownloadPath = Path.GetDirectoryName(filePath);
                txtDownloadPath.Text = "Download path: " + fileDownloadPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateTask(filePath, fileDownloadPath, cmbFrom.SelectedItem.ToString(), cmbTo.SelectedItem.ToString());
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            isHeaderPressed = true;
            prevMousePos = e.Location;
        }

        private void pnlHeader_MouseUp(object sender, MouseEventArgs e)
        {
            isHeaderPressed = false;
        }

        private void pnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (isHeaderPressed)
            {
                Point newMousePos = e.Location;
                var deltaX = newMousePos.X - prevMousePos.X;
                var deltaY = newMousePos.Y - prevMousePos.Y;
                Point newPos = new Point(this.Location.X + deltaX, this.Location.Y + deltaY);
                this.Location = newPos;
                this.Update();
            }
        }
    }
}