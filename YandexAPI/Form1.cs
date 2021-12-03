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
using YandexDisk.Client.Protocol;
using System.IO;

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
            label1.Text = "";
            api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
            var FileData1 = await api.MetaInfo.GetInfoAsync(new ResourceRequest { Path = "/" });
            foreach (var item in FileData1.Embedded.Items)
            {
                //MessageBox.Show(item.Name);
                a += item.Name.ToString() + "\t"+item.Type+"\n";
            }
            label1.Text = a;

        }
        public async void CreateFolderOnDisk()
        {
            try
            {
                api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                var FileData1 = await api.MetaInfo.GetInfoAsync(new ResourceRequest { Path = "/" });
                if (!FileData1.Embedded.Items.Any(i => i.Type == ResourceType.Dir && i.Name.Equals(textBox1.Text)))
                {
                    await api.Commands.CreateDictionaryAsync("/" + textBox1.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                OpenFileDialog _fileDialog = new OpenFileDialog();
                _fileDialog.Title = "chose file 2 load";
                _fileDialog.Filter = "All Files|*.*";
                //_fileDialog.Filter = "Image Files (*.bmp,*.png,*.jpg,*.jpeg)|*.bmp;*.png;*.jpg;*.jpeg";
                _fileDialog.ShowDialog();
                var link = await api.Files.GetUploadLinkAsync("/" + textBox1.Text + "/" + Path.GetFileName(_fileDialog.FileName), overwrite: false);
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
            try
            {
                label3.Text = "";
                api = new DiskHttpApi("AQAAAAAaPvlcAAeKXT1kcOTXzkdQmUTwbSQfRrQ");
                var FileData1 = await api.MetaInfo.GetInfoAsync(new ResourceRequest { Path = "/" + textBox1.Text});
                foreach (var item in FileData1.Embedded.Items)
                {
                    //MessageBox.Show(item.Name);
                    a += item.Name.ToString() + "\t" + item.Type + "\n";
                }
                label3.Text = a;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
