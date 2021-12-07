using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YandexDisk.Client.Http;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Protocol;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace YandexAPI
{
    public partial class Form1 : Form
    {
        string a;
        DiskHttpApi api;
        public Form1()
        {
            InitializeComponent();
            label2.Text += Environment.CurrentDirectory;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            GetInfoFromDisk();
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CreateFolderOnDisk();
        }
        public async void GetInfoFromDisk()
        {
            //Получение информации о всех файлах и каталогах на Яндекс Диске
            infoLabel.Text = "";
            api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
            var FileData1 = await api.MetaInfo.GetInfoAsync(new ResourceRequest { Path = "/" });
            foreach (var item in FileData1.Embedded.Items)
            {
                //MessageBox.Show(item.Name);
                a += item.Name.ToString() + "      "+item.Type+"\n";
            }
            infoLabel.Text = a;

        }
        public async void CreateFolderOnDisk()
        {
            //Создание каталога на диске 
            //можно создать каталог в каталоге, для этого очевидно пишел test/test1 и так далее
            try
            {
                api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                var FileData1 = await api.MetaInfo.GetInfoAsync(new ResourceRequest { Path = "/" });
                if (!FileData1.Embedded.Items.Any(i => i.Type == ResourceType.Dir && i.Name.Equals(tbFolderName.Text)))
                {
                    await api.Commands.CreateDictionaryAsync("/" + tbFolderName.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //асинхронная загрузка файла на Яндекс Диск
            try
            {
                api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                OpenFileDialog _fileDialog = new OpenFileDialog();
                _fileDialog.Title = "chose file 2 load";
                _fileDialog.Filter = "All Files|*.*";
                //без фильтров
                //_fileDialog.Filter = "Image Files (*.bmp,*.png,*.jpg,*.jpeg)|*.bmp;*.png;*.jpg;*.jpeg";
                _fileDialog.ShowDialog();
                var link = await api.Files.GetUploadLinkAsync("/" + tbFolderName.Text + "/" + Path.GetFileName(_fileDialog.FileName), overwrite: false);
                using (var fs = File.OpenRead(_fileDialog.FileName))
                {
                    await api.Files.UploadAsync(link, fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GetDataFromCustomFolder();
        }
        public async void GetDataFromCustomFolder()
        {
            //в tbFolderName пошем название каталога и получаем информацию о файлах и каталогах внутри него
            try
            {
                infoLabel.Text = "";
                api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                var FileData1 = await api.MetaInfo.GetInfoAsync(new ResourceRequest { Path = "/" + tbFolderName.Text});
                foreach (var item in FileData1.Embedded.Items)
                {
                    //MessageBox.Show(item.Name);
                    a += item.Name.ToString() + "      " + item.Type + "\n";
                }
                infoLabel.Text = a;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public async void DownloadSelectedFile()
        {
            //в tbFileName пишем полный путь к файл на диске и скачиваем его
            try
            {
                api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                var Path2download = Path.Combine(Environment.CurrentDirectory, "downloads");
                if (!Directory.Exists(Path2download))
                {
                    Directory.CreateDirectory(Path2download);
                }
                var FileData1 = await api.MetaInfo.GetInfoAsync(new ResourceRequest { Path = "/" + tbFolderName.Text });
                //можно скачать файл отдельно
                //await api.Files.DownloadFileAsync(path: "/" + tbFolderName.Text + "/" + tbFileName.Text, Path.Combine(Path2download, tbFileName.Text));

                //можно скачать все файлы из папки
                foreach (var item in FileData1.Embedded.Items)
                {
                    await api.Files.DownloadFileAsync(path: item.Path, Path.Combine(Path2download, item.Name));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DownloadSelectedFile();
        }

        private async void panel1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                label4.Text = "Перетащи сюда файлы!";
                //string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                List<string> filepaths = new List<string>();
                foreach (var obj in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    if (Directory.Exists(obj))
                    {
                        filepaths.AddRange(Directory.GetFiles(obj, "*.*", SearchOption.AllDirectories));
                    }
                    else
                    {
                        filepaths.Add(obj);
                    }

                }
                foreach (var fileInList in filepaths)
                {
                    MessageBox.Show(Path.GetFileNameWithoutExtension(fileInList));
                    if (radioButton1.Checked == true)
                    {
                        //Это для Яндекса
                        //*************************************
                        api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                        var link = await api.Files.GetUploadLinkAsync("/" + tbFileName.Text + "/" + Path.GetFileName(fileInList), overwrite: false);
                        using (var fs = File.OpenRead(fileInList))
                        {
                            await api.Files.UploadAsync(link, fs);
                        }
                    }
                    else if (radioButton2.Checked == true)
                    {
                        //массив байтов файла
                        byte[] imageArray = File.ReadAllBytes(fileInList);
                        //конвертируем в base64
                        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                        using (WebClient client = new WebClient())
                        {
                            //задаем параметры для коллекции
                            NameValueCollection param = new NameValueCollection();
                            param.Add("key", "04260c2bd7f6a34ce3ee4459e5991e56");
                            param.Add("image", base64ImageRepresentation);
                            //делаем запрос методом POST и получем массив байтов
                            var response = client.UploadValues("https://api.imgbb.com/1/upload", "POST", param);
                            //декодируем
                            var jsonResponse = Encoding.Default.GetString(response);
                            //десериализуем
                            ImageDataJSON UploadedImageData = JsonConvert.DeserializeObject<ImageDataJSON>(jsonResponse);
                            //скопируем URL в буфер обмена
                            Clipboard.SetData(DataFormats.Text, (Object)UploadedImageData.data.image.url);
                            //MessageBox.Show(UploadedImageData.data.image.url);

                        }
                    }
                    else if (radioButton3.Checked == true)
                    {
                        MessageBox.Show("потом сделаю");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                label4.Text = "Бросай!";
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void panel1_DragLeave(object sender, EventArgs e)
        {
            label4.Text = "Перетащи сюда файлы!";
        }
    }
}
